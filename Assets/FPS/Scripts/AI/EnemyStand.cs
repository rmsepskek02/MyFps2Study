using Unity.FPS.Game;
using UnityEngine;

namespace Unity.FPS.AI
{
    /// <summary>
    /// EnemyStand 상태
    /// </summary>
    public enum AIStandState
    {
        Idle,
        Detect,
        Attack
    }
    public class EnemyStand : MonoBehaviour
    {
        #region Variables
        public Animator animator;
        private EnemyController enemyController;

        public AIStandState AiStandState { get; private set; }

        //데미지 - 이펙트
        public ParticleSystem[] randomHitSparks;

        //Detected
        public ParticleSystem[] detectedVfxs;
        public AudioClip detectedSfx;

        const string k_AnimActiveParameter = "IsActive";
        const string k_AnimOnDamagedParameter = "OnDamaged";
        #endregion
        // Start is called before the first frame update
        void Start()
        {
            //참조            
            enemyController = GetComponent<EnemyController>();
            enemyController.Damaged += OnDamaged;
            enemyController.OnDetectedTarget += OnDetected;
            enemyController.OnLostTarget += OnLost;
            enemyController.OnAttack += Attacked;
        }

        // Update is called once per frame
        void Update()
        {
            //상태 변경/구현
            UpdateAiStandStateTransition();
            UpdateCurrentAiStandState();
        }

        //상태에 따른 Enemy 구현
        private void UpdateCurrentAiStandState()
        {
            switch (AiStandState)
            {
                case AIStandState.Idle:
                    break;
                case AIStandState.Detect:
                    enemyController.OrientToward(enemyController.KnonwDetectedTarget.transform.position);
                    enemyController.OrientWeaponsToward(enemyController.KnonwDetectedTarget.transform.position);
                    break;
                case AIStandState.Attack:
                    enemyController.OrientToward(enemyController.KnonwDetectedTarget.transform.position);
                    enemyController.OrientWeaponsToward(enemyController.KnonwDetectedTarget.transform.position);
                    enemyController.TryAttack(enemyController.KnonwDetectedTarget.transform.position);
                    break;
            }
        }

        //상태 변경에 따른 구현
        private void UpdateAiStandStateTransition()
        {
            switch (AiStandState)
            {
                case AIStandState.Idle:
                    break;
                case AIStandState.Detect:
                    if (enemyController.IsSeeingTarget && enemyController.IsTargetInAttackRange)
                    {
                        AiStandState = AIStandState.Attack;
                    }
                    break;
                case AIStandState.Attack:
                    if (enemyController.IsTargetInAttackRange == false)
                    {
                        AiStandState = AIStandState.Idle;
                    }
                    break;
            }
        }

        private void OnDamaged()
        {
            Debug.Log("TEST");
            //스파크 파티클 - 랜덤하게 하나 선택해서 플레이
            if (randomHitSparks.Length > 0)
            {
                int randNum = Random.Range(0, randomHitSparks.Length);
                randomHitSparks[randNum].Play();
            }

            //데미지 애니
            animator.SetTrigger(k_AnimOnDamagedParameter);
        }

        private void OnDetected()
        {
            //상태 변경
            if (AiStandState == AIStandState.Idle)
            {
                AiStandState = AIStandState.Detect;
            }

            //Vfx
            for (int i = 0; i < detectedVfxs.Length; i++)
            {
                detectedVfxs[i].Play();
            }

            //Sfx
            if (detectedSfx)
            {
                AudioUtility.CreateSfx(detectedSfx, this.transform.position, 1f);
            }

            //anim
            animator.SetBool(k_AnimActiveParameter, true);
        }

        private void OnLost()
        {
            //상태변경
            if (AiStandState == AIStandState.Detect || AiStandState == AIStandState.Attack)
            {
                AiStandState = AIStandState.Idle;
            }
            //Vfx
            for (int i = 0; i < detectedVfxs.Length; i++)
            {
                detectedVfxs[i].Stop();
            }

            //anim
            animator.SetBool(k_AnimActiveParameter, false);
        }
        private void Attacked()
        {
            //애니
            animator.SetTrigger(k_AnimActiveParameter);
        }
    }
}
