using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Unity.FPS.Game
{
    /// <summary>
    /// 체력을 관리하는 클래스
    /// </summary>
    public class Health : MonoBehaviour
    {
        #region Variables
        [SerializeField] private float maxHealth = 100f;    //최대 hp
        public float CurrentHealth { get; private set; }    //현재 hp
        private bool isDeath = false;                       //죽음 체크

        //무적
        public bool Invincible { get; private set; }

        public UnityAction<float, GameObject> OnDamaged;
        public UnityAction<float, GameObject> OnHealed;
        public UnityAction OnDie;

        //체력 위험 경계율
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

        //damageSource: 데미지를 주는 주체
        public void TakeDamage(float damage, GameObject damageSource)
        {
            if (Invincible) return;

            float beforeHealth = CurrentHealth;
            CurrentHealth -= damage;
            CurrentHealth = Mathf.Clamp(CurrentHealth, 0, maxHealth);

            //real Damage 구하기
            float realDamage = beforeHealth - CurrentHealth;
            if(realDamage > 0)
            {
                //데미지 구현
                OnDamaged?.Invoke(realDamage, damageSource);
            }

            HandleDeath();
        }

        public void TakeHeal(float heal, GameObject healSource)
        {
            float beforeHealth = CurrentHealth;
            CurrentHealth += heal;
            CurrentHealth = Mathf.Clamp(CurrentHealth, 0, maxHealth);

            //real Heal 구하기
            float realHeal = CurrentHealth - beforeHealth;
            if (realHeal > 0)
            {
                //데미지 구현
                OnHealed?.Invoke(realHeal, healSource);
            }
        }

        //힐 아이템을 먹을수 있는지 체크
        public bool CanPickUp() => CurrentHealth < maxHealth;
        //UI HP 게이지 값
        public float GetRatio() => CurrentHealth / maxHealth;
        // 위험 체크
        public bool isCritical() => GetRatio() <= criticalHealRatio;
        void HandleDeath()
        {
            if (isDeath) return;

            if(CurrentHealth <= 0f)
            {
                isDeath = true;

                //죽음 구현
                OnDie?.Invoke();
            }
        }
    }
}

