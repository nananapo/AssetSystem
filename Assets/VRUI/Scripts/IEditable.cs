namespace VRUI
{
    public interface IEditable<T>
    {
        ChangeEvent<T> OnValueEdit { get; set; }
        
        bool Editable { get; set; }
    }
}