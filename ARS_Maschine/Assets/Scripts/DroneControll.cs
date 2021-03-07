using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneControll : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private float _vRot = 0;
    private float _hRot = 0;
    private bool _forwardThrustOn = false;
    private float _forwardThrustForce = 0;
    private bool _upThrustOn = false;
    private float _upThrustForce = 0;
    private float _engineAcceleration = 2000;
    private float _angularSpeed = 500;
    private bool _isMoving = false;
    private float _maxVelocity = 1000;
    private Vector3 _velocity;
    private GameObject _mainSphere;
    private GameObject _ring1;
    private GameObject _ring2;
    private float _fitnessValue = 0;
    private float[][] _dronePolicy;
    private Vector3 _droneBasePosition;
    private int _numOfSensors = 6;
    private float _spotAngleOfSensors = 5f;
    private float[] _listOfSensorData;
    private float _sensorRange = 8f;
    private float[] _dronePolicyInput = new float[5];
    private Vector3 _centerPoint;

    public float FitnessValue
    {
        get { return _fitnessValue; }
        set { _fitnessValue = value; }
    }

    public Vector3 CenterPoint
    {
        get { return _centerPoint;}
        set {_centerPoint = value;}
    } 

    public float[][] DronePolicy
    {
        get {return _dronePolicy; }
        set {_dronePolicy = value; }
    }

    public Vector3 DroneBasePosition
    {
        get{return _droneBasePosition; }
        set{_droneBasePosition = value; }
    }

    void DroneRotator()
    {        
        _mainSphere.transform.Rotate(Vector3.forward, 30 * Mathf.Abs(Mathf.Sin( 5* Time.time)));
        _ring1.transform.Rotate(-Vector3.forward, 20 * Mathf.Abs(Mathf.Sin( 3* Time.time)));
        _ring2.transform.Rotate(-Vector3.forward, 15 * Mathf.Abs(Mathf.Sin( 10* Time.time)));
    }

    void TestRotation()
    {
        Vector3 point = new Vector3(_centerPoint.x, 0, _centerPoint.z );
        float angle = Vector3.Angle(this.transform.forward, point - this.transform.position);
        Debug.Log( "angle: " + angle.ToString());
    }
    void ManualDroneMovement()
    {
        _hRot = Input.GetAxis("Vertical");
        _vRot = Input.GetAxis("Horizontal");
        if (Input.GetKey("space"))
        {
            _forwardThrustOn = true;
        }

        else
        {
            _forwardThrustOn = false;
        }

        if(Input.GetKey(KeyCode.LeftControl))
        {
            _upThrustOn = true;
        }

        else
        {
            _upThrustOn = false;
        }

        if(Input.GetButton("Horizontal") && Input.GetButton("Vertical") && !Input.GetKey("space"))
        {
            _isMoving = false; 
        }
        else
        {
            _isMoving = true;
        }

    }

    private Vector3[] GenerateRayDetectors()
    {
        Vector3[] sensorDirections = new Vector3[_numOfSensors];
        float angle = 360f / (_numOfSensors - 1);
        Quaternion rotation = Quaternion.Euler(0, 0, angle);
        Vector3 baseSensor = this.transform.forward;
        Vector3 sensorDirection = baseSensor;
        sensorDirections[0] = sensorDirection;
        sensorDirection = Quaternion.AngleAxis(_spotAngleOfSensors, this.transform.right) * sensorDirection;
        sensorDirections[1] = sensorDirection;

        for (int i = 2; i < _numOfSensors; i++)
        {
            //sensorDirection = rotation * sensorDirection;
            sensorDirection = Quaternion.AngleAxis(angle, baseSensor) * sensorDirection;
            sensorDirections[i] = sensorDirection;
        }

        return sensorDirections;

    }
    public void BulletDetector( int layer)
    {   
        int i = 0;
        foreach (Vector3 detectorDirection in GenerateRayDetectors())
        {
            if(Physics.Raycast(this.transform.position, detectorDirection, out RaycastHit hitInfo, _sensorRange, layer))
            {
                Debug.DrawRay(this.transform.position, detectorDirection * hitInfo.distance, Color.red);
                _listOfSensorData[i] = 1 - hitInfo.distance / _sensorRange;
            }
            else
            {
                Debug.DrawRay(this.transform.position, detectorDirection * _sensorRange, Color.white);
                _listOfSensorData[i] = 0;
            }

            i++;           
        }

    }


    private void ObservationState()
    {
        /*
            Define input space:
            1) vector - distance between current position of drone and base position which is desired to maintain.
            In order to normalize inputs we need to prevent it from getting to high values. We do this by normalizing 
            our distance vector and multiplying by Min of 1 and its magnitude divided by 10.  When we divide we say that
            if magnitude of distance vector is higher than 10, we dont distinguish  any further movement away from base point.
            2) rotation - rotation between current rotaton and desired rotation

        */
        Vector3 distanceFromBasePos = this.transform.position - this._droneBasePosition;
        distanceFromBasePos = Mathf.Min(1, 0.1f * distanceFromBasePos.magnitude) * distanceFromBasePos.normalized;

        Vector3 point = new Vector3(_centerPoint.x, 0, _centerPoint.z );
        float angle = Vector3.Angle(this.transform.forward, point - this.transform.position) / 180;


    }

    //Start at awake
    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.useGravity = true;
        _rigidbody.mass = 2;
        _rigidbody.angularDrag = 20;
        _rigidbody.drag = 10;
        _mainSphere = this.transform.Find("DroneFull").gameObject;
        _mainSphere = _mainSphere.transform.Find("orb_drone_main").gameObject;
        _ring1 = this.transform.Find("DroneFull").gameObject;
        _ring1 = _ring1.transform.Find("lightRing_1").gameObject;
        _ring2 = this.transform.Find("DroneFull").gameObject;
        _ring2 = _ring2.transform.Find("lightRing_2").gameObject;      
        _listOfSensorData = new float[_numOfSensors + 3 + 1]; 

    }
    // Start is called before the first frame update
    void Start()
    {
 
       
    }

    // Update is called once per frame
    void Update()
    {
 
    }

    void FixedUpdate()
    {
        //ManualDroneMovement();
        _velocity = _rigidbody.velocity;

        if (_isMoving)
        {

        }

        if (_forwardThrustOn)
        {
            _forwardThrustForce += _engineAcceleration * Time.fixedDeltaTime;
            _rigidbody.AddRelativeForce(Vector3.forward * _forwardThrustForce);
        }

        else 
        {
            _forwardThrustForce = 0;
        }

        if (_upThrustOn)
        {
            _upThrustForce += _engineAcceleration * Time.fixedDeltaTime;
            _rigidbody.AddRelativeForce(Vector3.up * _upThrustForce);
        }

        else 
        {
            _upThrustForce = 0;
        }

        if (_rigidbody.velocity.magnitude > _maxVelocity)
        {
            _rigidbody.velocity = _rigidbody.velocity.normalized * _maxVelocity;
        }

        _rigidbody.AddRelativeTorque(Vector3.up * _angularSpeed * _vRot);

        DroneRotator();
        TestRotation();
    }
}
