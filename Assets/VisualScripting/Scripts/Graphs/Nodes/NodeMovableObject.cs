using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace VisualScripting.Scripts.Graphs.Nodes
{
    public class NodeMovableObject : MonoBehaviour
    {
        //移動用
        //TODO インターフェースをはさむ
        [SerializeField] private XRGrabInteractable xr;

        [SerializeField] private Material def;
        [SerializeField] private Material red;
        [SerializeField] private Material green;
        
        private NodeHandle _handle;

        private Vector3 _localPosition;
        
        private Quaternion _localRotation;

        private bool _isSelected = false;

        private NodeSolidObject _solidObject;

        private Renderer _renderer;

        private void Start()
        {
            tag = "NodeMovableObject";
            //繋ぐ用
            xr.selectEntered.AddListener(_ => OnSelectEntered());
            xr.selectExited.AddListener(_ => OnSelectExited());

            _renderer = GetComponent<Renderer>();

            //色
            SetMaterial(def);
        }

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="handle"></param>
        public void Init(NodeHandle handle)
        {
            _handle = handle;

            //詳細表示用
            xr.hoverEntered.AddListener(_ => _handle.ShowHelp());
            xr.hoverExited.AddListener(_ => _handle.DismissHelp());

            //場所を保存
            transform.parent = _handle.transform;
            _localPosition = transform.localPosition;
            _localRotation = transform.localRotation;
        }

        /// <summary>
        /// 色変える
        /// </summary>
        /// <param name="mat"></param>
        private void SetMaterial(Material mat)
        {
            _renderer.material = mat;
        }

        public void OnSelectEntered()
        {
            _isSelected = true;
            _solidObject = null;
        }

        public void OnSelectExited()
        {
            _isSelected = false;
            
            transform.parent = _handle.transform;
            transform.localPosition = _localPosition;
            transform.localRotation = _localRotation;

            if (_solidObject != null)
            {
                var target = _solidObject.GetHandle();
                if (_handle.Connector == target.Connector)
                {
                    _handle.Connector.ConnectNode(_handle.Node, target.Node);
                }
            }
            
            SetMaterial(def);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_isSelected && _solidObject == null && other.CompareTag("NodeSolidObject"))
            {
                if (other.TryGetComponent<NodeSolidObject>(out var result))
                {
                    _solidObject = result;
                    var anotherHandle = _solidObject.GetHandle();

                    //コネクタが同じか確認
                    if(_handle.Connector != anotherHandle.Connector)
                    {
                        SetMaterial(red);
                        return;
                    }

                    //接続できるか確認
                    if (_handle.Node.CanAttach(_handle.Connector, anotherHandle.Node) && anotherHandle.Node.CanAttach(anotherHandle.Connector, _handle.Node))
                    {
                        SetMaterial(green);
                    }
                    else
                    {
                        SetMaterial(red);
                    }
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (_solidObject != null && other.CompareTag("NodeSolidObject"))
            {
                if (other.TryGetComponent<NodeSolidObject>(out var result))
                {
                    if (result == _solidObject)
                    {
                        _solidObject = null;
                        SetMaterial(def);
                    }
                }
            }
        }
    }
}