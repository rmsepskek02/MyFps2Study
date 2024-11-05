using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.FPS.Game
{
    /// <summary>
    /// 죽었을때 Health를 가진 오브젝트를 킬하는 오브젝트
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
            //TODO : 데미지 효과 구현
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
