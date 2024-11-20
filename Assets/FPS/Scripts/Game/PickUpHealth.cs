using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.FPS.Game
{
    public class PickUpHealth : MonoBehaviour
    {
        PlayerHealth ph;
        [SerializeField] float healPoint;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnTriggerEnter(Collider other)
        {
            ph = other.GetComponent<PlayerHealth>();
            if (ph != null)
            {
                ph.Heal(healPoint, gameObject);
            }
        }
    }
}
