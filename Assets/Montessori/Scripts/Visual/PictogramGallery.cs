using UnityEngine;

namespace Sensori.Montessori
{
    public static partial class PictogramPainter
    {
        static void Avion(Raster raster)
        {
            Paper(raster, Rgb(126, 176, 214));
            raster.RoundRect(0.48f, 0.50f, 0.52f, 0.10f, 0.05f, White);
            raster.Polygon(Rgb(47, 116, 208), V(0.34f, 0.54f), V(0.62f, 0.54f), V(0.46f, 0.74f));
            raster.Polygon(Rgb(31, 90, 170), V(0.46f, 0.46f), V(0.68f, 0.46f), V(0.58f, 0.30f));
            raster.Polygon(Rgb(47, 116, 208), V(0.20f, 0.54f), V(0.34f, 0.54f), V(0.24f, 0.68f));
            raster.Circle(0.58f, 0.52f, 0.025f, Ink);
        }

        static void Bateau(Raster raster)
        {
            Paper(raster, Rgb(126, 176, 214));
            raster.Ellipse(0.50f, 0.28f, 0.32f, 0.06f, Rgb(47, 116, 208));
            raster.Polygon(Rgb(196, 146, 92), V(0.22f, 0.40f), V(0.78f, 0.40f), V(0.68f, 0.28f), V(0.32f, 0.28f));
            raster.Stroke(Ink, 0.015f, V(0.48f, 0.40f), V(0.48f, 0.78f));
            raster.Polygon(White, V(0.48f, 0.74f), V(0.48f, 0.44f), V(0.72f, 0.50f));
        }

        static void Chat(Raster raster)
        {
            Paper(raster, Rgb(242, 193, 140));
            raster.Polygon(Rgb(196, 146, 92), V(0.28f, 0.62f), V(0.38f, 0.62f), V(0.34f, 0.80f));
            raster.Polygon(Rgb(196, 146, 92), V(0.62f, 0.62f), V(0.72f, 0.62f), V(0.66f, 0.80f));
            raster.Circle(0.50f, 0.48f, 0.22f, Rgb(214, 164, 110));
            raster.Circle(0.42f, 0.52f, 0.035f, Ink);
            raster.Circle(0.58f, 0.52f, 0.035f, Ink);
            raster.Polygon(Rgb(226, 120, 130), V(0.50f, 0.46f), V(0.46f, 0.40f), V(0.54f, 0.40f));
            raster.Stroke(Ink, 0.012f, V(0.18f, 0.50f), V(0.36f, 0.48f));
            raster.Stroke(Ink, 0.012f, V(0.82f, 0.50f), V(0.64f, 0.48f));
        }

        static void Dauphin(Raster raster)
        {
            Paper(raster, Rgb(126, 190, 214));
            raster.Ellipse(0.52f, 0.48f, 0.28f, 0.12f, Rgb(70, 150, 190), -18f);
            raster.Polygon(Rgb(70, 150, 190), V(0.22f, 0.50f), V(0.10f, 0.64f), V(0.18f, 0.40f));
            raster.Polygon(Rgb(90, 170, 200), V(0.48f, 0.56f), V(0.58f, 0.56f), V(0.50f, 0.72f));
            raster.Circle(0.70f, 0.52f, 0.02f, Ink);
            raster.Ellipse(0.50f, 0.26f, 0.30f, 0.04f, Rgb(47, 116, 208, 160));
        }

        static void Elephant(Raster raster)
        {
            Paper(raster, Rgb(190, 198, 206));
            raster.Circle(0.42f, 0.58f, 0.16f, Rgb(168, 176, 186));
            raster.Circle(0.56f, 0.50f, 0.20f, Rgb(150, 160, 172));
            raster.Ellipse(0.34f, 0.52f, 0.10f, 0.14f, Rgb(168, 176, 186));
            raster.Stroke(Rgb(130, 140, 152), 0.045f, V(0.62f, 0.42f), V(0.70f, 0.28f), V(0.62f, 0.22f));
            raster.Circle(0.50f, 0.56f, 0.02f, Ink);
        }

