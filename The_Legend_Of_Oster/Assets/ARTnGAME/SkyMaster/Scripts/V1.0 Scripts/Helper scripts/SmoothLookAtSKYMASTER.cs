using UnityEngine;
using System.Collections;

namespace Artngame.SKYMASTER {

	public class SmoothLookAtSKYMASTER : MonoBehaviour {

		public Transform target;
		public float damping = 6.0f;
		public bool smooth = true;
		Transform This_transf;

		void Start () {
			if (GetComponent<Rigidbody> ()) {
				GetComponent<Rigidbody> ().freezeRotation = true;
			}
			This_transf = this.transform;
		}

		void LateUpdate () {
			if (target) {
				if (smooth)
				{
					// Look at and dampen the rotation
					Quaternion rotation = Quaternion.LookRotation(target.position - This_transf.position);
					This_transf.rotation = Quaternion.Slerp(This_transf.rotation, rotation, Time.deltaTime * damping);
				}
				else
				{
					// Just lookat
					This_transf.LookAt(target);
				}
			}
		}
	}

}