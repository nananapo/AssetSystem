namespace VisualScripting.Scripts.Window.Item
{
    public interface INamedItem
    {
        public string ItemName { get; set; }

        public INamedItem SetItemName(string itemName)
        {
            ItemName = itemName;
            return this;
        }
    }
}