        static void Fleur(Raster raster)
        {
            Paper(raster, Rgb(242, 180, 196));
            raster.Stroke(Rgb(90, 150, 90), 0.02f, V(0.50f, 0.18f), V(0.50f, 0.48f));
            raster.Ellipse(0.50f, 0.30f, 0.08f, 0.04f, Rgb(90, 150, 90), 40f);
            raster.Circle(0.50f, 0.66f, 0.09f, Rgb(242, 160, 181));
            raster.Circle(0.36f, 0.56f, 0.09f, Rgb(226, 120, 150));
            raster.Circle(0.64f, 0.56f, 0.09f, Rgb(226, 120, 150));
            raster.Circle(0.40f, 0.74f, 0.09f, Rgb(242, 180, 196));
            raster.Circle(0.60f, 0.74f, 0.09f, Rgb(242, 180, 196));
            raster.Circle(0.50f, 0.62f, 0.055f, Rgb(242, 193, 78));
        }

        static void Gateau(Raster raster)
        {
            Paper(raster, Rgb(255, 220, 210));
            raster.Polygon(Rgb(240, 170, 190), V(0.22f, 0.42f), V(0.78f, 0.42f), V(0.70f, 0.28f), V(0.30f, 0.28f));
            raster.Polygon(Rgb(196, 146, 92), V(0.26f, 0.58f), V(0.74f, 0.58f), V(0.78f, 0.42f), V(0.22f, 0.42f));
            raster.Polygon(Rgb(255, 248, 240), V(0.30f, 0.70f), V(0.70f, 0.70f), V(0.74f, 0.58f), V(0.26f, 0.58f));
            raster.Circle(0.50f, 0.74f, 0.045f, Rgb(226, 70, 80));
            raster.Stroke(Rgb(120, 170, 210), 0.012f, V(0.50f, 0.78f), V(0.50f, 0.88f));
        }

        static void Hibou(Raster raster)
        {
            Paper(raster, Rgb(210, 190, 160));
            raster.Ellipse(0.50f, 0.42f, 0.22f, 0.26f, Rgb(150, 110, 72));
            raster.Circle(0.50f, 0.66f, 0.16f, Rgb(130, 96, 64));
            raster.Polygon(Rgb(110, 80, 52), V(0.36f, 0.74f), V(0.42f, 0.74f), V(0.34f, 0.88f));
            raster.Polygon(Rgb(110, 80, 52), V(0.58f, 0.74f), V(0.64f, 0.74f), V(0.66f, 0.88f));
            raster.Circle(0.43f, 0.68f, 0.055f, White);
            raster.Circle(0.57f, 0.68f, 0.055f, White);
            raster.Circle(0.43f, 0.68f, 0.025f, Ink);
            raster.Circle(0.57f, 0.68f, 0.025f, Ink);
            raster.Polygon(Rgb(242, 193, 78), V(0.50f, 0.64f), V(0.46f, 0.58f), V(0.54f, 0.58f));
        }

        static void Igloo(Raster raster)
        {
            Paper(raster, Rgb(186, 214, 230));
            raster.Ellipse(0.50f, 0.22f, 0.34f, 0.05f, White);
            raster.Circle(0.50f, 0.46f, 0.28f, White);
            raster.RoundRect(0.50f, 0.18f, 0.70f, 0.22f, 0.02f, Cream);
            raster.RoundRect(0.50f, 0.30f, 0.12f, 0.16f, 0.04f, Rgb(126, 176, 214));
            raster.Stroke(Rgb(210, 224, 232), 0.012f, V(0.24f, 0.46f), V(0.76f, 0.46f));
        }

        static void Jus(Raster raster)
        {
            Paper(raster, Rgb(255, 214, 170));
            raster.Polygon(Rgb(255, 248, 240, 230), V(0.36f, 0.72f), V(0.64f, 0.72f), V(0.58f, 0.28f), V(0.42f, 0.28f));
            raster.Polygon(Rgb(240, 150, 50), V(0.40f, 0.62f), V(0.60f, 0.62f), V(0.56f, 0.30f), V(0.44f, 0.30f));
            raster.Stroke(Rgb(120, 170, 90), 0.018f, V(0.58f, 0.70f), V(0.70f, 0.86f));
            raster.Circle(0.28f, 0.36f, 0.10f, Rgb(240, 138, 44));
        }

