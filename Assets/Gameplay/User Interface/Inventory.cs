using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public GameObject handSlot;
    public GameObject ItemTooltip;

    private GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        player = transform.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        // Hand Slot is where mouse is
        handSlot.transform.position = player.GetComponentInChildren<Camera>().ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, GetComponent<Canvas>().planeDistance));

       
 
    }

    public void ObjectPressed(GameObject pressed)
    {


        InventorySlot inHand = handSlot.GetComponent<InventorySlot>();
        InventorySlot slot = pressed.GetComponent<InventorySlot>();

        if (inHand.item == null) // Hand Empty -> Take Item
        {
            slot.TakeItem(inHand);

        }
        else // Hand Full
        {
            slot.PutItem(inHand);
            
        }

    }


}
