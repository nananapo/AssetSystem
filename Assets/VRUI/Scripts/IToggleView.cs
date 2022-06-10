namespace VRUI
{
    public interface IToggleView : IEditable<bool>
    {
        public bool IsOn { get; set; }

        public void SetIsOnWithoutNotify(bool isOn);
    }
}