        static void Koala(Raster raster)
        {
            Paper(raster, Rgb(210, 214, 216));
            raster.Circle(0.32f, 0.70f, 0.10f, Rgb(140, 144, 148));
            raster.Circle(0.68f, 0.70f, 0.10f, Rgb(140, 144, 148));
            raster.Circle(0.50f, 0.50f, 0.22f, Rgb(160, 164, 168));
            raster.Ellipse(0.50f, 0.42f, 0.10f, 0.08f, Rgb(90, 94, 98));
            raster.Circle(0.42f, 0.56f, 0.02f, Ink);
            raster.Circle(0.58f, 0.56f, 0.02f, Ink);
        }

        static void Lune(Raster raster)
        {
            Paper(raster, Rgb(48, 64, 110));
            raster.Circle(0.46f, 0.52f, 0.24f, Rgb(242, 214, 120));
            raster.Circle(0.58f, 0.60f, 0.18f, Cream);
            raster.Circle(0.24f, 0.74f, 0.015f, White);
            raster.Circle(0.72f, 0.78f, 0.02f, White);
            raster.Circle(0.30f, 0.30f, 0.012f, White);
        }

        static void Maison(Raster raster)
        {
            Paper(raster, Rgb(255, 228, 196));
            raster.Polygon(Rgb(196, 90, 80), V(0.16f, 0.52f), V(0.84f, 0.52f), V(0.50f, 0.84f));
            raster.RoundRect(0.50f, 0.36f, 0.46f, 0.36f, 0.02f, Rgb(240, 220, 190));
            raster.RoundRect(0.50f, 0.28f, 0.12f, 0.18f, 0.02f, Rgb(120, 78, 52));
            raster.RoundRect(0.34f, 0.42f, 0.10f, 0.10f, 0.02f, Rgb(126, 176, 214));
        }

        static void Nuage(Raster raster)
        {
            Paper(raster, Rgb(186, 214, 232));
            raster.Circle(0.38f, 0.46f, 0.14f, White);
            raster.Circle(0.54f, 0.52f, 0.18f, White);
            raster.Circle(0.68f, 0.46f, 0.12f, White);
            raster.RoundRect(0.52f, 0.40f, 0.42f, 0.12f, 0.06f, White);
        }

        static void Oiseau(Raster raster)
        {
            Paper(raster, Rgb(186, 220, 236));
            raster.Ellipse(0.48f, 0.50f, 0.20f, 0.12f, Rgb(70, 140, 190));
            raster.Polygon(Rgb(47, 116, 208), V(0.40f, 0.56f), V(0.62f, 0.62f), V(0.46f, 0.72f));
            raster.Polygon(Rgb(242, 193, 78), V(0.66f, 0.52f), V(0.82f, 0.56f), V(0.66f, 0.46f));
            raster.Circle(0.60f, 0.54f, 0.018f, Ink);
            raster.Polygon(Rgb(70, 140, 190), V(0.22f, 0.52f), V(0.12f, 0.62f), V(0.20f, 0.44f));
        }

        static void Poisson(Raster raster)
        {
            Paper(raster, Rgb(170, 214, 220));
            raster.Ellipse(0.48f, 0.50f, 0.24f, 0.14f, Rgb(240, 138, 80));
            raster.Polygon(Rgb(226, 110, 70), V(0.20f, 0.50f), V(0.08f, 0.68f), V(0.08f, 0.32f));
            raster.Circle(0.62f, 0.54f, 0.025f, White);
            raster.Circle(0.62f, 0.54f, 0.012f, Ink);
            raster.Polygon(Rgb(255, 180, 120), V(0.46f, 0.62f), V(0.58f, 0.62f), V(0.50f, 0.74f));
        }

        static void Quille(Raster raster)
        {
            Paper(raster, Rgb(230, 214, 190));
            raster.Polygon(White, V(0.40f, 0.22f), V(0.60f, 0.22f), V(0.56f, 0.48f), V(0.44f, 0.48f));
            raster.Circle(0.50f, 0.60f, 0.10f, White);
            raster.RoundRect(0.50f, 0.46f, 0.08f, 0.08f, 0.02f, Rgb(226, 75, 106));
            raster.Ellipse(0.50f, 0.18f, 0.16f, 0.035f, Rgb(180, 160, 140));
        }

