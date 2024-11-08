using Unity.FPS.Gameplay;
using UnityEngine;

namespace Unity.FPS.UI
{
    public class ScopeUIManager : MonoBehaviour
    {
        #region Variables
        public GameObject scopeUI;

        private PlayerWeaponsManager weaponsManager;
        #endregion

        private void Start()
        {
            //����
            weaponsManager = GameObject.FindObjectOfType<PlayerWeaponsManager>();
            //Action �Լ� ���
            weaponsManager.OnScopedWeapon += OnScope;
            weaponsManager.OffScopedWeapon += OffScope;
        }

        public void OnScope()
        {
            scopeUI.SetActive(true);
        }

        public void OffScope()
        { 
            scopeUI.SetActive(false);
        }
    }
}