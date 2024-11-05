using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Unity.FPS.Game
{
    /// <summary>
    /// ü���� �����ϴ� Ŭ����
    /// </summary>
    public class Health : MonoBehaviour
    {
        #region Variables
        [SerializeField] private float maxHealth = 100f;    //�ִ� hp
        public float CurrentHealth { get; private set; }    //���� hp
        private bool isDeath = false;                       //���� üũ

        //����
        public bool Invincible { get; private set; }

        public UnityAction<float, GameObject> OnDamaged;
        public UnityAction<float, GameObject> OnHealed;
        public UnityAction OnDie;

        //ü�� ���� �����
        [SerializeField] private float criticalHealRatio = 0.3f;
        #endregion
        // Start is called before the first frame update
        void Start()
        {
            CurrentHealth = maxHealth;
            Invincible = false;
        }

        // Update is called once per frame
        void Update()
        {

        }

        //damageSource: �������� �ִ� ��ü
        public void TakeDamage(float damage, GameObject damageSource)
        {
            if (Invincible) return;

            float beforeHealth = CurrentHealth;
            CurrentHealth -= damage;
            CurrentHealth = Mathf.Clamp(CurrentHealth, 0, maxHealth);

            //real Damage ���ϱ�
            float realDamage = beforeHealth - CurrentHealth;
            if(realDamage > 0)
            {
                //������ ����
                OnDamaged?.Invoke(realDamage, damageSource);
            }

            HandleDeath();
        }

        public void TakeHeal(float heal, GameObject healSource)
        {
            float beforeHealth = CurrentHealth;
            CurrentHealth += heal;
            CurrentHealth = Mathf.Clamp(CurrentHealth, 0, maxHealth);

            //real Heal ���ϱ�
            float realHeal = CurrentHealth - beforeHealth;
            if (realHeal > 0)
            {
                //������ ����
                OnHealed?.Invoke(realHeal, healSource);
            }
        }

        //�� �������� ������ �ִ��� üũ
        public bool CanPickUp() => CurrentHealth < maxHealth;
        //UI HP ������ ��
        public float GetRatio() => CurrentHealth / maxHealth;
        // ���� üũ
        public bool isCritical() => GetRatio() <= criticalHealRatio;
        void HandleDeath()
        {
            if (isDeath) return;

            if(CurrentHealth <= 0f)
            {
                isDeath = true;

                //���� ����
                OnDie?.Invoke();
            }
        }
    }
}

