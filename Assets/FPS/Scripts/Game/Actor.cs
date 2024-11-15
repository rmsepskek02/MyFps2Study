using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.FPS.Game
{
    /// <summary>
    /// 게임에 등장하는 Actor
    /// </summary>
    public class Actor : MonoBehaviour
    {
        #region Variables
        // 소속 - 아군, 적군 구분
        public int affiliation;
        // 조준점
        public Transform aimPoint;
        private ActorManager actorManager;
        #endregion

        // Start is called before the first frame update

        void Start()
        {
            //Actor 리스트에 등록
            actorManager = GameObject.FindAnyObjectByType<ActorManager>();
            //리스트에 포함되어 있는지 체크
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
            //Actor 리스트에서 삭제
            if(actorManager != null)
            {
                actorManager.Actors.Remove(this);
            }
        }
    }
}