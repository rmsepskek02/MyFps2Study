using Unity.FPS.Game;
using UnityEngine;

namespace Unity.FPS.Gameplay
{
    /// <summary>
    /// ������ �߻�ü�� �߻��Ҷ� �������� �߻�ü�� �Ӽ����� ����
    /// </summary>
    public class PjojectileChargeParameters : MonoBehaviour
    {
        #region Variables
        private ProjectileBase projectileBase;

        public MinMaxFloat Damage;
        public MinMaxFloat Speed;
        public MinMaxFloat GravityDown;
        public MinMaxFloat Radius;
        #endregion

        private void OnEnable()
        {
            //����
            projectileBase = GetComponent<ProjectileBase>();
            projectileBase.OnShoot += OnShoot;
        }

        //�߻�ü �߻�� ProjectileBased�� OnShoot ��������Ʈ �Լ����� ȣ��
        //�߻��� �Ӽ����� Charge���� ���� ����
        void OnShoot()
        {
            //�������� ���� �߻�ü �Ӽ��� ����
            ProjectileStandard projectileStandard = GetComponent<ProjectileStandard>();
            projectileStandard.damage = Damage.GetValueFromRatio(projectileBase.InitialCharge);
            projectileStandard.speed = Speed.GetValueFromRatio(projectileBase.InitialCharge);
            projectileStandard.gravityDown = GravityDown.GetValueFromRatio(projectileBase.InitialCharge);
            projectileStandard.radius = Radius.GetValueFromRatio(projectileBase.InitialCharge);
        }

    }
}
