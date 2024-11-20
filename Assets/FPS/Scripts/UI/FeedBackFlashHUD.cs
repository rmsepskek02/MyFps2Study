using System.Collections;
using System.Collections.Generic;
using Unity.FPS.Game;
using Unity.FPS.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.FPS.UI
{
    /// <summary>
    /// 데미지, 힐 플레시 효과 구현
    /// </summary>
    public class FeedBackFlashHUD : MonoBehaviour
    {
        #region Variables
        private Health playerHealth;

        public Image flashImage;
        public CanvasGroup flashCanvasGroup;

        public Color damageFlashColor;
        public Color healFlashColor;

        [SerializeField] private float flashDuration = 1f;
        [SerializeField] private float flashMaxAlpha = 1f;

        private bool flashActive = false;
        private float lastTimeFlashStarted = Mathf.NegativeInfinity;
        #endregion

        // Start is called before the first frame update
        void Start()
        {
            //참조
            PlayerCharacterController playerCharacterController = GameObject.FindObjectOfType<PlayerCharacterController>();

            playerHealth = playerCharacterController.GetComponent<PlayerHealth>();
            playerHealth.OnDamaged += OnDamaged;
            playerHealth.OnHeal += OnHeal;
        }

        // Update is called once per frame
        void Update()
        {
            if (flashActive)
            {
                float normalizedTimeSinceDamage = (Time.time - lastTimeFlashStarted) / flashDuration;
                if(normalizedTimeSinceDamage < 1)
                {
                    float flashAmount = flashMaxAlpha * (1f - normalizedTimeSinceDamage);
                    flashCanvasGroup.alpha = flashAmount;
                }
                else
                {
                    flashCanvasGroup.gameObject.SetActive(false);
                    flashActive = false;
                }
            }
        }

        //효과 초기화
        void ResetFlash()
        {
            flashActive = true;
            lastTimeFlashStarted = Time.time;       //효과 시작 시간
            flashCanvasGroup.alpha = 0f;
            flashCanvasGroup.gameObject.SetActive(true);
        }

        //데미지 입을 때 데미지 플레시 시작
        void OnDamaged(float damage, GameObject damageSource)
        {
            ResetFlash();
            flashImage.color = damageFlashColor;
        }
        //힐 할때 힐 플레시 시작
        void OnHeal(float amount)
        {
            ResetFlash();
            flashImage.color = healFlashColor;
        }

    }
}
