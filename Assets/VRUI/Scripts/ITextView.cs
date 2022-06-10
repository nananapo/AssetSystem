namespace VRUI
{
    public interface ITextView
    {
        public string text { get; set; }

        void SetText(string text);

        string GetText();
    }
}