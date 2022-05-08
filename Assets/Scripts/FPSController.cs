using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class FPSController : MonoBehaviour
{
    public int lives = 3;
    int timesDied = 0;
    public Text textToDisplay;

    public GameObject pushAbilityPrefab;
    public GameObject BlastPrefab;
    public CompassScript compass;
    public GameObject[] checkPoints;
    int currentCheckpoint = 0;

    Vector3 startPosition;
    //Player class
    public InventoryObject inventory;
    public InventoryObject equipement;
    public Attribute[] attributes;

    public GameObject bloodPrefab;
    public GameObject uiBloodSplatter;
    public GameObject gameOverPrefab;
    public Transform shotDirection;
    public GameObject stevePrefab;
    public Slider healthbar;
    public Text ammoReserves;
    public AudioSource jumpSound;
    //public AudioSource shot;
    public AudioSource landSound;
    public AudioSource[] steps;
    public AudioSource healthPick;
    public AudioSource ammoPick;
    //public AudioSource noAmmoSound;
    public AudioSource deathSound;
    //public AudioSource reloadSound;
    public GameObject camera;
    public Animator anim;
    readonly float speed = 0.10f;
    readonly float xSensitivity = 2;
    readonly float ySensitivity = 2;
    float minX = -90;
    float maxX = 90;
    Rigidbody rb;
    public CapsuleCollider cap;
    public bool InventoryActive = false;
    GameObject[] inventories;

    public GameObject canvas;
    float cWidth;
    float cHeight;

    Quaternion cameraRotation;
    Quaternion characterRotation;

    bool cursorIsLocked = true;
    bool lockCursor = true;

    public float x;
    public float z;

    public Gun gun;

    public int health = 0;
    int maxHealth = 100;

    public bool playingWalking = false;
    bool previouslyGrounded = true;

    public bool[] AbillityArray = new bool[3];
    public int AbillityCount = 0;

    IEnumerator WaitTextTime()
    {
        yield return new WaitForSeconds(5);
        textToDisplay.enabled = false;
    }

    public void AttributeModified(Attribute attribute)
    {
        Debug.Log(string.Concat(attribute.type, "was updated! Value is now", attribute.value.ModifiedValue));
    }
    private void OnApplicationQuit()
    {
        inventory.Clear();
        equipement.Clear();
    }

    public void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Vacinee")
        {
            AbillityArray[AbillityCount] = true;
            if(AbillityCount == 0)
            {
                textToDisplay.text = "You have succesfully found the first vaccine dose, now you can use Dash ability by clicking SHIFT key";
                textToDisplay.enabled = true;
                StartCoroutine(WaitTextTime());
            }
            if(AbillityCount == 1)
            {
                textToDisplay.text = "You have succesfully found the second vaccine dose, now you can use Blast ability by clicking Q key";
                textToDisplay.enabled = true;
                StartCoroutine(WaitTextTime());
            }
            if(AbillityCount == 2)
            {
                textToDisplay.text = "You have succesfully found the third vaccine dose, now you can use Push ability by clicking Right Mouse key";
                textToDisplay.enabled = true;
                StartCoroutine(WaitTextTime());
            }
            AbillityCount++;           
        }

        if (col.gameObject.tag == "SpawnPoint")
        {
            startPosition = this.transform.position;

            textToDisplay.text = "You have succesfully reached the checkpoint now after you die you will respawn in this place";
            textToDisplay.enabled = true;
            StartCoroutine(WaitTextTime());

            if (col.gameObject == checkPoints[currentCheckpoint])
            {
                currentCheckpoint++;
                compass.target = checkPoints[currentCheckpoint];
            }
        }

        if (col.gameObject.tag == "Home")
        {
            Vector3 pos = new Vector3(this.transform.position.x,
                                        Terrain.activeTerrain.SampleHeight(this.transform.position),
                                        this.transform.position.z);

            GameObject steve = Instantiate(stevePrefab, pos, this.transform.rotation);
            steve.GetComponent<Animator>().SetTrigger("Dance");
            GameStats.gameOver = true;
            Destroy(this.gameObject);
            GameObject gameOverText = Instantiate(gameOverPrefab);
            gameOverText.transform.SetParent(canvas.transform);
            gameOverText.transform.localPosition = Vector3.zero;
        }

        var item = col.GetComponent<GroundItem>();
        if (item)
        {
            Item _item = new Item(item.item);
            if (inventory.AddItem(_item, 1))
            {
                Destroy(col.gameObject);
                //DestroyImmediate(col.gameObject, false);
            }

        }
    }

    GameObject steve;

    public void TakeHit(float amount)
    {

        if (GameStats.gameOver) return;

        health = (int)(Mathf.Clamp(health - amount, 0, maxHealth));
        healthbar.value = health;

        GameObject bloodSplatter = Instantiate(uiBloodSplatter);
        bloodSplatter.transform.SetParent(canvas.transform);
        bloodSplatter.transform.position = new Vector3(Random.Range(0, cWidth), Random.Range(0, cHeight), 0);
        Destroy(bloodSplatter, 2.2f);

        //Debug.Log("Health:" + health);
        if (health <= 0)
        {
            Vector3 pos = new Vector3(this.transform.position.x,
                                         Terrain.activeTerrain.SampleHeight(this.transform.position),
                                         this.transform.position.z);

            steve = Instantiate(stevePrefab, pos, this.transform.rotation);
            steve.GetComponent<Animator>().SetTrigger("Death");

            GameStats.gameOver = true;
            //steve.GetComponent<AudioSource>().enabled = false;
            timesDied++;

            if (timesDied == lives)
            {
                Destroy(this.gameObject);
            }
            else
            {
                steve.GetComponent<GoToMainMenu>().enabled = false;
                camera.SetActive(false);
                Invoke("Respawn", 4);
                Debug.Log("ShouldRespawn");
            }
            
        }
    }

    void Respawn()
    {
        Destroy(steve);
        camera.SetActive(true);
        GameStats.gameOver = false;
        health = maxHealth;
        healthbar.value = health;
        this.transform.position = startPosition;
    }

    // Start is called before the first frame update
    void Start()
    {
        startPosition = this.transform.position;

        gun = FindObjectOfType<Gun>();
        for (int i = 0; i< attributes.Length; i++)
        {
            attributes[i].SetParent(this);
        }

        for (int i = 0; i < equipement.GetSlots.Length; i++)
        {
            equipement.GetSlots[i].OnBeforeUpdate += OnBeforeSlotUpdate;
            equipement.GetSlots[i].OnAfterUpdate += OnAfterSlotUpdate;
        }

        rb = this.GetComponent<Rigidbody>();
        cap = this.GetComponent<CapsuleCollider>();

        cameraRotation = camera.transform.localRotation;
        characterRotation = this.transform.localRotation;

        health = maxHealth;
        //healthbar.value = health;
        //ammoReserves.text = ammoClip+ "/" + ammo + "";

        cWidth = canvas.GetComponent<RectTransform>().rect.width;
        cHeight = canvas.GetComponent<RectTransform>().rect.height;
        
        inventories = GameObject.FindGameObjectsWithTag("Inventory");
        for (int i = 0; i < inventories.Length; i++)
        {
            inventories[i].transform.position = new Vector3(inventories[i].transform.position.x, inventories[i].transform.position.y - 1000
                , inventories[i].transform.position.z);
        }

        compass.target = checkPoints[0];
    }

    public void OnBeforeSlotUpdate(InventorySlot _slot)
    {
        if(_slot.ItemObject != null)
        {
            return;
        }
        switch (_slot.parent.inventory.type)
        {
            case InterfaceType.Inventory:

                break;
            case InterfaceType.Equipement:
                print(string.Concat("Removed", _slot.ItemObject, "on", _slot.parent.inventory.type,
                        ", Allowed items:", string.Join(", ", _slot.AllowedItems)));

                for (int i = 0; i < _slot.item.buffs.Length; i++)
                {
                    for (int j = 0; j < attributes.Length; j++)
                    {
                        if (attributes[j].type == _slot.item.buffs[i].attribute)
                        {
                            attributes[j].value.RemoveModifiers(_slot.item.buffs[i]);
                        }
                    }
                }

                break;
            case InterfaceType.Chest:

                break;
            default:
                break;
        }

    }
    public void OnAfterSlotUpdate(InventorySlot _slot)
    {
        if(_slot.ItemObject == null)
        {
            return;
        }

        switch (_slot.parent.inventory.type)
        {
            case InterfaceType.Inventory:
                break;
            case InterfaceType.Equipement:
                //print(string.Concat("Placed", _slot.ItemObject, "on", _slot.parent.inventory.type,
                        //", Allowed items:", string.Join(", ", _slot.AllowedItems)));

                for (int i = 0; i < _slot.item.buffs.Length; i++)
                {
                    for (int j = 0; j < attributes.Length; j++)
                    {
                        if(attributes[j].type == _slot.item.buffs[i].attribute)
                        {
                            attributes[j].value.AddModifiers(_slot.item.buffs[i]);
                        }
                    }
                }

                break;
            case InterfaceType.Chest:

                break;
            default:
                break;
        }
    }

    float NextFire;
    float fireRate = 3f;
    // Update is called once per frame
    public void Update()
    {

        if (Input.GetKeyDown(KeyCode.Mouse1) && AbillityArray[1])
        {
            Debug.Log(AbillityArray[0]);
            var pushSphere = Instantiate(pushAbilityPrefab, transform.position, transform.rotation);
            pushSphere.GetComponent<PushAbility>().transformToFollow = transform;
        }

        if (Input.GetKeyDown(KeyCode.Q) && Time.time > NextFire && AbillityArray[2])
        {
            NextFire = Time.time + fireRate;
            var tempBlast = Instantiate(BlastPrefab, camera.transform.position + camera.transform.forward, camera.transform.rotation);
            tempBlast.GetComponent<Blast>().Player = transform;
        }

        //Home Village
        if (Input.GetKeyDown(KeyCode.Z))
        {
            this.transform.position = new Vector3(939, 1.89f, 85.3f);
        }

        //Village2

        if (Input.GetKeyDown(KeyCode.X))
        {
            this.transform.position = new Vector3(852, 3.4f, 440);
        }

        //Village 3
        if (Input.GetKeyDown(KeyCode.C))
        {
            this.transform.position = new Vector3(647.45f, 6.78f, 238.22f);
        }

        //Village 4
        if (Input.GetKeyDown(KeyCode.V))
        {
            this.transform.position = new Vector3(359.13f, 6.36f, 699f);
        }

        //Village 5
        if (Input.GetKeyDown(KeyCode.B))
        {
            this.transform.position = new Vector3(314.83f, 3.36f, 419f);
        }


        if (Input.GetKeyDown(KeyCode.F))
        {
            anim.SetBool("arm", !anim.GetBool("arm"));
        }

        if (Input.GetKeyDown(KeyCode.I) && !InventoryActive)
        {
            gun.enabled = false;
            //Debug.Log("I pressed set active");
            for (int i = 0; i < inventories.Length; i++)
            {
                
                inventories[i].transform.position = new Vector3(inventories[i].transform.position.x, inventories[i].transform.position.y 
                    + 1000, inventories[i].transform.position.z);

            }
            cursorIsLocked = false;
            anim.SetBool("arm", !anim.GetBool("arm"));
            InventoryActive = true;

        }else if (Input.GetKeyDown(KeyCode.I) && InventoryActive)
        {
            gun.enabled = true;
            for (int i = 0; i < inventories.Length; i++)
            {

                inventories[i].transform.position = new Vector3(inventories[i].transform.position.x, inventories[i].transform.position.y 
                    - 1000, inventories[i].transform.position.z);
                
            }
            cursorIsLocked = true;
            anim.SetBool("arm", !anim.GetBool("arm"));
            InventoryActive = false;

        }

        if(Mathf.Abs(x) > 0 || Mathf.Abs(z) > 0)
        {
            if (!anim.GetBool("moving"))
            {
                anim.SetBool("moving", true);
                InvokeRepeating("PlaySteps", 0, 0.45f);
            }
                
        }
        else if(anim.GetBool("moving"))
        {
            anim.SetBool("moving", false);
            CancelInvoke("PlaySteps");
            playingWalking = false;
        }

        bool grounded = IsGrounded();

        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            rb.AddForce(0, 300, 0);
            jumpSound.Play();
            if (anim.GetBool("moving"))
            {
                CancelInvoke("PlaySteps");
                playingWalking = false;
            }
        }
        else if(!previouslyGrounded && grounded)
        {
            landSound.Play();
        }

        previouslyGrounded = grounded;

    }

    public void PlaySteps()
    {
        AudioSource audioSource = new AudioSource();
        int random = Random.Range(1, steps.Length);
        audioSource = steps[random];
        audioSource.Play();
        steps[random] = steps[0];
        steps[0] = audioSource;
        playingWalking = true;
    }

    void FixedUpdate() 
    {
        float yRotation = Input.GetAxis("Mouse X") * xSensitivity;
        float xRotation = Input.GetAxis("Mouse Y") * ySensitivity;

        if (!InventoryActive) 
        {

            cameraRotation *= Quaternion.Euler(-xRotation, 0, 0);
            characterRotation *= Quaternion.Euler(0, yRotation, 0);

            cameraRotation = ClampRotationAroundXAxis(cameraRotation);

            this.transform.localRotation = characterRotation;
            camera.transform.localRotation = cameraRotation;
        }
       

        x = Input.GetAxis("Horizontal") * speed;
        z = Input.GetAxis("Vertical") * speed;

        
        //new Vector3(x * speed, 0 , z * speed);
        transform.position += camera.transform.forward * z + camera.transform.right * x;

        UpdateCursorLock();
    }

    public Quaternion ClampRotationAroundXAxis(Quaternion q)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;

        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);
        angleX = Mathf.Clamp(angleX, minX, maxX);
        q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

        return q;
    }
    public bool IsGrounded()
    {
        if (Physics.SphereCast(transform.position, cap.radius, Vector3.down,
            out _, (cap.height / 2f) - cap.radius + 0.12f))
        {
            return true;
        }
        else
        {
            return false;
        }

    }
    public void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.tag == "Ammo" && gun.Ammo < gun.maxAmmoReserve)
        {

            gun.Ammo = Mathf.Clamp(gun.Ammo + 50, 0, gun.maxAmmoReserve);
            ammoReserves.text = gun.currentAmmo + "/" + gun.Ammo + "";
            //Debug.Log("Ammo: " + ammo);
            Destroy(col.gameObject);
            ammoPick.Play();
            
        }
        else if (col.gameObject.tag == "Med" && health < maxHealth)
        {
            health = Mathf.Clamp(health + 50, 0, 100);
            healthbar.value = health;
            Debug.Log("Health " + health);
            Destroy(col.gameObject);
            healthPick.Play();

        }

        if (IsGrounded())
        {

            if (anim.GetBool("moving") && !playingWalking)
            {
                InvokeRepeating("PlaySteps", 0, 0.45f);
            }        
            
        }
    }
    public void SetCursorLock(bool v)
    {
        lockCursor = v;
        if (!lockCursor)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void UpdateCursorLock()
    {
        if (lockCursor)
            InternalLockUpdate();
    }

    public void InternalLockUpdate()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            cursorIsLocked = false;
        }
        else if (Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            cursorIsLocked = true;
        }

        if (cursorIsLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else if (!cursorIsLocked)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    [System.Serializable]
    public class Attribute
    {
        [System.NonSerialized]
        public FPSController parent;
        public Attributes type;
        public ModifiableInt value;

        public void SetParent(FPSController _parent)
        {
            parent = _parent;
            value = new ModifiableInt(AttributeModified);
        }

        public void AttributeModified()
        {
            parent.AttributeModified(this);
        }
    }
}


