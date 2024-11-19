using System.Collections;
using System.Collections.Generic;
using Unity.FPS.Game;
using Unity.FPS.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.FPS.UI
{

    public class PlayerHealthBar : MonoBehaviour
    {
        #region Variables
        private Health playerHealth;
        public Image healthFillImage;
        #endregion
        // Start is called before the first frame update
        void Start()
        {
            //ÂüÁ¶
            PlayerCharacterController playerCharacterController = GameObject.FindObjectOfType<PlayerCharacterController>();

            playerHealth = playerCharacterController.GetComponent<Health>();
        }

        // Update is called once per frame
        void Update()
        {
            healthFillImage.fillAmount = playerHealth.GetRatio();
        }

        
    }
}
