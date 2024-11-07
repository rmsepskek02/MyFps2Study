using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Unity.FPS.Game;

namespace Unity.FPS.Gameplay
{
    /// <summary>
    /// 무기 교체 상태
    /// </summary>
    public enum WeaponSwithState
    {
        Up,
        Down,
        PutDownPrvious,
        PutUpNew,
    }

    /// <summary>
    /// 플레이어가 가진 무기들을 관리하는 클래스
    /// </summary>
    public class PlayerWeaponsManager : MonoBehaviour
    {
        #region Variables
        //무기 지급 - 게임을 시작할때 처음 유저에게 지급되는 무기 리스트(인벤토리)
        public List<WeaponController> startingWeapons = new List<WeaponController>();

        //무기 장착
        //무기를 장착하는 오브젝트
        public Transform weaponParentSocket;

        //플레이어가 게임중에 들고 다니는 무기 리스트
        private WeaponController[] weaponSlots = new WeaponController[9];
        //무기 리스트(슬롯)중 활성화된 무기를 관리하는 인덱스
        public int ActiveWeaponIndex { get; private set; }

        //무기 교체
        public UnityAction<WeaponController> OnSwitchToWeapon;  //무기 교체시 등록된 함수 호출

        private WeaponSwithState weaponSwithState;          //무기 교체시 상태

        private PlayerInputHandler playerInputHandler;

        //무기 교체시 계산되는 최종 위치
        private Vector3 weaponMainLocalPosition;

        public Transform defaultWeaponPostion;
        public Transform downWeaponPostion;
        public Transform aimingWeaponPosition;

        private int weaponSwitchNewIndex;           //새로 바뀌는 무기 인덱스

        private float weaponSwitchTimeStarted = 0f;
        [SerializeField] private float weaponSwitchDelay = 1f;

        //적 포착
        public bool IsPointingAtEnemy { get; private set; } // 적포착 여부
        public Camera weaponCamera;                         // WeaponCamera에서 Ray로 적 확인

        //조준
        //카메라
        private PlayerCharacterController playerCharacterController;
        [SerializeField] public float defaultFov = 60f;                              //카메라 기본 FOV 값
        [SerializeField] public float weaponFovMultiplier = 1f;                      //FOV 연산 계수

        public bool IsAiming { get; private set; }                  //무기 조준 여부
        [SerializeField] private float aimingAnimationSpeed = 1f;   //무기 이동 연출

        //흔들림
        [SerializeField] private float bobFrequency = 10f;
        [SerializeField] private float bobSharpness = 10f;
        [SerializeField] private float defaultBobAmount = 0.05f;    //평상시 흔들림 량
        [SerializeField] private float aimingBobAmount = 0.02f;     //조준중 흔들림 량

        private float weaponBobFactor;          //계수
        private Vector3 lastCharacterPosition;  //현재 프레임에서의 이동속도를 구하기 위한 변수
        private Vector3 weaponBobLocalPosition; //이동시 흔들린 량 최종 계산값, 이동하지 않으면 0
        private bool clickDown = false;
        private bool clickHold = false;
        private bool clickUp = false;

        //반동
        [SerializeField] private float recoilSharpness = 50f;   //뒤로 밀리는 이동 속도
        [SerializeField] private float maxRecoilDistance = 0.5f;//반동시 뒤로 밀릴수 있는 최대거리
        private float recoilRepositionSharpness = 10f;          //제자리로 돌아오는 속도
        private Vector3 accumulateRecoil;                       //반동시 뒤로 밀리는 량

        private Vector3 weaponRecoilLocalPosition;          //반동시 이동한 최종 계산값, 반동후 제자리에 돌아오면 0
        private bool isScopeOn = false;
        public UnityAction OnScopedWeapon;          //저격 모드 시작시 등록된 함수 호출
        public UnityAction OffScopedWeapon;         //저격 모드 끝날시 등록된 함수 호출
        [SerializeField] private float distanceOnScope = 0.5f;
        #endregion

