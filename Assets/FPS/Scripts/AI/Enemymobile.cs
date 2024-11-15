using System.Collections;
using System.Collections.Generic;
using Unity.FPS.Game;
using UnityEngine;

namespace Unity.FPS.AI
{
    /// <summary>
    /// Enemy 상태
    /// </summary>
    public enum AIState
    {
        Patrol,
        Follow,
        Attack,
    }

    /// <summary>
    /// 이동하는 Enemy의 상태들을 구현하는 클래스
    /// </summary>
    public class Enemymobile : MonoBehaviour
    {
        #region Variables
        public Animator animator;

        private EnemyController enemyController;

        public AIState AiState { get; private set; }
        public AudioClip movementSound;
        public MinMaxFloat pitchMovementSpeed;

        private AudioSource audioSource;

        //데미지 - 이펙트
        public ParticleSystem[] randomHitSparks;

        //animation parameter
        const string k_AnimAttackParameter = "Attack";
        const string k_AnimMoveSpeedParameter = "MoveSpeed";
        const string k_AnimAlertedParameter = "Alert";
        const string k_AnimOnDamagedParameter = "OnDamaged";
        const string k_AnimDeathParameter = "Death";
        #endregion
        // Start is called before the first frame update
        void Start()
        {
            enemyController = GetComponent<EnemyController>();
            enemyController.Damaged += OnDamaged;

            audioSource = GetComponent<AudioSource>();
            audioSource.clip = movementSound;
            audioSource.Play();

            AiState = AIState.Patrol;
        }

        // Update is called once per frame
        void Update()
        {
            //상태구현
            UpdateCurrentAiState();

            //속도에 따른 애니/사운드 효과
            float moveSpeed = enemyController.Agent.velocity.magnitude;
            animator.SetFloat(k_AnimMoveSpeedParameter, moveSpeed);
            audioSource.pitch = pitchMovementSpeed.GetValueFromRatio(moveSpeed/enemyController.Agent.speed);
        }

        //상태에 따른 Enemy 구현
        private void UpdateCurrentAiState()
        {
            switch (AiState)
            {
                case AIState.Patrol:
                    enemyController.UpdatePathDestination(true);
                    enemyController.SetNavDestination(enemyController.GetDestinationOnPath());
                    break;
                case AIState.Attack:
                    break;
                case AIState.Follow:
                    break;
            }
        }

        private void OnDamaged()
        {
            //스파크 파티클
            if(randomHitSparks.Length > 0)
            {
                int random = Random.Range(0, randomHitSparks.Length);
                randomHitSparks[random].Play();
            }

            //데미지 애니
            animator.SetTrigger(k_AnimOnDamagedParameter);
        }
    }
}
