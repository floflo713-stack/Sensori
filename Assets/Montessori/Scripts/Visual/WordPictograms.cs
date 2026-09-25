using UnityEngine;

namespace Sensori.Montessori
{
    public static partial class PictogramPainter
    {
        static bool DrawWord(Raster raster, string id)
        {
            switch (id)
            {
                case "abricot": Abricot(raster); return true;
                case "ami": Ami(raster); return true;
                case "arbre": Arbre(raster); return true;
                case "avion": Avion(raster); return true;
                case "ballon": Ballon(raster); return true;
                case "banane": Banane(raster); return true;
                case "bateau": Bateau(raster); return true;
                case "bébé": Bebe(raster); return true;
                case "cadeau": Cadeau(raster); return true;
                case "chat": Chat(raster); return true;
                case "ciel": Ciel(raster); return true;
                case "cœur": Coeur(raster); return true;
                case "danse": Danse(raster); return true;
                case "dents": Dents(raster); return true;
                case "dodo": Dodo(raster); return true;
                case "voiture-police": VoiturePolice(raster); return true;
                case "dauphin": Dauphin(raster); return true;
                case "doudou": Doudou(raster); return true;
                case "dragon": Dragon(raster); return true;
                case "eau": Eau(raster); return true;
                case "école": Ecole(raster); return true;
                case "éléphant": Elephant(raster); return true;
                case "étoile": Etoile(raster); return true;
                case "famille": Famille(raster); return true;
                case "fleur": Fleur(raster); return true;
                case "forêt": Foret(raster); return true;
                case "fraise": Fraise(raster); return true;
                case "gâteau": Gateau(raster); return true;
                case "girafe": Girafe(raster); return true;
                case "glace": Glace(raster); return true;
                case "grenouille": Grenouille(raster); return true;
                case "hamac": Hamac(raster); return true;
                case "hélicoptère": Helicoptere(raster); return true;
                case "hérisson": Herisson(raster); return true;
                case "hibou": Hibou(raster); return true;
                case "igloo": Igloo(raster); return true;
                case "île": Ile(raster); return true;
                case "insecte": Insecte(raster); return true;
                case "iris": Iris(raster); return true;
                case "jardin": Jardin(raster); return true;
                case "jouet": Jouet(raster); return true;
                case "jupe": Jupe(raster); return true;
                case "jus": Jus(raster); return true;
                case "kangourou": Kangourou(raster); return true;
                case "kayak": Kayak(raster); return true;
                case "kiwi": Kiwi(raster); return true;
                case "koala": Koala(raster); return true;
                case "lait": Lait(raster); return true;
                case "lapin": Lapin(raster); return true;
                case "livre": Livre(raster); return true;
                case "lune": Lune(raster); return true;
                case "maison": Maison(raster); return true;
                case "maman": Maman(raster); return true;
                case "miel": Miel(raster); return true;
                case "mouton": Mouton(raster); return true;
                case "neige": Neige(raster); return true;
                case "nid": Nid(raster); return true;
                case "nounours": Nounours(raster); return true;
                case "nuage": Nuage(raster); return true;
                case "oiseau": Oiseau(raster); return true;
                case "orange": OrangeFruit(raster); return true;
                case "oreille": Oreille(raster); return true;
                case "ours": Ours(raster); return true;
                case "pain": Pain(raster); return true;
                case "papa": Papa(raster); return true;
                case "poisson": Poisson(raster); return true;
                case "pomme": Pomme(raster); return true;
                case "quatre": Quatre(raster); return true;
                case "queue": Queue(raster); return true;
                case "quille": Quille(raster); return true;
                case "quinze": Quinze(raster); return true;
                case "raisin": Raisin(raster); return true;
                case "rivière": Riviere(raster); return true;
                case "robe": Robe(raster); return true;
                case "robot": Robot(raster); return true;
                case "sable": Sable(raster); return true;
                case "sapin": Sapin(raster); return true;
                case "sirène": Sirene(raster); return true;
                case "soleil": Soleil(raster); return true;
                case "table": Table(raster); return true;
                case "tigre": Tigre(raster); return true;
                case "tomate": Tomate(raster); return true;
                case "train": Train(raster); return true;
                case "ukulélé": Ukulélé(raster); return true;
                case "uniforme": Uniforme(raster); return true;
                case "univers": Univers(raster); return true;
                case "usine": Usine(raster); return true;
                case "vache": Vache(raster); return true;
                case "vélo": Velo(raster); return true;
                case "vent": Vent(raster); return true;
                case "voiture": Voiture(raster); return true;
                case "wagon": Wagon(raster); return true;
                case "wallaby": Wallaby(raster); return true;
                case "wombat": Wombat(raster); return true;
                case "xylophone": Xylophone(raster); return true;
                case "xérus": Xerus(raster); return true;
                case "yacht": Yacht(raster); return true;
                case "yaourt": Yaourt(raster); return true;
                case "yoga": Yoga(raster); return true;
                case "yoyo": Yoyo(raster); return true;
                case "zèbre": Zebre(raster); return true;
                case "zéro": Zero(raster); return true;
                case "zigzag": Zigzag(raster); return true;
                case "zoo": Zoo(raster); return true;
                default: return false;
            }
        }

        static void VoiturePolice(Raster raster)
        {
            Paper(raster, Rgb(214, 228, 242));
            raster.RoundRect(0.50f, 0.34f, 0.70f, 0.18f, 0.05f, White);
            raster.RoundRect(0.50f, 0.34f, 0.70f, 0.07f, 0.02f, Rgb(31, 92, 196));
            raster.RoundRect(0.48f, 0.52f, 0.40f, 0.16f, 0.05f, White);
            raster.RoundRect(0.38f, 0.54f, 0.13f, 0.09f, 0.02f, Rgb(186, 220, 236));
            raster.RoundRect(0.58f, 0.54f, 0.13f, 0.09f, 0.02f, Rgb(186, 220, 236));
            raster.RoundRect(0.50f, 0.66f, 0.28f, 0.06f, 0.02f, Rgb(40, 48, 64));
            raster.Circle(0.40f, 0.68f, 0.035f, Rgb(226, 48, 58));
            raster.Circle(0.60f, 0.68f, 0.035f, Rgb(47, 116, 208));
            raster.Circle(0.30f, 0.22f, 0.065f, Ink);
            raster.Circle(0.30f, 0.22f, 0.028f, Rgb(210, 210, 210));
            raster.Circle(0.70f, 0.22f, 0.065f, Ink);
            raster.Circle(0.70f, 0.22f, 0.028f, Rgb(210, 210, 210));
        }

