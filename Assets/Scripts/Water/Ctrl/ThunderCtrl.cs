using Spine.Unity;
using UnityEngine;
using UnityEngine.Animations;

namespace Game.Water
{
    public class ThunderCtrl : MonoBehaviour
    {
        //public Vector3 fromPos;
        public Transform target;
        public SkeletonGraphic useSpine;
        public float totalTime;
        public PositionConstraint positionConstraint;

        // Start is called before the first frame update
        void OnEnable()
        {
            useSpine.AnimationState.SetEmptyAnimation(0, 0f);
            //thunderGo.transform.position = (fromPos + transform.position) / 2;
            useSpine.AnimationState.SetAnimation(0, "bullet", false);
        }

        // Update is called once per frame
        void Update()
        {
            //transform.localScale = new Vector3(transform.localScale.x, Vector3.Distance(fromPos, transform.position) / 4f, transform.localScale.z);
            var fromPos = target.position;
            transform.localScale = new Vector3(transform.localScale.x, Vector3.Distance(fromPos, transform.position) / 7f, transform.localScale.z);
            totalTime += Time.deltaTime;
            var angle = Vector3.Angle(fromPos - transform.position, Vector3.up);

            //�жϷ���
            int sign = 1;
            if (fromPos.x > transform.position.x)
            {
                sign = -1;
            }
            transform.rotation = Quaternion.Euler(0, 0, angle * sign);


            if (totalTime > 2)
            {
                gameObject.SetActive(false);
                //��������
                Destroy(this.gameObject);
            }
        }
    }
}
