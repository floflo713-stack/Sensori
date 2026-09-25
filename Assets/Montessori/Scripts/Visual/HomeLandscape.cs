using UnityEngine;

namespace Sensori.Montessori
{
    public static class ScreenBackdrop
    {
        public static void Ensure(Transform parent, string scene)
        {
            if (!Application.isPlaying || parent == null)
                return;
            LivingLandscape.Mount(parent, scene);
        }
    }
}
