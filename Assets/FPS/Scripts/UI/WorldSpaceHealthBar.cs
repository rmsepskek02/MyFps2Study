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

        //hp�� Ǯ�̸� healthBar�� �����
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
            //UI�� �÷��̾ �ٶ󺸵��� �Ѵ�.
            hpBarPivot.LookAt(Camera.main.transform);

            //hp�� Ǯ�̸� healthBar�� �����
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