        static void Robot(Raster raster)
        {
            Paper(raster, Rgb(210, 224, 232));
            raster.RoundRect(0.50f, 0.62f, 0.28f, 0.22f, 0.04f, Rgb(140, 168, 186));
            raster.Stroke(Ink, 0.015f, V(0.50f, 0.74f), V(0.50f, 0.86f));
            raster.Circle(0.50f, 0.88f, 0.03f, Rgb(226, 75, 106));
            raster.RoundRect(0.50f, 0.38f, 0.32f, 0.28f, 0.04f, Rgb(176, 196, 208));
            raster.Circle(0.42f, 0.44f, 0.035f, Rgb(47, 116, 208));
            raster.Circle(0.58f, 0.44f, 0.035f, Rgb(47, 116, 208));
            raster.RoundRect(0.42f, 0.22f, 0.06f, 0.12f, 0.02f, Rgb(120, 140, 156));
            raster.RoundRect(0.58f, 0.22f, 0.06f, 0.12f, 0.02f, Rgb(120, 140, 156));
        }

        static void Soleil(Raster raster)
        {
            Paper(raster, Rgb(255, 236, 186));
            for (int i = 0; i < 8; i++)
            {
                float angle = i * 45f;
                raster.RoundRect(0.50f, 0.50f, 0.08f, 0.22f, 0.03f, Rgb(242, 193, 78), angle);
            }
            raster.Circle(0.50f, 0.50f, 0.16f, Rgb(255, 214, 90));
            raster.Circle(0.50f, 0.54f, 0.06f, Rgb(255, 236, 180));
        }

        static void Train(Raster raster)
        {
            Paper(raster, Rgb(230, 236, 240));
            raster.RoundRect(0.42f, 0.42f, 0.46f, 0.22f, 0.04f, Rgb(70, 120, 170));
            raster.RoundRect(0.62f, 0.58f, 0.22f, 0.18f, 0.03f, Rgb(47, 96, 150));
            raster.RoundRect(0.62f, 0.60f, 0.10f, 0.08f, 0.02f, Rgb(186, 220, 236));
            raster.Circle(0.30f, 0.28f, 0.06f, Ink);
            raster.Circle(0.48f, 0.28f, 0.06f, Ink);
            raster.Circle(0.66f, 0.28f, 0.06f, Ink);
            raster.Circle(0.72f, 0.72f, 0.04f, Rgb(200, 206, 210, 180));
        }

        static void Uniforme(Raster raster)
        {
            Paper(raster, Rgb(214, 226, 236));
            raster.Polygon(Rgb(47, 96, 160), V(0.28f, 0.62f), V(0.18f, 0.40f), V(0.34f, 0.42f), V(0.50f, 0.58f), V(0.66f, 0.42f), V(0.82f, 0.40f), V(0.72f, 0.62f));
            raster.RoundRect(0.50f, 0.36f, 0.36f, 0.28f, 0.04f, Rgb(47, 116, 208));
            raster.Polygon(White, V(0.42f, 0.62f), V(0.50f, 0.48f), V(0.58f, 0.62f));
            raster.Circle(0.50f, 0.34f, 0.025f, Rgb(242, 193, 78));
        }

        static void Voiture(Raster raster)
        {
            Paper(raster, Rgb(255, 228, 210));
            raster.RoundRect(0.50f, 0.36f, 0.62f, 0.16f, 0.06f, Rgb(226, 75, 106));
            raster.RoundRect(0.50f, 0.52f, 0.36f, 0.16f, 0.06f, Rgb(226, 100, 120));
            raster.RoundRect(0.40f, 0.54f, 0.12f, 0.08f, 0.02f, Rgb(186, 220, 236));
            raster.RoundRect(0.60f, 0.54f, 0.12f, 0.08f, 0.02f, Rgb(186, 220, 236));
            raster.Circle(0.32f, 0.26f, 0.055f, Ink);
            raster.Circle(0.68f, 0.26f, 0.055f, Ink);
        }

        static void Wagon(Raster raster)
        {
            Paper(raster, Rgb(236, 228, 214));
            raster.RoundRect(0.50f, 0.46f, 0.62f, 0.32f, 0.04f, Rgb(176, 96, 64));
            raster.RoundRect(0.36f, 0.50f, 0.12f, 0.12f, 0.02f, Rgb(255, 236, 200));
            raster.RoundRect(0.64f, 0.50f, 0.12f, 0.12f, 0.02f, Rgb(255, 236, 200));
            raster.Circle(0.34f, 0.24f, 0.055f, Ink);
            raster.Circle(0.66f, 0.24f, 0.055f, Ink);
        }

