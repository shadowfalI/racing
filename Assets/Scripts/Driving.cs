using UnityEngine;
using System.Collections;

public class Driving : MonoBehaviour {

	private WheelCollider m_flWheelCollider;
    private WheelCollider m_frWheelCollider;
    private WheelCollider m_rlWheelCollider;
    private WheelCollider m_rrWheelCollider;

    private Transform m_flWheelModel;
    private Transform m_frWheelModel;
    private Transform m_rlWheelModel;
    private Transform m_rrWheelModel;

    private Transform m_flDiscBrake;
    private Transform m_frDiscBrake;

    private Transform centerOfMass;

    private EllipsoidParticleEmitter leftEmitter;
    private EllipsoidParticleEmitter rightEmitter;

    private GameObject leftLight;
    private GameObject rightLight;

    public GameObject skidMask;
    private GameObject skidMarkParent;

    private GameObject n2O;

    private Rigidbody carRigibody;
    private Transform carTransform;

    private float motorTorque = 400;
    private float steerAngle = 20;

    private float maxSpeed = 210;
    private float minSpeed = 30;

    private float brakeTorque = 2000;
    private bool isBraking = false;
    private bool handBraking = false;
    private bool isN2O = false;

    private AudioSource carEngineAudio;
    public AudioSource skidAudio;
    public AudioSource crashAudio;

    private float currentSpeed;

    public int[] speedArray;
    private bool isGround = false;

    //private float[] gearRatios = new float[5{-2.66f,2.66f,1.78f,1.3f,1f};
    private float[] gearRatios = new float[6]{-2.66f,3.0f,2.36f,1.78f,1.3f,1f};
    private float finalDriveRatio = 3.4f;
    private float shiftDownRPM = 3000f;
    private float shiftUpRPM = 7000f;
    private float minRPM = 1000f;
    private float maxRPM = 8000f;
    public float engineRPM = 0f;
    private float motor = 0;
    public int gear = 1;
    private bool isHandGear = false;

    public float n2oNum = 0;

	void Start () {
        m_flWheelCollider = GameObject.Find("WheelColliders/WheelFLCollider").GetComponent<WheelCollider>();
        m_frWheelCollider = GameObject.Find("WheelColliders/WheelFRCollider").GetComponent<WheelCollider>();
        m_rlWheelCollider = GameObject.Find("WheelColliders/WheelRLCollider").GetComponent<WheelCollider>();
        m_rrWheelCollider = GameObject.Find("WheelColliders/WheelRRCollider").GetComponent<WheelCollider>();

        m_flWheelModel = GameObject.Find("WheelFL").GetComponent<Transform>();
        m_frWheelModel = GameObject.Find("WheelFR").GetComponent<Transform>();
        m_rlWheelModel = GameObject.Find("WheelRL").GetComponent<Transform>();
        m_rrWheelModel = GameObject.Find("WheelRR").GetComponent<Transform>();

        m_flDiscBrake = GameObject.Find("WheelFL/DiscBrakeFL").GetComponent<Transform>();
        m_frDiscBrake = GameObject.Find("WheelFR/DiscBrakeFR").GetComponent<Transform>();

        carRigibody = GameObject.Find("Car").GetComponent<Rigidbody>();
        carTransform = GameObject.Find("Car").GetComponent<Transform>();

        n2O = GameObject.Find("N2O");
        n2O.SetActive(false);

        skidMarkParent = GameObject.Find("SkidMarks");

        centerOfMass = GameObject.Find("CenterOfMass").GetComponent<Transform>();
        gameObject.GetComponent<Rigidbody>().centerOfMass = centerOfMass.localPosition;

        carEngineAudio = gameObject.GetComponent<AudioSource>();

        leftEmitter = GameObject.Find("LeftSkidSmoke").GetComponent<EllipsoidParticleEmitter>();
        rightEmitter = GameObject.Find("RightSkidSmoke").GetComponent<EllipsoidParticleEmitter>();

        leftLight = GameObject.Find("Lights/LeftLight");
        rightLight = GameObject.Find("Lights/RightLight");
        leftLight.SetActive(false);
        rightLight.SetActive(false); //初始化刹车灯.

	}
	
