using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : Item
{
    public bool isRaised = false;
    public override void OnLeftClick(PlayerController pc)
    {
        Debug.Log("SHIELD BASH");
    }

    public override void OnRightClick(PlayerController pc)
    {
        throw new System.NotImplementedException();
    }
}
