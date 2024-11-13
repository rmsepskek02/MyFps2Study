using System.Collections;
using System.Collections.Generic;
using Unity.FPS.Game;
using UnityEngine;

namespace Unity.FPS.AI
{

    public class EnemyController : MonoBehaviour
    {
        #region Variables
        private Health health;

        //death
        public GameObject deathVfxPrefab;
        public Transform deathVfxSpawnPosition;
        #endregion

        // Start is called before the first frame update
        void Start()
        {
            health = GetComponent<Health>();
            health.OnDamaged += OnDamaged;
            health.OnDie += OnDie;
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnDamaged(float damage, GameObject damageSource)
        {

        }
        private void OnDie()
        {
            //Æø¹ß È¿°ú
            GameObject effectGo = Instantiate(deathVfxPrefab, deathVfxSpawnPosition.position, Quaternion.identity);

            Destroy(effectGo, 5f);
            Destroy(gameObject);
        }
    }
}
