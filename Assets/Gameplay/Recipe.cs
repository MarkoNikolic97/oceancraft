using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recipe : ScriptableObject
{
    [Header("Recipe")]
    public ArrayLayout recipeTable;

    public GameObject product;
    public int quantity;

    public List<CraftingData> getCraftingData()
    {
        List<CraftingData> currentData = new List<CraftingData>();

        

        for (int i = 0; i < 3; i++) // Fill currentData with items in Crafting Slots
        {
            for (int j = 0; j < 3; j++)
            {
                if (recipeTable.rows[i].row[j] != null) // Ima item na ovom mestu
                {
                    CraftingData data = new CraftingData();
                    data.item = recipeTable.rows[i].row[j];

                    if (i - 1 < 0)
                        data.left = null;
                    else
                        data.left = recipeTable.rows[i - 1].row[j];

                    if (i + 1 >= 3)
                        data.right = null;
                    else
                        data.right = recipeTable.rows[i + 1].row[j];

                    if (j - 1 < 0)
                        data.up = null;
                    else
                        data.up = recipeTable.rows[i].row[j - 1];

                    if (j + 1 >= 3)
                        data.down = null;
                    else
                        data.down = recipeTable.rows[i].row[j + 1];

                    currentData.Add(data);

                }
            } 
        }

        
        return currentData;

    }
}
