using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.FPS.Utillity;
using UnityEngine;

namespace Unity.FPS.AI
{
    /// <summary>
    /// Enemy ����Ʈ�� �����ϴ� Ŭ����
    /// </summary>
    public class EnemyManager : MonoBehaviour
    {
        #region Variables
        public List<EnemyController> Enemies { get; private set; }
        public int NumberOfEnemiesTotal { get; private set; }   //�� ����� Enemy ��
        public int NumberOfEnemiesRemaining => Enemies.Count;   //���� ����ִ� Enemy ���� ��
        public GameObject fader;
        private SceneFader sencefader;
        EnemyStand es;
        public int bossCount;
        [SerializeField] private string winScene = "WinScene";
        #endregion

        private void Awake()
        {
            Enemies = new List<EnemyController>();
            sencefader = fader.GetComponent<SceneFader>();
        }

        // Start is called before the first frame update
        void Start()
        {
            bossCount = Enemies.Where(e => e.GetComponent<EnemyStand>() != null).Count();
        }

        // Update is called once per frame
        void Update()
        {

        }

        //���
        public void RegisterEnemy(EnemyController newEnemy)
        {
            Enemies.Add(newEnemy);
            NumberOfEnemiesTotal++;
        }

        //����
        public void RemoveEnemy(EnemyController killedEnemy)
        {
            Enemies.Remove(killedEnemy);
            bossCount = Enemies.Where(e => e.GetComponent<EnemyStand>() != null).Count();
            CheckBossCount();
            CheckEnemyCount();
        }

        void CheckEnemyCount()
        {
            if (NumberOfEnemiesRemaining <= 0)
            {
                sencefader.FadeTo(winScene);
            }
        }

        void CheckBossCount()
        {
            if (bossCount <= 0)
            {
                sencefader.FadeTo(winScene);
            }
        }
    }
}
