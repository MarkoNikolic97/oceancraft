using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    //
    public GameObject item;
    public int quantity;

    public bool isArmorSlot, isShieldSlot, isCraftingSlot, isProductSlot;

    private SpriteRenderer thumbnailDisplay;
    private Text quantityDisplay;
    
    void Update()
    {
        // Set thumbnail and quantity
        //   if (thumbnailDisplay.sprite != item.GetComponent<Item>().thumbnail)
        if (quantity <= 0)
            item = null;

        if (item != null)
        {
            thumbnailDisplay.gameObject.SetActive(true);
            quantityDisplay.gameObject.SetActive(true);

            thumbnailDisplay.sprite = item.GetComponent<Item>().thumbnail;
            quantityDisplay.text = quantity.ToString();
        }
        else
        {
            thumbnailDisplay.gameObject.SetActive(false);
            quantityDisplay.gameObject.SetActive(false);
        }
       
    }

    void Start()
    {
        thumbnailDisplay = GetComponentInChildren<SpriteRenderer>();
        quantityDisplay = GetComponentInChildren<Text>();
    }

    public InventorySlot PutItem(InventorySlot inHand)
    {
       

        if (inHand.item.GetComponent<Armor>() == null && isArmorSlot)
            return null;

        if (inHand.item.GetComponent<Shield>() == null && isShieldSlot)
            return null;

        if (item == null && !isProductSlot) // Slot is free
        {
            item = inHand.item;
            quantity = inHand.quantity;

            inHand.item = null;
            inHand.quantity = 0;
        }
        else // Slot is full
        {
            if (item == inHand.item) // Same item
            {
                if (isProductSlot)
                {
                    inHand.quantity += quantity;
                    transform.parent.GetComponent<CraftingSystem>().TakeProduct();
                }
                else
                {
                    quantity += inHand.quantity;

                    inHand.item = null;
                    inHand.quantity = 0;
                }
            }
            else if(!isProductSlot)// Different Item - Switch
            {
                GameObject i = inHand.item;
                int q = inHand.quantity;

                inHand.item = item;
                inHand.quantity = quantity;

                item = i;
                quantity = q;
                
            }
        }

        
        
        return null;
    }

    public InventorySlot TakeItem(InventorySlot inHand)
    {
        if (isProductSlot && item != null)
        {
            transform.parent.GetComponent<CraftingSystem>().TakeProduct();

           
            inHand.quantity = quantity;
            inHand.item = item;
            
            quantity = 0;
            item = null;

            return null;
        }

        if (Input.GetKey(KeyCode.LeftShift)) // Split Stack
        {
            inHand.item = item;

            inHand.quantity = quantity / 2;
            quantity /= 2;

        }
        else
        {
            inHand.quantity = quantity;
            inHand.item = item;

            quantity = 0;
            item = null;
        }

        return null;
    }

     void OnMouseOver()
    {
        Debug.Log("EEEEEJ");
        transform.parent.parent.GetComponent<Inventory>().ItemTooltip.GetComponent<Text>().text = item.GetComponent<Item>().name;
    }
    private void OnMouseDown()
    {
        Debug.Log("EEEEEJ");
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item != null && transform.parent.parent.GetComponent<Inventory>() != null)
        {

            Text text = transform.parent.parent.GetComponent<Inventory>().ItemTooltip.GetComponent<Text>();
            GameObject tooltip = transform.parent.parent.GetComponent<Inventory>().ItemTooltip;

            tooltip.transform.position = transform.position + new Vector3(0, 0.05f,0);
            text.text = item.GetComponent<Item>().ItemName;
            tooltip.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (transform.parent.parent.GetComponent<Inventory>() != null)
        {


            GameObject tooltip = transform.parent.parent.GetComponent<Inventory>().ItemTooltip;

            tooltip.SetActive(false);
        }
    }

}
