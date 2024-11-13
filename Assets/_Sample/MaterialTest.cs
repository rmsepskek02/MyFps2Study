using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MySample
{
    public class MaterialTest : MonoBehaviour
    {
        #region Varialbes
        private Renderer renderer;

        private MaterialPropertyBlock materialPropertyBlock;
        #endregion
        // Start is called before the first frame update
        void Start()
        {
            renderer = GetComponent<Renderer>();
            //renderer.material.SetColor("_BaseColor", Color.white);
            //renderer.sharedMaterial.SetColor("_BaseColor", Color.white);

            materialPropertyBlock = new MaterialPropertyBlock();
            materialPropertyBlock.SetColor("_BaseColor", Color.green);
            renderer.SetPropertyBlock(materialPropertyBlock);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