        private void Start()
        {
            //참조
            playerInputHandler = GetComponent<PlayerInputHandler>();
            playerCharacterController = GetComponent<PlayerCharacterController>();
            //초기화
            ActiveWeaponIndex = -1;
            weaponSwithState = WeaponSwithState.Down;

            //
            OnSwitchToWeapon += OnWeaponSwitched;
            OnScopedWeapon += OnScope;
            OffScopedWeapon += OffScope;
            //Fov 초기값 설정
            SetFov(defaultFov);

            //지급 받은 무기 장착
            foreach (var weapon in startingWeapons)
            {
                AddWeapon(weapon);
            }
            SwitchWeapon(true);
        }

        private void Update()
        {
            //클릭 이벤트
            clickDown = playerInputHandler.GetFireInputDown();        //누를때 
            clickHold = playerInputHandler.GetFireInputHeld();        //클릭중
            clickUp = playerInputHandler.GetFireInputReleased();      //풀때
            //현재 액티브 무기
            WeaponController activeWeapon = GetActiveWeapon();
            if (activeWeapon.shootType == ShootType.Sniper)
            {
                if (playerInputHandler.GetAimInputDown())
                {
                    isScopeOn = true;
                    //OnScopedWeapon?.Invoke();
                }
                if (playerInputHandler.GetAimInputUp())
                {
                    //isScopeOn = false;
                    OffScopedWeapon?.Invoke();
                }
            }

            if (weaponSwithState == WeaponSwithState.Up)
            {
                //조준 입력값 처리
                IsAiming = playerInputHandler.GetAimInputHeld();
                //슛 처리
                bool isFire = activeWeapon.HandleShootInputs(clickDown, clickHold, clickUp);

                if (isFire)
                {
                    //반동 효과
                    accumulateRecoil += Vector3.back * activeWeapon.recoilForce;
                    accumulateRecoil = Vector3.ClampMagnitude(accumulateRecoil, maxRecoilDistance);
                }
            }

            if (IsAiming == false && weaponSwithState == WeaponSwithState.Up || weaponSwithState == WeaponSwithState.Down)
            {
                int switchWeaponInput = playerInputHandler.GetSwitchWeaponInput();
                if (switchWeaponInput != 0)
                {
                    bool switchUp = switchWeaponInput > 0;
                    SwitchWeapon(switchUp);
                }
            }

            IsPointingAtEnemy = false;
            if (activeWeapon)
            {
                RaycastHit hit;
                if (Physics.Raycast(weaponCamera.transform.position, weaponCamera.transform.forward, out hit, 300f))
                {
                    //콜라이더 체크 - 적 판별
                    Health health = hit.collider.GetComponent<Health>();
                    if (health != null)
                    {
                        IsPointingAtEnemy = true;
                    }
                }
            }
        }
        //카메라 Fov 값 셋팅: 줌인, 줌아웃
        private void SetFov(float fov)
        {
            playerCharacterController.PlayerCamera.fieldOfView = fov;
            weaponCamera.fieldOfView = fov * weaponFovMultiplier;
        }

        //무기 조준에 따른 연출, 무기위치 조정, Fov값 조정
        void UpdateWeaponAiming()
        {
            if (weaponSwithState == WeaponSwithState.Up)
            {
                WeaponController activeWeapon = GetActiveWeapon();

                // 조준시: 디폴트 -> Aming 위치로 이동, fov: 디폴트 -> aimZoomRatio
                if (IsAiming && activeWeapon != null)
                {
                    weaponMainLocalPosition = Vector3.Lerp(weaponMainLocalPosition, aimingWeaponPosition.localPosition + activeWeapon.aimOffset, Time.deltaTime * aimingAnimationSpeed);
                    //저격 모드 시작
                    if (isScopeOn)
                    {
                        //weaponMainLocalPosition, 목표지점까지의 거리를 구한다
                        float dist = Vector3.Distance(weaponMainLocalPosition, aimingWeaponPosition.localPosition + activeWeapon.aimOffset);
                        if (dist < distanceOnScope)
                        {
                            OnScopedWeapon?.Invoke();
                            isScopeOn = false;
                        }
                    }
                    else
                    {
                        float fov = Mathf.Lerp(playerCharacterController.PlayerCamera.fieldOfView,
                            activeWeapon.aimZoomRatio * defaultFov, aimingAnimationSpeed * Time.deltaTime);
                        SetFov(fov);
                    }
                }
                // 비조준시: Aming -> 디폴트 위치로 이동
                else
                {
                    weaponMainLocalPosition = Vector3.Lerp(weaponMainLocalPosition, defaultWeaponPostion.localPosition, Time.deltaTime * aimingAnimationSpeed);
                    float fov = Mathf.Lerp(playerCharacterController.PlayerCamera.fieldOfView, defaultFov, Time.deltaTime * aimingAnimationSpeed);
                    SetFov(fov);
                }
            }
        }
        private void LateUpdate()
        {
            UpdateWeaponBob();
            UpdateWeaponRecoil();
            UpdateWeaponAiming();
            UpdateWeaponSwitching();

            //무기 최종 위치
            weaponParentSocket.localPosition = weaponMainLocalPosition + weaponBobLocalPosition + weaponRecoilLocalPosition;
        }

