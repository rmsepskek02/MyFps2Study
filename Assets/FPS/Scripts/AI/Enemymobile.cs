using Unity.FPS.Game;
using UnityEngine;

namespace Unity.FPS.AI
{
    /// <summary>
    /// Enemy ����  
    /// </summary>
    public enum AIState
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

        public AIState AiState { get; private set; }

        //�̵�
        public AudioClip movementSound;
        public MinMaxFloat pitchMovenemtSpeed;

        private AudioSource audioSource;

        //������ - ����Ʈ
        public ParticleSystem[] randomHitSparks;

        //Detected
        public ParticleSystem[] detectedVfxs;
        public AudioClip detectedSfx;

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

            audioSource = GetComponent<AudioSource>();
            audioSource.clip = movementSound;
            audioSource.Play();

            //�ʱ�ȭ
            AiState = AIState.Patrol;
        }

        private void Update()
        {
            //���� ����/����
            UpdateAiStateTransition();
            UpdateCurrentAiState();

            //�ӵ��� ���� �ִ�/���� ȿ��
            float moveSpeed = enemyController.Agent.velocity.magnitude;
            animator.SetFloat(k_AnimMoveSpeedParameter, moveSpeed);         //�ִ�
            audioSource.pitch = pitchMovenemtSpeed.GetValueFromRatio(moveSpeed/enemyController.Agent.speed);
        }

        //���¿� ���� Enemy ����
        private void UpdateCurrentAiState()
        {
            switch(AiState)
            {
                case AIState.Patrol:
                    enemyController.UpdatePathDestination(true);
                    enemyController.SetNavDestination(enemyController.GetDestinationOnPath());
                    break;
                case AIState.Follow:
                    enemyController.SetNavDestination(enemyController.KnonwDetectedTarget.transform.position);
                    enemyController.OrientToward(enemyController.KnonwDetectedTarget.transform.position);
                    enemyController.OrientWeaponsToward(enemyController.KnonwDetectedTarget.transform.position);
                    break;
                case AIState.Attack:
                    enemyController.OrientToward(enemyController.KnonwDetectedTarget.transform.position);
                    enemyController.OrientWeaponsToward(enemyController.KnonwDetectedTarget.transform.position);
                    enemyController.TryAttack(enemyController.KnonwDetectedTarget.transform.position);
                    break;
            }
        }

        //���� ���濡 ���� ����
        private void UpdateAiStateTransition()
        {
            switch (AiState)
            {
                case AIState.Patrol: 
                    break;
                case AIState.Follow:
                    if(enemyController.IsSeeingTarget && enemyController.IsTargetInAttackRange)
                    {
                        AiState = AIState.Attack;
                        enemyController.SetNavDestination(transform.position);  //����
                    }
                    break;
                case AIState.Attack:
                    if (enemyController.IsTargetInAttackRange == false)
                    {
                        AiState = AIState.Follow;
                    }
                    break;
            }
        }

        private void OnDamaged()
        {
            //����ũ ��ƼŬ - �����ϰ� �ϳ� �����ؼ� �÷���
            if(randomHitSparks.Length > 0)
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
            AiState = AIState.Follow;

            //Vfx
            for (int i = 0; i < detectedVfxs.Length; i++)
            {
                detectedVfxs[i].Play();
            }

            //Sfx
            if(detectedSfx)
            {
                AudioUtility.CreateSfx(detectedSfx, this.transform.position, 1f);
            }

            //anim
            animator.SetBool(k_AnimAlertedParameter, true);
        }

        private void OnLost()
        {
            //Vfx
            for (int i = 0; i < detectedVfxs.Length; i++)
            {
                detectedVfxs[i].Stop();
            }

            //anim
            animator.SetBool(k_AnimAlertedParameter, false);
        }

    }
}