        static void Xylophone(Raster raster)
        {
            Paper(raster, Rgb(255, 236, 220));
            Color32[] bars =
            {
                Rgb(226, 75, 75),
                Rgb(240, 138, 44),
                Rgb(242, 193, 78),
                Rgb(62, 154, 86),
                Rgb(47, 116, 208)
            };
            for (int i = 0; i < bars.Length; i++)
            {
                float y = 0.30f + i * 0.09f;
                float width = 0.62f - i * 0.06f;
                raster.RoundRect(0.48f, y, width, 0.055f, 0.02f, bars[i]);
            }
            raster.Stroke(Rgb(120, 78, 52), 0.02f, V(0.70f, 0.24f), V(0.82f, 0.42f));
            raster.Circle(0.84f, 0.44f, 0.03f, Rgb(196, 146, 92));
        }

        static void Yacht(Raster raster)
        {
            Paper(raster, Rgb(170, 210, 230));
            raster.Ellipse(0.50f, 0.24f, 0.34f, 0.05f, Rgb(47, 116, 208));
            raster.Polygon(Rgb(240, 244, 248), V(0.28f, 0.36f), V(0.74f, 0.36f), V(0.62f, 0.28f), V(0.36f, 0.28f));
            raster.Stroke(Ink, 0.014f, V(0.50f, 0.36f), V(0.50f, 0.82f));
            raster.Polygon(White, V(0.50f, 0.78f), V(0.50f, 0.42f), V(0.74f, 0.48f));
            raster.Polygon(Rgb(226, 75, 106), V(0.50f, 0.74f), V(0.50f, 0.48f), V(0.32f, 0.52f));
        }

        static void Zebre(Raster raster)
        {
            Paper(raster, Rgb(230, 236, 220));
            raster.Ellipse(0.48f, 0.46f, 0.26f, 0.14f, White);
            raster.RoundRect(0.36f, 0.48f, 0.03f, 0.16f, 0.01f, Ink);
            raster.RoundRect(0.46f, 0.48f, 0.03f, 0.18f, 0.01f, Ink);
            raster.RoundRect(0.56f, 0.48f, 0.03f, 0.16f, 0.01f, Ink);
            raster.Circle(0.72f, 0.56f, 0.09f, White);
            raster.RoundRect(0.74f, 0.62f, 0.02f, 0.08f, 0.01f, Ink);
            raster.Circle(0.76f, 0.60f, 0.015f, Ink);
            raster.RoundRect(0.38f, 0.28f, 0.04f, 0.12f, 0.015f, Ink);
            raster.RoundRect(0.52f, 0.28f, 0.04f, 0.12f, 0.015f, Ink);
            raster.Polygon(Ink, V(0.66f, 0.62f), V(0.78f, 0.62f), V(0.74f, 0.74f));
        }

        static void Oeuf(Raster raster)
        {
            Paper(raster, Rgb(255, 236, 214));
            raster.Ellipse(0.50f, 0.48f, 0.20f, 0.28f, White);
            raster.Ellipse(0.46f, 0.58f, 0.06f, 0.10f, Rgb(255, 248, 240));
        }

        static void Bougie(Raster raster)
        {
            Paper(raster, Rgb(255, 228, 196));
            raster.RoundRect(0.50f, 0.40f, 0.16f, 0.40f, 0.03f, Rgb(255, 214, 170));
            raster.Ellipse(0.50f, 0.64f, 0.07f, 0.12f, Rgb(240, 138, 44));
            raster.Ellipse(0.50f, 0.68f, 0.03f, 0.06f, Rgb(255, 220, 120));
            raster.Stroke(Ink, 0.01f, V(0.50f, 0.60f), V(0.50f, 0.64f));
        }

        static void DeuxBallons(Raster raster)
        {
            Paper(raster, Rgb(230, 240, 248));
            raster.Circle(0.38f, 0.60f, 0.14f, Rgb(226, 75, 106));
            raster.Circle(0.62f, 0.58f, 0.14f, Rgb(47, 116, 208));
            raster.Stroke(Ink, 0.01f, V(0.38f, 0.46f), V(0.36f, 0.22f));
            raster.Stroke(Ink, 0.01f, V(0.62f, 0.44f), V(0.66f, 0.20f));
        }

