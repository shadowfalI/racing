using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SpeedNumDisplay : MonoBehaviour {

    private WheelCollider m_flWheelCollider;

    private float currentSpeed;

    private RectTransform speedPoint;
    private float zRotation;

    private Text speedNum;

    private Rigidbody carRigibody;
    private Driving driving;

    private RectTransform tachometerPoint;
    private Text gearNum;
    private float gearRotation;

    private RectTransform n2oPoint;
    private float n2oRotation;

	void Start () {
        carRigibody = GameObject.Find("Car").GetComponent<Rigidbody>();
        driving = GameObject.Find("Car").GetComponent<Driving>();
        m_flWheelCollider = GameObject.Find("WheelColliders/WheelFLCollider").GetComponent<WheelCollider>();
        speedPoint = GameObject.Find("Canvas/SpeedPoint").GetComponent<RectTransform>();
        tachometerPoint = GameObject.Find("Canvas/TachometerPoint").GetComponent<RectTransform>();
        n2oPoint = GameObject.Find("Canvas/N2OPoint").GetComponent<RectTransform>();
        speedNum = gameObject.GetComponent<Text>();
        gearNum = GameObject.Find("Tachometer/Mask/Text").GetComponent<Text>();
        zRotation = speedPoint.eulerAngles.z;
        gearRotation = tachometerPoint.eulerAngles.z;
        n2oRotation = n2oPoint.eulerAngles.z;
	}
	
	
	void Update () {
        //currentSpeed = m_flWheelCollider.rpm * (m_flWheelCollider.radius * 2 * Mathf.PI) * 60 / 1000;
        currentSpeed = carRigibody.velocity.magnitude * 3.6f;
        currentSpeed = Mathf.Round(currentSpeed);
        speedNum.text = currentSpeed.ToString();
        //speedNum.text = driving.gear.ToString() + "  " + Mathf.Round(Mathf.Abs(driving.engineRPM)).ToString();

        if (driving.gear == -1)
        {
            gearNum.text = "R";
        }
        else
        {
            gearNum.text = driving.gear.ToString();
        }
        if (currentSpeed <= 0)
        {
            currentSpeed = 0;
        }
        if (currentSpeed > 210f)
        {
            currentSpeed = 210f;
        }
        float newZRotation = zRotation - currentSpeed * (240 / 210f);
        speedPoint.eulerAngles = new Vector3(0, 0, newZRotation);
        float newGearRotation = gearRotation - Mathf.Abs(driving.engineRPM) * (240 / 8000f);
        tachometerPoint.eulerAngles = new Vector3(0, 0, newGearRotation);
        float newN2ORotation = n2oRotation - driving.n2oNum * (150 / 10f);
        n2oPoint.eulerAngles = new Vector3(0, 0, newN2ORotation);

	}
}