        //반동
        void UpdateWeaponRecoil()
        {
            if (weaponRecoilLocalPosition.z >= accumulateRecoil.z * 0.99f)
            {
                weaponRecoilLocalPosition = Vector3.Lerp(weaponRecoilLocalPosition, accumulateRecoil, recoilSharpness * Time.deltaTime);
            }
            else
            {
                weaponRecoilLocalPosition = Vector3.Lerp(weaponRecoilLocalPosition, Vector3.zero, recoilRepositionSharpness * Time.deltaTime);
                accumulateRecoil = weaponRecoilLocalPosition;
            }
        }

        //이동에 의한 무기 흔들린 값 구하기
        void UpdateWeaponBob()
        {
            if (Time.deltaTime > 0)
            {
                //플레이어가 한 프레임동안 이동한 거리
                //Vector3 dis = playerCharacterController.transform.position - lastCharacterPosition;
                //현재 프레임에서 플레이어 이동 속도
                Vector3 playerCharacterVelocity = (playerCharacterController.transform.position - lastCharacterPosition) / Time.deltaTime;

                float charactorMovementFactor = 0;
                if (playerCharacterController.IsGrounded)
                {
                    charactorMovementFactor = Mathf.Clamp01(playerCharacterVelocity.magnitude / (playerCharacterController.MaxSpeedOnGround * playerCharacterController.SprintSpeedModifier));
                }

                //속도에 의한 흔들림 계수
                weaponBobFactor = Mathf.Lerp(weaponBobFactor, charactorMovementFactor, bobSharpness * Time.deltaTime);

                //흔들림량 (조준시, 평상시)
                float bobAmount = IsAiming ? aimingBobAmount : defaultBobAmount;
                float frequency = bobFrequency;
                //좌우 흔들림
                float vBobValue = (Mathf.Sin(Time.time * frequency) * 0.5f + 0.5f) * bobAmount * weaponBobFactor;
                float hBobValue = Mathf.Sin(Time.time * frequency) * bobAmount * weaponBobFactor;

                weaponBobLocalPosition.x = hBobValue;
                weaponBobLocalPosition.y = Mathf.Abs(vBobValue);

                //플레이어의 현재 프레임의 마지막 위치를 저장
                lastCharacterPosition = playerCharacterController.transform.position;
            }
        }

        //상태에 따른 무기 연출
        void UpdateWeaponSwitching()
        {
            //Lerp 변수
            float switchingTimeFactor = 0f;
            if (weaponSwitchDelay == 0f)
            {
                switchingTimeFactor = 1f;
            }
            else
            {
                switchingTimeFactor = Mathf.Clamp01((Time.time - weaponSwitchTimeStarted) / weaponSwitchDelay);
            }

            //지연시간이후 무기 상태 바꾸기
            if (switchingTimeFactor >= 1f)
            {
                if (weaponSwithState == WeaponSwithState.PutDownPrvious)
                {
                    //현재무기 false, 새로운 무기 true
                    WeaponController oldWeapon = GetActiveWeapon();
                    if (oldWeapon != null)
                    {
                        oldWeapon.ShowWeapon(false);
                    }

                    ActiveWeaponIndex = weaponSwitchNewIndex;
                    WeaponController newWeapon = GetActiveWeapon();
                    OnSwitchToWeapon?.Invoke(newWeapon);

                    switchingTimeFactor = 0f;
                    if (newWeapon != null)
                    {
                        weaponSwitchTimeStarted = Time.time;
                        weaponSwithState = WeaponSwithState.PutUpNew;
                    }
                    else
                    {
                        weaponSwithState = WeaponSwithState.Down;
                    }
                }
                else if (weaponSwithState == WeaponSwithState.PutUpNew)
                {
                    weaponSwithState = WeaponSwithState.Up;
                }
            }

            //지연시간동안 무기의 위치 이동
            if (weaponSwithState == WeaponSwithState.PutDownPrvious)
            {
                weaponMainLocalPosition = Vector3.Lerp(defaultWeaponPostion.localPosition, downWeaponPostion.localPosition, switchingTimeFactor);
            }
            else if (weaponSwithState == WeaponSwithState.PutUpNew)
            {
                weaponMainLocalPosition = Vector3.Lerp(downWeaponPostion.localPosition, defaultWeaponPostion.localPosition, switchingTimeFactor);
            }
        }

