using UnityEngine;

namespace Sensori.Montessori
{
    public static class LearningProgress
    {
        const string Prefix = "sensori.v1.";

        public static bool IsDone(string categoryId, string gameId, string itemId)
        {
            if (string.IsNullOrEmpty(categoryId) || string.IsNullOrEmpty(gameId) || string.IsNullOrEmpty(itemId))
                return false;
            return PlayerPrefs.GetInt(Key(categoryId, gameId, itemId), 0) == 1;
        }

        public static void Mark(string categoryId, string gameId, string itemId)
        {
            if (string.IsNullOrEmpty(categoryId) || string.IsNullOrEmpty(gameId) || string.IsNullOrEmpty(itemId))
                return;
            PlayerPrefs.SetInt(Key(categoryId, gameId, itemId), 1);
            PlayerPrefs.Save();
        }

        public static int CountDiscovered(LearningCategory category)
        {
            if (category == null)
                return 0;
            var items = category.Items;
            var games = category.Games;
            int discovered = 0;
            for (int i = 0; i < items.Length; i++)
            {
                var item = items[i];
                if (item == null)
                    continue;
                bool any = false;
                for (int g = 0; g < games.Length; g++)
                {
                    var game = games[g];
                    if (game == null)
                        continue;
                    if (IsDone(category.CategoryId, game.GameId, item.ItemId))
                    {
                        any = true;
                        break;
                    }
                }
                if (any)
                    discovered++;
            }
            return discovered;
        }

        public static int CountDone(LearningCategory category, string gameId)
        {
            if (category == null)
                return 0;
            var items = category.Items;
            int done = 0;
            for (int i = 0; i < items.Length; i++)
            {
                var item = items[i];
                if (item != null && IsDone(category.CategoryId, gameId, item.ItemId))
                    done++;
            }
            return done;
        }

        public static void ResetCategory(LearningCategory category)
        {
            if (category == null)
                return;
            var items = category.Items;
            var games = category.Games;
            for (int i = 0; i < items.Length; i++)
            {
                var item = items[i];
                if (item == null)
                    continue;
                for (int g = 0; g < games.Length; g++)
                {
                    var game = games[g];
                    if (game == null)
                        continue;
                    PlayerPrefs.DeleteKey(Key(category.CategoryId, game.GameId, item.ItemId));
                }
            }
            PlayerPrefs.Save();
        }

        static string Key(string categoryId, string gameId, string itemId)
        {
            return Prefix + categoryId + "." + gameId + "." + itemId;
        }
    }
}
