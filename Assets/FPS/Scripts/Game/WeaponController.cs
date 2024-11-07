using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.FPS.Game
{
    /// <summary>
    /// 크로스헤어를 관리하는 데이터
    /// </summary>
    [System.Serializable]
    public struct CrossHairData
    {
        public Sprite CrosshairSprite;
        public float CrosshairSize;
        public Color CrosshairColor;
    }
    public enum ShootType
    {
        None,
        Manual,
        Autimatic,
        Charge,
        Sniper,
    }

    /// <summary>
    /// 무기(총기)를 관리하는 클래스
    /// </summary>
    public class WeaponController : MonoBehaviour
    {
        #region Variables
        //무기 활성화, 비활성화
        public GameObject weaponRoot;

        public GameObject Owner { get; set; }           //무기의 주인
        public GameObject SourcePrefab { get; set; }    //무기를 생성한 오리지널 프리팹
        public bool IsWeaponActive { get; private set; }// 무기 활성화 여부

        private AudioSource shootAudioSource;
        public AudioClip switchWeaponSfx;

        public CrossHairData crosshairDefault;
        public CrossHairData crosshairTargetInSight;

        //조준
        public float aimZoomRatio = 1f;     //조준시 줌인 설정값
        public Vector3 aimOffset;           //조준시 무기 위치 조정값

        public ShootType shootType;
        [SerializeField] private float maxAmmo = 8f;    //장전할수 있는 최대 총알 갯수
        private float currentAmmo;
        [SerializeField] private float delayBetweenShots = 0.5f;    //슛 간격
        private float lastTimeShot;

        //Vfx, Sfx
        public Transform weaponMuzzle;                              //총구 위치
        public GameObject muzzleFlashPrefab;                        //총구 발사 효과
        public AudioClip shootSfx;                                  //총 발사 사운드

        //반동
        public float recoilForce = 0.5f;

        //projectile
        public ProjectileBase projectilePrefab;
        public Vector3 MuzzleWorldVelocity { get; private set; }
        private Vector3 lastMuzzlePosition;
        public float CurrentCharge { get; private set; }

        #endregion

        private void Awake()
        {
            shootAudioSource = this.GetComponent<AudioSource>();
        }

        // Start is called before the first frame update
        void Start()
        {
            currentAmmo = maxAmmo;

        }

        // Update is called once per frame
        void Update()
        {

        }

        //무기 활성화, 비활성화
        public void ShowWeapon(bool show)
        {
            weaponRoot.SetActive(show);

            //this 무기로 변경
            if (show == true)
            {
                // 무기 변경 효과음 플레이
                shootAudioSource.PlayOneShot(switchWeaponSfx);
            }
            IsWeaponActive = show;
        }
        /*
            : 버튼 누르는 상태에 따라 슛 타입에 따라 슛 - HandleShootInputs()
            : 슛 구현 TryShoot() : Debug.Log("Shoot!!!!!!");
            : 슛 쏘는 간격은 0.5초
            : 슛 연출 HandleShoot() : 총구에서  Muzzle 이펙트, 발사 사운드
         */

        public bool HandleShootInputs(bool inputDown, bool inputHeld, bool inputUp)
        {
            switch (shootType)
            {
                case ShootType.Manual:
                    if (inputDown)
                    {
                        return TryShoot(1);
                    }
                    break;
                case ShootType.Autimatic:
                    if (inputHeld)
                    {
                        return TryShoot(2);
                    }
                    break;
                case ShootType.Charge:
                    if (inputUp)
                    {
                        return TryShoot(3);
                    }
                    break;
                case ShootType.Sniper:
                    if (inputDown)
                    {
                        return TryShoot(4);
                    }
                    break;
                    //case ShootType.None:
                    //    break;
            }
            return false;
        }

        public bool TryShoot(int i)
        {
            if(currentAmmo >= 1f && (lastTimeShot + delayBetweenShots) < Time.time)
            {
                currentAmmo -= 1f;
                Debug.Log($"SHOOOOOOOOOOOOOOOT ca = {currentAmmo} / " + i);
                HandleShoot();

                return true;
            }
            return false;
        }
        //슛 연출
        void HandleShoot()
        {
            //Vfx
            if(muzzleFlashPrefab != null)
            {
                GameObject effectGo = Instantiate(muzzleFlashPrefab, weaponMuzzle.position, weaponMuzzle.rotation,weaponMuzzle);
                Destroy(effectGo, 2f);
            }

            //Sfx
            if(shootSfx != null)
            {
                shootAudioSource.PlayOneShot(shootSfx);
            }

            //슛한 시간 저장
            lastTimeShot = Time.time;
        }
    }
}
