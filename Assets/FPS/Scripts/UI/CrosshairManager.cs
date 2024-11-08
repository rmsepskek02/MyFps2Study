using UnityEngine;
using UnityEngine.UI;
using Unity.FPS.Game;
using Unity.FPS.Gameplay;

namespace Unity.FPS.UI
{
    public class CrosshairManager : MonoBehaviour
    {
        #region Variables
        public Image crosshairImage;                    //ũ�ν���� UI �̹���
        public Sprite nullCrosshairSprite;              //��Ƽ���� ���Ⱑ ������

        private RectTransform crosshairRectTransform;

        private CrossHairData crosshairDefault;         //����, �⺻
        private CrossHairData crosshairTarget;          //Ÿ���� �Ǿ�����

        private CrossHairData crosshairCurrent;         //���������� �׸��� ũ�ν����
        [SerializeField] private float crosshairUpdateShrpness = 5.0f;   //Lerp ����

        private PlayerWeaponsManager weaponsManager;

        private bool wasPointingAtEnemy;
        #endregion

        private void Start()
        {
            //����
            weaponsManager = GameObject.FindObjectOfType<PlayerWeaponsManager>();
            //��Ƽ���� ���� ũ�ν� ��� ���̱�
            OnWeaponChanged(weaponsManager.GetActiveWeapon());

            weaponsManager.OnSwitchToWeapon += OnWeaponChanged;
        }

        private void Update()
        {
            UpdateCrosshairPointingAtEnemy(false);

            wasPointingAtEnemy = weaponsManager.IsPointingAtEnemy;
        }

        //ũ�ν� ��� �׸���
        void UpdateCrosshairPointingAtEnemy(bool force)
        {
            if (crosshairDefault.CrossHairSprite == null)
                return;

            //����?, Ÿ����?
            if((force || wasPointingAtEnemy == false) && weaponsManager.IsPointingAtEnemy == true) //���� �����ϴ� ����
            {
                crosshairCurrent = crosshairTarget;
                crosshairImage.sprite = crosshairCurrent.CrossHairSprite;
                crosshairRectTransform.sizeDelta = crosshairCurrent.CrossHairSize * Vector2.one;
            }
            else if ((force || wasPointingAtEnemy == true) &&  weaponsManager.IsPointingAtEnemy == false) //���� ��ġ�� ����
            {
                crosshairCurrent = crosshairDefault;
                crosshairImage.sprite = crosshairCurrent.CrossHairSprite;
                crosshairRectTransform.sizeDelta = crosshairCurrent.CrossHairSize * Vector2.one;
            }

            crosshairImage.color = Color.Lerp(crosshairImage.color, crosshairCurrent.CrossHairColor,
                crosshairUpdateShrpness * Time.deltaTime);
            crosshairRectTransform.sizeDelta = Mathf.Lerp(crosshairRectTransform.sizeDelta.x, crosshairCurrent.CrossHairSize,
                crosshairUpdateShrpness * Time.deltaTime) * Vector2.one;
        }

        //���Ⱑ �ٲ𶧸��� crosshairImage�� ������ ���� CrossHair�̹����� �ٲٱ�
        void OnWeaponChanged(WeaponController newWeapon)
        {
            if(newWeapon)
            {
                crosshairImage.enabled = true;
                crosshairRectTransform = crosshairImage.GetComponent<RectTransform>();

                //��Ƽ�� ������ ũ�ν���� ���� ��������
                crosshairDefault = newWeapon.crosshairDefault;
                crosshairTarget = newWeapon.crosshairTargetInSight;                
            }
            else
            {
                if(nullCrosshairSprite)
                {
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