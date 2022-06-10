namespace VisualScripting.Scripts.Graphs.Parts
{
    public interface IInputStreamer<T>
    {
        public bool TryGetInputValue(out T result);
    }
}