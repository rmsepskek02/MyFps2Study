using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.FPS.Game
{
    /// <summary>
    /// 게임에 등장하는 Actor를 관리하는 클래스
    /// </summary>
    public class ActorManager : MonoBehaviour
    {
        #region Variables
        public List<Actor> Actors { get; private set; }

        public GameObject Player { get; private set; }

        public void SetPlayer(GameObject player) => Player = player;
        #endregion

        private void Awake()
        {
            Actors = new List<Actor>();
        }


        // Update is called once per frame
        void Update()
        {

        }
    }
}
