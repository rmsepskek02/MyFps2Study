using System.Collections;
using System.Collections.Generic;
using Unity.FPS.Game;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace Unity.FPS.AI
{
    [System.Serializable]
    public struct RendererIndexData
    {
        public Renderer renderer;
        public int metarialIndex;

        public RendererIndexData(Renderer _renderer, int index)
        {
            renderer = _renderer;
            metarialIndex = index;
        }
    }

    /// <summary>
    /// Enemy�� �����ϴ� Ŭ����
    /// </summary>
    public class EnemyController : MonoBehaviour
    {
        #region Variables
        private Health health;

        //death
        public GameObject deathVfxPrefab;
        public Transform deathVfxSpawnPosition;

        //damage
        public UnityAction Damaged;

        //Sfx
        public AudioClip damageSfx;

        //Vfx
        public Material bodyMaterial;      //�������� �� ���͸���
        [GradientUsage(true)]
        public Gradient OnHitBodyGradient;  //�������� �÷� �׶���Ʈ ȿ��
        private List<RendererIndexData> bodyRenderer = new List<RendererIndexData>();
        MaterialPropertyBlock bodyFlashMaterialPropertyBlock;

        [SerializeField] private float flashOnHitDuration = 0.5f;
        float lastTimeDamaged = float.NegativeInfinity;
        bool wasDamagedThisFrame = false;

        //Patrol
        public NavMeshAgent Agent { get; private set; }
        public PatrolPath PatrolPath { get; set; }
        private int pathDestinationIndex;       //��ǥ waypointindex
        private float pathReachingRadius = 1f;  //��������
        #endregion

        // Start is called before the first frame update
        void Start()
        {
            Agent = GetComponent<NavMeshAgent>();
            bodyFlashMaterialPropertyBlock = new MaterialPropertyBlock();
            health = GetComponent<Health>();
            health.OnDamaged += OnDamaged;
            health.OnDie += OnDie;

            //body Material�� ������ �ִ� ������ ���� ����Ʈ �����
            Renderer[] renderers = GetComponentsInChildren<Renderer>(true);
            foreach (var renderer in GetComponentsInChildren<Renderer>(true))
            {
                for (int i = 0; i < renderer.sharedMaterials.Length; i++)
                {
                    if (renderer.sharedMaterials[i] == bodyMaterial)
                    {
                        bodyRenderer.Add(new RendererIndexData(renderer, i));
                    }
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            //������ ȿ��
            Color currentColor = OnHitBodyGradient.Evaluate((Time.time - lastTimeDamaged) / flashOnHitDuration);
            bodyFlashMaterialPropertyBlock.SetColor("_EmissionColor", currentColor);

            foreach (var data in bodyRenderer)
            {
                data.renderer.SetPropertyBlock(bodyFlashMaterialPropertyBlock, data.metarialIndex);
            }

            wasDamagedThisFrame = false;
        }

        private void OnDamaged(float damage, GameObject damageSource)
        {
            if (damageSource && damageSource.GetComponent<EnemyController>() == null)
            {
                //��ϵ� �Լ� ȣ��
                Damaged?.Invoke();

                //�������� �� �ð�
                lastTimeDamaged = Time.time;

                //Sfx
                if (damageSfx && wasDamagedThisFrame == false)
                {
                    AudioUtility.CreateSfx(damageSfx, this.transform.position, 0f);
                }
                wasDamagedThisFrame = true;
            }
        }
        private void OnDie()
        {
            //���� ȿ��
            GameObject effectGo = Instantiate(deathVfxPrefab, deathVfxSpawnPosition.position, Quaternion.identity);

            Destroy(effectGo, 5f);
            Destroy(gameObject);
        }

        //��Ʈ���� ��ȿ����? ��Ʈ���� ��������?
        private bool IsPathValid()
        {
            return PatrolPath && PatrolPath.wayPoints.Count > 0;
        }

        //���� ����� WayPoint ã��
        private void SetPathDestinationToClosestWayPoint()
        {
            if (IsPathValid() == false)
            {
                pathDestinationIndex = 0;
                return;
            }

            int closestPathWaypointIndex = 0;

            for (int i = 0; i < PatrolPath.wayPoints.Count; i++)
            {
                float distance = PatrolPath.GetDistanceToWayPoint(transform.position, i);
                float closestDistance = PatrolPath.GetDistanceToWayPoint(transform.position, closestPathWaypointIndex);
                if (distance < closestDistance)
                {
                    closestPathWaypointIndex = i;
                }
                pathDestinationIndex = closestPathWaypointIndex;
            }
        }

        //��ǥ ������ ��ġ �� ������
        public Vector3 GetDestinationOnPath()
        {
            if (IsPathValid() == false)
            {
                return this.transform.position;
            }
            return PatrolPath.GetPositionOfWayPoint(pathDestinationIndex);
        }

        //��ǥ ���� ���� - Nav �ý��� �̿�
        public void SetNavDestination(Vector3 destination)
        {
            if (Agent)
            {
                Agent.SetDestination(destination);
            }
        }

        //���� ���� �� ���� ��ǥ���� ����
        public void UpdatePathDestination(bool inverseOrder = false)
        {
            if (IsPathValid() == false) return;
            //���� ����
            float distance = (transform.position - GetDestinationOnPath()).magnitude;
            if (distance <= pathReachingRadius)
            {
                pathDestinationIndex = inverseOrder ? pathDestinationIndex - 1 : pathDestinationIndex + 1;
                if (pathDestinationIndex < 0)
                {
                    pathDestinationIndex += PatrolPath.wayPoints.Count;
                }
                if (pathDestinationIndex >= PatrolPath.wayPoints.Count)
                {
                    pathDestinationIndex -= PatrolPath.wayPoints.Count;
                }
            }
        }
    }
}