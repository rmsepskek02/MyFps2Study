using Unity.FPS.Game;
using UnityEngine;

namespace Unity.FPS.AI
{
    /// <summary>
    /// EnemyMobile ����  
    /// </summary>
    public enum AIMobileState
    {
        Patrol,
        Follow,
        Attack
    }

    /// <summary>
    /// �̵��ϴ� Enemy�� ���µ��� �����ϴ� Ŭ����
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

            //�ʱ�ȭ
            AiMobileState = AIMobileState.Patrol;
        }

        protected override void Update()
        {
            base.Update();

            //�ӵ��� ���� �ִ�/���� ȿ��
            float moveSpeed = enemyController.Agent.velocity.magnitude;
            animator.SetFloat(k_AnimMoveSpeedParameter, moveSpeed);         //�ִ�
            audioSource.pitch = pitchMovenemtSpeed.GetValueFromRatio(moveSpeed / enemyController.Agent.speed);
        }

        //���¿� ���� Enemy ����
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
                    //�����Ÿ������� �̵��ϸ鼭 ����
                    float distance = Vector3.Distance(enemyController.KnonwDetectedTarget.transform.position, enemyController.DetectionModule.detectionSourcePoint.position);
                    if(distance >= enemyController.DetectionModule.attackRange * attackSkipDistanceRatio)
                    {
                        enemyController.SetNavDestination   (enemyController.KnonwDetectedTarget.transform.position);
                    }
                    else
                    {
                        enemyController.SetNavDestination(transform.position);  //���ڸ��� ����
                    }
                    enemyController.OrientToward(enemyController.KnonwDetectedTarget.transform.position);
                    enemyController.OrientWeaponsToward(enemyController.KnonwDetectedTarget.transform.position);
                    enemyController.TryAttack(enemyController.KnonwDetectedTarget.transform.position);
                    break;
            }
        }

        //���� ���濡 ���� ����
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
                        enemyController.SetNavDestination(transform.position);  //����
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
            //���� ����
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
            //���º���
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
            //�ִ�
            animator.SetTrigger(k_AnimAttackParameter);
        }
    }
}
