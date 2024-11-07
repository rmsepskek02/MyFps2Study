using UnityEngine;
using UnityEngine.Events;

namespace Unity.FPS.Game
{
    /// <summary>
    /// 발사체의 기본이 되는 부모클래스
    /// </summary>
    public abstract class ProjectileBase : MonoBehaviour
    {
        #region Variables
        public GameObject Owner { get; private set; }   //발사한 주체
        public Vector3 InitialPosition { get; private set; }
        public Vector3 InitialDirection { get; private set; }
        public Vector3 InheritedMuzzleVelocity { get; private set; }
        public float InitailCharge { get; private set; }

        public UnityAction OnShoot;                     //발사한 등록된 함수 호출
        #endregion

        public void Shoot(WeaponController controller)
        {
            Owner = controller.Owner;
            InitialPosition = this.transform.position;
            InitialDirection = this.transform.forward;
            InheritedMuzzleVelocity = controller.MuzzleWorldVelocity;
            InitailCharge = controller.CurrentCharge;

            OnShoot.Invoke();
        }
    }
}
