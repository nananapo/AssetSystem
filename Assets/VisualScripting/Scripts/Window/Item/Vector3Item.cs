using System.Globalization;
using UnityEngine;
using VRUI;

namespace VisualScripting.Scripts.Window.Item
{
    public class Vector3Item : NamedItem,IEditable<Vector3>
    {
        
        [SerializeField] private InputTextView x;
        [SerializeField] private InputTextView y;
        [SerializeField] private InputTextView z;

        private Vector3 _value;

        public Vector3 Value
        {
            get => _value;
            set
            {
                SetWithoutNotify(value);
                OnValueEdit?.Invoke(_value);
            }
        }

        private bool _editable = true;

        public bool Editable
        {
            get => _editable;
            set
            {
                _editable = value;
                x.Editable = value;
                y.Editable = value;
                z.Editable = value;
            }
        }

        public ChangeEvent<Vector3> OnValueEdit { get; set; } = new();

        private void Awake()
        {
            x.OnValueEdit.AddListener(str =>
            {
                if (float.TryParse(str, out var v))
                {
                    _value.x = v;
                    OnValueEdit?.Invoke(_value);
                }
            });
            y.OnValueEdit.AddListener(str =>
            {
                if (float.TryParse(str, out var v))
                {
                    _value.y = v;
                    OnValueEdit?.Invoke(_value);
                }
            });
            z.OnValueEdit.AddListener(str =>
            {
                if (float.TryParse(str, out var v))
                {
                    _value.z = v;
                    OnValueEdit?.Invoke(_value);
                }
            });
        }

        public void SetWithoutNotify(Vector3 vec)
        {
            _value = vec;
            x.SetTextWithoutNotify(vec.x.ToString(CultureInfo.InvariantCulture));
            y.SetTextWithoutNotify(vec.y.ToString(CultureInfo.InvariantCulture));
            z.SetTextWithoutNotify(vec.z.ToString(CultureInfo.InvariantCulture));
        }
    }
}