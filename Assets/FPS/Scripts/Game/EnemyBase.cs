using Unity.FPS.Game;
using UnityEngine;

namespace Unity.FPS.AI
{
    /// <summary>
    /// Enemy�� ������� ����
    /// </summary>

    public abstract class EnemyBase: MonoBehaviour
    {
        #region Variables
        public Animator animator;
        protected EnemyController enemyController;

        //�̵�
        public AudioClip movementSound;
        public MinMaxFloat pitchMovenemtSpeed;

        protected AudioSource audioSource;

        //������ - ����Ʈ
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
            //����            
            enemyController = GetComponent<EnemyController>();
            enemyController.Damaged += OnDamaged;
            enemyController.OnDetectedTarget += OnDetected;
            enemyController.OnLostTarget += OnLost;
            enemyController.OnAttack += Attacked;
        }

        protected virtual void Update()
        {
            //���� ����/����
            UpdateAiStateTransition();
            UpdateCurrentAiState();
        }

        //���¿� ���� Enemy ����
        protected abstract void UpdateCurrentAiState();

        //���� ���濡 ���� ����
        protected abstract void UpdateAiStateTransition();

        protected virtual void OnDamaged()
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

        protected abstract void OnDetected();
        protected abstract void OnLost();
        protected abstract void Attacked();
    }
}
