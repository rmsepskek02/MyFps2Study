using System.Collections;
using System.Collections.Generic;
using Unity.FPS.Game;
using UnityEngine;
using UnityEngine.Events;

namespace Unity.FPS.AI
{
    /// <summary>
    /// 적군 디텍팅 구현
    /// </summary>
    public class DetectionModule : MonoBehaviour
    {
        #region Variables
        private ActorManager actorManager;

        public UnityAction OnDetectedTarget;        //적을 감지하면 등록된 함수 호출
        public UnityAction OnLostTarget;            //적을 놓치면 등록된 함수 호출
        #endregion

        // Start is called before the first frame update
        void Start()
        {
            //참조
            actorManager = GameObject.FindAnyObjectByType<ActorManager>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        //디텍팅
        public void HandleTargetDetection(Actor actor, Collider[] selfCollierd)
        {

        }

        //적을 감지하면
        public void OnDetect()
        {
            OnDetectedTarget?.Invoke();
        }

        //적을 놓치면
        public void OnLost()
        {
            OnLostTarget?.Invoke();
        }
    }
}
