using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Sensori.Montessori.Editor
{
    public static class CategoryCanvasRestyle
    {
        const string ScenePath = "Assets/Montessori/Scenes/Atelier.unity";
        const string CatalogPath = "Assets/Montessori/Content/Catalogue.asset";

        [MenuItem("Montessori/Refonte Catégories")]
        public static void Apply()
        {
            var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            var catalog = AssetDatabase.LoadAssetAtPath<ContentCatalog>(CatalogPath);
            var presenters = Object.FindObjectsByType<CategoryPresenter>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            if (presenters == null || presenters.Length == 0)
            {
                Debug.LogError("[Montessori] CategoryPresenter introuvable dans Atelier.");
                return;
            }
            for (int i = 0; i < presenters.Length; i++)
                presenters[i].EnsureMenus(catalog, null, null);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("[Montessori] Canvas Catégories refondu.");
        }
    }
}
