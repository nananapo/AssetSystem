using UnityEngine;

namespace VisualScripting.Scripts.Graphs.Nodes
{
    [RequireComponent(typeof(LineRenderer))]
    public class EdgeRenderer : MonoBehaviour
    {

        private LineRenderer _line;

        public Transform t1;
        public Transform t2;
        
        private void Start()
        {
            _line = GetComponent<LineRenderer>();
            
            t1 = t1 ? t1 : transform;
            t2 = t2 ? t2 : transform;
        }

        public void Init(Transform tr1, Transform tr2)
        {
            t1 = tr1;
            t2 = tr2;
        }

        private void Update()
        {
            _line.SetPositions(new []
            {
                t1.position,
                t2.position
            });
        }
    }
}