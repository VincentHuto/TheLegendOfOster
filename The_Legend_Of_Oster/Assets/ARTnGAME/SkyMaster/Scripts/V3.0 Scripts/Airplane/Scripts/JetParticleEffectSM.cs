using System;
using UnityEngine;

namespace ARTNGAME.Skymaster
{
    [RequireComponent(typeof (ParticleSystem))]
    public class JetParticleEffectSM : MonoBehaviour
    {
        // this script controls the jet's exhaust particle system, controlling the
        // size and colour based on the jet's current throttle value.
        public Color minColour; // The base colour for the effect to start at

		private AeroplaneControllerSM m_Jet; // The jet that the particle effect is attached to
        private ParticleSystem m_System; // The particle system that is being controlled
        private float m_OriginalStartSize; // The original starting size of the particle system
        private float m_OriginalLifetime; // The original lifetime of the particle system
        private Color m_OriginalStartColor; // The original starting colout of the particle system

        // Use this for initialization
        private void Start()
        {
            // get the aeroplane from the object hierarchy
            m_Jet = FindAeroplaneParent();

            // get the particle system ( it will be on the object as we have a require component set up
            m_System = GetComponent<ParticleSystem>();

            // set the original properties from the particle system
//            m_OriginalLifetime = m_System.startLifetime;
//            m_OriginalStartSize = m_System.startSize;
//            m_OriginalStartColor = m_System.startColor;

			//v3.4.9
			ParticleSystem.MainModule MainMod = m_System.main; //v3.4.9
			m_OriginalLifetime = MainMod.startLifetime.constant;
			m_OriginalStartSize = MainMod.startSize.constant;
			m_OriginalStartColor = MainMod.startColor.color;
        }


        // Update is called once per frame
        private void Update()
        {
            // update the particle system based on the jets throttle
//            m_System.startLifetime = Mathf.Lerp(0.0f, m_OriginalLifetime, m_Jet.Throttle);
//            m_System.startSize = Mathf.Lerp(m_OriginalStartSize*.3f, m_OriginalStartSize, m_Jet.Throttle);
//            m_System.startColor = Color.Lerp(minColour, m_OriginalStartColor, m_Jet.Throttle);

			//v3.4.9
			ParticleSystem.MainModule MainMod = m_System.main; //v3.4.9
			ParticleSystem.MinMaxCurve Curve = MainMod.startLifetime;
			Curve.constant = Mathf.Lerp(0.0f, m_OriginalLifetime, m_Jet.Throttle);
			ParticleSystem.MinMaxCurve startSize = MainMod.startSize;
			startSize.constant = Mathf.Lerp(m_OriginalStartSize*.3f, m_OriginalStartSize, m_Jet.Throttle);
			ParticleSystem.MinMaxGradient startColor = MainMod.startColor;
			startColor.color = Color.Lerp(minColour, m_OriginalStartColor, m_Jet.Throttle);
        }


		private AeroplaneControllerSM FindAeroplaneParent()
        {
            // get reference to the object transform
            var t = transform;

            // traverse the object hierarchy upwards to find the AeroplaneController
            // (since this is placed on a child object)
            while (t != null)
            {
				var aero = t.GetComponent<AeroplaneControllerSM>();
                if (aero == null)
                {
                    // try next parent
                    t = t.parent;
                }
                else
                {
                    return aero;
                }
            }

            // controller not found!
            throw new Exception(" AeroplaneContoller not found in object hierarchy");
        }
    }
}
