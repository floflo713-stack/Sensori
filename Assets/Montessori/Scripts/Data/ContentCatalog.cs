using UnityEngine;

namespace Sensori.Montessori
{
    [CreateAssetMenu(fileName = "Catalogue", menuName = "Montessori/Catalogue")]
    public sealed class ContentCatalog : ScriptableObject
    {
        [SerializeField] LearningCategory[] _categories = new LearningCategory[0];

        public LearningCategory[] Categories => _categories ?? new LearningCategory[0];

        public void Define(LearningCategory[] categories)
        {
            _categories = categories ?? new LearningCategory[0];
        }
    }
}
