using UnityEngine;

namespace Unity.FPS.Game
{
    /// <summary>
    /// TimeSelfDestruct ������ ���� ������Ʈ�� ���� �� ������ �ð��� ų 
    /// </summary>
    public class TimeSelfDestruct : MonoBehaviour
    {
        #region Variables
        public float lifeTime = 1f;
        private float spawnTime;        //�����ɶ��� �ð�
        #endregion

        private void Awake()
        {
            //���� �ð��� ����
            spawnTime = Time.time;
        }

        private void Update()
        {
            if((spawnTime + lifeTime) <= Time.time )
            {
                Destroy(gameObject);
            }
        }
    }
}
