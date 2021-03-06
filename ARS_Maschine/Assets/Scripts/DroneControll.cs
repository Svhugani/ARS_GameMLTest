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
    private int _numberOfSensors = 5;
    private float _spotAngleOfSensors = 20f;
    private float[] _listOfSensorData;
    private float _sensorRange = 8f;


    public float FitnessValue
    {
        get { return _fitnessValue; }
        set { _fitnessValue = value; }
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

    void GenerateRayDetectors()
    {
        float angle = 360f / _numberOfSensors;
        Quaternion rotation = Quaternion.Euler(0, angle, 0);
        Vector3 detectorDirection = this.transform.forward;

    }
    public void BulletDetector(float rayRange, int layer)
    {   
        //SortingLayer.NameToID("DamageLayer")
        //Vector3 detectorDirection = Random.onUnitSphere;
        Vector3 detectorDirection = this.transform.forward;
        if(Physics.Raycast(this.transform.position, detectorDirection, out RaycastHit hitInfo, rayRange))
        {
            Debug.DrawRay(this.transform.position, detectorDirection * hitInfo.distance, Color.red);
            Debug.Log("hitted bullet");
        }
        else
        {
            Debug.DrawRay(this.transform.position, detectorDirection * rayRange, Color.white);
            Debug.Log("----");
        }
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
    }
}
