using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingSystem : MonoBehaviour
{
    public GameObject Product;

    public Recipe recipe;

    private InventorySlot productSlot;
    private List<InventorySlot> craftingSlots = new List<InventorySlot>();
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).GetComponent<InventorySlot>() != null && transform.GetChild(i).GetComponent<InventorySlot>().isCraftingSlot) 
            {
                craftingSlots.Add(transform.GetChild(i).GetComponent<InventorySlot>());
                Debug.Log("Added Slot");
            }
        }
        productSlot = Product.GetComponent<InventorySlot>();
    }

    public void UpdateCrafting(int n)
    {
       


        // Get Crafting Items and construct CraftingData
        InventorySlot[,] slots = new InventorySlot[n,n];
        List<CraftingData> currentData = new List<CraftingData>();
        for (int index = 0; index < craftingSlots.Count; index++)
        {
            int i = index / n; int j = index % n;
            slots[i, j] = craftingSlots[index];
        }
        for (int i = 0; i < n; i++) // Fill currentData with items in Crafting Slots
        {
            for (int j = 0; j < n; j++)
            {
                if (slots[i,j].item != null) // Ima item na ovom mestu
                {
                    CraftingData data = new CraftingData();
                    data.item = slots[i, j].item;

                    if (i - 1 < 0)
                        data.left = null;
                    else
                        data.left = slots[i - 1, j].item;

                    if (i + 1 >= n)
                        data.right = null;
                    else
                        data.right = slots[i + 1, j].item;

                    if (j - 1 < 0)
                        data.up = null;
                    else
                        data.up = slots[i, j - 1].item;

                    if (j + 1 >= n)
                        data.down = null;
                    else
                        data.down = slots[i, j + 1].item;

                    currentData.Add(data);

                }
            }
        }
        if (currentData.Count == 0)
            return;

        if (CompareCraftingData(recipe.getCraftingData(), currentData))
        {
            productSlot.item = recipe.product;
            productSlot.quantity = recipe.quantity;
        }
        else
        {
            productSlot.item = null;
            productSlot.quantity = 0;
        }


        





    }
    public float frequency = 0.1f;

    private float elapsedTime = 0f;
    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= frequency)
        {
            ///////
            UpdateCrafting(2);
            //////
            elapsedTime = 0f;
        }
    }

    // Compare 2 litsts of CraftingData
    bool CompareCraftingData(List<CraftingData> craftingData, List<CraftingData> recipeData)
    {

      

        for (int i = 0; i < recipeData.Count; i++)
        {
            bool there = false;
            for (int j = 0; j < craftingData.Count; j++)           
                if (recipeData[i].Equals(craftingData[j]))
                    there = true;
            if (there == false)
                return false;
            
        }

        return true;
    }

    public void TakeProduct()
    {
        foreach (InventorySlot slot in craftingSlots)
        {
            if (slot.item != null)          
                slot.quantity--;
            
        }
    }
    
}