        static void Dents(Raster raster)
        {
            Paper(raster, Rgb(186, 220, 232));
            raster.RoundRect(0.50f, 0.42f, 0.46f, 0.16f, 0.06f, White);
            raster.Circle(0.32f, 0.46f, 0.045f, White);
            raster.Circle(0.44f, 0.50f, 0.045f, White);
            raster.Circle(0.56f, 0.50f, 0.045f, White);
            raster.Circle(0.68f, 0.46f, 0.045f, White);
            raster.RoundRect(0.50f, 0.28f, 0.08f, 0.22f, 0.03f, Rgb(120, 168, 210));
            raster.Ellipse(0.50f, 0.62f, 0.16f, 0.08f, Rgb(244, 196, 160));
        }

        static void Dodo(Raster raster)
        {
            Paper(raster, Rgb(186, 176, 214));
            raster.Ellipse(0.50f, 0.28f, 0.28f, 0.08f, Rgb(226, 214, 196));
            raster.Circle(0.42f, 0.48f, 0.14f, Rgb(244, 214, 186));
            raster.Ellipse(0.62f, 0.40f, 0.16f, 0.10f, Rgb(226, 120, 140));
            raster.Circle(0.38f, 0.50f, 0.02f, Ink);
            raster.Circle(0.70f, 0.72f, 0.04f, Rgb(242, 193, 78));
            raster.Circle(0.80f, 0.64f, 0.025f, Rgb(242, 193, 78));
        }

        static void Abricot(Raster raster)
        {
            Paper(raster, Rgb(255, 220, 186));
            raster.Circle(0.50f, 0.46f, 0.22f, Rgb(232, 140, 64));
            raster.Ellipse(0.42f, 0.52f, 0.07f, 0.09f, Rgb(255, 186, 120, 140));
            raster.Stroke(Rgb(186, 96, 48), 0.012f, V(0.50f, 0.66f), V(0.50f, 0.50f));
            raster.Ellipse(0.58f, 0.70f, 0.07f, 0.035f, Rgb(70, 140, 70), 30f);
        }

        static void Ami(Raster raster)
        {
            Paper(raster, Rgb(255, 228, 210));
            Smile(raster, 0.36f, 0.50f, 0.14f, Rgb(244, 196, 160));
            Smile(raster, 0.64f, 0.50f, 0.14f, Rgb(210, 150, 110));
        }

        static void Arbre(Raster raster)
        {
            Paper(raster, Rgb(214, 232, 196));
            raster.RoundRect(0.50f, 0.28f, 0.10f, 0.28f, 0.02f, Rgb(138, 90, 52));
            raster.Circle(0.50f, 0.58f, 0.20f, Rgb(62, 150, 78));
            raster.Circle(0.36f, 0.50f, 0.12f, Rgb(46, 130, 64));
            raster.Circle(0.64f, 0.50f, 0.12f, Rgb(80, 168, 90));
        }

        static void Banane(Raster raster)
        {
            Paper(raster, Rgb(255, 236, 186));
            raster.Stroke(Rgb(232, 186, 48), 0.09f, V(0.32f, 0.28f), V(0.42f, 0.48f), V(0.62f, 0.62f), V(0.74f, 0.48f));
            raster.Stroke(Rgb(90, 60, 30), 0.02f, V(0.30f, 0.26f), V(0.36f, 0.34f));
        }

        static void Bebe(Raster raster)
        {
            Paper(raster, Rgb(255, 228, 220));
            raster.Circle(0.50f, 0.58f, 0.16f, Rgb(255, 214, 196));
            raster.Circle(0.44f, 0.62f, 0.016f, Ink);
            raster.Circle(0.56f, 0.62f, 0.016f, Ink);
            raster.Ellipse(0.50f, 0.54f, 0.03f, 0.016f, Rgb(226, 140, 150));
            raster.Ellipse(0.50f, 0.32f, 0.16f, 0.12f, Rgb(242, 220, 230));
        }

        static void Ciel(Raster raster)
        {
            Paper(raster, Rgb(150, 196, 230));
            raster.Circle(0.30f, 0.66f, 0.10f, Rgb(242, 193, 78));
            raster.Circle(0.58f, 0.48f, 0.12f, White);
            raster.Circle(0.70f, 0.44f, 0.08f, White);
            raster.Circle(0.48f, 0.42f, 0.08f, White);
        }

        static void Danse(Raster raster)
        {
            Paper(raster, Rgb(255, 220, 210));
            raster.Circle(0.52f, 0.74f, 0.07f, Rgb(244, 196, 160));
            raster.Stroke(Rgb(80, 90, 140), 0.03f, V(0.52f, 0.66f), V(0.48f, 0.42f));
            raster.Stroke(Rgb(80, 90, 140), 0.025f, V(0.48f, 0.58f), V(0.30f, 0.66f));
            raster.Stroke(Rgb(80, 90, 140), 0.025f, V(0.50f, 0.56f), V(0.70f, 0.62f));
            raster.Polygon(Rgb(226, 100, 130), V(0.40f, 0.44f), V(0.62f, 0.44f), V(0.70f, 0.22f), V(0.36f, 0.22f));
            raster.Stroke(Rgb(80, 90, 140), 0.022f, V(0.46f, 0.40f), V(0.34f, 0.22f));
            raster.Stroke(Rgb(80, 90, 140), 0.022f, V(0.54f, 0.40f), V(0.68f, 0.24f));
        }

        static void Doudou(Raster raster)
        {
            Paper(raster, Rgb(236, 220, 200));
            raster.Circle(0.34f, 0.68f, 0.08f, Rgb(176, 120, 80));
            raster.Circle(0.66f, 0.68f, 0.08f, Rgb(176, 120, 80));
            raster.Circle(0.50f, 0.48f, 0.20f, Rgb(196, 140, 96));
            raster.Circle(0.43f, 0.52f, 0.02f, Ink);
            raster.Circle(0.57f, 0.52f, 0.02f, Ink);
            raster.Ellipse(0.50f, 0.44f, 0.04f, 0.025f, Rgb(120, 70, 50));
        }

