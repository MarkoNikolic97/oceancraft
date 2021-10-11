using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedWeapon : Item
{
    
    


    public float forceMod;
    public float maxDamageBonus = 5f;
    // Accuracy depends on player, spreadFactor depends on Weapon
    public float spreadFactor = 0.02f;
    public float currentDamageMod = 0f;

    public GameObject projectile;
    public bool isAiming = false;
    public bool isMaxPower = false;

    public override void OnLeftClick(PlayerController pc)
    {
        if (isAiming)
        {
            GameObject arrow = Instantiate(projectile, transform.position, Quaternion.identity);
            Vector3 direction = pc.MainPlayerCamera.transform.forward;

            if (isMaxPower)
            {
                arrow.GetComponent<Projectile>().damage = damage + maxDamageBonus;
            }
            else
            {
                arrow.GetComponent<Projectile>().damage = damage * currentDamageMod;
            }

            float spread = Mathf.Lerp(spreadFactor, 0f, pc.player.accuracy);


            direction.x += Random.Range(-spread, spread);
            direction.y += Random.Range(-spread, spread);
            direction.z += Random.Range(-spread, spread);

            arrow.transform.up = direction;
            arrow.SetActive(true);
            arrow.GetComponent<Rigidbody>().AddForce(arrow.transform.up * forceMod * forceMod);

            isMaxPower = false;
            isAiming = false;

           
        }
    }

    float elapsedAimTime = 0f;
    public override void OnRightClick(PlayerController pc)
    {
        isAiming = true;

        if (!isMaxPower)
        {
            elapsedAimTime += Time.deltaTime;
        }
        
       
        if (elapsedAimTime >= attackSpeed)
        {
            isMaxPower = true;
            elapsedAimTime = 0f;
        }
        else
        {
            currentDamageMod = Mathf.InverseLerp(0f, attackSpeed, elapsedAimTime);
        }

        // Update crosshair

    }
}
