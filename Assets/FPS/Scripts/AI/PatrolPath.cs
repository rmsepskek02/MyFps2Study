using System.Collections.Generic;
using UnityEngine;

namespace Unity.FPS.AI
{
    /// <summary>
    /// ��Ʈ�� Waypoint���� �����ϴ� Ŭ����
    /// </summary>
    public class PatrolPath : MonoBehaviour
    {
        #region Variables
        public List<Transform> wayPoints = new List<Transform>();

        //this Path�� ��Ʈ���ϴ� enemy��
        public List<EnemyController> enemiesToAssign = new List<EnemyController>();
        #endregion

        private void Start()
        {
            //��ϵ� enemy���� ��Ʈ���� �н�(this) ����
            foreach (var enemy in enemiesToAssign)
            {
                enemy.PatrolPath = this;
            }
        }

        //Ư��(enemy) ��ġ�� ���� ������ WayPoint���� �Ÿ� ���ϱ�
        public float GetDistanceToWayPoint(Vector3 origin, int wayPointIndex)
        {
            if (wayPointIndex < 0 || wayPointIndex >= wayPoints.Count
                || wayPoints[wayPointIndex] == null)
            {
                return -1f;
            }

            return (wayPoints[wayPointIndex].position - origin).magnitude;
        }

        //index�� ������ WayPoint�� ��ġ ��ȯ
        public Vector3 GetPositionOfWayPoint(int wayPointIndex)
        {
            if (wayPointIndex < 0 || wayPointIndex >= wayPoints.Count
                || wayPoints[wayPointIndex] == null)
            {
                return Vector3.zero;
            }

            return wayPoints[wayPointIndex].position;
        }



        //������ Path �׸���
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            for (int i = 0; i < wayPoints.Count; i++)
            {
                int nextIndex = i + 1;
                if(nextIndex >= wayPoints.Count)
                {
                    nextIndex -= wayPoints.Count;
                }
                Gizmos.DrawLine(wayPoints[i].position, wayPoints[nextIndex].position);
                Gizmos.DrawSphere(wayPoints[i].position, 0.1f);
            }
        }

    }
}