        static void Dragon(Raster raster)
        {
            Paper(raster, Rgb(210, 230, 200));
            raster.Ellipse(0.48f, 0.42f, 0.22f, 0.14f, Rgb(70, 150, 90));
            raster.Circle(0.70f, 0.56f, 0.12f, Rgb(60, 130, 80));
            raster.Polygon(Rgb(60, 130, 80), V(0.62f, 0.64f), V(0.70f, 0.64f), V(0.60f, 0.80f));
            raster.Polygon(Rgb(90, 170, 110), V(0.40f, 0.52f), V(0.55f, 0.52f), V(0.30f, 0.74f));
            raster.Circle(0.76f, 0.60f, 0.015f, Ink);
            raster.Polygon(Rgb(240, 120, 60), V(0.82f, 0.54f), V(0.92f, 0.50f), V(0.82f, 0.46f));
        }

        static void Eau(Raster raster)
        {
            Paper(raster, Rgb(186, 220, 236));
            raster.Polygon(Rgb(47, 140, 200), V(0.50f, 0.82f), V(0.28f, 0.36f), V(0.72f, 0.36f));
            raster.Circle(0.50f, 0.34f, 0.16f, Rgb(47, 140, 200));
            raster.Ellipse(0.44f, 0.48f, 0.04f, 0.08f, White);
        }

        static void Ecole(Raster raster)
        {
            Paper(raster, Rgb(255, 228, 196));
            raster.Polygon(Rgb(180, 70, 70), V(0.16f, 0.55f), V(0.84f, 0.55f), V(0.50f, 0.82f));
            raster.RoundRect(0.50f, 0.38f, 0.50f, 0.34f, 0.02f, Rgb(245, 230, 200));
            raster.RoundRect(0.50f, 0.30f, 0.12f, 0.16f, 0.02f, Rgb(120, 78, 52));
            raster.Stroke(Rgb(80, 80, 90), 0.012f, V(0.72f, 0.70f), V(0.72f, 0.88f));
            raster.Polygon(Rgb(47, 116, 208), V(0.72f, 0.86f), V(0.86f, 0.82f), V(0.72f, 0.76f));
        }

        static void Famille(Raster raster)
        {
            Paper(raster, Rgb(255, 230, 210));
            Person(raster, 0.30f, 0.42f, 0.9f, Rgb(80, 120, 170));
            Person(raster, 0.68f, 0.44f, 0.95f, Rgb(210, 100, 120));
            Person(raster, 0.50f, 0.30f, 0.62f, Rgb(240, 180, 80));
        }

        static void Foret(Raster raster)
        {
            Paper(raster, Rgb(200, 224, 190));
            Tree(raster, 0.32f, 0.9f);
            Tree(raster, 0.55f, 1.15f);
            Tree(raster, 0.74f, 0.8f);
        }

        static void Fraise(Raster raster)
        {
            Paper(raster, Rgb(255, 220, 220));
            raster.Polygon(Rgb(210, 50, 70), V(0.50f, 0.74f), V(0.28f, 0.48f), V(0.36f, 0.26f), V(0.64f, 0.26f), V(0.72f, 0.48f));
            raster.Ellipse(0.50f, 0.76f, 0.08f, 0.035f, Rgb(60, 140, 70));
            raster.Circle(0.42f, 0.50f, 0.012f, Rgb(255, 220, 80));
            raster.Circle(0.54f, 0.42f, 0.012f, Rgb(255, 220, 80));
            raster.Circle(0.48f, 0.58f, 0.012f, Rgb(255, 220, 80));
        }

        static void Girafe(Raster raster)
        {
            Paper(raster, Rgb(255, 230, 190));
            raster.RoundRect(0.46f, 0.42f, 0.08f, 0.46f, 0.03f, Rgb(230, 170, 70));
            raster.Circle(0.48f, 0.72f, 0.08f, Rgb(230, 170, 70));
            raster.Polygon(Rgb(210, 140, 50), V(0.42f, 0.76f), V(0.46f, 0.76f), V(0.40f, 0.88f));
            raster.Polygon(Rgb(210, 140, 50), V(0.52f, 0.76f), V(0.56f, 0.76f), V(0.58f, 0.88f));
            raster.Circle(0.52f, 0.74f, 0.012f, Ink);
            raster.Circle(0.42f, 0.50f, 0.018f, Rgb(160, 90, 40));
            raster.Circle(0.48f, 0.36f, 0.016f, Rgb(160, 90, 40));
            raster.Stroke(Rgb(160, 90, 40), 0.02f, V(0.40f, 0.22f), V(0.34f, 0.12f));
            raster.Stroke(Rgb(160, 90, 40), 0.02f, V(0.52f, 0.22f), V(0.58f, 0.12f));
        }

        static void Glace(Raster raster)
        {
            Paper(raster, Rgb(255, 230, 220));
            raster.Polygon(Rgb(196, 140, 80), V(0.42f, 0.48f), V(0.58f, 0.48f), V(0.50f, 0.18f));
            raster.Circle(0.50f, 0.58f, 0.12f, Rgb(242, 160, 181));
            raster.Circle(0.50f, 0.70f, 0.09f, Rgb(255, 248, 240));
            raster.Circle(0.50f, 0.78f, 0.035f, Rgb(210, 50, 70));
        }

        static void Grenouille(Raster raster)
        {
            Paper(raster, Rgb(200, 230, 190));
            raster.Ellipse(0.50f, 0.40f, 0.22f, 0.14f, Rgb(70, 160, 80));
            raster.Circle(0.36f, 0.62f, 0.09f, Rgb(70, 160, 80));
            raster.Circle(0.64f, 0.62f, 0.09f, Rgb(70, 160, 80));
            raster.Circle(0.36f, 0.64f, 0.035f, White);
            raster.Circle(0.64f, 0.64f, 0.035f, White);
            raster.Circle(0.36f, 0.64f, 0.016f, Ink);
            raster.Circle(0.64f, 0.64f, 0.016f, Ink);
            raster.Ellipse(0.50f, 0.46f, 0.06f, 0.03f, Rgb(240, 120, 140));
        }

        static void Hamac(Raster raster)
        {
            Paper(raster, Rgb(210, 230, 210));
            raster.RoundRect(0.22f, 0.48f, 0.06f, 0.50f, 0.02f, Rgb(120, 80, 50));
            raster.RoundRect(0.78f, 0.48f, 0.06f, 0.50f, 0.02f, Rgb(120, 80, 50));
            raster.Stroke(Rgb(200, 90, 80), 0.03f, V(0.24f, 0.62f), V(0.50f, 0.42f), V(0.76f, 0.62f));
            raster.Stroke(Rgb(180, 70, 70), 0.02f, V(0.32f, 0.52f), V(0.68f, 0.52f));
        }

