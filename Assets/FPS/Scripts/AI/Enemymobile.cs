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
    public class EnemyMobile : MonoBehaviour
    {
        #region Variables
        public Animator animator;
        private EnemyController enemyController;

        public AIMobileState AiMobileState { get; private set; }

        //�̵�
        public AudioClip movementSound;
        public MinMaxFloat pitchMovenemtSpeed;

        private AudioSource audioSource;

        //������ - ����Ʈ
        public ParticleSystem[] randomHitSparks;

        //Detected
        public ParticleSystem[] detectedVfxs;
        public AudioClip detectedSfx;

        //attack
        [Range(0f,1f)]
        public float attackSkipDistanceRatio = 0.5f;

        //animation parameter
        const string k_AnimAttackParameter = "Attack";
        const string k_AnimMoveSpeedParameter = "MoveSpeed";
        const string k_AnimAlertedParameter = "Alerted";
        const string k_AnimOnDamagedParameter = "OnDamaged";
        const string k_AnimDeathParameter = "Death";
        #endregion

        private void Start()
        {
            //����            
            enemyController = GetComponent<EnemyController>();
            enemyController.Damaged += OnDamaged;
            enemyController.OnDetectedTarget += OnDetected;
            enemyController.OnLostTarget += OnLost;
            enemyController.OnAttack += Attacked;

            audioSource = GetComponent<AudioSource>();
            audioSource.clip = movementSound;
            audioSource.Play();

            //�ʱ�ȭ
            AiMobileState = AIMobileState.Patrol;
        }

        private void Update()
        {
            //���� ����/����
            UpdateAiMobileStateTransition();
            UpdateCurrentAiMobileState();

            //�ӵ��� ���� �ִ�/���� ȿ��
            float moveSpeed = enemyController.Agent.velocity.magnitude;
            animator.SetFloat(k_AnimMoveSpeedParameter, moveSpeed);         //�ִ�
            audioSource.pitch = pitchMovenemtSpeed.GetValueFromRatio(moveSpeed / enemyController.Agent.speed);
        }

        //���¿� ���� Enemy ����
        private void UpdateCurrentAiMobileState()
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
        private void UpdateAiMobileStateTransition()
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

        private void OnDamaged()
        {
            //����ũ ��ƼŬ - �����ϰ� �ϳ� �����ؼ� �÷���
            if (randomHitSparks.Length > 0)
            {
                int randNum = Random.Range(0, randomHitSparks.Length);
                randomHitSparks[randNum].Play();
            }

            //������ �ִ�
            animator.SetTrigger(k_AnimOnDamagedParameter);
        }

        private void OnDetected()
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

        private void OnLost()
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
        private void Attacked()
        {
            //�ִ�
            animator.SetTrigger(k_AnimAttackParameter);
        }
    }
}
