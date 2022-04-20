using System;
using UnityEngine;

namespace Artngame.SKYMASTER
{
    [ExecuteInEditMode]
    public class WaterTileSM : MonoBehaviour
    {
        public PlanarReflectionSM reflection;
        public WaterBaseSM waterBase;


        public void Start()
        {
            AcquireComponents();
        }


        void AcquireComponents()
        {
            if (!reflection)
            {
                if (transform.parent)
                {
					reflection = transform.parent.GetComponent<PlanarReflectionSM>();
                }
                else
                {
					reflection = transform.GetComponent<PlanarReflectionSM>();
                }
            }

            if (!waterBase)
            {
                if (transform.parent)
                {
					waterBase = transform.parent.GetComponent<WaterBaseSM>();
                }
                else
                {
					waterBase = transform.GetComponent<WaterBaseSM>();
                }
            }
        }


#if UNITY_EDITOR
        public void Update()
        {
            AcquireComponents();
        }
#endif


        public void OnWillRenderObject()
        {
            if (reflection)
            {
				//v3.2
				if(Camera.current != null){// && Camera.current.transform.eulerAngles != Vector3.zero){ //v4.2
                	reflection.WaterTileBeingRendered(transform, Camera.current);
				}
            }
            if (waterBase)
            {
                //v3.2 - //v4.2
                //if (Camera.current != null && Camera.current.transform.eulerAngles != Vector3.zero) {
                    //waterBase.WaterTileBeingRendered (transform, Camera.current); //v4.2
                //}
            }
        }
    }
}