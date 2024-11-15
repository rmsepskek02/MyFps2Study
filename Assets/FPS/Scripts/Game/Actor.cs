using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.FPS.Game
{
    /// <summary>
    /// ���ӿ� �����ϴ� Actor
    /// </summary>
    public class Actor : MonoBehaviour
    {
        #region Variables
        // �Ҽ� - �Ʊ�, ���� ����
        public int affiliation;
        // ������
        public Transform aimPoint;
        private ActorManager actorManager;
        #endregion

        // Start is called before the first frame update

        void Start()
        {
            //Actor ����Ʈ�� ���
            actorManager = GameObject.FindAnyObjectByType<ActorManager>();
            //����Ʈ�� ���ԵǾ� �ִ��� üũ
            if (actorManager.Actors.Contains(this) == false)
            {
                actorManager.Actors.Add(this);
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnDestroy()
        {
            //Actor ����Ʈ���� ����
            if(actorManager != null)
            {
                actorManager.Actors.Remove(this);
            }
        }
    }
}