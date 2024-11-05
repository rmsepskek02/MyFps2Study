using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.FPS.Game
{
    /// <summary>
    /// �׾����� Health�� ���� ������Ʈ�� ų�ϴ� ������Ʈ
    /// </summary>
    public class Destructable : MonoBehaviour
    {
        #region Variables
        private Health health;
        #endregion

        // Start is called before the first frame update
        void Start()
        {
            health = GetComponent<Health>();

            DebugUtility.HandleErrorIfNullGetComponent<Health, Destructable>(health, this, gameObject);

            health.OnDamaged += OnDamaged;
            health.OnDie += OnDie;
        }

        void OnDamaged(float damage, GameObject damageSource)
        {
            //TODO : ������ ȿ�� ����
        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnDie()
        {
            Destroy(gameObject);
        }
    }
}
