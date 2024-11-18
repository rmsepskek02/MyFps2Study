using System.Linq;
using Unity.FPS.Game;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

namespace Unity.FPS.AI
{
    /// <summary>
    /// �� ������ ����
    /// </summary>
    public class DetectionModule : MonoBehaviour
    {
        #region Variables
        private ActorManager actorManager;

        public UnityAction OnDetectedTarget;     //���� �����ϸ� ��ϵ� �Լ� ȣ��
        public UnityAction OnLostTarget;         //���� ��ġ�� ��ϵ� �Լ� ȣ��

        public GameObject KnownDetectedTarget { get; private set; }
        public bool HadKnownTarget { get; private set; }
        public bool IsSeeingTarget { get; private set; }

        public Transform detectionSourcePoint;
        public float detectionRange = 20f;                          //�� ���� �Ÿ�

        public float knownTargetTimeout = 4f;
        private float TimeLastSeenTarget = Mathf.NegativeInfinity;

        //attack
        public float attackRange = 10f;                             //�� ���� �Ÿ�
        public bool IsTargetInAttackRange { get; private set; }
        #endregion

        private void Start()
        {
            //����
            actorManager = GameObject.FindObjectOfType<ActorManager>();
        }

        //������
        public void HandleTargetDetection(Actor actor, Collider[] selfCollider)
        {
            if(KnownDetectedTarget && !IsSeeingTarget && (Time.time - TimeLastSeenTarget) > knownTargetTimeout)
            {
                KnownDetectedTarget = null;
            }

            float sqrDetectionRange = detectionRange * detectionRange;
            IsSeeingTarget = false;
            float closetSqrdistance = Mathf.Infinity;

            foreach (var otherActor in actorManager.Actors)
            {
                //�Ʊ��̸�
                if (otherActor.affiliation == actor.affiliation)
                    continue;

                float sqrDistance = (otherActor.aimPoint.position - detectionSourcePoint.position).sqrMagnitude;
                if(sqrDistance < sqrDetectionRange && sqrDistance < closetSqrdistance)
                {
                    RaycastHit[] hits = Physics.RaycastAll(detectionSourcePoint.position,
                        (otherActor.aimPoint.position - detectionSourcePoint.position).normalized, detectionRange,
                        -1, QueryTriggerInteraction.Ignore);

                    RaycastHit cloestHit = new RaycastHit();
                    cloestHit.distance = Mathf.Infinity;
                    bool foundValidHit = false;
                    foreach (var hit in hits)
                    {
                        if(hit.distance < cloestHit.distance && selfCollider.Contains(hit.collider) == false)
                        {
                            cloestHit = hit;
                            foundValidHit = true;
                        }
                    }

                    //���� ã������
                    if(foundValidHit)
                    {
                        Actor hitActor = cloestHit.collider.GetComponentInParent<Actor>();
                        if(hitActor == otherActor)
                        {
                            IsSeeingTarget = true;
                            closetSqrdistance = sqrDistance;

                            TimeLastSeenTarget = Time.time;
                            KnownDetectedTarget = otherActor.aimPoint.gameObject;
                        }
                    }
                }
            }

            //attack Range check
            IsTargetInAttackRange = (KnownDetectedTarget != null) &&
                Vector3.Distance(transform.position, KnownDetectedTarget.transform.position) <= attackRange;

            //���� �𸣰� �ִٰ� ���� �߰��� ������ ����
            if (HadKnownTarget == false && KnownDetectedTarget != null)
            {
                OnDetected();
            }

            //���� ��� �ֽ��ϰ� �ִٰ� ��ġ�� ���� ����
            if(HadKnownTarget == true &&  KnownDetectedTarget == null)
            {
                OnLost();
            }

            //������ ���� ����
            HadKnownTarget = (KnownDetectedTarget != null);
        }

        //���� �����ϸ� ����
        public void OnDetected()
        {
            OnDetectedTarget?.Invoke();
        }

        //���� ��ġ��
        public void OnLost()
        {
            OnLostTarget?.Invoke();
        }
    }
}