using UnityEngine;
using System.Collections;

public class CarIdleEngine : MonoBehaviour {

    public AudioSource engineSound;
    private EllipsoidParticleEmitter leftEmitter;
    private EllipsoidParticleEmitter rightEmitter;

	void Start () {
        leftEmitter = GameObject.Find("LeftSkidSmoke").GetComponent<EllipsoidParticleEmitter>();
        rightEmitter = GameObject.Find("RightSkidSmoke").GetComponent<EllipsoidParticleEmitter>();
	}
	
	
	void Update () {
        if (Input.GetAxis("Vertical") != 0)
        {
            engineSound.pitch = 0.2f + Mathf.Abs(Input.GetAxis("Vertical"));
            leftEmitter.emit = true;
            rightEmitter.emit = true;
        }
        else
        {
            engineSound.pitch = 0.2f;
            leftEmitter.emit = false;
            rightEmitter.emit = false;
        }
	}

    public void DisableSelf()
    {
        engineSound.pitch = 0.2f;
        leftEmitter.emit = false;
        rightEmitter.emit = false;
        this.enabled = false;
    }
}
