namespace Sensori.Montessori
{
    public sealed class GameRequest
    {
        public LearningCategory Category;
        public MiniGameDefinition Definition;
    }

    public abstract class MiniGameController : UnityEngine.MonoBehaviour
    {
        public abstract string GameId { get; }
        public abstract void Begin(GameRequest request);
        public virtual void Close() { }
    }
}