	void Update () {
        //carRigibody.drag = carRigibody.velocity.magnitude / 1000f;
        //获取当前速度.
        //currentSpeed = m_flWheelCollider.rpm * (m_flWheelCollider.radius * 2 * Mathf.PI) * 60 / 1000;
        HandGear();
        GetCurrentSpeed();
        Barking();
        CalcEngineRPM();
        AutomaticTransmission();
        AddTorque();
        N2O();
        Steer();              
        RotateWheel();
        SteerWheel();
        EngineSound();       
	}

    void FixedUpdate()
    {
        Skid();
    }

    /// <summary>
    /// C键切换手动挡；
    /// </summary>
    private void HandGear()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (isHandGear == false)
            {
                isHandGear = true;
            }
            else
            {
                isHandGear = false;
            }
        }
    }

    //氮气推进.
    private void N2O()
    {
        if (!isBraking && !handBraking)
        {
            if (Input.GetKey(KeyCode.Space) && n2oNum > 0.1f)
            {
                isN2O = true;
            }
            else
            {
                isN2O = false;
            }
        }
        else
        {
            isN2O = false;
        }
        
        if (isN2O)
        {
            motorTorque = 800f;
            n2oNum -= Time.deltaTime * 1.5f;
            n2O.SetActive(true);
        }
        else
        {
            motorTorque = 400f;
            n2O.SetActive(false);
        }
        
    }

    /// <summary>
    /// 换挡装置.
    /// </summary>
    private void AutomaticTransmission()
    {
        if (currentSpeed < 0 && Input.GetAxis("Vertical") < 0)
        {
            gear = -1;
        }
        if (!isHandGear)
        {
            if (gear > 0)
            {
                if (engineRPM > shiftUpRPM && gear < gearRatios.Length - 1)
                    gear++;
                if (engineRPM < shiftDownRPM && gear > 1)
                    gear--;
            }
            else
            {
                if (currentSpeed > 0)
                {
                    gear = 1;
                }
            }
        }
        else
        {
            if (gear > 0)
            {
                if (Input.GetKeyDown(KeyCode.Period) && gear < gearRatios.Length - 1)
                {
                    gear++;
                }
                if (Input.GetKeyDown(KeyCode.Slash) && gear > 1)
                {
                    gear--;
                }
            }
            else
            {
                if (currentSpeed > 0)
                {
                    gear = 1;
                }
            }
        }
    }

    /// <summary>
    /// 计算引擎转速.
    /// </summary>
    private void CalcEngineRPM()
    {
        engineRPM = (m_rrWheelCollider.rpm + m_rlWheelCollider.rpm) / 2 * gearRatios[Mathf.Abs(gear)] * finalDriveRatio;
        if (engineRPM < minRPM)
        {
            engineRPM = minRPM;
        }
        if (engineRPM > maxRPM)
        {
            engineRPM = maxRPM;
        }
    }

    /// <summary>
    /// 转向控制.
    /// </summary>
    private void Steer()
    {
        m_flWheelCollider.steerAngle = Input.GetAxis("Horizontal") * steerAngle;
        m_frWheelCollider.steerAngle = Input.GetAxis("Horizontal") * steerAngle;
    }

    /// <summary>
    /// 添加动力，制动力.
    /// </summary>
    private void AddTorque()
    {
        //在规定范围内添加动力.
        //if ((currentSpeed > maxSpeed && Input.GetAxis("Vertical") > 0) || (currentSpeed < -minSpeed && Input.GetAxis("Vertical") < 0))
        if ((engineRPM > (maxRPM - 200f) && Input.GetAxis("Vertical") > 0) || (currentSpeed < -minSpeed && Input.GetAxis("Vertical") < 0))
        {
            m_flWheelCollider.motorTorque = 0;
            m_frWheelCollider.motorTorque = 0;
            m_rrWheelCollider.motorTorque = 0;
            m_rlWheelCollider.motorTorque = 0;
        }
        else
        {
            m_flWheelCollider.motorTorque = Input.GetAxis("Vertical") * motorTorque * gearRatios[Mathf.Abs(gear)];
            m_frWheelCollider.motorTorque = Input.GetAxis("Vertical") * motorTorque * gearRatios[Mathf.Abs(gear)];
            m_rrWheelCollider.motorTorque = Input.GetAxis("Vertical") * motorTorque * gearRatios[Mathf.Abs(gear)];
            m_rlWheelCollider.motorTorque = Input.GetAxis("Vertical") * motorTorque * gearRatios[Mathf.Abs(gear)];
            
            //m_flWheelCollider.motorTorque = Input.GetAxis("Vertical") * motorTorque;
            //m_frWheelCollider.motorTorque = Input.GetAxis("Vertical") * motorTorque;
            //m_rrWheelCollider.motorTorque = Input.GetAxis("Vertical") * motorTorque;
            //m_rlWheelCollider.motorTorque = Input.GetAxis("Vertical") * motorTorque;
        }

        //添加刹车动力.
        if (isBraking)
        {
            m_flWheelCollider.motorTorque = 0;
            m_frWheelCollider.motorTorque = 0;
            m_rlWheelCollider.motorTorque = 0;
            m_rrWheelCollider.motorTorque = 0;

            m_flWheelCollider.brakeTorque = brakeTorque;
            m_frWheelCollider.brakeTorque = brakeTorque;
            m_rlWheelCollider.brakeTorque = brakeTorque;
            m_rrWheelCollider.brakeTorque = brakeTorque;
        }
        else
        {
            m_flWheelCollider.brakeTorque = 0;
            m_frWheelCollider.brakeTorque = 0;
            m_rlWheelCollider.brakeTorque = 0;
            m_rrWheelCollider.brakeTorque = 0;
        }

        //手刹动力
        if (handBraking)
        {
            m_rlWheelCollider.motorTorque = 0;
            m_rrWheelCollider.motorTorque = 0;

            m_rlWheelCollider.brakeTorque = brakeTorque;
            m_rrWheelCollider.brakeTorque = brakeTorque;
        }
        else
        {
            m_rlWheelCollider.brakeTorque = 0;
            m_rrWheelCollider.brakeTorque = 0;
        }
    }

    /// <summary>
    /// 判定刹车事件.
    /// </summary>
    private void Barking()
    {
        //使用刹车的条件.
        if ((currentSpeed > 0 && Input.GetAxis("Vertical") < 0) || (currentSpeed < 0 && Input.GetAxis("Vertical") > 0))
        {
            isBraking = true;
            isN2O = false;  //氮气停止.
        }
        else
        {
            isBraking = false;
        }

        //手刹.
        if (Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.LeftShift))
        {
            handBraking = true;
        }
        else
        {
            handBraking = false;
        }

        //刹车灯.
        if (isBraking || handBraking)
        {
            leftLight.SetActive(true);
            rightLight.SetActive(true);
        }
        else
        {
            leftLight.SetActive(false);
            rightLight.SetActive(false);
        }
    }

    /// <summary>
    /// 获取当前速度.
    /// </summary>
    private void GetCurrentSpeed()
    {
        if (Vector3.Dot(carRigibody.velocity.normalized, carTransform.forward.normalized) >= 0)
        {
            currentSpeed = carRigibody.velocity.magnitude * 3.6f;
        }
        else
        {
            currentSpeed = -(carRigibody.velocity.magnitude * 3.6f);
        }
    }

    /// <summary>
    /// 漂移.
    /// </summary>
    private Vector3 lastFLSkidPos = Vector3.zero;
    private Vector3 lastFRSkidPos = Vector3.zero;
    private void Skid()
    {
        float handbrakeSlip = (float)(carRigibody.velocity.magnitude * 0.035);
        if (handbrakeSlip > 1)  handbrakeSlip = 1;
        float sild = (float)(m_rlWheelCollider.sidewaysFriction.extremumSlip + (handBraking ? handbrakeSlip : 0.0));
        //if (currentSpeed > 40 && Mathf.Abs( m_flWheelCollider.steerAngle) > 5)
        //Debug.LogWarning(Vector3.Dot(carRigibody.velocity.normalized, carTransform.forward.normalized));
        if (sild > 0.75f || Mathf.Abs(Vector3.Dot(carRigibody.velocity.normalized, carTransform.forward.normalized)) < 0.95)
	    {
            isN2O = false; //氮气停止.
            if (n2oNum < 10.1f)
            {
                n2oNum += Time.deltaTime * 0.75f; //氮气开始收集.                
            }
            WheelHit hit;
            if (m_flWheelCollider.GetGroundHit(out hit))
            {
                isGround = true;
                leftEmitter.emit = true;
                if (lastFLSkidPos.x != 0 && lastFLSkidPos.y != 0 && lastFLSkidPos.z != 0)
                {
                    Vector3 pos = hit.point;
                    pos.y += 0.05f;
                    Quaternion rotation = Quaternion.LookRotation(hit.point - lastFLSkidPos);
                    GameObject go = GameObject.Instantiate(skidMask, pos, rotation) as GameObject;
                    go.transform.SetParent(skidMarkParent.transform);

                }
                lastFLSkidPos = hit.point;
            }
            else
            {
                leftEmitter.emit = false;
            }
            if (m_frWheelCollider.GetGroundHit(out hit))
            {
                isGround = true;
                rightEmitter.emit = true;
                if (lastFRSkidPos.x != 0 && lastFRSkidPos.y != 0 && lastFRSkidPos.z != 0)
                {
                    Vector3 pos = hit.point;
                    pos.y += 0.05f;
                    Quaternion rotation = Quaternion.LookRotation(hit.point - lastFRSkidPos);
                    GameObject go = GameObject.Instantiate(skidMask, pos, rotation) as GameObject;
                    go.transform.SetParent(skidMarkParent.transform);
                }
                lastFRSkidPos = hit.point;
            }
            else
            {
                rightEmitter.emit = false;
            }
            if (skidAudio.isPlaying == false && isGround)
            {
                skidAudio.Play();
            }
            else if (skidAudio.isPlaying && !isGround)
            {
                skidAudio.Stop();
            }
        }
        else
        {
            if (skidAudio.isPlaying)
            {
                skidAudio.Stop();
            }
            leftEmitter.emit = false;
            rightEmitter.emit = false;
        }
    }

    /// <summary>
    /// 车轮滚滚.
    /// </summary>
    private void RotateWheel()
    {
        m_flDiscBrake.Rotate(m_flWheelCollider.rpm * 6 * Time.deltaTime * Vector3.right);
        m_frDiscBrake.Rotate(m_frWheelCollider.rpm * 6 * Time.deltaTime * Vector3.right);
        m_rlWheelModel.Rotate(m_rlWheelCollider.rpm * 6 * Time.deltaTime * Vector3.right);
        m_rrWheelModel.Rotate(m_rrWheelCollider.rpm * 6 * Time.deltaTime * Vector3.right);
    }

    /// <summary>
    /// 前轮转向.
    /// </summary>
    private void SteerWheel()
    {
        Vector3 localEulerAngles = m_flWheelModel.localEulerAngles;
        localEulerAngles.y = m_flWheelCollider.steerAngle;

        m_flWheelModel.localEulerAngles = localEulerAngles;
        m_frWheelModel.localEulerAngles = localEulerAngles;
    }

    /// <summary>
    /// 引擎声音控制.
    /// </summary>
    private void EngineSound()
    {
        //int index = 0;
        //for (int i = 0; i < speedArray.Length - 1; i++)
        //{
        //    if (currentSpeed > speedArray[i])
        //    {
        //        index = i;
        //    }
        //}
        //int minSpeed = speedArray[index];
        //int maxSpeed = speedArray[index + 1];
        //if (index < 1)
        //{
        //    carEngineAudio.pitch = 0.4f + (currentSpeed - minSpeed) / (maxSpeed - minSpeed);
        //}
        //else
        //{
        //    carEngineAudio.pitch = 0.8f + (currentSpeed - minSpeed) / (maxSpeed - minSpeed)*0.6f;
        //}
        //carEngineAudio.volume = 0.5f + (currentSpeed - minSpeed) / (maxSpeed - minSpeed) * 0.5f;
        carEngineAudio.pitch = 0.5f + 0.8f * engineRPM / shiftUpRPM;
        if (isN2O)
        {
            carEngineAudio.volume = 0.5f;
        }
        else
        {
            carEngineAudio.volume = 0.5f + 0.2f * engineRPM / shiftUpRPM;
        }
    }


    /// <summary>
    /// 碰撞声音.
    /// </summary>
    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Wall")
        {
            isN2O = false; //氮气停止.
            crashAudio.Play();
        }
    }
}
