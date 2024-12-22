using UnityEngine;
using UnityEngine.Serialization;

namespace DevelopKit
{
    public class JemView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer render;
        [SerializeField] private Rigidbody2D rigid;
        
        public void Initialize(int jemID, Transform parent, Vector3 position, Quaternion rotation = default)
        {
#if UNITASK_INSTALLED
            render.sprite = ManagerHub.Atlas.GetJemSprite(jemID);
#endif
            
            var randomInitialForce = Random.insideUnitCircle * 10f;
            randomInitialForce.y = Mathf.Abs(randomInitialForce.y);
            rigid.AddForce(randomInitialForce, ForceMode2D.Impulse);
            
            transform.SetParent(parent);
            transform.position = position;
            transform.rotation = rotation;
        }
    }
}
