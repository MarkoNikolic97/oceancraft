using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBlock : Item
{
    public Block block;

    //
   

    public override void OnLeftClick(PlayerController pc)
    {
        Ray ray = pc.MainPlayerCamera.GetComponent<Camera>().ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit = new RaycastHit();
        Physics.Raycast(ray, out hit, pc.player.range);


        // Debug.Log("CLICK");
        if (hit.collider == null)
        {
            // Debug.Log("Collider is null");
        }
        else if (hit.collider.gameObject.GetComponent<Chunk>() == null)// Something other then voxel hit
        {

            if (hit.collider.gameObject.GetComponent<Enemy>())
            {
                GameObject enemy = hit.collider.gameObject;
                // Implement DMG dealing
            }

            // Check if ShipVox is hit
            if (hit.collider.gameObject.GetComponent<ShipVoxel>())
            {
                hit.collider.gameObject.GetComponent<ShipVoxel>().TakeDamage(1);
            }

        }
        else // Block was hit
        {

            

            hit.collider.gameObject.GetComponent<Chunk>().VoxelHit(hit.point, damage * blockDamageModifier, pc.gameObject);

        }
    }

    public override void OnRightClick(PlayerController pc)
    {
        Ray ray = pc.MainPlayerCamera.GetComponent<Camera>().ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit = new RaycastHit();
        Physics.Raycast(ray, out hit, pc.player.range);

        if (hit.collider == null)
        {

        }
        else if (hit.collider.gameObject.GetComponent<Chunk>() == null)
        {

        }
        else
        {
            pc.EquippedItems.transform.GetChild(pc.currentEquipped).GetComponent<InventorySlot>().quantity--;
            hit.collider.gameObject.GetComponent<Chunk>().AddVoxel(hit.point, hit.normal, block);


        }

        
    }
}
