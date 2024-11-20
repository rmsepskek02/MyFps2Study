using Unity.FPS.Game;
using UnityEngine;

namespace Unity.FPS.AI
{
    /// <summary>
    /// EnemyMobile 상태  
    /// </summary>
    public enum AIMobileState
    {
        Patrol,
        Follow,
        Attack
    }

    /// <summary>
    /// 이동하는 Enemy의 상태들을 구현하는 클래스
    /// </summary>
    public class EnemyMobile : EnemyBase
    {
        #region Variables
        public AIMobileState AiMobileState { get; private set; }

        //animation parameter
        const string k_AnimAttackParameter = "Attack";
        const string k_AnimMoveSpeedParameter = "MoveSpeed";
        const string k_AnimAlertedParameter = "Alerted";
        const string k_AnimDeathParameter = "Death";
        #endregion

        protected override void Start()
        {
            base.Start();

            audioSource = GetComponent<AudioSource>();
            audioSource.clip = movementSound;
            audioSource.Play();

            //초기화
            AiMobileState = AIMobileState.Patrol;
        }

        protected override void Update()
        {
            base.Update();

            //속도에 따른 애니/사운드 효과
            float moveSpeed = enemyController.Agent.velocity.magnitude;
            animator.SetFloat(k_AnimMoveSpeedParameter, moveSpeed);         //애니
            audioSource.pitch = pitchMovenemtSpeed.GetValueFromRatio(moveSpeed / enemyController.Agent.speed);
        }

        //상태에 따른 Enemy 구현
        protected override void UpdateCurrentAiState()
        {
            switch (AiMobileState)
            {
                case AIMobileState.Patrol:
                    enemyController.UpdatePathDestination(true);
                    enemyController.SetNavDestination(enemyController.GetDestinationOnPath());
                    break;
                case AIMobileState.Follow:
                    enemyController.SetNavDestination(enemyController.KnonwDetectedTarget.transform.position);
                    enemyController.OrientToward(enemyController.KnonwDetectedTarget.transform.position);
                    enemyController.OrientWeaponsToward(enemyController.KnonwDetectedTarget.transform.position);
                    break;
                case AIMobileState.Attack:
                    //일정거리까지는 이동하면서 공격
                    float distance = Vector3.Distance(enemyController.KnonwDetectedTarget.transform.position, enemyController.DetectionModule.detectionSourcePoint.position);
                    if(distance >= enemyController.DetectionModule.attackRange * attackSkipDistanceRatio)
                    {
                        enemyController.SetNavDestination   (enemyController.KnonwDetectedTarget.transform.position);
                    }
                    else
                    {
                        enemyController.SetNavDestination(transform.position);  //제자리에 서기
                    }
                    enemyController.OrientToward(enemyController.KnonwDetectedTarget.transform.position);
                    enemyController.OrientWeaponsToward(enemyController.KnonwDetectedTarget.transform.position);
                    enemyController.TryAttack(enemyController.KnonwDetectedTarget.transform.position);
                    break;
            }
        }

        //상태 변경에 따른 구현
        protected override void UpdateAiStateTransition()
        {
            switch (AiMobileState)
            {
                case AIMobileState.Patrol:
                    break;
                case AIMobileState.Follow:
                    if (enemyController.IsSeeingTarget && enemyController.IsTargetInAttackRange)
                    {
                        AiMobileState = AIMobileState.Attack;
                        enemyController.SetNavDestination(transform.position);  //정지
                    }
                    break;
                case AIMobileState.Attack:
                    if (enemyController.IsTargetInAttackRange == false)
                    {
                        AiMobileState = AIMobileState.Follow;
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
            if (AiMobileState == AIMobileState.Patrol)
            {
                AiMobileState = AIMobileState.Follow;
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
            animator.SetBool(k_AnimAlertedParameter, true);
        }

        protected override void OnLost()
        {
            //상태변경
            if (AiMobileState == AIMobileState.Follow || AiMobileState == AIMobileState.Attack)
            {
                AiMobileState = AIMobileState.Patrol;
            }
            //Vfx
            for (int i = 0; i < detectedVfxs.Length; i++)
            {
                detectedVfxs[i].Stop();
            }

            //anim
            animator.SetBool(k_AnimAlertedParameter, false);
        }
        protected override void Attacked()
        {
            //애니
            animator.SetTrigger(k_AnimAttackParameter);
        }
    }
}