        static void TroisFleurs(Raster raster)
        {
            Paper(raster, Rgb(230, 242, 220));
            MiniFleur(raster, 0.30f, 0.42f, Rgb(226, 75, 106));
            MiniFleur(raster, 0.50f, 0.58f, Rgb(242, 193, 78));
            MiniFleur(raster, 0.70f, 0.42f, Rgb(123, 94, 167));
        }

        static void MiniFleur(Raster raster, float x, float y, Color32 color)
        {
            raster.Stroke(Rgb(90, 150, 90), 0.012f, V(x, y - 0.16f), V(x, y));
            raster.Circle(x, y + 0.06f, 0.045f, color);
            raster.Circle(x - 0.06f, y, 0.04f, color);
            raster.Circle(x + 0.06f, y, 0.04f, color);
            raster.Circle(x, y, 0.025f, Rgb(242, 193, 78));
        }

        static void Trefle(Raster raster)
        {
            Paper(raster, Rgb(220, 236, 210));
            raster.Circle(0.50f, 0.62f, 0.10f, Rgb(62, 154, 86));
            raster.Circle(0.38f, 0.48f, 0.10f, Rgb(62, 154, 86));
            raster.Circle(0.62f, 0.48f, 0.10f, Rgb(62, 154, 86));
            raster.Circle(0.50f, 0.40f, 0.10f, Rgb(46, 130, 70));
            raster.Stroke(Rgb(40, 110, 60), 0.018f, V(0.50f, 0.32f), V(0.50f, 0.16f));
        }

        static void Main(Raster raster)
        {
            Paper(raster, Rgb(255, 228, 210));
            raster.Circle(0.50f, 0.36f, 0.16f, Rgb(255, 214, 186));
            raster.RoundRect(0.34f, 0.58f, 0.07f, 0.22f, 0.03f, Rgb(255, 206, 176));
            raster.RoundRect(0.44f, 0.64f, 0.07f, 0.26f, 0.03f, Rgb(255, 206, 176));
            raster.RoundRect(0.54f, 0.66f, 0.07f, 0.28f, 0.03f, Rgb(255, 206, 176));
            raster.RoundRect(0.64f, 0.60f, 0.07f, 0.22f, 0.03f, Rgb(255, 206, 176));
            raster.RoundRect(0.28f, 0.42f, 0.08f, 0.16f, 0.03f, Rgb(255, 206, 176), 28f);
        }

        static void De(Raster raster)
        {
            Paper(raster, Rgb(230, 236, 242));
            raster.RoundRect(0.50f, 0.50f, 0.46f, 0.46f, 0.08f, White);
            raster.Circle(0.36f, 0.64f, 0.035f, Ink);
            raster.Circle(0.50f, 0.64f, 0.035f, Ink);
            raster.Circle(0.64f, 0.64f, 0.035f, Ink);
            raster.Circle(0.36f, 0.36f, 0.035f, Ink);
            raster.Circle(0.50f, 0.36f, 0.035f, Ink);
            raster.Circle(0.64f, 0.36f, 0.035f, Ink);
        }

        static void Etoiles(Raster raster)
        {
            Paper(raster, Rgb(48, 58, 110));
            float[] xs = { 0.28f, 0.50f, 0.72f, 0.36f, 0.64f, 0.42f, 0.58f };
            float[] ys = { 0.70f, 0.78f, 0.68f, 0.52f, 0.50f, 0.34f, 0.30f };
            for (int i = 0; i < xs.Length; i++)
                raster.Polygon(Rgb(255, 220, 120), StarPoints(xs[i], ys[i], 0.07f, 0.03f));
        }

        static void Bonhomme(Raster raster)
        {
            Paper(raster, Rgb(210, 228, 236));
            raster.Circle(0.50f, 0.34f, 0.16f, White);
            raster.Circle(0.50f, 0.62f, 0.12f, White);
            raster.Circle(0.46f, 0.66f, 0.015f, Ink);
            raster.Circle(0.54f, 0.66f, 0.015f, Ink);
            raster.Polygon(Rgb(240, 140, 70), V(0.50f, 0.62f), V(0.47f, 0.58f), V(0.53f, 0.58f));
            raster.RoundRect(0.50f, 0.74f, 0.10f, 0.04f, 0.02f, Rgb(47, 116, 208));
        }

