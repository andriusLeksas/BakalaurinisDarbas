using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FPSController : MonoBehaviour
{
    public GameObject bloodPrefab;
    public GameObject uiBloodSplatter;
    public GameObject gameOverPrefab;
    public Transform shotDirection;
    public GameObject stevePrefab;
    public Slider healthbar;
    public Text ammoReserves;
    public AudioSource jumpSound;
    public AudioSource shot;
    public AudioSource landSound;
    public AudioSource[] steps;
    public AudioSource healthPick;
    public AudioSource ammoPick;
    public AudioSource noAmmoSound;
    public AudioSource deathSound;
    public AudioSource reloadSound;
    public GameObject camera;
    public Animator anim;
    readonly float speed = 0.10f;
    readonly float xSensitivity = 2;
    readonly float ySensitivity = 2;
    float minX = -90;
    float maxX = 90;
    Rigidbody rb;
    CapsuleCollider cap;

    public GameObject canvas;
    float cWidth;
    float cHeight;

    Quaternion cameraRotation;
    Quaternion characterRotation;

    bool cursorIsLocked = true;
    bool lockCursor = true;

    float x;
    float z;

    int ammo = 80;
    int maxAmmo = 80;
    int ammoClip = 0;
    int ammoClipMax = 12;
    int health = 0;
    int maxHealth = 100;

    bool playingWalking = false;
    bool previouslyGrounded = true;

    void OnTriggerEnter(Collider col)
    {
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
    }

    public void TakeHit(float amount)
    {
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

            GameObject steve = Instantiate(stevePrefab, pos, this.transform.rotation);
            steve.GetComponent<Animator>().SetTrigger("Death");
            GameStats.gameOver = true;
            Destroy(this.gameObject);           
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        cap = this.GetComponent<CapsuleCollider>();

        cameraRotation = camera.transform.localRotation;
        characterRotation = this.transform.localRotation;

        health = maxHealth;
        ammoClip = ammoClipMax;
        healthbar.value = health;
        ammoReserves.text = ammoClip+ "/" + ammo + "";

        cWidth = canvas.GetComponent<RectTransform>().rect.width;
        cHeight = canvas.GetComponent<RectTransform>().rect.height;

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            anim.SetBool("arm", !anim.GetBool("arm"));
        }

        if (Input.GetMouseButtonDown(0) && !anim.GetBool("fire") && anim.GetBool("arm") && GameStats.canShoot && !anim.GetBool("reload"))
        {
            if(ammoClip > 0)
            {
                anim.SetTrigger("fire");
                ProcessZombieHit();
                ammoClip--;
                ammoReserves.text = ammoClip + "/" + ammo + "";
                GameStats.canShoot = false;
            }
            else
            {
                noAmmoSound.Play();
            }

            
            Debug.Log("Ammo Left in clip:" + ammoClip);
        }

        if (Input.GetKeyDown(KeyCode.R) && anim.GetBool("arm"))
        {           
            anim.SetTrigger("reload");
            reloadSound.Play();
            int amount = ammoClipMax - ammoClip;
            int amountAvailable = amount < ammo ? amount : ammo;
            ammo -= amountAvailable;
            ammoClip += amountAvailable;
            ammoReserves.text = ammoClip + "/" + ammo + "";
            GameStats.canShoot = false;
            Debug.Log("Ammo Left" + ammo);
            Debug.Log("Ammo in clip" + ammoClip);
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

    void ProcessZombieHit()
    {
        RaycastHit hitInfo;
        if(Physics.Raycast(shotDirection.position, shotDirection.forward, out hitInfo, 500))
        {
            GameObject hitZombie = hitInfo.collider.gameObject;
            if(hitZombie.tag == "Zombie")
            {
                GameObject blood = Instantiate(bloodPrefab, hitInfo.point, Quaternion.identity);
                blood.transform.LookAt(this.transform.position);
                Destroy(blood, 0.5f);

                if (Random.Range(0, 10) < 5)
                {
                    GameObject rdPrefab = hitZombie.GetComponent<ZombieController>().ragdoll;
                    GameObject newRd = Instantiate(rdPrefab, hitZombie.transform.position, hitZombie.transform.rotation);
                    newRd.transform.Find("Hips").GetComponent<Rigidbody>().AddForce(shotDirection.forward * 10000);
                    Destroy(hitZombie);
                }
                else
                {
                    hitZombie.GetComponent<ZombieController>().KillZombie();
                }
                    
            }
        }
    }

    void PlaySteps()
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

        cameraRotation *= Quaternion.Euler(-xRotation, 0, 0);
        characterRotation *= Quaternion.Euler(0, yRotation, 0);

        cameraRotation = ClampRotationAroundXAxis(cameraRotation);

        this.transform.localRotation = characterRotation;
        camera.transform.localRotation = cameraRotation; 

        x = Input.GetAxis("Horizontal") * speed;
        z = Input.GetAxis("Vertical") * speed;

        //new Vector3(x * speed, 0 , z * speed);
        transform.position += camera.transform.forward * z + camera.transform.right * x;

        UpdateCursorLock();
    }

    Quaternion ClampRotationAroundXAxis(Quaternion q)
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
    bool IsGrounded()
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
    void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.tag == "Ammo" && ammo < maxAmmo)
        {
            ammo = Mathf.Clamp(ammo + 12, 0, maxAmmo);
            ammoReserves.text = ammoClip + "/" + ammo + "";
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
        else if (col.gameObject.tag == "Lava")
        {
            health = Mathf.Clamp(health - 50, 0, 100);
            healthbar.value = health;
            Debug.Log("Health:" + health);
            if(health <= 0)
            {
                deathSound.Play();
            }
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
}
