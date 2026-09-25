using System.Collections.Generic;
using UnityEngine;

namespace Sensori.Montessori
{
    public static partial class PictogramPainter
    {
        static readonly Dictionary<string, Sprite> Sprites = new Dictionary<string, Sprite>();
        static readonly Color32 Cream = new Color32(255, 249, 242, 255);
        static readonly Color32 Ink = new Color32(61, 48, 40, 255);
        static readonly Color32 White = new Color32(255, 250, 245, 255);

        public static Sprite Get(string id)
        {
            if (string.IsNullOrEmpty(id))
                id = "badge";
            if (Sprites.TryGetValue(id, out var cached) && cached != null)
                return cached;
            var texture = CreateTexture(id, 256);
            var sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f);
            sprite.name = id;
            Sprites[id] = sprite;
            return sprite;
        }

        public static Sprite GetSilhouette(string id)
        {
            if (!SupportsSilhouette(id))
                return null;
            string key = id + "|silhouette";
            if (Sprites.TryGetValue(key, out var cached) && cached != null)
                return cached;
            var raster = new Raster(256);
            DrawSilhouette(raster, id);
            var texture = raster.ToTexture(key);
            var sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f);
            sprite.name = key;
            Sprites[key] = sprite;
            return sprite;
        }

        public static bool SupportsSilhouette(string id)
        {
            switch (id)
            {
                case "shape-circle":
                case "shape-square":
                case "shape-triangle":
                case "shape-rectangle":
                case "shape-star":
                case "shape-heart":
                case "shape-oval":
                case "shape-diamond":
                    return true;
                default:
                    return false;
            }
        }

        public static Texture2D CreateTexture(string id, int size)
        {
            var raster = new Raster(size);
            Draw(raster, id);
            return raster.ToTexture(string.IsNullOrEmpty(id) ? "pictogram" : id);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetCache()
        {
            foreach (var pair in Sprites)
            {
                var sprite = pair.Value;
                if (sprite == null)
                    continue;
                var texture = sprite.texture;
                Object.DestroyImmediate(sprite);
                if (texture != null)
                    Object.DestroyImmediate(texture);
            }
            Sprites.Clear();
        }

        static void Draw(Raster raster, string id)
        {
            switch (id)
            {
                case "letter-a": Avion(raster); return;
                case "letter-b": Bateau(raster); return;
                case "letter-c": Chat(raster); return;
                case "letter-d": Dauphin(raster); return;
                case "letter-e": Elephant(raster); return;
                case "letter-f": Fleur(raster); return;
                case "letter-g": Gateau(raster); return;
                case "letter-h": Hibou(raster); return;
                case "letter-i": Igloo(raster); return;
                case "letter-j": Jus(raster); return;
                case "letter-k": Koala(raster); return;
                case "letter-l": Lune(raster); return;
                case "letter-m": Maison(raster); return;
                case "letter-n": Nuage(raster); return;
                case "letter-o": Oiseau(raster); return;
                case "letter-p": Poisson(raster); return;
                case "letter-q": Quille(raster); return;
                case "letter-r": Robot(raster); return;
                case "letter-s": Soleil(raster); return;
                case "letter-t": Train(raster); return;
                case "letter-u": Uniforme(raster); return;
                case "letter-v": Voiture(raster); return;
                case "letter-w": Wagon(raster); return;
                case "letter-x": Xylophone(raster); return;
                case "letter-y": Yacht(raster); return;
                case "letter-z": Zebre(raster); return;
                case "digit-0": Oeuf(raster); return;
                case "digit-1": Bougie(raster); return;
                case "digit-2": DeuxBallons(raster); return;
                case "digit-3": TroisFleurs(raster); return;
                case "digit-4": Trefle(raster); return;
                case "digit-5": Main(raster); return;
                case "digit-6": De(raster); return;
                case "digit-7": Etoiles(raster); return;
                case "digit-8": Bonhomme(raster); return;
                case "digit-9": Bouquet(raster); return;
                case "shape-circle": Ballon(raster); return;
                case "shape-square": Cadeau(raster); return;
                case "shape-triangle": Montagne(raster); return;
                case "shape-rectangle": Livre(raster); return;
                case "shape-star": Etoile(raster); return;
                case "shape-heart": Coeur(raster); return;
                case "shape-oval": Oeuf(raster); return;
                case "shape-diamond": CerfVolant(raster); return;
                case "color-red": Pomme(raster); return;
                case "color-blue": Mer(raster); return;
                case "color-yellow": Soleil(raster); return;
                case "color-green": Feuille(raster); return;
                case "color-orange": OrangeFruit(raster); return;
                case "color-purple": Raisin(raster); return;
                case "color-pink": Fleur(raster); return;
                case "color-brown": Ours(raster); return;
                case "color-black": Nuit(raster); return;
                case "color-white": Nuage(raster); return;
                case "home-alphabet": HomeAlphabet(raster); return;
                case "home-digits": HomeDigits(raster); return;
                case "home-shapes": HomeShapes(raster); return;
                case "home-colors": HomeColors(raster); return;
                case "game-puzzle": GamePuzzle(raster); return;
                case "game-imagier": GameImagier(raster); return;
                case "game-trace": GameTrace(raster); return;
                case "home-parlant": HomeParlant(raster); return;
                default:
                    if (DrawWord(raster, id))
                        return;
                    Badge(raster, id);
                    return;
            }
        }

        static void DrawSilhouette(Raster raster, string id)
        {
            var white = new Color32(255, 255, 255, 255);
            switch (id)
            {
                case "shape-circle":
                    raster.Circle(0.5f, 0.5f, 0.36f, white);
                    break;
                case "shape-square":
                    raster.RoundRect(0.5f, 0.5f, 0.64f, 0.64f, 0.06f, white);
                    break;
                case "shape-triangle":
                    raster.Polygon(white, V(0.50f, 0.86f), V(0.88f, 0.16f), V(0.12f, 0.16f));
                    break;
                case "shape-rectangle":
                    raster.RoundRect(0.5f, 0.5f, 0.76f, 0.46f, 0.05f, white);
                    break;
                case "shape-star":
                    raster.Polygon(white, StarPoints(0.5f, 0.5f, 0.40f, 0.17f));
                    break;
                case "shape-heart":
                    HeartShape(raster, white);
                    break;
                case "shape-oval":
                    raster.Ellipse(0.5f, 0.5f, 0.28f, 0.40f, white);
                    break;
                case "shape-diamond":
                    raster.Polygon(white, V(0.50f, 0.90f), V(0.86f, 0.50f), V(0.50f, 0.10f), V(0.14f, 0.50f));
                    break;
            }
        }

        static void Paper(Raster raster, Color32 tint)
        {
            raster.Circle(0.5f, 0.5f, 0.47f, Cream);
            var wash = tint;
            wash.a = 48;
            raster.Circle(0.5f, 0.5f, 0.47f, wash);
        }

        static void Badge(Raster raster, string id)
        {
            var tint = ColorFromId(id);
            Paper(raster, tint);
            raster.Circle(0.5f, 0.5f, 0.18f, tint);
            raster.Circle(0.5f, 0.54f, 0.07f, White);
        }

        static void HomeAlphabet(Raster raster)
        {
            Paper(raster, new Color32(226, 75, 106, 255));
            raster.Stroke(new Color32(31, 111, 191, 255), 0.055f, V(0.22f, 0.28f), V(0.36f, 0.74f), V(0.50f, 0.28f));
            raster.Stroke(new Color32(31, 111, 191, 255), 0.04f, V(0.28f, 0.46f), V(0.44f, 0.46f));
            raster.Stroke(new Color32(226, 75, 106, 255), 0.05f, V(0.56f, 0.28f), V(0.56f, 0.74f), V(0.72f, 0.46f), V(0.88f, 0.74f), V(0.88f, 0.28f));
        }

        static void HomeDigits(Raster raster)
        {
            Paper(raster, new Color32(196, 146, 92, 255));
            raster.Circle(0.30f, 0.52f, 0.11f, new Color32(31, 111, 191, 255));
            raster.Circle(0.50f, 0.52f, 0.11f, new Color32(226, 176, 67, 255));
            raster.Circle(0.70f, 0.52f, 0.11f, new Color32(226, 75, 106, 255));
        }

        static void HomeShapes(Raster raster)
        {
            Paper(raster, new Color32(110, 154, 114, 255));
            raster.Circle(0.28f, 0.50f, 0.12f, new Color32(106, 143, 191, 255));
            raster.RoundRect(0.50f, 0.50f, 0.22f, 0.22f, 0.03f, new Color32(196, 146, 92, 255));
            raster.Polygon(new Color32(110, 154, 114, 255), V(0.72f, 0.36f), V(0.86f, 0.36f), V(0.79f, 0.66f));
        }

        static void HomeColors(Raster raster)
        {
            Paper(raster, new Color32(242, 193, 78, 255));
            raster.Circle(0.40f, 0.56f, 0.16f, new Color32(226, 75, 75, 180));
            raster.Circle(0.60f, 0.56f, 0.16f, new Color32(47, 116, 208, 180));
            raster.Circle(0.50f, 0.42f, 0.16f, new Color32(242, 193, 78, 190));
        }

        static void GamePuzzle(Raster raster)
        {
            Paper(raster, new Color32(196, 146, 92, 255));
            raster.RoundRect(0.50f, 0.34f, 0.62f, 0.16f, 0.05f, new Color32(92, 64, 42, 255));
            raster.RoundRect(0.36f, 0.62f, 0.22f, 0.22f, 0.05f, new Color32(226, 192, 154, 255));
            raster.RoundRect(0.64f, 0.64f, 0.22f, 0.22f, 0.05f, new Color32(196, 146, 92, 255));
        }

        static void GameImagier(Raster raster)
        {
            Paper(raster, new Color32(110, 154, 114, 255));
            raster.RoundRect(0.50f, 0.50f, 0.46f, 0.58f, 0.06f, White);
            HeartShape(raster, new Color32(226, 75, 106, 255), 0.50f, 0.52f, 0.62f);
        }

        static void HomeParlant(Raster raster)
        {
            Paper(raster, new Color32(110, 154, 114, 255));
            raster.RoundRect(0.46f, 0.48f, 0.42f, 0.52f, 0.06f, White);
            raster.Circle(0.46f, 0.58f, 0.08f, new Color32(196, 146, 92, 255));
            raster.Polygon(new Color32(196, 146, 92, 255), V(0.34f, 0.66f), V(0.42f, 0.66f), V(0.38f, 0.78f));
            raster.Polygon(new Color32(196, 146, 92, 255), V(0.50f, 0.66f), V(0.58f, 0.66f), V(0.54f, 0.78f));
            raster.Stroke(new Color32(106, 143, 191, 255), 0.035f, V(0.72f, 0.62f), V(0.84f, 0.62f));
            raster.Stroke(new Color32(106, 143, 191, 255), 0.028f, V(0.74f, 0.50f), V(0.88f, 0.50f));
            raster.Stroke(new Color32(106, 143, 191, 255), 0.022f, V(0.76f, 0.38f), V(0.90f, 0.38f));
        }

        static void GameTrace(Raster raster)
        {
            Paper(raster, new Color32(106, 143, 191, 255));
            raster.Stroke(new Color32(31, 111, 191, 255), 0.055f, V(0.22f, 0.30f), V(0.38f, 0.62f), V(0.58f, 0.40f), V(0.78f, 0.72f));
            raster.Circle(0.78f, 0.72f, 0.06f, new Color32(255, 249, 242, 255));
        }

        static void HeartShape(Raster raster, Color32 color, float cx = 0.5f, float cy = 0.48f, float scale = 1f)
        {
            raster.Circle(cx - 0.10f * scale, cy + 0.08f * scale, 0.12f * scale, color);
            raster.Circle(cx + 0.10f * scale, cy + 0.08f * scale, 0.12f * scale, color);
            raster.Polygon(color,
                V(cx - 0.20f * scale, cy + 0.04f * scale),
                V(cx + 0.20f * scale, cy + 0.04f * scale),
                V(cx, cy - 0.22f * scale));
        }

        static Vector2[] StarPoints(float cx, float cy, float outer, float inner)
        {
            var points = new Vector2[10];
            for (int i = 0; i < 10; i++)
            {
                float angle = (90f - i * 36f) * Mathf.Deg2Rad;
                float radius = (i % 2 == 0) ? outer : inner;
                points[i] = new Vector2(cx + Mathf.Cos(angle) * radius, cy + Mathf.Sin(angle) * radius);
            }
            return points;
        }

        static Vector2 V(float x, float y)
        {
            return new Vector2(x, y);
        }

        static Color32 Rgb(byte r, byte g, byte b, byte a = 255)
        {
            return new Color32(r, g, b, a);
        }

        static Color32 ColorFromId(string id)
        {
            int hash = 17;
            if (!string.IsNullOrEmpty(id))
            {
                for (int i = 0; i < id.Length; i++)
                    hash = hash * 31 + id[i];
            }
            return new Color32(
                (byte)(150 + (hash & 55)),
                (byte)(120 + ((hash >> 5) & 55)),
                (byte)(110 + ((hash >> 10) & 55)),
                255);
        }
    }
}
