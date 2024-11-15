using System.Collections;
using System.Collections.Generic;
using Unity.FPS.Game;
using UnityEngine;
using UnityEngine.Events;

namespace Unity.FPS.AI
{
    /// <summary>
    /// ���� ������ ����
    /// </summary>
    public class DetectionModule : MonoBehaviour
    {
        #region Variables
        private ActorManager actorManager;

        public UnityAction OnDetectedTarget;        //���� �����ϸ� ��ϵ� �Լ� ȣ��
        public UnityAction OnLostTarget;            //���� ��ġ�� ��ϵ� �Լ� ȣ��
        #endregion

        // Start is called before the first frame update
        void Start()
        {
            //����
            actorManager = GameObject.FindAnyObjectByType<ActorManager>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        //������
        public void HandleTargetDetection(Actor actor, Collider[] selfCollierd)
        {

        }

        //���� �����ϸ�
        public void OnDetect()
        {
            OnDetectedTarget?.Invoke();
        }

        //���� ��ġ��
        public void OnLost()
        {
            OnLostTarget?.Invoke();
        }
    }
}
