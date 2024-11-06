using System.Collections;
using System.Collections.Generic;
using Unity.FPS.Game;
using Unity.FPS.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.FPS.UI
{
    public class CrosshairManager : MonoBehaviour
    {
        #region Variables
        public Image crosshairImage;
        public Sprite nullCrosshairSprite;

        private RectTransform crosshairRectTransform;
        private CrossHairData crosshairDefalut;     //����, �⺻
        private CrossHairData crosshairTarget;      //Ÿ���� �Ǿ�����
        private CrossHairData crosshairCurrent;     //���������� �׸��� ũ�ν����
        [SerializeField]private float crosshairUpdateSharpness = 5.0f; // Lerp
        //public Transform player;
        private PlayerWeaponsManager pw;
        private bool wasPointingAtEnemy;
        #endregion
        // Start is called before the first frame update
        void Start()
        {
            //pw = player.GetComponent<PlayerWeaponsManager>();
            pw = FindObjectOfType<PlayerWeaponsManager>();
            SetCrossHair(pw.GetActiveWeapon());

            pw.OnSwitchToWeapon += SetCrossHair;
        }

        // Update is called once per frame
        void Update()
        {
            UpdateCrosshairPointingAtEnemy(false);

            wasPointingAtEnemy = pw.IsPointingAtEnemy;
        }
        void UpdateCrosshairPointingAtEnemy(bool force)
        {
            if(crosshairDefalut.CrosshairSprite == null)
                return;

            // ���� �����ϴ� ����
            if((force ||wasPointingAtEnemy == false) && pw.IsPointingAtEnemy == true)
            {
                crosshairCurrent = crosshairTarget;
                crosshairImage.sprite = crosshairCurrent.CrosshairSprite;
                crosshairRectTransform.sizeDelta = crosshairCurrent.CrosshairSize * Vector2.one;
            }
            // ���� ��ġ�� ����
            else if ((force || wasPointingAtEnemy == true) && pw.IsPointingAtEnemy == false)
            {
                crosshairCurrent = crosshairDefalut;
                crosshairImage.sprite = crosshairCurrent.CrosshairSprite;
                crosshairRectTransform.sizeDelta = crosshairCurrent.CrosshairSize * Vector2.one;
            }
            crosshairImage.color = Color.Lerp(crosshairImage.color, crosshairCurrent.CrosshairColor,crosshairUpdateSharpness * Time.deltaTime);
            crosshairRectTransform.sizeDelta = Mathf.Lerp(crosshairRectTransform.sizeDelta.x, crosshairCurrent.CrosshairSize, crosshairUpdateSharpness * Time.deltaTime) *Vector2.one;
        }
        void SetCrossHair(WeaponController newWeapon)
        {
            if(newWeapon)
            {
                crosshairImage.enabled = true;
                crosshairRectTransform = crosshairImage.GetComponent<RectTransform>();
                crosshairDefalut = newWeapon.crosshairDefault;
                crosshairTarget = newWeapon.crosshairTargetInSight;
            }
            else
            {
                if (nullCrosshairSprite)
                {
                    crosshairImage.enabled = true;
                    crosshairImage.sprite = nullCrosshairSprite;
                }
                else
                {
                    crosshairImage.enabled = false;
                }
            }
            UpdateCrosshairPointingAtEnemy(true);
        }
    }
}
