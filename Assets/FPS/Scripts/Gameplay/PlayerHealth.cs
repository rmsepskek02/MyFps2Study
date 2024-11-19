using System.Collections;
using System.Collections.Generic;
using Unity.FPS.Utillity;
using UnityEngine;

namespace Unity.FPS.Game
{

    public class PlayerHealth : Health 
    {
        public GameObject fader;
        private SceneFader sencefader;
        [SerializeField] private string loseScene = "LoseScene";
        
        // Start is called before the first frame update
        protected override void Start()
       {
            base.Start();
            sencefader = fader.GetComponent<SceneFader>();
       }

       // Update is called once per frame
       void Update()
       {
           
       }
       protected override void HandleDeath()
       {
            //Á×À½ Ã¼Å©
            if (isDeath)
                return;

            if (CurrentHealth <= 0f)
            {
                isDeath = true;
                //Á×À½ ±¸Çö
                sencefader.FadeTo(loseScene);
            }
        }
    }   
}