        static void Bouquet(Raster raster)
        {
            Paper(raster, Rgb(255, 236, 230));
            Color32[] colors =
            {
                Rgb(226, 75, 106), Rgb(242, 193, 78), Rgb(123, 94, 167),
                Rgb(240, 138, 44), Rgb(47, 116, 208), Rgb(242, 160, 181),
                Rgb(62, 154, 86), Rgb(226, 120, 150), Rgb(255, 214, 90)
            };
            for (int i = 0; i < colors.Length; i++)
            {
                float column = i % 3;
                float row = i / 3;
                float x = 0.34f + column * 0.16f;
                float y = 0.36f + row * 0.16f;
                raster.Circle(x, y, 0.055f, colors[i]);
            }
            raster.Stroke(Rgb(90, 150, 90), 0.016f, V(0.50f, 0.30f), V(0.50f, 0.16f));
        }

        static void Ballon(Raster raster)
        {
            Paper(raster, Rgb(255, 236, 220));
            raster.Circle(0.50f, 0.58f, 0.22f, Rgb(226, 75, 106));
            raster.Ellipse(0.44f, 0.66f, 0.06f, 0.08f, Rgb(255, 220, 220, 160));
            raster.Polygon(Rgb(196, 146, 92), V(0.46f, 0.36f), V(0.54f, 0.36f), V(0.50f, 0.30f));
            raster.Stroke(Ink, 0.012f, V(0.50f, 0.30f), V(0.48f, 0.14f));
        }

        static void Cadeau(Raster raster)
        {
            Paper(raster, Rgb(255, 236, 220));
            raster.RoundRect(0.50f, 0.40f, 0.46f, 0.36f, 0.04f, Rgb(226, 75, 106));
            raster.RoundRect(0.50f, 0.64f, 0.50f, 0.12f, 0.03f, Rgb(240, 100, 120));
            raster.RoundRect(0.50f, 0.48f, 0.08f, 0.56f, 0.02f, Rgb(242, 193, 78));
            raster.RoundRect(0.50f, 0.64f, 0.50f, 0.06f, 0.02f, Rgb(242, 193, 78));
        }

        static void Montagne(Raster raster)
        {
            Paper(raster, Rgb(186, 220, 236));
            raster.Polygon(Rgb(120, 150, 130), V(0.08f, 0.28f), V(0.40f, 0.28f), V(0.24f, 0.62f));
            raster.Polygon(Rgb(90, 140, 120), V(0.28f, 0.24f), V(0.92f, 0.24f), V(0.60f, 0.82f));
            raster.Polygon(White, V(0.52f, 0.64f), V(0.68f, 0.64f), V(0.60f, 0.82f));
            raster.Circle(0.78f, 0.74f, 0.06f, Rgb(255, 214, 90));
        }

        static void Livre(Raster raster)
        {
            Paper(raster, Rgb(255, 236, 214));
            raster.RoundRect(0.42f, 0.48f, 0.28f, 0.42f, 0.03f, Rgb(196, 90, 80));
            raster.RoundRect(0.60f, 0.48f, 0.28f, 0.42f, 0.03f, Rgb(226, 120, 100));
            raster.Stroke(Rgb(255, 236, 214), 0.01f, V(0.34f, 0.62f), V(0.34f, 0.34f));
            raster.Stroke(Rgb(255, 236, 214), 0.01f, V(0.70f, 0.62f), V(0.70f, 0.34f));
        }

        static void Etoile(Raster raster)
        {
            Paper(raster, Rgb(48, 64, 120));
            raster.Polygon(Rgb(255, 214, 90), StarPoints(0.50f, 0.50f, 0.32f, 0.14f));
            raster.Circle(0.50f, 0.52f, 0.05f, Rgb(255, 236, 180));
        }

        static void Coeur(Raster raster)
        {
            Paper(raster, Rgb(255, 220, 226));
            HeartShape(raster, Rgb(226, 75, 106));
            raster.Circle(0.40f, 0.58f, 0.03f, Rgb(255, 220, 220, 180));
        }

        static void CerfVolant(Raster raster)
        {
            Paper(raster, Rgb(210, 230, 245));
            raster.Polygon(Rgb(47, 116, 208), V(0.50f, 0.82f), V(0.78f, 0.52f), V(0.50f, 0.22f), V(0.22f, 0.52f));
            raster.Polygon(Rgb(242, 193, 78), V(0.50f, 0.82f), V(0.50f, 0.22f), V(0.22f, 0.52f));
            raster.Stroke(Ink, 0.012f, V(0.50f, 0.22f), V(0.62f, 0.10f));
            raster.Polygon(Rgb(226, 75, 106), V(0.58f, 0.16f), V(0.70f, 0.12f), V(0.62f, 0.08f));
        }

