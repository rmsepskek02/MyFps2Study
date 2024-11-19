using System.Collections;
using System.Collections.Generic;
using Unity.FPS.AI;
using UnityEngine;

namespace Unity.FPS.Ga
{
    /// <summary>
    /// Game Èå¸§ °ü¸®
    /// </summary>
    public class GameFlowManager : MonoBehaviour
    {
        EnemyManager em;
        // Start is called before the first frame update
        void Start()
        {
            em = GameObject.FindObjectOfType<EnemyManager>();
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
