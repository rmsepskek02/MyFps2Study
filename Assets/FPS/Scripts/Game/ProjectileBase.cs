using UnityEngine;
using UnityEngine.Events;

namespace Unity.FPS.Game
{
    /// <summary>
    /// �߻�ü�� �⺻�� �Ǵ� �θ�Ŭ����
    /// </summary>
    public abstract class ProjectileBase : MonoBehaviour
    {
        #region Variables
        public GameObject Owner { get; private set; }   //�߻��� ��ü
        public Vector3 InitialPosition { get; private set; }
        public Vector3 InitialDirection { get; private set; }
        public Vector3 InheritedMuzzleVelocity { get; private set; }
        public float InitailCharge { get; private set; }

        public UnityAction OnShoot;                     //�߻��� ��ϵ� �Լ� ȣ��
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
