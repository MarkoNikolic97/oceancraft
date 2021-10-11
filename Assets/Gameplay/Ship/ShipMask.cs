using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipMask : MonoBehaviour
{
    public GameObject Ship;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().InShip = true;
            //other.transform.SetParent(transform);
        }

    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().InShip = false;
            //other.transform.SetParent(null);
        }
    }
}
