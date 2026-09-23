using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.Rendering.Universal;

namespace Sensori.Montessori.Editor
{
    public static class MontessoriSceneAutoSetup
    {
        const string ArtFolder = "Assets/Montessori/Art/Generated";
        const string ContentFolder = "Assets/Montessori/Content";
        const string SceneFolder = "Assets/Montessori/Scenes";
        const string ScenePath = "Assets/Montessori/Scenes/Atelier.unity";
        const string CatalogPath = "Assets/Montessori/Content/Catalogue.asset";
        const string ActionsPath = "Assets/Settings/InputSystem_Actions.inputactions";

        [MenuItem("Montessori/Auto-Setup Scene")]
        public static void Setup()
        {
            if (EditorApplication.isPlaying)
            {
                EditorUtility.DisplayDialog("Sensori", "Arrête le mode Lecture avant de lancer l'auto-setup.", "OK");
                return;
            }

            bool accepted = EditorUtility.DisplayDialog(
                "Sensori",
                "Cette action crée ou met à jour les textures, le catalogue Montessori et la scène Atelier.\n\nLe contenu par défaut (alphabet, chiffres, formes, couleurs et les trois mini-jeux) est réécrit. La scène ouverte sera remplacée après une demande d'enregistrement.",
                "Continuer",
                "Annuler");
            if (!accepted)
                return;

            try
            {
                EditorUtility.DisplayProgressBar("Sensori", "Préparation des dossiers", 0.05f);
                EnsureFolder(ArtFolder);
                EnsureFolder(ContentFolder + "/Jeux");
                EnsureFolder(ContentFolder + "/Elements");
                EnsureFolder(ContentFolder + "/Categories");
                EnsureFolder(SceneFolder);

                EditorUtility.DisplayProgressBar("Sensori", "Textures de bois", 0.2f);
                var theme = CreateTheme();

                EditorUtility.DisplayProgressBar("Sensori", "Catalogue", 0.55f);
                var games = CreateGames();
                var categories = CreateCategories(games);
                var catalog = Upsert<ContentCatalog>(CatalogPath);
                catalog.Define(categories);
                EditorUtility.SetDirty(catalog);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                ConfigurePlayer();

                EditorUtility.ClearProgressBar();
                if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    EditorUtility.DisplayDialog(
                        "Sensori",
                        "Le catalogue et les textures sont enregistrés. La scène n'a pas été remplacée.",
                        "OK");
                    return;
                }

                EditorUtility.DisplayProgressBar("Sensori", "Scène", 0.8f);
                var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                CreateCamera();
                CreateLight();
                CreateEventSystem();

                var root = new GameObject("Sensori");
                MontessoriUiBuilder.Build(root, catalog, theme, games);
                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene, ScenePath);
                PromoteScene(ScenePath);
                Selection.activeGameObject = root;

