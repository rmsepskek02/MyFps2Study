using System.Collections;
using System.Collections.Generic;
using Unity.FPS.AI;
using UnityEngine;
using UnityEngine.Events;

namespace Unity.FPS.Game
{
    /// <summary>
    /// Enemy Spawn �����ϴ� �Ŵ���
    /// </summary>
    public class SpawnManager : MonoBehaviour
    {
        public static UnityAction<EnemyController> OnSpawn;
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
            randX = Random.Range(-spawnRange, spawnRange);
            randZ = Random.Range(-spawnRange, spawnRange);
            randPos = new Vector3(randX, 0f, randZ);
        }

        // Update is called once per frame
        void Update()
        {
            //maxCount �˻�
            if (em.Enemies.Count < maxEnemyCount)
            {
                spawnTime += Time.deltaTime;
            }

            // Update���� �׾����� ��������Ʈ�� �ѹ� ȣ���ϸ� ������
            SpawnEnemy();
        }

        public void SpawnEnemy()
        {
            //Spawn Delay
            if (spawnTime > spawnDelayTime)
            {
                //Spawn
                GameObject go = Instantiate(enemyObject, enemies);
                OnSpawn?.Invoke(go.GetComponent<EnemyController>());
                go.transform.position = randPos;

                spawnTime = 0;
            }
        }

        // ����� �׸���
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;

            // ũ�Ⱑ 10x10x10�� ť��
            Vector3 cubeSize = new Vector3(20f, 1f, 20f);
            Gizmos.DrawWireCube(transform.position, cubeSize);
        }
    }
}