        static void Helicoptere(Raster raster)
        {
            Paper(raster, Rgb(210, 225, 235));
            raster.Ellipse(0.48f, 0.46f, 0.22f, 0.10f, Rgb(70, 120, 170));
            raster.Circle(0.66f, 0.50f, 0.07f, Rgb(180, 210, 230));
            raster.Stroke(Ink, 0.015f, V(0.48f, 0.56f), V(0.48f, 0.72f));
            raster.RoundRect(0.48f, 0.74f, 0.46f, 0.025f, 0.01f, Ink);
            raster.Stroke(Rgb(70, 120, 170), 0.02f, V(0.30f, 0.40f), V(0.22f, 0.28f));
        }

        static void Herisson(Raster raster)
        {
            Paper(raster, Rgb(236, 220, 190));
            raster.Ellipse(0.50f, 0.42f, 0.22f, 0.14f, Rgb(150, 100, 60));
            raster.Circle(0.70f, 0.48f, 0.08f, Rgb(190, 140, 100));
            raster.Circle(0.74f, 0.52f, 0.012f, Ink);
            raster.Circle(0.78f, 0.46f, 0.012f, Rgb(180, 60, 70));
            for (int i = 0; i < 6; i++)
                raster.Stroke(Rgb(90, 60, 40), 0.012f, V(0.30f + i * 0.06f, 0.50f), V(0.26f + i * 0.06f, 0.68f));
        }

        static void Ile(Raster raster)
        {
            Paper(raster, Rgb(140, 196, 220));
            raster.Ellipse(0.50f, 0.34f, 0.28f, 0.08f, Rgb(226, 190, 110));
            raster.Stroke(Rgb(120, 80, 40), 0.02f, V(0.50f, 0.40f), V(0.50f, 0.72f));
            raster.Ellipse(0.58f, 0.66f, 0.10f, 0.05f, Rgb(60, 150, 80), 40f);
            raster.Ellipse(0.42f, 0.62f, 0.08f, 0.04f, Rgb(50, 130, 70), -30f);
        }

        static void Insecte(Raster raster)
        {
            Paper(raster, Rgb(230, 240, 220));
            raster.Ellipse(0.38f, 0.58f, 0.12f, 0.16f, Rgb(180, 210, 240, 180));
            raster.Ellipse(0.62f, 0.58f, 0.12f, 0.16f, Rgb(180, 210, 240, 180));
            raster.Ellipse(0.50f, 0.42f, 0.10f, 0.14f, Rgb(40, 40, 48));
            raster.Circle(0.50f, 0.60f, 0.06f, Rgb(40, 40, 48));
            raster.Stroke(Ink, 0.01f, V(0.46f, 0.66f), V(0.40f, 0.78f));
            raster.Stroke(Ink, 0.01f, V(0.54f, 0.66f), V(0.60f, 0.78f));
        }

        static void Iris(Raster raster)
        {
            Paper(raster, Rgb(230, 220, 242));
            raster.Stroke(Rgb(60, 130, 70), 0.016f, V(0.50f, 0.16f), V(0.50f, 0.48f));
            raster.Ellipse(0.50f, 0.34f, 0.06f, 0.03f, Rgb(60, 140, 70), 50f);
            raster.Polygon(Rgb(110, 70, 170), V(0.50f, 0.78f), V(0.34f, 0.58f), V(0.50f, 0.52f), V(0.66f, 0.58f));
            raster.Polygon(Rgb(150, 100, 200), V(0.50f, 0.50f), V(0.38f, 0.40f), V(0.62f, 0.40f));
            raster.Circle(0.50f, 0.56f, 0.03f, Rgb(242, 193, 78));
        }

        static void Jardin(Raster raster)
        {
            Paper(raster, Rgb(210, 230, 190));
            raster.Ellipse(0.50f, 0.28f, 0.34f, 0.08f, Rgb(90, 150, 80));
            Bloom(raster, 0.32f, 0.55f, Rgb(226, 90, 110));
            Bloom(raster, 0.52f, 0.64f, Rgb(242, 193, 78));
            Bloom(raster, 0.70f, 0.52f, Rgb(120, 140, 210));
        }

        static void Jouet(Raster raster)
        {
            Paper(raster, Rgb(255, 230, 210));
            raster.RoundRect(0.40f, 0.36f, 0.22f, 0.22f, 0.03f, Rgb(220, 70, 80));
            raster.RoundRect(0.62f, 0.40f, 0.20f, 0.20f, 0.03f, Rgb(50, 120, 190));
            raster.RoundRect(0.50f, 0.60f, 0.22f, 0.22f, 0.03f, Rgb(240, 180, 50));
        }

        static void Jupe(Raster raster)
        {
            Paper(raster, Rgb(255, 228, 230));
            raster.Polygon(Rgb(210, 80, 120), V(0.38f, 0.70f), V(0.62f, 0.70f), V(0.78f, 0.28f), V(0.22f, 0.28f));
            raster.RoundRect(0.50f, 0.74f, 0.16f, 0.08f, 0.02f, Rgb(240, 180, 190));
        }

        static void Kangourou(Raster raster)
        {
            Paper(raster, Rgb(236, 220, 190));
            raster.Ellipse(0.48f, 0.40f, 0.16f, 0.20f, Rgb(176, 120, 70));
            raster.Circle(0.62f, 0.66f, 0.09f, Rgb(176, 120, 70));
            raster.Polygon(Rgb(160, 100, 60), V(0.56f, 0.70f), V(0.62f, 0.70f), V(0.66f, 0.86f));
            raster.Ellipse(0.44f, 0.36f, 0.08f, 0.07f, Rgb(210, 170, 130));
            raster.Circle(0.30f, 0.40f, 0.045f, Rgb(200, 150, 110));
            raster.Stroke(Rgb(140, 90, 50), 0.025f, V(0.36f, 0.24f), V(0.28f, 0.12f));
            raster.Stroke(Rgb(140, 90, 50), 0.03f, V(0.58f, 0.28f), V(0.74f, 0.18f));
            raster.Circle(0.66f, 0.68f, 0.012f, Ink);
        }

