using System.Collections;
using System.Collections.Generic;
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
        #endregion

        private void Awake()
        {
            Enemies = new List<EnemyController>();
        }

        // Start is called before the first frame update
        void Start()
        {

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
        }
    }
}
