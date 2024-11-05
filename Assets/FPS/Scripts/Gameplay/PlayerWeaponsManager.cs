using System.Collections;
using System.Collections.Generic;
using Unity.FPS.Game;
using UnityEngine;
using UnityEngine.Events;

namespace Unity.FPS.Gameplay
{
    public enum WeaponSwitchState
    {
        Up,
        Down,
        PutDownPrevious,
        PutUpNew,
    }

    /// <summary>
    /// �÷��̾ ���� ������� �����ϴ� Ŭ����
    /// </summary>
    public class PlayerWeaponsManager : MonoBehaviour
    {
        #region Variables
        //���� ����- ������ �����Ҷ� ó�� �������� ���޵Ǵ� ���� ����Ʈ
        public List<WeaponController> startingWeapons = new List<WeaponController>();

        //���� ����
        //���⸦ �����ϴ� ������Ʈ
        public Transform weaponParentSocket;

        //�÷��̾ �����߿� ��� �ٴϴ� ���� ����Ʈ
        private WeaponController[] weaponSlots = new WeaponController[9];

        //���� ����Ʈ�� �����ϴ� �ε���
        public int ActiveWeaponIndex { get; private set; }

        //���� ��ü
        public UnityAction<WeaponController> OnSwitchToWeapon;

        private WeaponSwitchState weaponSwitchState;     //���� ��ü�� ����

        private PlayerInputHandler playerInputHandler;

        //���� ��ü�� ���Ǵ� ������ġ
        private Vector3 weaponMainLocalPosition;

        public Transform defaultWeaponPosition;
        public Transform downWeaponPosition;

        private int weaponSwitchNewIndex;       // ���� �ٲ�� ���� �ε���

        private float weaponSwitchTimeStart = 0f;
        [SerializeField] private float weaponSwitchDelay = 1f;
        #endregion
        // Start is called before the first frame update
        void Start()
        {
            playerInputHandler = GetComponent<PlayerInputHandler>();

            ActiveWeaponIndex = -1;
            weaponSwitchState = WeaponSwitchState.Down;

            foreach (var weapon in startingWeapons)
            {
                AddWeapon(weapon);
            }

            OnSwitchToWeapon += OnWeaponSwitched;
        }

        // Update is called once per frame
        void Update()
        {
            if(weaponSwitchState == WeaponSwitchState.Up || weaponSwitchState == WeaponSwitchState.Down)
            {
                int switchWeaponInput = playerInputHandler.GetSwitchWeaponInput();
                if(switchWeaponInput != 0)
                {
                    bool switchUp = switchWeaponInput > 0;
                    SwitchWeapon(switchUp);
                }
            }
        }

        private void LateUpdate()
        {
            UpdateWeaponSwitching();

            //���� ���� ��ġ
            weaponParentSocket.localPosition = weaponMainLocalPosition;
        }

        // ���¿� ���� ���� ����
        void UpdateWeaponSwitching()
        {
            //Lerp ����
            float switchingTimeFactor = 0f;
            if(weaponSwitchDelay == 0f)
            {
                switchingTimeFactor = 1f;
            }
            else
            {
                switchingTimeFactor = Mathf.Clamp01((Time.time - weaponSwitchTimeStart) / weaponSwitchDelay);
            }
            // �����ð� �� ���� ��ġ �̵�
            if(switchingTimeFactor >= 1f)
            {
                if(weaponSwitchState == WeaponSwitchState.PutDownPrevious)
                {
                    // ���繫�� false, ���ο� ���� true
                    WeaponController oldWeapon = GetActiveWeapon();
                    if(oldWeapon != null)
                    {
                        oldWeapon.ShowWeapon(false);
                    }
                    ActiveWeaponIndex = weaponSwitchNewIndex;
                    WeaponController newWeapon = GetActiveWeapon();
                    OnSwitchToWeapon?.Invoke(newWeapon);

                    switchingTimeFactor = 0;

                    if (newWeapon != null)
                    {
                        weaponSwitchState = WeaponSwitchState.PutUpNew;
                    }
                    else
                    {
                        weaponSwitchState = WeaponSwitchState.Down;
                    }
                }
                else if(weaponSwitchState == WeaponSwitchState.PutUpNew)
                {
                    weaponSwitchState = WeaponSwitchState.Up;
                }
            }

            //�����ð����� ������ ��ġ �̵�
            if(weaponSwitchState == WeaponSwitchState.PutDownPrevious)
            {
                weaponMainLocalPosition = Vector3.Lerp(defaultWeaponPosition.localPosition, downWeaponPosition.localPosition, switchingTimeFactor);
            }
            else
            {
                weaponMainLocalPosition = Vector3.Lerp(downWeaponPosition.localPosition, defaultWeaponPosition.localPosition, switchingTimeFactor);
            }
        }

