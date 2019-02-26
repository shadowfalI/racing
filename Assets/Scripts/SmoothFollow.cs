using UnityEngine;
using System.Collections;

public class SmoothFollow : MonoBehaviour {

    private Transform target;

    private float height = 3.5f;
    private float distance = 7.0f;
    private float smoothSpeed = 1;

	void Start () {
        target = GameObject.Find("Car").GetComponent<Transform>();
	}
	
	
	void Update () {
        Vector3 targetForward = target.forward;
        targetForward.y = 0;

        Vector3 currentForward = transform.forward;
        currentForward.y = 0;

        Vector3 forward = Vector3.Lerp(currentForward.normalized, targetForward.normalized, smoothSpeed * Time.deltaTime);
        Vector3 targetPos = target.position + Vector3.up * height - forward * distance;
        this.transform.position = targetPos;
        transform.LookAt(target);
	}
}
