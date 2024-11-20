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
    public class EnemyStand : EnemyBase
    {
        #region Variables
        public AIStandState AiStandState { get; private set; }
        const string k_AnimActiveParameter = "IsActive";
        #endregion
        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
        }

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();
        }

        //상태에 따른 Enemy 구현
        protected override void UpdateAiStateTransition()
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
        protected override void UpdateCurrentAiState()
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

        protected override void OnDamaged()
        {
            base.OnDamaged();
        }

        protected override void OnDetected()
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

        protected override void OnLost()
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
        protected override void Attacked()
        {
            //애니
            animator.SetTrigger(k_AnimActiveParameter);
        }
    }
}