                EditorUtility.ClearProgressBar();
                EditorUtility.DisplayDialog(
                    "Sensori",
                    "La scène Atelier est prête. Appuie sur Lecture.\n\nLes voyelles sont en bleu, les consonnes en rose. Relancer ce menu met à jour le contenu par défaut.",
                    "OK");
            }
            catch (Exception exception)
            {
                EditorUtility.ClearProgressBar();
                Debug.LogException(exception);
                EditorUtility.DisplayDialog("Sensori", "L'auto-setup s'est arrêté :\n\n" + exception.Message, "OK");
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        [MenuItem("Montessori/Réinitialiser la progression")]
        public static void ResetProgress()
        {
            var catalog = AssetDatabase.LoadAssetAtPath<ContentCatalog>(CatalogPath);
            if (catalog == null)
            {
                EditorUtility.DisplayDialog("Sensori", "Catalogue introuvable. Lance d'abord Montessori > Auto-Setup Scene.", "OK");
                return;
            }

            var categories = catalog.Categories;
            for (int i = 0; i < categories.Length; i++)
                LearningProgress.ResetCategory(categories[i]);
            EditorUtility.DisplayDialog("Sensori", "La progression de l'atelier a été remise à zéro.", "OK");
        }

        static ThemeAssets CreateTheme()
        {
            var panel = Opaque(MontessoriPalette.Maple);
            var inset = Opaque(MontessoriPalette.Walnut);
            var piece = Opaque(Color.Lerp(MontessoriPalette.Maple, MontessoriPalette.Cream, 0.42f));
            var border = new Vector4(40f, 40f, 40f, 40f);
            var pieceBorder = new Vector4(36f, 36f, 36f, 36f);

            return new ThemeAssets
            {
                Panel = SaveSprite("panel", WoodTextures.CreatePlate(256, 32, panel, false, 1f), border),
                Inset = SaveSprite("inset", WoodTextures.CreatePlate(256, 32, inset, true, 0.85f), border),
                Piece = SaveSprite("piece", WoodTextures.CreatePlate(256, 28, piece, false, 1f), pieceBorder),
                Shadow = SaveSprite("shadow", WoodTextures.CreateShadow(256), Vector4.zero),
                Pearl = SaveSprite("pearl", WoodTextures.CreatePearl(256), Vector4.zero),
                Chip = SaveSprite("chip", WoodTextures.CreateChip(128, 18), new Vector4(22f, 22f, 22f, 22f)),
                Solid = SaveSprite("solid", WoodTextures.CreateSolid(8), Vector4.zero),
                Background = SaveSprite("background", WoodTextures.CreateBackground(1024, 512), Vector4.zero),
                IconAlphabet = SaveSprite("icon-alphabet", PictogramPainter.CreateTexture("home-alphabet", 256), Vector4.zero),
                IconDigits = SaveSprite("icon-digits", PictogramPainter.CreateTexture("home-digits", 256), Vector4.zero),
                IconShapes = SaveSprite("icon-shapes", PictogramPainter.CreateTexture("home-shapes", 256), Vector4.zero),
                IconColors = SaveSprite("icon-colors", PictogramPainter.CreateTexture("home-colors", 256), Vector4.zero),
                IconPuzzle = SaveSprite("icon-puzzle", PictogramPainter.CreateTexture("game-puzzle", 256), Vector4.zero),
                IconImagier = SaveSprite("icon-imagier", PictogramPainter.CreateTexture("game-imagier", 256), Vector4.zero),
                IconTrace = SaveSprite("icon-trace", PictogramPainter.CreateTexture("game-trace", 256), Vector4.zero)
            };
        }

        static MiniGameDefinition[] CreateGames()
        {
            var seeds = DefaultCurriculum.Games();
            var games = new MiniGameDefinition[seeds.Length];
            for (int i = 0; i < seeds.Length; i++)
            {
                var seed = seeds[i];
                var asset = Upsert<MiniGameDefinition>(ContentFolder + "/Jeux/" + seed.Id + ".asset");
                asset.Define(seed.Id, seed.Title, seed.Description, seed.Accent);
                EditorUtility.SetDirty(asset);
                games[i] = asset;
            }
            return games;
        }

        static LearningCategory[] CreateCategories(MiniGameDefinition[] games)
        {
            var seeds = DefaultCurriculum.Categories();
            var categories = new LearningCategory[seeds.Length];
            for (int c = 0; c < seeds.Length; c++)
            {
                var seed = seeds[c];
                var items = new LearningItem[seed.Items.Length];
                for (int i = 0; i < seed.Items.Length; i++)
                {
                    var itemSeed = seed.Items[i];
                    var item = Upsert<LearningItem>(ContentFolder + "/Elements/" + itemSeed.Id + ".asset");
                    item.Define(
                        itemSeed.Id,
                        itemSeed.Symbol,
                        itemSeed.DisplayName,
                        itemSeed.Word,
                        itemSeed.Kind,
                        itemSeed.Role,
                        itemSeed.Visual,
                        itemSeed.Accent,
                        itemSeed.PictogramId,
                        StrokeLibrary.For(itemSeed.StrokeKey));
                    EditorUtility.SetDirty(item);
                    items[i] = item;
                }

                var category = Upsert<LearningCategory>(ContentFolder + "/Categories/" + seed.Id + ".asset");
                category.Define(
                    seed.Id,
                    seed.Title,
                    seed.Subtitle,
                    seed.CountLabel,
                    seed.Accent,
                    seed.PuzzleGroupSize,
                    items,
                    games);
                EditorUtility.SetDirty(category);
                categories[c] = category;
            }
            return categories;
        }

        static void CreateCamera()
        {
            var cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            var camera = cameraObject.AddComponent<Camera>();
            camera.orthographic = true;
            camera.orthographicSize = 5f;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = MontessoriPalette.Paper;
            camera.nearClipPlane = 0.3f;
            camera.farClipPlane = 1000f;
            cameraObject.transform.position = new Vector3(0f, 0f, -10f);
            cameraObject.AddComponent<AudioListener>();
            cameraObject.AddComponent<UniversalAdditionalCameraData>();
        }

        static void CreateLight()
        {
            var lightObject = new GameObject("Lumiere");
            var light = lightObject.AddComponent<Light2D>();
            light.lightType = Light2D.LightType.Global;
            light.intensity = 1f;
            light.color = Color.white;
        }

        static void CreateEventSystem()
        {
            var eventObject = new GameObject("EventSystem");
            var eventSystem = eventObject.AddComponent<EventSystem>();
            eventSystem.pixelDragThreshold = 8;
            eventSystem.sendNavigationEvents = false;

            var legacy = eventObject.GetComponent<StandaloneInputModule>();
            if (legacy != null)
                UnityEngine.Object.DestroyImmediate(legacy);

            var module = eventObject.GetComponent<InputSystemUIInputModule>();
            if (module == null)
                module = eventObject.AddComponent<InputSystemUIInputModule>();
            WireInput(module);
        }

        static void WireInput(InputSystemUIInputModule module)
        {
            module.enabled = false;
            var asset = AssetDatabase.LoadAssetAtPath<InputActionAsset>(ActionsPath);
            if (asset == null)
            {
                module.actionsAsset = null;
                module.enabled = true;
                Debug.LogWarning("Sensori : " + ActionsPath + " est introuvable. Les actions UI par défaut seront utilisées à la lecture.");
                return;
            }

            var references = AssetDatabase.LoadAllAssetsAtPath(ActionsPath)
                .OfType<InputActionReference>()
                .Where(reference => reference != null && (reference.hideFlags & HideFlags.HideInHierarchy) == 0)
                .ToArray();

            module.actionsAsset = asset;
            Assign(value => module.point = value, references, "Point");
            Assign(value => module.leftClick = value, references, "Click");
            Assign(value => module.rightClick = value, references, "RightClick");
            Assign(value => module.middleClick = value, references, "MiddleClick");
            Assign(value => module.scrollWheel = value, references, "ScrollWheel");
            Assign(value => module.move = value, references, "Navigate");
            Assign(value => module.submit = value, references, "Submit");
            Assign(value => module.cancel = value, references, "Cancel");
            Assign(value => module.trackedDevicePosition = value, references, "TrackedDevicePosition");
            Assign(value => module.trackedDeviceOrientation = value, references, "TrackedDeviceOrientation");

            if (module.point == null || module.point.action == null || module.leftClick == null || module.leftClick.action == null)
            {
                module.actionsAsset = null;
                module.point = null;
                module.leftClick = null;
                module.rightClick = null;
                module.middleClick = null;
                module.scrollWheel = null;
                module.move = null;
                module.submit = null;
                module.cancel = null;
                module.trackedDevicePosition = null;
                module.trackedDeviceOrientation = null;
                Debug.LogWarning("Sensori : actions Point ou Click introuvables. Les actions UI par défaut seront utilisées.");
            }

            module.enabled = true;
        }

        static void Assign(Action<InputActionReference> setter, InputActionReference[] references, string actionName)
        {
            setter(null);
            setter(FindUi(references, actionName));
        }

        static InputActionReference FindUi(InputActionReference[] references, string actionName)
        {
            for (int i = 0; i < references.Length; i++)
            {
                var action = references[i].action;
                if (action == null || action.actionMap == null)
                    continue;
                if (action.actionMap.name == "UI" && action.name == actionName)
                    return references[i];
            }
            return null;
        }

        static void ConfigurePlayer()
        {
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.AutoRotation;
            PlayerSettings.allowedAutorotateToLandscapeLeft = true;
            PlayerSettings.allowedAutorotateToLandscapeRight = true;
            PlayerSettings.allowedAutorotateToPortrait = false;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
        }

        static void PromoteScene(string scenePath)
        {
            var scenes = new List<EditorBuildSettingsScene>();
            var existing = EditorBuildSettings.scenes;
            for (int i = 0; i < existing.Length; i++)
            {
                if (existing[i].path != scenePath)
                    scenes.Add(existing[i]);
            }
            scenes.Insert(0, new EditorBuildSettingsScene(scenePath, true));
            EditorBuildSettings.scenes = scenes.ToArray();
        }

        static Sprite SaveSprite(string fileName, Texture2D texture, Vector4 border)
        {
            if (texture == null)
                throw new InvalidOperationException("Texture vide : " + fileName);

            string assetPath = ArtFolder + "/" + fileName + ".png";
            File.WriteAllBytes(Absolute(assetPath), texture.EncodeToPNG());
            UnityEngine.Object.DestroyImmediate(texture);
            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);

            var importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (importer == null)
                throw new InvalidOperationException("Import impossible : " + assetPath);

            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.mipmapEnabled = false;
            importer.alphaIsTransparency = true;
            importer.sRGBTexture = true;
            importer.alphaSource = TextureImporterAlphaSource.FromInput;
            importer.filterMode = FilterMode.Bilinear;
            importer.wrapMode = TextureWrapMode.Clamp;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.npotScale = TextureImporterNPOTScale.None;
            importer.isReadable = false;
            importer.spritePixelsPerUnit = 100f;
            importer.spriteBorder = border;

            var settings = new TextureImporterSettings();
            importer.ReadTextureSettings(settings);
            settings.spriteMeshType = SpriteMeshType.FullRect;
            settings.spriteExtrude = 1;
            settings.spriteGenerateFallbackPhysicsShape = false;
            settings.alphaIsTransparency = true;
            settings.mipmapEnabled = false;
            settings.readable = false;
            settings.spriteBorder = border;
            importer.SetTextureSettings(settings);
            importer.SaveAndReimport();

            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
            if (sprite != null)
                return sprite;

            var subAssets = AssetDatabase.LoadAllAssetsAtPath(assetPath);
            for (int i = 0; i < subAssets.Length; i++)
            {
                if (subAssets[i] is Sprite found)
                    return found;
            }

            throw new InvalidOperationException("Sprite introuvable : " + assetPath);
        }