        //weaponSlots에 무기 프리팹으로 생성한 WeaponController 오브젝트 추가
        public bool AddWeapon(WeaponController weaponPrefab)
        {
            //추가하는 무기 소지 여부 체크 - 중복검사
            if (HasWeapon(weaponPrefab) != null)
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

            Debug.Log("weaponSlots full");
            return false;
        }

        //매개변수로 들어온 
        private WeaponController HasWeapon(WeaponController weaponPrefab)
        {
            for (int i = 0; i < weaponSlots.Length; i++)
            {
                if (weaponSlots[i] != null && weaponSlots[i].SourcePrefab == weaponPrefab)
                {
                    return weaponSlots[i];
                }
            }

            return null;
        }

        public WeaponController GetActiveWeapon()
        {
            return GetWeaponAtSlotIndex(ActiveWeaponIndex);
        }

        //지정된 슬롯에 무기가 있는지 여부
        public WeaponController GetWeaponAtSlotIndex(int index)
        {
            if (index >= 0 && index < weaponSlots.Length)
            {
                return weaponSlots[index];
            }

            return null;
        }

        //0~9  
        //무기 바꾸기, 현재 들고 있는 무기 false, 새로운 무기 true
        public void SwitchWeapon(bool ascendingOrder)
        {
            int newWeaponIndex = -1;    //새로 액티브할 무기 인덱스
            int closestSlotDistance = weaponSlots.Length;
            for (int i = 0; i < weaponSlots.Length; i++)
            {
                if (i != ActiveWeaponIndex && GetWeaponAtSlotIndex(i) != null)
                {
                    int distanceToActiveIndex = GetDistanceBetweenWeaponSlot(ActiveWeaponIndex, i, ascendingOrder);
                    if (distanceToActiveIndex < closestSlotDistance)
                    {
                        closestSlotDistance = distanceToActiveIndex;
                        newWeaponIndex = i;
                    }
                }
            }

            //새로 액티브할 무기 인덱스로 무기 교체
            SwitchToWeaponIndex(newWeaponIndex);
        }

        private void SwitchToWeaponIndex(int newWeaponIndex)
        {
            //newWeaponIndex 값 체크
            if (newWeaponIndex >= 0 && newWeaponIndex != ActiveWeaponIndex)
            {
                weaponSwitchNewIndex = newWeaponIndex;
                weaponSwitchTimeStarted = Time.time;

                //현재 액티브한 무기가 있느냐?
                if (GetActiveWeapon() == null)
                {
                    weaponMainLocalPosition = downWeaponPostion.position;
                    weaponSwithState = WeaponSwithState.PutUpNew;
                    ActiveWeaponIndex = newWeaponIndex;

                    WeaponController weaponController = GetWeaponAtSlotIndex(newWeaponIndex);
                    OnSwitchToWeapon?.Invoke(weaponController);
                }
                else
                {
                    weaponSwithState = WeaponSwithState.PutDownPrvious;
                }
            }
        }

        //슬롯간 거리
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

            if (distanceBetweenSlots < 0)
            {
                distanceBetweenSlots = distanceBetweenSlots + weaponSlots.Length;
            }

            return distanceBetweenSlots;
        }

        void OnWeaponSwitched(WeaponController newWeapon)
        {
            if (newWeapon != null)
            {
                newWeapon.ShowWeapon(true);
            }
        }

        void OnScope()
        {
            weaponCamera.enabled = false;
        }

        void OffScope()
        {
            weaponCamera.enabled = true;
        }
    }
}