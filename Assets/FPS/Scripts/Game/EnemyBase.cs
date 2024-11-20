using Unity.FPS.Game;
using UnityEngine;

namespace Unity.FPS.AI
{
    /// <summary>
    /// Enemy의 공통사항 정의
    /// </summary>

    public abstract class EnemyBase: MonoBehaviour
    {
        #region Variables
        public Animator animator;
        protected EnemyController enemyController;

        //이동
        public AudioClip movementSound;
        public MinMaxFloat pitchMovenemtSpeed;

        protected AudioSource audioSource;

        //데미지 - 이펙트
        public ParticleSystem[] randomHitSparks;

        //Detected
        public ParticleSystem[] detectedVfxs;
        public AudioClip detectedSfx;

        //attack
        [Range(0f, 1f)]
        public float attackSkipDistanceRatio = 0.5f;

        protected const string k_AnimOnDamagedParameter = "OnDamaged";
        #endregion

        protected virtual void Start()
        {
            //참조            
            enemyController = GetComponent<EnemyController>();
            enemyController.Damaged += OnDamaged;
            enemyController.OnDetectedTarget += OnDetected;
            enemyController.OnLostTarget += OnLost;
            enemyController.OnAttack += Attacked;
        }

        protected virtual void Update()
        {
            //상태 변경/구현
            UpdateAiStateTransition();
            UpdateCurrentAiState();
        }

        //상태에 따른 Enemy 구현
        protected abstract void UpdateCurrentAiState();

        //상태 변경에 따른 구현
        protected abstract void UpdateAiStateTransition();

        protected virtual void OnDamaged()
        {
            //스파크 파티클 - 랜덤하게 하나 선택해서 플레이
            if (randomHitSparks.Length > 0)
            {
                int randNum = Random.Range(0, randomHitSparks.Length);
                randomHitSparks[randNum].Play();
            }

            //데미지 애니
            animator.SetTrigger(k_AnimOnDamagedParameter);
        }

        protected abstract void OnDetected();
        protected abstract void OnLost();
        protected abstract void Attacked();
    }
}