        static T Upsert<T>(string assetPath) where T : ScriptableObject
        {
            var existing = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            if (existing != null)
                return existing;
            var created = ScriptableObject.CreateInstance<T>();
            created.name = Path.GetFileNameWithoutExtension(assetPath);
            AssetDatabase.CreateAsset(created, assetPath);
            return created;
        }

        static void EnsureFolder(string assetPath)
        {
            if (AssetDatabase.IsValidFolder(assetPath))
                return;
            string parent = Path.GetDirectoryName(assetPath);
            if (string.IsNullOrEmpty(parent))
                return;
            parent = parent.Replace('\\', '/');
            EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, Path.GetFileName(assetPath));
        }

        static string Absolute(string assetPath)
        {
            string relative = assetPath.StartsWith("Assets/", StringComparison.Ordinal)
                ? assetPath.Substring("Assets/".Length)
                : assetPath;
            return Path.Combine(Application.dataPath, relative);
        }

        static Color32 Opaque(Color color)
        {
            return new Color32(
                (byte)Mathf.RoundToInt(Mathf.Clamp01(color.r) * 255f),
                (byte)Mathf.RoundToInt(Mathf.Clamp01(color.g) * 255f),
                (byte)Mathf.RoundToInt(Mathf.Clamp01(color.b) * 255f),
                255);
        }
    }
}
