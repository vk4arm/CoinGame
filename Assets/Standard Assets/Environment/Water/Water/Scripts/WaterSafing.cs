using UnityEngine;
using System.Collections;

public class WaterSafing : MonoBehaviour {
	public float power;

	void OnTriggerStay(Collider collider)
	{
		if (collider.tag == "Player") {
			collider.GetComponent<Rigidbody>().AddForce(Vector3.up * power, ForceMode.VelocityChange);
		}
	}
}