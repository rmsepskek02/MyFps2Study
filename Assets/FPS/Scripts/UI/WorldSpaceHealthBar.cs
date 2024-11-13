using System.Collections;
using System.Collections.Generic;
using Unity.FPS.Game;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.FPS.UI
{
    public class WorldSpaceHealthBar : MonoBehaviour
    {
        #region Variables
        public Health health;
        public Image hpImage;
        public Transform hpBarPivot;

        //hp가 풀이면 healthBar를 숨긴다
        [SerializeField] private bool hideFullHealthBar = true;
        #endregion
        // Start is called before the first frame update
        void Start()
        {
            health.OnDamaged += OnDamaged;
        }

        // Update is called once per frame
        void Update()
        {
            //UI가 플레이어를 바라보도록 한다.
            hpBarPivot.LookAt(Camera.main.transform);

            //hp가 풀이면 healthBar를 숨긴다
            if (hideFullHealthBar)
            {
                hpBarPivot.gameObject.SetActive(hpImage.fillAmount != 1);
            }
        }
        void OnDamaged(float damage, GameObject damageSource)
        {
            hpImage.fillAmount = health.GetRatio();
        }
    }
}
