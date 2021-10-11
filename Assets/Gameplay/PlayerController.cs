using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PlayerController : MonoBehaviour
{
    public bool InShip;

    [Header("Inventory Settings")]
    public GameObject EquippedItem;
    public GameObject EquippedShield;

    public GameObject EquippedItems;
    public GameObject Inventory;
    public GameObject Crafting;
    public GameObject Armor;
    public GameObject PlayerView;
    public GameObject EquippedItemOutline;
    public int currentEquipped = 0;
    [Header("Movement Settings")]
    public bool isSprinting;
    public bool isStealth;
    public float sprintStepOffset = 1f;
    public float defaultStepOffset = 0.75f;
    public float gravity = 9.81f;
    private Vector3 velocityVector = Vector3.zero;

    [Header("Camera Settings")]
    public GameObject MainPlayerCamera;
    public float HorizontalLookSpeed, VerticalLookSpeed;
    public PostProcessProfile defaultPostProfile, underwaterPostProfile;
   
    public Block OutlineBlock;
    public GameObject OutlineBox;
    public GameObject CrossHairObject;



    [Header("Melee Combat Settings")]
    public float pushBackForce = 1f;

    [Header("Other Settings")]
    public Block block;

    public Player player;

    Animator playerAnimator;
    public GameObject playerTransform;
    private CharacterController characterController;
    private bool isLaunchPhase = false;
    // Use this for initialization
    void Start()
    {
        player = GetComponent<Player>();
        playerAnimator = GetComponentInChildren<Animator>();
        characterController = GetComponent<CharacterController>();
    }


    int currentJumpCount = 0;
    private Vector3Int currentOutlineVoxel;
    private float elapsedTimeFromLastHit = 0f;

   
    private bool inventoryOpen;
    private bool craftingOpen;
    void Update()
    {
     
        // INVENTORY //
        if (Input.GetKeyDown(KeyCode.I))
            inventoryOpen = !inventoryOpen;

        if (Input.GetKeyDown(KeyCode.C))
            craftingOpen = !craftingOpen;


        if (inventoryOpen)
        {
            Inventory.SetActive(true);
            //Armor.SetActive(true);
           // Crafting.SetActive(true);
            //PlayerView.SetActive(true);
           // Crafting.SetActive(true);

            return;
        }
        else
        {
            Inventory.SetActive(false);
            Armor.SetActive(false);
            Crafting.SetActive(false);
            PlayerView.SetActive(false);
            Crafting.SetActive(false);
        }

        

        if (Input.GetKeyDown(KeyCode.Alpha1))
            currentEquipped = 0;
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            currentEquipped = 1;
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            currentEquipped = 2;
        else if (Input.GetKeyDown(KeyCode.Alpha4))
            currentEquipped = 3;
        else if (Input.GetKeyDown(KeyCode.Alpha5))
            currentEquipped = 4;
        else if (Input.GetKeyDown(KeyCode.Alpha6))
            currentEquipped = 5;
        else if (Input.GetKeyDown(KeyCode.Alpha7))
            currentEquipped = 6;
        else if (Input.GetKeyDown(KeyCode.Alpha8))
            currentEquipped = 7;
        else if (Input.GetKeyDown(KeyCode.Alpha9))
            currentEquipped = 8;
        else if (Input.GetKeyDown(KeyCode.Alpha0))
            currentEquipped = 9;


        EquippedItem = EquippedItems.transform.GetChild(currentEquipped).GetComponent<InventorySlot>().item;
        EquippedItemOutline.transform.position = EquippedItems.transform.GetChild(currentEquipped).position;

        //// 
        if (GetComponentInChildren<Camera>().GetComponentInChildren<BoatAlignNormal>().InWater && !InShip)
        {
            GetComponentInChildren<PostProcessVolume>().profile = underwaterPostProfile;
        }
        else
        {
            GetComponentInChildren<PostProcessVolume>().profile = defaultPostProfile;
        }

        //Jump
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (currentJumpCount == 0 && characterController.isGrounded == false)
            {
                //Debug.Log("NE");
            }
            else if (currentJumpCount < player.jumpCount)
            {
                // Debug.Log("Current Jump: " + currentJumpCount + "-----" + player.jumpCount);
                StartCoroutine(JumpCoroutine());
                currentJumpCount = currentJumpCount + 1;

            }


        }
        if (characterController.isGrounded)
        {
            currentJumpCount = 0;
        }

        // Sprinting
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (GetComponentInChildren<BoatAlignNormal>().InWater && !InShip)
            {
                characterController.Move(playerTransform.transform.TransformVector(Vector3.up * Time.deltaTime * player.underwaterSpeed));
            }
            else
            {
                isSprinting = true;
                GetComponent<CharacterController>().stepOffset = sprintStepOffset;
            }
        }
        else
        {
            isSprinting = false;
            GetComponent<CharacterController>().stepOffset = defaultStepOffset;
        }

        // Stealth / Crouch
        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (GetComponentInChildren<BoatAlignNormal>().InWater && !InShip)
            {
                characterController.Move(playerTransform.transform.TransformVector(Vector3.down * Time.deltaTime * player.underwaterSpeed));
            }
            else
            {
                isStealth = true;
                GetComponent<CharacterController>().height = GetComponent<Player>().stealthHeight;
            }
        }
        else
        {
            isStealth = false;
            GetComponent<CharacterController>().height = GetComponent<Player>().characterHeight;
        }



        Ray ray = MainPlayerCamera.GetComponent<Camera>().ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit = new RaycastHit();
        Physics.Raycast(ray, out hit, player.range);

        elapsedTimeFromLastHit += Time.deltaTime;
        // Handle Input
        if (Input.GetMouseButton(0)) // Left Click
        {
            
            if (EquippedItem != null && EquippedItem.GetComponent<Item>())
            {
                // Implement Attack Speed
                if (elapsedTimeFromLastHit >= EquippedItem.GetComponent<Item>().attackSpeed)
                {
                    EquippedItem.GetComponent<Item>().OnLeftClick(this);

                    elapsedTimeFromLastHit = 0f;
                }

            }
           

            

        }



        if (EquippedShield)
            EquippedShield.GetComponent<Shield>().isRaised = false;
        if (EquippedItem && EquippedItem.GetComponent<RangedWeapon>())
        {
           // CrossHairObject.GetComponent<CrossHairController>().distanceFromCenter = 
            if (EquippedItem.GetComponent<RangedWeapon>().isAiming == false)
            {
                EquippedItem.GetComponent<RangedWeapon>().isMaxPower = false;
                EquippedItem.GetComponent<RangedWeapon>().currentDamageMod = 0f;
            }
            EquippedItem.GetComponent<RangedWeapon>().isAiming = false;
            
        }

        if (Input.GetMouseButtonDown(1)) // Build Voxel
        {
            if (EquippedItem && EquippedItem.GetComponent<Item>())
            {
               

                EquippedItem.GetComponent<Item>().OnRightClick(this);
                Debug.Log("Clicked");
            }
          

        }
        else
        {
            // Handle Outline and Block damage visuals
            if (hit.collider == null)
            {
              
                OutlineBox.SetActive(false);
            }
            else if (hit.collider.gameObject.GetComponent<Chunk>() == null)
            {
              
                OutlineBox.SetActive(false);

            }
            else if (hit.collider)
            {
                // Voxel You are looking at
                Vector3Int voxelHitIndex = hit.collider.gameObject.GetComponent<Chunk>().getVoxelFromWorldPosition(hit.point);
                if (voxelHitIndex != currentOutlineVoxel)
                {
                    OutlineBox.SetActive(true);

                    SetOutlineTexture(hit.collider.GetComponent<Chunk>().biome.VoxelData[voxelHitIndex.x, voxelHitIndex.y, voxelHitIndex.z]);
                    OutlineBox.GetComponent<MeshFilter>().mesh.uv = OutlineBlock.GetUV();
                    OutlineBox.GetComponent<MeshFilter>().mesh.RecalculateNormals();
                    OutlineBox.GetComponent<MeshFilter>().mesh.RecalculateBounds();

                     OutlineBox.transform.position = hit.collider.GetComponent<Chunk>().biome.VoxelData[voxelHitIndex.x, voxelHitIndex.y, voxelHitIndex.z].position;
                   // OutlineBox.transform.position = new Vector3(voxelHitIndex.x, voxelHitIndex.y, voxelHitIndex.y);
                    currentOutlineVoxel = voxelHitIndex;
                }
                //hit.collider.gameObject.GetComponent<Chunk>().AddVoxel(hit.point, hit.normal, block);

            }
        }


        // Handle Camera Rotation
        float horizontalAxis = Input.GetAxis("Mouse X");
        float verticalAxis = Input.GetAxis("Mouse Y") * -1;
        MainPlayerCamera.transform.Rotate(verticalAxis, horizontalAxis, 0);
        
        

        Vector3 CamRotation = MainPlayerCamera.transform.localEulerAngles;
        CamRotation.z = 0;
        MainPlayerCamera.transform.localEulerAngles = CamRotation;


        // Set up Movement Transform Helper
         CamRotation.x = 0;

        playerTransform.transform.localEulerAngles = CamRotation;

        transform.localEulerAngles = playerTransform.transform.localEulerAngles;



        /////////// MOVEMENT

        Vector3 movementVector = new Vector3(0, 0, 0);

        // Calculate movement Vector
        if (Input.GetKey(KeyCode.W))
        {
            movementVector += Vector3.forward;
        }
        if (Input.GetKey(KeyCode.A))
        {
            movementVector += Vector3.left;
        }
        if (Input.GetKey(KeyCode.S))
        {
            movementVector += Vector3.back;
        }
        if (Input.GetKey(KeyCode.D))
        {
            movementVector += Vector3.right;
        }
        // Apply Motion
        movementVector.Normalize();

        if (isSprinting && Input.GetKey(KeyCode.W))
        {
            movementVector = Vector3.forward;
            movementVector *= GetComponent<Player>().sprintSpeed;         
        }
        else if (isStealth)
        {
            movementVector *= GetComponent<Player>().stealthSpeed;
        }
        else
        {
            movementVector *= GetComponent<Player>().movementSpeed;
        }

        
      

       
      

        

        // Bouyancy
        if (GetComponentInChildren<BoatAlignNormal>().InWater && !InShip)
        {
            
           // velocityVector += Time.deltaTime * player.characterBouyancy * Vector3.up;
            movementVector = movementVector.normalized * player.underwaterSpeed;
            if (velocityVector.magnitude > 0.01f)
            {
                velocityVector *= 0.8f;
            }
        }
        else
        {
            // Gravity
            if (characterController.isGrounded == false && isLaunchPhase == false)
            {
                velocityVector = velocityVector + Time.deltaTime * gravity * Vector3.down;
            }
            else
            {
                velocityVector = Vector3.down;
            }
        }

        // Apply Final Movement
        movementVector += velocityVector;
        characterController.Move(playerTransform.transform.TransformVector(movementVector * Time.deltaTime));
      

    }
  
    // Update is called once per frame
   public IEnumerator JumpCoroutine()
    {
        Vector3 velocityVector = Vector3.up * player.GetComponent<Player>().jumpHeight;

        characterController.Move(velocityVector * Time.deltaTime);


        isLaunchPhase = true;
        while (velocityVector.y > 0)
        {
            
            characterController.Move(velocityVector*Time.deltaTime);
            velocityVector -= Time.deltaTime * gravity * Vector3.up;
            

            yield return null;
        }


        isLaunchPhase = false;
        


        yield return null;
    }
    

    public void SetOutlineTexture(Voxel voxel)
    {
        float dmgTakenFactor = voxel.currentHP / voxel.HP;
        dmgTakenFactor = 1 - dmgTakenFactor;
        //Debug.Log(dmgTakenFactor);

        if (dmgTakenFactor <= 0)
        {
            OutlineBlock.topTile = new Vector2Int(10, 7);
            OutlineBlock.sideTile = new Vector2Int(10, 7);
            OutlineBlock.bottomTile = new Vector2Int(10, 7);


            return;
        }


        //  Debug.Log((int)(i * 10));
        OutlineBlock.topTile = new Vector2Int((int)((dmgTakenFactor * 10) - 1), 0);
        OutlineBlock.sideTile = new Vector2Int((int)((dmgTakenFactor * 10) - 1), 0);
        OutlineBlock.bottomTile = new Vector2Int((int)((dmgTakenFactor * 10) - 1), 0);

        return;


    }



    public void OnHit()
    {
        Ray ray = MainPlayerCamera.GetComponent<Camera>().ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit = new RaycastHit();
        Physics.Raycast(ray, out hit, player.range);


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

        }
        else // Block/Voxel was hit
        {


            hit.collider.gameObject.GetComponent<Chunk>().VoxelHit(hit.point, 1, gameObject);

        }
    }
}
