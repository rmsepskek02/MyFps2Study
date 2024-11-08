using UnityEngine;
using Unity.FPS.Game;
using Unity.FPS.Gameplay;

namespace Unity.FPS.UI
{
    public class WeaponHUDManager : MonoBehaviour
    {
        #region Variables
        public RectTransform ammoPanel;             //ammoCountUI �θ� ������Ʈ
        public GameObject ammoCountPrefab;          //ammoCountUI ������

        private PlayerWeaponsManager playerWeaponsManager;
        #endregion

        private void Awake()
        {
            //����
            playerWeaponsManager = GameObject.FindObjectOfType<PlayerWeaponsManager>();

            playerWeaponsManager.OnAddedWeapon += AddWeapon;
            playerWeaponsManager.OnRemoveWeapon += RemoveWeapon;
            playerWeaponsManager.OnSwitchToWeapon += SwitchWeapon;
        }

        //�����߰� �ϸ� ammo UI �ϳ� �߰�
        void AddWeapon(WeaponController newWeapon, int weaponIndex)
        {
            GameObject ammoCountGo = Instantiate(ammoCountPrefab, ammoPanel);
            AmmoCount ammoCount = ammoCountGo.GetComponent<AmmoCount>();
            ammoCount.Initialzie(newWeapon, weaponIndex);
        }

        //���� ���� �ϸ� ammo UI �ϳ� ����
        void RemoveWeapon(WeaponController oldWeapon, int weaponIndex)
        {

        }

        //
        void SwitchWeapon(WeaponController weapon)
        {
            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(ammoPanel);
        }
    }
}
