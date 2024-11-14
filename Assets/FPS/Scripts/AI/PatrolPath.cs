using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Unity.FPS.AI
{
    /// <summary>
    /// ��Ʈ�� Waypoint�� �����ϴ� Ŭ����
    /// </summary>
    public class PatrolPath : MonoBehaviour
    {
        #region Variables
        public List<Transform> wayPoints = new List<Transform>();

        //this Path�� ��Ʈ�� �ϴ� enemy��
        public List<EnemyController> enemiesToAssign = new List<EnemyController>();
        #endregion
        //Ư��(Enemy) ��ġ�κ��� ������ wayPoint���� �Ÿ� ���ϱ�
        public float GetDistanceToWayPoint(Vector3 origin, int wayPointIndex)
        {
            if (wayPointIndex < 0 || wayPointIndex >= wayPoints.Count || wayPoints[wayPointIndex] == null)
            {
                return -1f;
            }

            return (wayPoints[wayPointIndex].position - origin).magnitude;
        }

        //index�� ������ WayPoint�� ��ġ ��ȯ
        public Vector3 GetPositionOfWayPoint(int wayPointIndex)
        {
            if (wayPointIndex < 0 || wayPointIndex >= wayPoints.Count || wayPoints[wayPointIndex] == null)
            {
                return Vector3.zero;
            }
            return wayPoints[wayPointIndex].position;
        }
        // Start is called before the first frame update
        void Start()
        {
            //��ϵ� enemy���� ��Ʈ���� path ����
            foreach (var enemy in enemiesToAssign)
            {
                enemy.PatrolPath = this;
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            for (int i = 0; i < wayPoints.Count; i++)
            {
                int nextIndex = i + 1;
                if (nextIndex >= wayPoints.Count)
                {
                    nextIndex = 0;
                }

                Gizmos.DrawLine(wayPoints[i].position, wayPoints[nextIndex].position);
                Gizmos.DrawSphere(wayPoints[i].position, 0.1f);
            }
        }
    }
}
