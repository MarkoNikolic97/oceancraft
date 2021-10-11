using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, Damagaeble
{
    public string name;
    public int Level;

    public int STR, DEX, CON, INT;
    public float Armor, Health;

    public float mass = 1f;

    float hpClass = 10;
    public float damageReduction;
    public float maxHP;
    public float movementSpeed;
    public float sprintSpeed;
    public float stealthSpeed;
    public float underwaterSpeed;
    public float jumpHeight = 10f;
    public float healthRegeneration;
    public float range;
    public float accuracy;
    public float critChance;
    public float critDmgMultiplier = 2f;
    public float stealth;
    public float characterHeight = 2f, stealthHeight = 1.5f;
    public float characterBouyancy;

     float STRWeaponMod, DEXWeaponMod, relicEffectivnes;

    protected float Food;
    protected float Air;
    public int jumpCount = 2;

    //  public List<Aura> Auras = new List(Aura)


    // Start is called before the first frame update
    void Start()
    {
        

    }

    private float statsUpdateFrequency = 1f;
    private float statsElapsedTime = 0f;
    // Update is called once per frame
    void Update()
    {
        statsElapsedTime += Time.deltaTime;

        if (statsElapsedTime >= statsUpdateFrequency )
        {
            UpdateStats();
            statsElapsedTime = 0;
        }
    }

    public void UpdateStats()
    {
        maxHP = (hpClass + CON) * Level;

        // Set WPN modifiers
        STRWeaponMod = STR / 2;
        DEXWeaponMod = DEX / 2;
        relicEffectivnes = INT / 2;

        // Set stats




       
    }


    public void TakeDamage(float dmg)
    {
        throw new System.NotImplementedException();
    }
}