        static void Kayak(Raster raster)
        {
            Paper(raster, Rgb(170, 210, 220));
            raster.Ellipse(0.50f, 0.42f, 0.32f, 0.07f, Rgb(210, 80, 70));
            raster.Circle(0.50f, 0.50f, 0.05f, Rgb(244, 196, 160));
            raster.Stroke(Rgb(120, 80, 40), 0.018f, V(0.28f, 0.62f), V(0.72f, 0.30f));
            raster.Ellipse(0.50f, 0.28f, 0.28f, 0.04f, Rgb(47, 116, 208));
        }

        static void Kiwi(Raster raster)
        {
            Paper(raster, Rgb(230, 236, 210));
            raster.Circle(0.50f, 0.48f, 0.22f, Rgb(120, 78, 48));
            raster.Circle(0.50f, 0.48f, 0.16f, Rgb(150, 190, 70));
            raster.Circle(0.50f, 0.48f, 0.04f, White);
            raster.Circle(0.42f, 0.54f, 0.012f, Ink);
            raster.Circle(0.58f, 0.54f, 0.012f, Ink);
            raster.Circle(0.46f, 0.42f, 0.012f, Ink);
            raster.Circle(0.56f, 0.42f, 0.012f, Ink);
        }

        static void Lait(Raster raster)
        {
            Paper(raster, Rgb(230, 240, 245));
            raster.Polygon(White, V(0.38f, 0.72f), V(0.62f, 0.72f), V(0.58f, 0.28f), V(0.42f, 0.28f));
            raster.Ellipse(0.50f, 0.74f, 0.14f, 0.05f, Rgb(47, 116, 208));
            raster.RoundRect(0.50f, 0.48f, 0.10f, 0.16f, 0.02f, Rgb(186, 214, 232));
        }

        static void Lapin(Raster raster)
        {
            Paper(raster, Rgb(255, 236, 230));
            raster.Ellipse(0.40f, 0.72f, 0.05f, 0.14f, White);
            raster.Ellipse(0.60f, 0.72f, 0.05f, 0.14f, White);
            raster.Ellipse(0.40f, 0.72f, 0.025f, 0.08f, Rgb(244, 180, 190));
            raster.Ellipse(0.60f, 0.72f, 0.025f, 0.08f, Rgb(244, 180, 190));
            raster.Circle(0.50f, 0.46f, 0.16f, White);
            raster.Circle(0.44f, 0.50f, 0.016f, Ink);
            raster.Circle(0.56f, 0.50f, 0.016f, Ink);
            raster.Circle(0.50f, 0.44f, 0.018f, Rgb(240, 150, 160));
        }

        static void Maman(Raster raster)
        {
            Paper(raster, Rgb(255, 228, 220));
            raster.Circle(0.50f, 0.66f, 0.12f, Rgb(120, 70, 48));
            raster.Circle(0.50f, 0.58f, 0.10f, Rgb(244, 196, 160));
            raster.Polygon(Rgb(190, 80, 110), V(0.34f, 0.46f), V(0.66f, 0.46f), V(0.60f, 0.18f), V(0.40f, 0.18f));
            raster.Circle(0.46f, 0.60f, 0.012f, Ink);
            raster.Circle(0.54f, 0.60f, 0.012f, Ink);
        }

        static void Miel(Raster raster)
        {
            Paper(raster, Rgb(255, 230, 180));
            raster.RoundRect(0.50f, 0.42f, 0.28f, 0.32f, 0.06f, Rgb(210, 140, 40));
            raster.Ellipse(0.50f, 0.60f, 0.16f, 0.06f, Rgb(240, 180, 60));
            raster.RoundRect(0.50f, 0.66f, 0.16f, 0.06f, 0.02f, Rgb(120, 78, 40));
            raster.Circle(0.62f, 0.74f, 0.04f, Rgb(240, 180, 40));
        }

        static void Mouton(Raster raster)
        {
            Paper(raster, Rgb(230, 240, 230));
            raster.Circle(0.40f, 0.48f, 0.12f, White);
            raster.Circle(0.55f, 0.54f, 0.13f, White);
            raster.Circle(0.66f, 0.46f, 0.10f, White);
            raster.Circle(0.48f, 0.40f, 0.10f, White);
            raster.Circle(0.72f, 0.50f, 0.07f, Rgb(80, 70, 64));
            raster.Circle(0.75f, 0.52f, 0.012f, White);
            raster.Stroke(Rgb(80, 70, 64), 0.02f, V(0.42f, 0.30f), V(0.42f, 0.16f));
            raster.Stroke(Rgb(80, 70, 64), 0.02f, V(0.56f, 0.30f), V(0.56f, 0.16f));
        }

        static void Neige(Raster raster)
        {
            Paper(raster, Rgb(210, 228, 240));
            raster.Stroke(Rgb(120, 170, 210), 0.02f, V(0.50f, 0.18f), V(0.50f, 0.82f));
            raster.Stroke(Rgb(120, 170, 210), 0.02f, V(0.22f, 0.34f), V(0.78f, 0.66f));
            raster.Stroke(Rgb(120, 170, 210), 0.02f, V(0.22f, 0.66f), V(0.78f, 0.34f));
            raster.Circle(0.50f, 0.50f, 0.04f, White);
        }

        static void Nid(Raster raster)
        {
            Paper(raster, Rgb(236, 220, 190));
            raster.Ellipse(0.50f, 0.40f, 0.26f, 0.12f, Rgb(150, 100, 60));
            raster.Ellipse(0.50f, 0.44f, 0.16f, 0.07f, Rgb(210, 170, 120));
            raster.Ellipse(0.44f, 0.46f, 0.045f, 0.055f, White);
            raster.Ellipse(0.56f, 0.46f, 0.045f, 0.055f, White);
        }

        static void Nounours(Raster raster)
        {
            Doudou(raster);
            raster.Ellipse(0.50f, 0.34f, 0.08f, 0.05f, Rgb(180, 40, 60));
        }

        static void Oreille(Raster raster)
        {
            Paper(raster, Rgb(255, 228, 214));
            raster.Ellipse(0.52f, 0.48f, 0.16f, 0.26f, Rgb(244, 186, 160));
            raster.Ellipse(0.52f, 0.46f, 0.07f, 0.14f, Rgb(226, 140, 140));
            raster.Circle(0.52f, 0.36f, 0.035f, Rgb(190, 90, 100));
        }

