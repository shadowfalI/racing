using UnityEngine;
using System.Collections;

public class FollowCollider : MonoBehaviour {


    public WheelCollider collider;
	
	
	void Update () {
        RaycastHit hit;
        if (Physics.Raycast(collider.transform.position,Vector3.down,out hit,collider.radius + collider.suspensionDistance))
        {
            transform.position = hit.point + Vector3.up * collider.radius;
        }
        else
        {
            transform.position = collider.transform.position - collider.transform.up * collider.suspensionDistance;
        }
	}
}
