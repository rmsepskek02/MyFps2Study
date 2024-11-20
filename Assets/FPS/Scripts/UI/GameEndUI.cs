using System.Collections;
using System.Collections.Generic;
using Unity.FPS.Utillity;
using UnityEngine;

namespace Unity.FPS.UI
{
    /// <summary>
    /// 게임 종료 후 나오는 Scene의 UI 관리
    /// </summary>
    public class GameEndUI : MonoBehaviour
    {
        public GameObject restartButton;
        public GameObject menuButton;
        public GameObject fader;
        private SceneFader sencefader;
        [SerializeField] private string mainScene = "MainScene";
        // Start is called before the first frame update
        void Start()
        {
            sencefader = fader.GetComponent<SceneFader>();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // Update is called once per frame
        void Update()
        {

        }
        public void OnClickRestart()
        {
            sencefader.FadeTo(mainScene);
        }
        public void OnClickMenu()
        {
            Debug.Log("Menu");
        }
    }

}
