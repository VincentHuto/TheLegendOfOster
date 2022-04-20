using System;
using UnityEngine;

namespace Artngame.SKYMASTER
{
    [RequireComponent(typeof(WaterBaseSM))]
    [ExecuteInEditMode]
    public class SpecularLightingSM : MonoBehaviour
    {
        public Transform specularLight;
		private WaterBaseSM m_WaterBase;


        public void Start()
        {
			m_WaterBase = (WaterBaseSM)gameObject.GetComponent(typeof(WaterBaseSM));
        }


        public void Update()
        {
            if (!m_WaterBase)
            {
				m_WaterBase = (WaterBaseSM)gameObject.GetComponent(typeof(WaterBaseSM));
            }

            if (specularLight && m_WaterBase.sharedMaterial)
            {
                m_WaterBase.sharedMaterial.SetVector("_WorldLightDir", specularLight.transform.forward);
            }
        }
    }
}