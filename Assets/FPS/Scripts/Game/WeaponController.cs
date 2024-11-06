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
        #endregion

        private void Awake()
        {
            shootAudioSource = this.GetComponent<AudioSource>();
        }

        // Start is called before the first frame update
        void Start()
        {

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
            if(show == true)
            {
                // ���� ���� ȿ���� �÷���
                shootAudioSource.PlayOneShot(switchWeaponSfx);
            }
            IsWeaponActive = show;
        }
    }
}