        static void Pain(Raster raster)
        {
            Paper(raster, Rgb(255, 228, 190));
            raster.RoundRect(0.50f, 0.48f, 0.55f, 0.16f, 0.08f, Rgb(210, 150, 70));
            raster.Stroke(Rgb(160, 100, 40), 0.012f, V(0.32f, 0.52f), V(0.36f, 0.44f));
            raster.Stroke(Rgb(160, 100, 40), 0.012f, V(0.48f, 0.54f), V(0.52f, 0.44f));
            raster.Stroke(Rgb(160, 100, 40), 0.012f, V(0.64f, 0.52f), V(0.68f, 0.44f));
        }

        static void Papa(Raster raster)
        {
            Paper(raster, Rgb(220, 230, 240));
            raster.Circle(0.50f, 0.68f, 0.10f, Rgb(210, 160, 120));
            raster.Ellipse(0.50f, 0.62f, 0.08f, 0.04f, Rgb(90, 60, 40));
            raster.Polygon(Rgb(50, 90, 150), V(0.32f, 0.52f), V(0.68f, 0.52f), V(0.62f, 0.20f), V(0.38f, 0.20f));
            raster.RoundRect(0.50f, 0.40f, 0.04f, 0.16f, 0.01f, Rgb(200, 50, 60));
            raster.Circle(0.46f, 0.70f, 0.012f, Ink);
            raster.Circle(0.54f, 0.70f, 0.012f, Ink);
        }

        static void Quatre(Raster raster)
        {
            Paper(raster, Rgb(255, 230, 200));
            raster.Circle(0.38f, 0.60f, 0.08f, Rgb(220, 80, 80));
            raster.Circle(0.62f, 0.60f, 0.08f, Rgb(50, 120, 190));
            raster.Circle(0.38f, 0.38f, 0.08f, Rgb(240, 180, 50));
            raster.Circle(0.62f, 0.38f, 0.08f, Rgb(70, 160, 90));
        }

        static void Queue(Raster raster)
        {
            Paper(raster, Rgb(255, 236, 220));
            raster.Circle(0.34f, 0.48f, 0.14f, Rgb(230, 150, 70));
            raster.Polygon(Rgb(230, 150, 70), V(0.28f, 0.58f), V(0.34f, 0.58f), V(0.30f, 0.74f));
            raster.Polygon(Rgb(230, 150, 70), V(0.36f, 0.58f), V(0.42f, 0.58f), V(0.44f, 0.74f));
            raster.Stroke(Rgb(230, 150, 70), 0.05f, V(0.46f, 0.46f), V(0.62f, 0.62f), V(0.78f, 0.50f), V(0.70f, 0.36f));
            raster.Circle(0.30f, 0.52f, 0.015f, Ink);
        }

