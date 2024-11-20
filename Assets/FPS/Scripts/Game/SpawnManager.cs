using System.Collections;
using System.Collections.Generic;
using Unity.FPS.AI;
using UnityEngine;
using UnityEngine.Events;

namespace Unity.FPS.Game
{
    /// <summary>
    /// Enemy Spawn 관리하는 매니저
    /// </summary>
    public class SpawnManager : MonoBehaviour
    {
        public UnityAction<EnemyController> OnSpawn;
        public GameObject enemyObject;
        public Transform enemies;
        [SerializeField] float maxEnemyCount;
        [SerializeField] float spawnDelayTime;
        [SerializeField] float spawnRange;
        EnemyManager em;
        float spawnTime;
        float randX;
        float randZ;
        Vector3 randPos = new Vector3();
        // Start is called before the first frame update
        void Start()
        {
            em = GameObject.FindObjectOfType<EnemyManager>();
        }

        // Update is called once per frame
        void Update()
        {
            //maxCount 검사
            if (em.Enemies.Count - em.bossCount < maxEnemyCount)
            {
                spawnTime += Time.deltaTime;
            }

            // Update보다 죽었을때 델리게이트로 한번 호출하면 좋을듯
            SpawnEnemy();
        }

        public void SpawnEnemy()
        {
            //Spawn Delay
            if (spawnTime > spawnDelayTime)
            {
                //Spawn
                GameObject go = Instantiate(enemyObject);
                //GameObject go = Instantiate(enemyObject, enemies);
                OnSpawn?.Invoke(go.GetComponent<EnemyController>());
                randX = Random.Range(-spawnRange, spawnRange);
                randZ = Random.Range(-spawnRange, spawnRange);
                randPos = new Vector3(randX, 0f, randZ);
                go.transform.position = randPos;

                spawnTime = 0;
            }
        }

        // 기즈모 그리기
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;

            Vector3 cubeSize = new Vector3(spawnRange * 2, 1f, spawnRange * 2);
            Gizmos.DrawWireCube(transform.position, cubeSize);
        }
    }
}
