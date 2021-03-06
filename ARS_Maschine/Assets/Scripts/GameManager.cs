using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int _numOfDrones = 16;
    [SerializeField] private float _distOfDrones = 1;
    public GameObject dronePrefab;
    public GameObject cannonPrefab;
    public GameObject bulletPrefab;

    public GameObject bulletBoundaryPrefab;
    private bool _cannonInstantiated = false;

    private float _nextActionTime = 0f;

    public float attackPeriod = 1f;
    private List<GameObject> _listOfDrones = new List<GameObject>();
    private ARSNetwork _arsNetwork;

    void SetUpEnvironment(int numOfInputs, int numOfOutputs, int numOfDirections, int numOfBestDirections)
    {
        _arsNetwork = new ARSNetwork(numOfInputs, numOfOutputs, numOfDirections, numOfBestDirections);
    }
    
    void GenerateDrones(Vector3 center, float distance, int numOfDrones)
    {   
        float angle = 360f / numOfDrones;
        Quaternion rotation = Quaternion.Euler(0, angle, 0);
        Vector3 droneInitPos =  (-distance * Vector3.forward + center);
        GameObject tempDrone;

        for (int i = 0; i < numOfDrones;  i++)
        {   
            Quaternion droneRotation = Quaternion.Euler(0, i * angle ,0);
            tempDrone = Instantiate(dronePrefab, droneInitPos, droneRotation);
            tempDrone.GetComponent<DroneControll>().DroneBasePosition = droneInitPos;
            _listOfDrones.Add(tempDrone);
            
            droneInitPos = center + rotation * (droneInitPos - center);
        }
    }

    void CannonAttack(Vector3 center, List<GameObject> listOfDrones)
    {
        if (! _cannonInstantiated)
        {
            Instantiate(cannonPrefab, center, Quaternion.identity);
            Instantiate(bulletBoundaryPrefab, center, Quaternion.identity);
            _cannonInstantiated = true;
        }

        for (int i = 0; i < listOfDrones.Count; i++)
        {
            Vector3 targetpos = listOfDrones[i].transform.position;
            Vector3 bulletpos = center + 0.2f * targetpos;
            GameObject bullet = Instantiate(bulletPrefab, bulletpos, Quaternion.identity);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            rb.useGravity = false;
            rb.mass = 10;
            rb.drag = 0;
            rb.angularDrag = 0;
            rb.AddForce(500f * (targetpos - bulletpos));

        }
    }

    void RandomDroneMovement(List<GameObject> listOfDrones)
    {
        for (int i = 0; i < listOfDrones.Count; i++)
        {
            int direction;
            float r = Random.Range(0, 1f);
            //Debug.Log("Random number: " + r.ToString());

            if (r > 0.8)
            {
                direction = 1;
            }
            else if (r < 0.2)
            {
                direction = -1;
            }
            else 
            {
                direction = 0;
            }

            GameObject drone = listOfDrones[i];
            Rigidbody rb = drone.GetComponent<Rigidbody>();
            rb.AddForce(new Vector3(0, direction * 3000 , 0));
        }
    }

    private float[] ObservationState(GameObject drone)
    {
        /*
            Define input space:
            1) vector - distance between current position of drone and base position which is desired to maintain.
            In order to normalize inputs we need to prevent it from getting to high values. We do this by normalizing 
            our distance vector and multiplying by Min of 1 and its magnitude divided by 10.  When we divide we say that
            if magnitude of distance vector is higher than 10, we dont distinguish  any further movement away from base point.

        */
        Vector3 distanceFromBasePos = drone.transform.position - drone.GetComponent<DroneControll>().DroneBasePosition;
        distanceFromBasePos = Mathf.Min(1, 0.1f * distanceFromBasePos.magnitude) * distanceFromBasePos.normalized;

        return new float[]{1,2,3};;

    }

   // Start is called before the first frame update
    void Start()
    {
        GenerateDrones(new Vector3(0,.1f,0), _distOfDrones, _numOfDrones);       
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        if (Time.time > _nextActionTime)
        {
            _nextActionTime = Time.time + attackPeriod;
            //RandomDroneMovement(_listOfDrones);
            //CannonAttack(new Vector3(0,.5f,0), _listOfDrones);

        }

        foreach (GameObject drone in _listOfDrones)
        {
            drone.GetComponent<DroneControll>().BulletDetector(0);
        }       
    }


}
