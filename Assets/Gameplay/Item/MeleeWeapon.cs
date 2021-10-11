using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : Item
{

    public float pushbackForce = 10f;
    public float sweep;
  

    public override void OnLeftClick(PlayerController pc)
    {
        // Moramo da weepujemo raycastove ispred playera i uzmemo sve hitovane
        Ray ray = pc.MainPlayerCamera.GetComponent<Camera>().ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        RaycastHit[] objectsHit = Physics.BoxCastAll(pc.MainPlayerCamera.transform.position, new Vector3(sweep,sweep,sweep), pc.MainPlayerCamera.transform.forward,Quaternion.identity, pc.player.range);

        if (pc.EquippedShield && pc.EquippedShield.GetComponent<Shield>().isRaised) // Shield je equipovan i podignut
        {
            pc.EquippedShield.GetComponent<Shield>().OnLeftClick(pc);
            return;
        }
       
        foreach (RaycastHit hit in objectsHit)
        {
           // Debug.Log(hit.collider.gameObject.name);
            GameObject hitObject = hit.collider.gameObject;
            if (hitObject.GetComponent<Enemy>())
            {
                hitObject.GetComponent<Rigidbody>().AddExplosionForce(pushbackForce * pushbackForce, pc.transform.position + Vector3.down*3, 100f);
                // PushBack
            }
        }


       
    }

    public override void OnRightClick(PlayerController pc)
    {
        if (pc.EquippedShield)
        {
            pc.EquippedShield.GetComponent<Shield>().isRaised = true;
        }
        else
        {
            Debug.Log("No Shield");
        }
    }
}