        static void Quinze(Raster raster)
        {
            Paper(raster, Rgb(255, 232, 210));
            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 5; col++)
                    raster.Circle(0.24f + col * 0.13f, 0.68f - row * 0.16f, 0.04f, Rgb(226, 120, 60));
            }
        }

        static void Riviere(Raster raster)
        {
            Paper(raster, Rgb(210, 230, 200));
            raster.Stroke(Rgb(47, 140, 200), 0.08f, V(0.10f, 0.72f), V(0.32f, 0.58f), V(0.48f, 0.64f), V(0.70f, 0.40f), V(0.90f, 0.28f));
            raster.Stroke(Rgb(180, 220, 240), 0.03f, V(0.16f, 0.70f), V(0.36f, 0.58f), V(0.52f, 0.62f), V(0.74f, 0.40f));
        }

        static void Robe(Raster raster)
        {
            Paper(raster, Rgb(240, 230, 245));
            raster.Circle(0.50f, 0.74f, 0.07f, Rgb(244, 196, 160));
            raster.Polygon(Rgb(120, 90, 180), V(0.40f, 0.66f), V(0.60f, 0.66f), V(0.76f, 0.22f), V(0.24f, 0.22f));
            raster.Circle(0.50f, 0.48f, 0.02f, Rgb(242, 193, 78));
        }

        static void Sable(Raster raster)
        {
            Paper(raster, Rgb(190, 220, 230));
            raster.Ellipse(0.50f, 0.26f, 0.36f, 0.08f, Rgb(230, 200, 120));
            raster.Polygon(Rgb(240, 210, 130), V(0.34f, 0.32f), V(0.66f, 0.32f), V(0.58f, 0.55f), V(0.42f, 0.55f));
            raster.Polygon(Rgb(250, 220, 150), V(0.42f, 0.55f), V(0.58f, 0.55f), V(0.52f, 0.70f), V(0.48f, 0.70f));
            raster.Stroke(Rgb(200, 80, 80), 0.012f, V(0.50f, 0.70f), V(0.50f, 0.82f));
        }

        static void Sapin(Raster raster)
        {
            Paper(raster, Rgb(210, 230, 220));
            raster.RoundRect(0.50f, 0.20f, 0.08f, 0.16f, 0.02f, Rgb(120, 78, 48));
            raster.Polygon(Rgb(40, 120, 70), V(0.50f, 0.86f), V(0.22f, 0.48f), V(0.78f, 0.48f));
            raster.Polygon(Rgb(50, 140, 80), V(0.50f, 0.70f), V(0.28f, 0.36f), V(0.72f, 0.36f));
            raster.Polygon(Rgb(60, 150, 90), V(0.50f, 0.52f), V(0.34f, 0.26f), V(0.66f, 0.26f));
        }

        static void Sirene(Raster raster)
        {
            Paper(raster, Rgb(170, 210, 220));
            raster.Circle(0.50f, 0.70f, 0.08f, Rgb(244, 196, 160));
            raster.Polygon(Rgb(80, 160, 190), V(0.42f, 0.60f), V(0.58f, 0.60f), V(0.62f, 0.36f), V(0.38f, 0.36f));
            raster.Polygon(Rgb(40, 140, 160), V(0.36f, 0.36f), V(0.64f, 0.36f), V(0.70f, 0.18f), V(0.30f, 0.18f));
            raster.Circle(0.47f, 0.72f, 0.01f, Ink);
            raster.Circle(0.53f, 0.72f, 0.01f, Ink);
        }

        static void Table(Raster raster)
        {
            Paper(raster, Rgb(236, 220, 200));
            raster.RoundRect(0.50f, 0.58f, 0.56f, 0.08f, 0.02f, Rgb(160, 100, 60));
            raster.RoundRect(0.30f, 0.36f, 0.05f, 0.28f, 0.015f, Rgb(120, 78, 48));
            raster.RoundRect(0.70f, 0.36f, 0.05f, 0.28f, 0.015f, Rgb(120, 78, 48));
        }

        static void Tigre(Raster raster)
        {
            Paper(raster, Rgb(255, 230, 190));
            raster.Circle(0.34f, 0.66f, 0.07f, Rgb(230, 140, 40));
            raster.Circle(0.66f, 0.66f, 0.07f, Rgb(230, 140, 40));
            raster.Circle(0.50f, 0.46f, 0.20f, Rgb(240, 150, 40));
            raster.Stroke(Ink, 0.012f, V(0.36f, 0.52f), V(0.46f, 0.48f));
            raster.Stroke(Ink, 0.012f, V(0.54f, 0.56f), V(0.64f, 0.50f));
            raster.Stroke(Ink, 0.012f, V(0.40f, 0.38f), V(0.58f, 0.36f));
            raster.Circle(0.42f, 0.52f, 0.018f, Ink);
            raster.Circle(0.58f, 0.52f, 0.018f, Ink);
            raster.Polygon(Rgb(255, 160, 160), V(0.50f, 0.44f), V(0.46f, 0.38f), V(0.54f, 0.38f));
        }

        static void Tomate(Raster raster)
        {
            Paper(raster, Rgb(255, 220, 210));
            raster.Circle(0.50f, 0.46f, 0.20f, Rgb(210, 50, 50));
            raster.Ellipse(0.50f, 0.66f, 0.08f, 0.035f, Rgb(50, 140, 60));
            raster.Ellipse(0.44f, 0.52f, 0.04f, 0.06f, Rgb(255, 120, 110, 120));
        }

        static void Ukulélé(Raster raster)
        {
            Paper(raster, Rgb(255, 230, 200));
            raster.Ellipse(0.50f, 0.36f, 0.16f, 0.18f, Rgb(196, 130, 70));
            raster.Circle(0.50f, 0.36f, 0.05f, Rgb(80, 50, 30));
            raster.RoundRect(0.50f, 0.66f, 0.06f, 0.36f, 0.02f, Rgb(160, 100, 55));
            raster.Stroke(White, 0.008f, V(0.48f, 0.50f), V(0.48f, 0.82f));
            raster.Stroke(White, 0.008f, V(0.52f, 0.50f), V(0.52f, 0.82f));
        }

        static void Univers(Raster raster)
        {
            Paper(raster, Rgb(36, 48, 90));
            raster.Circle(0.46f, 0.48f, 0.16f, Rgb(70, 130, 200));
            raster.Ellipse(0.46f, 0.48f, 0.22f, 0.05f, Rgb(120, 180, 220), 20f);
            raster.Circle(0.72f, 0.72f, 0.015f, White);
            raster.Circle(0.24f, 0.70f, 0.02f, Rgb(242, 193, 78));
            raster.Circle(0.78f, 0.30f, 0.012f, White);
            raster.Circle(0.30f, 0.28f, 0.01f, White);
        }

        static void Usine(Raster raster)
        {
            Paper(raster, Rgb(220, 226, 230));
            raster.RoundRect(0.42f, 0.36f, 0.40f, 0.32f, 0.02f, Rgb(120, 130, 140));
            raster.Polygon(Rgb(90, 100, 110), V(0.24f, 0.52f), V(0.60f, 0.52f), V(0.60f, 0.64f), V(0.24f, 0.64f));
            raster.RoundRect(0.70f, 0.62f, 0.08f, 0.28f, 0.015f, Rgb(80, 86, 94));
            raster.Ellipse(0.74f, 0.78f, 0.06f, 0.04f, Rgb(180, 180, 186));
            raster.RoundRect(0.36f, 0.40f, 0.08f, 0.08f, 0.01f, Rgb(186, 214, 230));
        }

        static void Vache(Raster raster)
        {
            Paper(raster, Rgb(230, 240, 220));
            raster.Circle(0.36f, 0.70f, 0.06f, White);
            raster.Circle(0.64f, 0.70f, 0.06f, White);
            raster.Polygon(Rgb(210, 60, 70), V(0.32f, 0.74f), V(0.36f, 0.74f), V(0.30f, 0.86f));
            raster.Polygon(Rgb(210, 60, 70), V(0.64f, 0.74f), V(0.68f, 0.74f), V(0.72f, 0.86f));
            raster.Ellipse(0.50f, 0.46f, 0.20f, 0.16f, White);
            raster.Ellipse(0.38f, 0.50f, 0.06f, 0.05f, Ink);
            raster.Circle(0.44f, 0.52f, 0.016f, Ink);
            raster.Circle(0.58f, 0.52f, 0.016f, Ink);
            raster.Ellipse(0.50f, 0.38f, 0.06f, 0.04f, Rgb(255, 170, 180));
        }

        static void Velo(Raster raster)
        {
            Paper(raster, Rgb(230, 236, 240));
            raster.Circle(0.32f, 0.32f, 0.12f, Rgb(40, 40, 48));
            raster.Circle(0.32f, 0.32f, 0.07f, Cream);
            raster.Circle(0.68f, 0.32f, 0.12f, Rgb(40, 40, 48));
            raster.Circle(0.68f, 0.32f, 0.07f, Cream);
            raster.Stroke(Rgb(50, 120, 180), 0.02f, V(0.32f, 0.32f), V(0.48f, 0.55f), V(0.68f, 0.32f), V(0.52f, 0.48f), V(0.32f, 0.32f));
            raster.Stroke(Rgb(50, 120, 180), 0.016f, V(0.48f, 0.55f), V(0.58f, 0.72f));
        }

        static void Vent(Raster raster)
        {
            Paper(raster, Rgb(210, 230, 240));
            raster.Stroke(Rgb(80, 150, 200), 0.03f, V(0.16f, 0.66f), V(0.55f, 0.66f), V(0.48f, 0.56f));
            raster.Stroke(Rgb(80, 150, 200), 0.03f, V(0.20f, 0.48f), V(0.70f, 0.48f), V(0.62f, 0.38f));
            raster.Stroke(Rgb(80, 150, 200), 0.025f, V(0.28f, 0.32f), V(0.62f, 0.32f));
            raster.Ellipse(0.74f, 0.62f, 0.05f, 0.03f, Rgb(70, 150, 80), 20f);
        }

        static void Wallaby(Raster raster)
        {
            Kangourou(raster);
        }

        static void Wombat(Raster raster)
        {
            Paper(raster, Rgb(230, 220, 200));
            raster.Ellipse(0.50f, 0.40f, 0.24f, 0.16f, Rgb(140, 100, 70));
            raster.Circle(0.68f, 0.52f, 0.10f, Rgb(150, 110, 78));
            raster.Circle(0.62f, 0.66f, 0.045f, Rgb(130, 90, 64));
            raster.Circle(0.74f, 0.66f, 0.045f, Rgb(130, 90, 64));
            raster.Circle(0.72f, 0.54f, 0.012f, Ink);
            raster.Circle(0.40f, 0.28f, 0.04f, Rgb(120, 80, 55));
            raster.Circle(0.58f, 0.28f, 0.04f, Rgb(120, 80, 55));
        }

        static void Xerus(Raster raster)
        {
            Paper(raster, Rgb(236, 220, 180));
            raster.Ellipse(0.48f, 0.46f, 0.18f, 0.10f, Rgb(196, 140, 70));
            raster.Circle(0.68f, 0.54f, 0.07f, Rgb(180, 120, 60));
            raster.Stroke(Rgb(160, 100, 40), 0.04f, V(0.30f, 0.48f), V(0.16f, 0.62f));
            raster.Circle(0.72f, 0.56f, 0.012f, Ink);
            raster.Stroke(Rgb(120, 80, 40), 0.012f, V(0.40f, 0.52f), V(0.55f, 0.50f));
        }

        static void Yaourt(Raster raster)
        {
            Paper(raster, Rgb(255, 236, 230));
            raster.Polygon(White, V(0.34f, 0.62f), V(0.66f, 0.62f), V(0.60f, 0.28f), V(0.40f, 0.28f));
            raster.RoundRect(0.50f, 0.66f, 0.36f, 0.08f, 0.02f, Rgb(220, 70, 90));
            raster.Ellipse(0.50f, 0.48f, 0.08f, 0.05f, Rgb(255, 220, 230));
        }

        static void Yoga(Raster raster)
        {
            Paper(raster, Rgb(220, 235, 220));
            raster.Circle(0.50f, 0.70f, 0.07f, Rgb(244, 196, 160));
            raster.Stroke(Rgb(70, 130, 120), 0.03f, V(0.50f, 0.62f), V(0.50f, 0.42f));
            raster.Stroke(Rgb(70, 130, 120), 0.025f, V(0.50f, 0.54f), V(0.34f, 0.48f));
            raster.Stroke(Rgb(70, 130, 120), 0.025f, V(0.50f, 0.54f), V(0.66f, 0.48f));
            raster.Stroke(Rgb(70, 130, 120), 0.025f, V(0.42f, 0.40f), V(0.34f, 0.28f), V(0.48f, 0.30f));
            raster.Stroke(Rgb(70, 130, 120), 0.025f, V(0.58f, 0.40f), V(0.66f, 0.28f), V(0.52f, 0.30f));
        }

        static void Yoyo(Raster raster)
        {
            Paper(raster, Rgb(255, 230, 210));
            raster.Circle(0.42f, 0.48f, 0.12f, Rgb(210, 60, 80));
            raster.Circle(0.58f, 0.48f, 0.12f, Rgb(50, 110, 180));
            raster.RoundRect(0.50f, 0.48f, 0.04f, 0.16f, 0.01f, Rgb(240, 200, 80));
            raster.Stroke(Ink, 0.01f, V(0.50f, 0.56f), V(0.50f, 0.78f));
        }

        static void Zero(Raster raster)
        {
            Paper(raster, Rgb(230, 236, 242));
            raster.Ellipse(0.50f, 0.50f, 0.16f, 0.24f, Rgb(50, 110, 180));
            raster.Ellipse(0.50f, 0.50f, 0.09f, 0.15f, Cream);
        }

        static void Zigzag(Raster raster)
        {
            Paper(raster, Rgb(255, 230, 190));
            raster.Stroke(Rgb(220, 120, 40), 0.045f, V(0.18f, 0.30f), V(0.38f, 0.62f), V(0.55f, 0.34f), V(0.82f, 0.72f));
        }

        static void Zoo(Raster raster)
        {
            Paper(raster, Rgb(220, 235, 210));
            raster.RoundRect(0.28f, 0.48f, 0.08f, 0.46f, 0.02f, Rgb(120, 80, 50));
            raster.RoundRect(0.72f, 0.48f, 0.08f, 0.46f, 0.02f, Rgb(120, 80, 50));
            raster.Stroke(Rgb(80, 110, 70), 0.02f, V(0.32f, 0.62f), V(0.68f, 0.62f));
            raster.Stroke(Rgb(80, 110, 70), 0.02f, V(0.32f, 0.48f), V(0.68f, 0.48f));
            raster.Circle(0.50f, 0.40f, 0.08f, Rgb(230, 160, 60));
            raster.Circle(0.47f, 0.42f, 0.012f, Ink);
            raster.Circle(0.53f, 0.42f, 0.012f, Ink);
        }

        static void Smile(Raster raster, float x, float y, float radius, Color32 skin)
        {
            raster.Circle(x, y, radius, skin);
            raster.Circle(x - radius * 0.35f, y + radius * 0.15f, radius * 0.12f, Ink);
            raster.Circle(x + radius * 0.35f, y + radius * 0.15f, radius * 0.12f, Ink);
            raster.Ellipse(x, y - radius * 0.25f, radius * 0.28f, radius * 0.14f, Rgb(200, 80, 90));
        }

        static void Person(Raster raster, float x, float y, float scale, Color32 cloth)
        {
            raster.Circle(x, y + 0.28f * scale, 0.07f * scale, Rgb(244, 196, 160));
            raster.RoundRect(x, y + 0.08f * scale, 0.12f * scale, 0.22f * scale, 0.04f, cloth);
        }

        static void Tree(Raster raster, float x, float scale)
        {
            raster.RoundRect(x, 0.28f, 0.06f * scale, 0.18f * scale, 0.015f, Rgb(120, 78, 48));
            raster.Circle(x, 0.48f * scale + 0.1f, 0.10f * scale, Rgb(50, 140, 70));
        }

        static void Bloom(Raster raster, float x, float y, Color32 petal)
        {
            raster.Stroke(Rgb(60, 130, 70), 0.012f, V(x, 0.30f), V(x, y - 0.06f));
            raster.Circle(x, y, 0.055f, petal);
            raster.Circle(x, y, 0.02f, Rgb(242, 193, 78));
        }
    }
}
