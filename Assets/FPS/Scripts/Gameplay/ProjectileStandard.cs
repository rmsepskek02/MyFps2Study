using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.FPS.Game;
using Unity.FPS.Gameplay;

namespace Unity.FPS.Gameplay
{
    /// <summary>
    /// �߻�ü ǥ����
    /// </summary>
    public class ProjectileStandard : ProjectileBase
    {
        #region Variables
        //����
        private ProjectileBase projectileBase;
        private float maxLiftTime = 5f;

        //�̵�
        public float speed = 20f;
        public float gravityDown = 0f;
        public Transform root;
        public Transform tip;

        private Vector3 velocity;
        private Vector3 lastRootPosition;
        private float shootTime;

        //�浹
        public float radius = 0.01f;                   //�浹 �˻��ϴ� ��ü�� �ݰ�

        public LayerMask hittableLayers = -1;           //Hit�� ������ Layer
        private List<Collider> ignoredColliers;         //Hit ������ �����ϴ� �浹ü ����Ʈ

        //�浹 ����
        public GameObject impackVfxPrefab;              //Ÿ�� ����Ʈ
        [SerializeField] private float impactVfxLifeTime = 5f;
        private float impactVfxSpawnOffset = 0.1f;

        public AudioClip impactSfxClip;                //Ÿ�� ȿ����

        //������
        public float damage = 20f;
        private DamageArea damageArea;
        #endregion

        private void OnEnable()
        {
            projectileBase = GetComponent<ProjectileBase>();
            projectileBase.OnShoot += OnShoot;

            damageArea = GetComponent<DamageArea>();

            Destroy(gameObject, maxLiftTime);
        }


        //shoot �� ����
        new void OnShoot()
        {
            velocity = transform.forward * speed;
            transform.position += projectileBase.InheritedMuzzleVelocity * Time.deltaTime;

            lastRootPosition = root.position;

            //���� �浹 ����Ʈ ���� - projectil�� �߻��ϴ� �ڽ��� ��� �浹ü�� �����ͼ� ���
            ignoredColliers = new List<Collider>();
            Collider[] ownerColliders = projectileBase.Owner.GetComponentsInChildren<Collider>();
            ignoredColliers.AddRange(ownerColliders);

            //������Ÿ���� ���� �հ� ���ư��� ���� ����
            PlayerWeaponsManager weaponsManager = projectileBase.Owner.GetComponent<PlayerWeaponsManager>();
            if (weaponsManager)
            {
                Vector3 cameraToMuzzle = projectileBase.InitialPosition - weaponsManager.weaponCamera.transform.position;
                if(Physics.Raycast(weaponsManager.weaponCamera.transform.position, cameraToMuzzle.normalized,
                    out RaycastHit hit, cameraToMuzzle.magnitude, hittableLayers,
                    QueryTriggerInteraction.Collide))
                {
                    if (IsHitValid(hit))
                    {
                        OnHit(hit.point, hit.normal, hit.collider);
                    }
                }
            }
        }

        private void Update()
        {
            //�̵�
            transform.position += velocity * Time.deltaTime;

            //�߷�
            if (gravityDown > 0f)
            {
                velocity += Vector3.down * gravityDown * Time.deltaTime;
            }

            //�浹
            RaycastHit cloestHit = new RaycastHit();
            cloestHit.distance = Mathf.Infinity;
            bool foundHit = false;                  //hit�� �浹ü�� ã�Ҵ��� ����

            //Sphere Cast
            Vector3 displacementSinceLastFrame = tip.position - lastRootPosition;
            RaycastHit[] hits = Physics.SphereCastAll(lastRootPosition, radius,
                displacementSinceLastFrame.normalized, displacementSinceLastFrame.magnitude,
                hittableLayers, QueryTriggerInteraction.Collide);

            foreach (var hit in hits)
            {
                if(IsHitValid(hit) && hit.distance < cloestHit.distance)
                {
                    foundHit = true;
                    cloestHit = hit;
                }
            }

            //hit�� �浹ü�� ã�Ҵ�
            if (foundHit)
            {
                if(cloestHit.distance <= 0f)
                {
                    cloestHit.point = root.position;
                    cloestHit.normal = -transform.forward;
                }

                OnHit(cloestHit.point, cloestHit.normal, cloestHit.collider);
            }

            lastRootPosition = root.position;
        }

        //��ȿ�� hit���� ����
        bool IsHitValid(RaycastHit hit)
        {
            //IgnoreHitDectection ������Ʈ�� ���� �ݶ��̴� ����
            if(hit.collider.GetComponent<IgnoreHitDectection>())
            {
                return false;
            }

            //ignoredColliers�� ���Ե� �ݶ��̴� ����
            if (ignoredColliers != null && ignoredColliers.Contains(hit.collider))
            {
                return false;
            }

            //trigger collider�� Damageable�� ����� �ȴ�
            if (hit.collider.isTrigger && hit.collider.GetComponent<Damageable>() == null)
            {
                return false;
            }

            return true;
        }

        //Hit ����: ������, Vfx, Sfx,
        void OnHit(Vector3 point, Vector3 normal, Collider collider)
        {
            //������
            if(damageArea)
            {
                damageArea.InflictDamageArea(damage, point, hittableLayers, QueryTriggerInteraction.Collide, projectileBase.Owner);
            }
            else
            {
                Damageable damageable = collider.GetComponent<Damageable>();
                if (damageable)
                {
                    damageable.InflictDamage(damage, false, projectileBase.Owner);
                }
            }
            

            //Vfx
            if (impackVfxPrefab)
            {
                GameObject impactObject = Instantiate(impackVfxPrefab, point + (normal * impactVfxSpawnOffset), Quaternion.LookRotation(normal));
                if(impactVfxLifeTime > 0f)
                {
                    Destroy(impactObject, impactVfxLifeTime);
                }
            }

            //Sfx
            if(impactSfxClip)
            {
                //�浹��ġ�� ���ӿ�����Ʈ�� �����ϰ� AudioSource ������Ʈ�� �߰��ؼ� ������ Ŭ���� �÷����Ѵ�
                AudioUtility.CreateSfx(impactSfxClip, point, 1f, 3f);
            }

            //�߻�ü ų
            Destroy(gameObject);
        }


    }
}
