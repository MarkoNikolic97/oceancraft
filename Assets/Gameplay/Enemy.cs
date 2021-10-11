using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, Damagaeble
{

    public float HP;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void TakeDamage(float dmg)
    {
        Debug.Log("Hit and dealt:" + dmg);
    }
}
