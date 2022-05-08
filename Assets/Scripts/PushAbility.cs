using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushAbility : MonoBehaviour
{

    public float value;
    public float radius;
    public float duration;
    public float maximumValue;
    public float tickInterval;
    public float damagePerTick;
    public float valueIncreasePerTick;
    public float endFXDestroyTime;

    [HideInInspector]
    public Transform transformToFollow;
    Transform areaVisual;
    Transform areaOfDamage;
    GameObject endFX;

    float tick;
    float t;
    float savedDuration;
    Vector3 endScale;
    // Start is called before the first frame update
    void Start()
    {
        endScale = new Vector3(radius * 3, 0.05f, radius * 3);
        savedDuration = duration;

        areaVisual = transform.Find("AreaVisual");
        areaOfDamage = transform.Find("AreaDamage");
        endFX = transform.Find("EndFX").gameObject;

        endFX.transform.localScale = endScale;
        areaOfDamage.localScale = endScale;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = transformToFollow.position;

        if(duration >= 0)
        {
            duration -= Time.deltaTime;
            t += Time.deltaTime / savedDuration;
            areaVisual.localScale = Vector3.Lerp(Vector3.zero, endScale, t);

            if(tick >= 0)
            {
                tick -= Time.deltaTime;
            }
            else
            {
                Tick();
            }
        }
        else
        {
            Explode();
        }

    }

    void Tick()
    {
        tick = tickInterval;
        float addition = 0;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
        foreach(Collider col in hitColliders)
        {
            if(col.tag == "Zombie" )
            {
                addition += valueIncreasePerTick;
                //Damage enemies
                col.GetComponent<ZombieController>().TakeDamage(damagePerTick);
                
            }
            else if(col.tag == "Boss")
            {
                addition += valueIncreasePerTick;
                //Damage enemies
                col.GetComponent<Boss>().TakeDamage(damagePerTick);
            }
        }

        value += addition;
    }

    void Explode()
    {

        float clampedValue = Mathf.Clamp(value, 0, maximumValue);

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider col in hitColliders)
        {
            if (col.tag == "Zombie")
            {

                Rigidbody rb = col.GetComponent<Rigidbody>();
                Vector3 pushDirection = col.transform.position - transform.position;
                rb.AddForce(pushDirection * clampedValue, ForceMode.Impulse);

            }
            else if (col.tag == "Boss")
            {
                Rigidbody rb = col.GetComponent<Rigidbody>();
                Vector3 pushDirection = col.transform.position - transform.position;
                rb.AddForce(pushDirection * clampedValue, ForceMode.Impulse);
            }
        }

        Destroy(gameObject);
    }

}