        public bool AddWeapon(WeaponController weaponPrefab)
        {
            //�߰��ϴ� ���� ���� ���� üũ
            if(HasWeapon(weaponPrefab) != null)
            {
                Debug.Log("Has Same Weapon");
                return false;
            }
            for (int i = 0; i < weaponSlots.Length; i++)
            {
                if (weaponSlots[i] == null)
                {
                    WeaponController weaponInstance = Instantiate(weaponPrefab, weaponParentSocket);
                    weaponInstance.transform.localPosition = Vector3.zero;
                    weaponInstance.transform.localRotation = Quaternion.identity;

                    weaponInstance.Owner = this.gameObject;
                    weaponInstance.SourcePrefab = weaponPrefab.gameObject;
                    weaponInstance.ShowWeapon(false);

                    weaponSlots[i] = weaponInstance;

                    return true;
                }
            }
            Debug.Log("SLOT FULL");
            return true;
        }

        // �Ű������� ����
        private WeaponController HasWeapon(WeaponController weaponPreFab)
        {
            for(int i=0; i < weaponSlots.Length; i++)
            {
                if(weaponSlots[i] != null && weaponSlots[i].SourcePrefab == weaponPreFab)
                {
                    return weaponSlots[i];
                }
            }

            return null;
        }

        //0~9 0,1,2
        //���� �ٲٱ�, ���� ��� �ִ� ���� false, ���ο� ���� true
        public void SwitchWeapon(bool ascendingOrder)
        {
            int newWeaponIndex = -1;    // ���� ��Ƽ���� ���� �ε���
            int closestSlotDistance = weaponSlots.Length;
            for(int i = 0; i < weaponSlots.Length; i++)
            {
                if(i != ActiveWeaponIndex && GetWeaponAtSlotIndex(i) != null)
                {
                    int distanceToActiveIndex = GetDistanceBetweenWeaponSlot(ActiveWeaponIndex, i, ascendingOrder);
                    if(distanceToActiveIndex < closestSlotDistance)
                    {
                        closestSlotDistance = distanceToActiveIndex;
                        newWeaponIndex = i;
                    }
                }
            }

            SwitchToWeaponIndex(newWeaponIndex);
        }

        public WeaponController GetWeaponAtSlotIndex(int index)
        {
            if(index >= 0 && index < weaponSlots.Length)
            {
                return weaponSlots[index];
            }
            return null;
        }
        
        // ���԰� �Ÿ�
        private int GetDistanceBetweenWeaponSlot(int fromSlotIndex, int toSlotIndex, bool ascendingOrder)
        {
            int distanceBetweenSlots = 0;

            if (ascendingOrder)
            {
                distanceBetweenSlots = toSlotIndex - fromSlotIndex;
            }
            else
            {
                distanceBetweenSlots = fromSlotIndex - toSlotIndex;
            }

            return distanceBetweenSlots;
        }

        private void SwitchToWeaponIndex(int newWeaponIndex)
        {
            //newWeaponIndex �� üũ
            if(newWeaponIndex >= 0 && newWeaponIndex != ActiveWeaponIndex)
            {
                weaponSwitchNewIndex = newWeaponIndex;
                weaponSwitchTimeStart = Time.time;
                //WeaponController nowWeapon = GetWeaponAtSlotIndex(ActiveWeaponIndex);
                //nowWeapon.ShowWeapon(false);

                // ���� ��Ƽ���� ���Ⱑ �ִ���?
                if(GetActiveWeapon() == null)
                {
                    weaponMainLocalPosition = downWeaponPosition.position;
                    weaponSwitchState = WeaponSwitchState.PutUpNew;
                    ActiveWeaponIndex = newWeaponIndex;

                    WeaponController weaponController = GetWeaponAtSlotIndex(newWeaponIndex);
                    OnSwitchToWeapon?.Invoke(weaponController);
                }
                else
                {
                    weaponSwitchState = WeaponSwitchState.PutDownPrevious;
                }
            }

            //WeaponController newWeapon = GetWeaponAtSlotIndex(newWeaponIndex);
            //newWeapon.ShowWeapon(true);
            //ActiveWeaponIndex = newWeaponIndex;
        }
        public WeaponController GetActiveWeapon()
        {
            return GetWeaponAtSlotIndex(ActiveWeaponIndex);
        }

        void OnWeaponSwitched(WeaponController newWeapon)
        {
            if(newWeapon != null)
            {
                newWeapon.ShowWeapon(true);
            }
        }
    }
}
