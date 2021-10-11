using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipVoxel : MonoBehaviour
{

    public Block block;
    // Start is called before the first frame update
    void Start()
    {
        block.currentHP = block.HP;
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
          //  other.GetComponent<PlayerController>().InShip = true;
            Debug.Log("IN SHIP");
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
           // other.GetComponent<PlayerController>().InShip = false;
        }
    }

    public void TakeDamage(int dmg)
    {
        block.currentHP -= dmg;
        if (block.currentHP <= 0)
        {
            Destroy(gameObject);
        }
    }
}
