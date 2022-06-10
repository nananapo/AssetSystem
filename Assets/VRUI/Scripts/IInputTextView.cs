namespace VRUI
{
    public interface IInputTextView : ITextView,IEditable<string>
    {
        void SetTextWithoutNotify(string text);
    }
}