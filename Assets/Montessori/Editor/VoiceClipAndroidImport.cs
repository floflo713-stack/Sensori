using UnityEditor;
using UnityEngine;

namespace Sensori.Montessori.Editor
{
    public static class VoiceClipAndroidImport
    {
        const float ShortClipSeconds = 12f;

        [MenuItem("Montessori/Audio/Forcer DecompressOnLoad (voix courtes)")]
        public static void ForceDecompressOnLoad()
        {
            string[] guids = AssetDatabase.FindAssets("t:AudioClip", new[] { "Assets/Montessori", "Assets/StreamingAssets" });
            int changed = 0;
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                var clip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
                var importer = AssetImporter.GetAtPath(path) as AudioImporter;
                if (clip == null || importer == null)
                    continue;
                if (clip.length > ShortClipSeconds)
                    continue;
                var settings = importer.defaultSampleSettings;
                bool already = settings.loadType == AudioClipLoadType.DecompressOnLoad
                    && settings.preloadAudioData
                    && !importer.loadInBackground;
                if (already)
                    continue;
                settings.loadType = AudioClipLoadType.DecompressOnLoad;
                settings.compressionFormat = AudioCompressionFormat.PCM;
                settings.preloadAudioData = true;
                importer.defaultSampleSettings = settings;
                importer.SetOverrideSampleSettings(BuildTargetGroup.Android, settings);
                importer.loadInBackground = false;
                importer.SaveAndReimport();
                changed++;
                Debug.Log("[Voix] Import Android : " + path + " -> DecompressOnLoad");
            }
            Debug.Log("[Voix] Clips de voix courts mis à jour : " + changed);
        }
    }
}
