using Unity.FPS.Game;
using UnityEngine;

namespace Unity.FPS.AI
{
    /// <summary>
    /// EnemyStand ����
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

        //������ - ����Ʈ
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
            //����            
            enemyController = GetComponent<EnemyController>();
            enemyController.Damaged += OnDamaged;
            enemyController.OnDetectedTarget += OnDetected;
            enemyController.OnLostTarget += OnLost;
            enemyController.OnAttack += Attacked;
        }

        // Update is called once per frame
        void Update()
        {
            //���� ����/����
            UpdateAiStandStateTransition();
            UpdateCurrentAiStandState();
        }

        //���¿� ���� Enemy ����
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

        //���� ���濡 ���� ����
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
            //���º���
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
            //�ִ�
            animator.SetTrigger(k_AnimActiveParameter);
        }
    }
}
