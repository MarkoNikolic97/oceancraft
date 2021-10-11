using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    public float halfLife = 10f;


    public float damage;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private float elapsedTime = 0f;
    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= halfLife)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collison");
        if (collision.collider.gameObject.GetComponent<Damagaeble>() != null) 
        {
            Destroy(GetComponent<Rigidbody>());
            transform.parent = collision.collider.transform;
            collision.collider.gameObject.GetComponent<Damagaeble>().TakeDamage(damage);
        }
    }
}
