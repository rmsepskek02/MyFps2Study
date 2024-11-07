using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.FPS.Game
{
    /// <summary>
    /// ũ�ν��� �����ϴ� ������
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
    /// ����(�ѱ�)�� �����ϴ� Ŭ����
    /// </summary>
    public class WeaponController : MonoBehaviour
    {
        #region Variables
        //���� Ȱ��ȭ, ��Ȱ��ȭ
        public GameObject weaponRoot;

        public GameObject Owner { get; set; }           //������ ����
        public GameObject SourcePrefab { get; set; }    //���⸦ ������ �������� ������
        public bool IsWeaponActive { get; private set; }// ���� Ȱ��ȭ ����

        private AudioSource shootAudioSource;
        public AudioClip switchWeaponSfx;

        public CrossHairData crosshairDefault;
        public CrossHairData crosshairTargetInSight;

        //����
        public float aimZoomRatio = 1f;     //���ؽ� ���� ������
        public Vector3 aimOffset;           //���ؽ� ���� ��ġ ������

        public ShootType shootType;
        [SerializeField] private float maxAmmo = 8f;    //�����Ҽ� �ִ� �ִ� �Ѿ� ����
        private float currentAmmo;
        [SerializeField] private float delayBetweenShots = 0.5f;    //�� ����
        private float lastTimeShot;

        //Vfx, Sfx
        public Transform weaponMuzzle;                              //�ѱ� ��ġ
        public GameObject muzzleFlashPrefab;                        //�ѱ� �߻� ȿ��
        public AudioClip shootSfx;                                  //�� �߻� ����

        //�ݵ�
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

        //���� Ȱ��ȭ, ��Ȱ��ȭ
        public void ShowWeapon(bool show)
        {
            weaponRoot.SetActive(show);

            //this ����� ����
            if (show == true)
            {
                // ���� ���� ȿ���� �÷���
                shootAudioSource.PlayOneShot(switchWeaponSfx);
            }
            IsWeaponActive = show;
        }
        /*
            : ��ư ������ ���¿� ���� �� Ÿ�Կ� ���� �� - HandleShootInputs()
            : �� ���� TryShoot() : Debug.Log("Shoot!!!!!!");
            : �� ��� ������ 0.5��
            : �� ���� HandleShoot() : �ѱ�����  Muzzle ����Ʈ, �߻� ����
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
        //�� ����
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

            //���� �ð� ����
            lastTimeShot = Time.time;
        }
    }
}
