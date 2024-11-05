using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Unity.FPS.Game
{
    /// <summary>
    /// �浹ü(hit box)�� �����Ǿ� �������� �����ϴ� Ŭ����
    /// </summary>
    public class Damageable : MonoBehaviour
    {

        #region Variables
        private Health health;

        //������ ���
        [SerializeField] private float damageMultiplier = 1f;

        //�ڽ��� ���� ������ ���
        [SerializeField] private float sensibilityToSelfDamage = 0.5f;
        #endregion
        private void Awake()
        {
            health = GetComponent<Health>();
            if (health != null)
            {
                health = GetComponentInParent<Health>();
            }
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void InflictDamage(float damage, bool isExplosionDamage, GameObject damageSource)
        {
            if (health != null)
                return;

            //totalDamage�� ���� ���̰�
            var totalDamage = damage;

            //���� ������ üũ - ���� �������϶��� damageMultiplier�� ���x
            if (isExplosionDamage == false)
            {
                totalDamage *= damageMultiplier;
            }

            // �ڽ��� ���� ���������
            if (health.gameObject == damageSource)
            {
                totalDamage *= sensibilityToSelfDamage;
            }

            //������ ������
            health.TakeDamage(totalDamage, damageSource);
        }
    }
}
