using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public string ItemName;

    public float damage;
    public float blockDamageModifier;
    public float attackSpeed; // razlika u vremena udaraca u sekundama

    public Sprite thumbnail;

    public abstract void OnLeftClick(PlayerController pc);
    public abstract void OnRightClick(PlayerController pc);
}
