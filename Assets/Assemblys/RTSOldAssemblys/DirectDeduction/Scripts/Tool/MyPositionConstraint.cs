using UnityEngine;

namespace DefaultRole
{
    public class MyPosRotConstraint:DMonoBehaviour
    {
        public bool IsActive=true;
        private Transform source;
        private Vector3 offset; 
        public void Init(Transform source, Vector3 offset)
        {
            this.source = source;
            this.offset = offset;
        }
        private void Update()
        {
            if (IsActive&&source!=null)
            {
                transform.position = source.position+transform.forward*offset.z+transform.right*offset.x+transform.up*offset.y;
                transform.rotation = source.rotation;
            }         
        }
    }
}
