using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gun : MonoBehaviour
{
    public Animator anim;
    public GameObject bloodPrefab;
    public LayerMask checkPointLayer;

    public float damage = 10f;
    public float range = 100f;
    public float fireRate = 15f;

    private float nextTimeToFire = 0f;

    public Camera fpsCam;
    public ParticleSystem flash;

    public AudioSource reloadSound;
    public AudioSource noAmmoSound;
    public AudioSource shot;

    public Text ammoReserves;

    public int maxAmmo = 10;
    public int currentAmmo;
    public float reloadTime = 2f;
    public int Ammo = 200;
    public int maxAmmoReserve;

    public bool isRealoading = false;
    FPSController player;

    public void Start()
    {
        currentAmmo = maxAmmo;
        maxAmmoReserve = Ammo;
        ammoReserves.text = currentAmmo + "/" + Ammo + "";
        player = FindObjectOfType<FPSController>();
    }

    public void OnEnable()
    {
        isRealoading = false;
        ammoReserves.text = currentAmmo + "/" + Ammo + "";
    }

    public bool fire1Input;
    // Update is called once per frame
    public void Update()
    {
        if (isRealoading)
        {
            return;
        }

        if(currentAmmo <= 0)
        {
            StartCoroutine(Reload());

            return;
        }

        fire1Input = Input.GetButton("Fire1");
        if (fire1Input && Time.time >= nextTimeToFire && !player.InventoryActive)
        {

            nextTimeToFire = Time.time + 1f / fireRate;
            if(currentAmmo > 0)
            {
                Shoot();
            }
            else
            {
                noAmmoSound.Play();
            }
 
        }

        if (Input.GetKeyDown(KeyCode.R) && anim.GetBool("arm"))
        {
            StartCoroutine(Reload());
        }
    }

    public IEnumerator Reload()
    {
        isRealoading = true;

        anim.SetTrigger("reload");
        reloadSound.Play();


        yield return new WaitForSeconds(reloadTime -.25f);

        int amount = maxAmmo - currentAmmo;
        int amountAvailable = amount < Ammo ? amount : Ammo;
        Ammo -= amountAvailable;
        currentAmmo += amountAvailable;
        ammoReserves.text = currentAmmo + "/" + Ammo + "";

        currentAmmo = maxAmmo;

        isRealoading = false;
    }


    public void Shoot()
    {
        anim.SetTrigger("fire");

        flash.Play();

        shot.Play();

        currentAmmo--;
        ammoReserves.text = currentAmmo + "/" + Ammo + "";


        RaycastHit hitInfo;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hitInfo, range, ~checkPointLayer))
        {
            GameObject hitZombie = hitInfo.collider.gameObject;
            if (hitZombie.tag == "Zombie")
            {
                GameObject blood = Instantiate(bloodPrefab, hitInfo.point, Quaternion.identity);
                blood.transform.LookAt(this.transform.position);
                Destroy(blood, 0.5f);

                if (Random.Range(0, 10) < 5)
                {
                    if (hitZombie.GetComponent<ZombieController>().health > 0)
                    {
                        hitZombie.GetComponent<ZombieController>().TakeDamage(damage);
                    }
                    else
                    {
                        GameObject rdPrefab = hitZombie.GetComponent<ZombieController>().ragdoll;
                        GameObject newRd = Instantiate(rdPrefab, hitZombie.transform.position, hitZombie.transform.rotation);
                        newRd.transform.Find("Hips").GetComponent<Rigidbody>().AddForce(fpsCam.transform.forward * 10000);
                        Destroy(hitZombie);
                    }

                }
                else
                {
                    hitZombie.GetComponent<ZombieController>().TakeDamage(damage);
                }

            }

            if (hitZombie.tag == "Boss")
            {
                hitZombie.GetComponent<Boss>().TakeDamage(damage + 3);
                //Debug.Log("hit boss");
            }

        }

    }
}