        static void Pomme(Raster raster)
        {
            Paper(raster, Rgb(255, 228, 220));
            raster.Circle(0.46f, 0.46f, 0.16f, Rgb(226, 60, 60));
            raster.Circle(0.58f, 0.48f, 0.15f, Rgb(210, 50, 55));
            raster.Ellipse(0.42f, 0.56f, 0.04f, 0.07f, Rgb(255, 180, 180, 140));
            raster.Stroke(Rgb(90, 130, 70), 0.016f, V(0.52f, 0.60f), V(0.56f, 0.78f));
            raster.Ellipse(0.60f, 0.70f, 0.06f, 0.03f, Rgb(90, 150, 80), 30f);
        }

        static void Mer(Raster raster)
        {
            Paper(raster, Rgb(186, 220, 236));
            raster.Ellipse(0.50f, 0.34f, 0.36f, 0.08f, Rgb(47, 116, 208));
            raster.Ellipse(0.42f, 0.48f, 0.30f, 0.07f, Rgb(70, 150, 210));
            raster.Ellipse(0.58f, 0.60f, 0.32f, 0.07f, Rgb(47, 140, 200));
            raster.Circle(0.74f, 0.78f, 0.06f, Rgb(255, 214, 90));
        }

        static void Feuille(Raster raster)
        {
            Paper(raster, Rgb(226, 240, 214));
            raster.Ellipse(0.50f, 0.52f, 0.16f, 0.28f, Rgb(62, 154, 86), 20f);
            raster.Stroke(Rgb(30, 110, 60), 0.012f, V(0.42f, 0.30f), V(0.58f, 0.74f));
        }

        static void OrangeFruit(Raster raster)
        {
            Paper(raster, Rgb(255, 228, 196));
            raster.Circle(0.50f, 0.48f, 0.22f, Rgb(240, 138, 44));
            raster.Ellipse(0.44f, 0.56f, 0.06f, 0.08f, Rgb(255, 190, 110, 130));
            raster.Ellipse(0.50f, 0.72f, 0.05f, 0.03f, Rgb(70, 140, 70));
        }

        static void Raisin(Raster raster)
        {
            Paper(raster, Rgb(236, 226, 242));
            float[] xs = { 0.42f, 0.58f, 0.36f, 0.50f, 0.64f, 0.42f, 0.58f, 0.50f };
            float[] ys = { 0.64f, 0.64f, 0.52f, 0.52f, 0.52f, 0.40f, 0.40f, 0.30f };
            for (int i = 0; i < xs.Length; i++)
                raster.Circle(xs[i], ys[i], 0.07f, Rgb(123, 80, 160));
            raster.Stroke(Rgb(70, 120, 70), 0.014f, V(0.50f, 0.70f), V(0.56f, 0.84f));
        }

        static void Ours(Raster raster)
        {
            Paper(raster, Rgb(236, 220, 200));
            raster.Circle(0.32f, 0.70f, 0.09f, Rgb(138, 90, 59));
            raster.Circle(0.68f, 0.70f, 0.09f, Rgb(138, 90, 59));
            raster.Circle(0.50f, 0.50f, 0.22f, Rgb(160, 108, 70));
            raster.Ellipse(0.50f, 0.40f, 0.10f, 0.07f, Rgb(210, 170, 130));
            raster.Circle(0.46f, 0.42f, 0.015f, Ink);
            raster.Circle(0.54f, 0.42f, 0.015f, Ink);
            raster.Circle(0.42f, 0.56f, 0.02f, Ink);
            raster.Circle(0.58f, 0.56f, 0.02f, Ink);
        }

        static void Nuit(Raster raster)
        {
            Paper(raster, Rgb(36, 42, 72));
            raster.Circle(0.58f, 0.56f, 0.16f, Rgb(255, 220, 140));
            raster.Circle(0.66f, 0.62f, 0.13f, Rgb(36, 42, 72));
            raster.Polygon(Rgb(255, 220, 140), StarPoints(0.28f, 0.72f, 0.06f, 0.025f));
            raster.Circle(0.24f, 0.40f, 0.012f, White);
            raster.Circle(0.40f, 0.30f, 0.01f, White);
        }
    }
}
