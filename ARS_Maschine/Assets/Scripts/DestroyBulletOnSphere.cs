using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyBulletOnSphere : MonoBehaviour
{
    // Start is called before the first frame update

    void OnTriggerExit (Collider other)
    {
        if (other.gameObject.tag == "bullet")
        {
            //Debug.Log("Collision detected");
            Destroy(other.gameObject);
        }

    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
