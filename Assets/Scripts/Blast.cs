using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blast : MonoBehaviour
{
    public bool scaleUp = true;
    public float flySpeed;
    public float flyRotateSpeed;
    public float waitingRotateSpeed;
    public float scalingSpeed;
    public float maxScale;
    public float minScale;
    public float timeToReachTarget;
    public float impactDestroyDelay;
    public float maxDistance;
    public float fxScaleSpeed;

    [HideInInspector]
    public Transform Player;

    GameObject impactFX;
    GameObject flyingFX;
    GameObject blastFX;

    float t;
    bool hasBlasted = false;
    Vector3 startPosition;
    Vector3 endPosition;
    Vector3 endScale;
    public float cooldown = 3f;

    // Start is called before the first frame update
    void Start()
    {
        impactFX = transform.Find("ImpactFX").gameObject;
        flyingFX = transform.Find("FlyingFX").gameObject;

        transform.localScale = new Vector3(minScale, minScale, minScale);
        endScale = transform.localScale;
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && !hasBlasted)
        {
            CreateBlast();
        }

        if (hasBlasted)
        {
            t += Time.deltaTime / timeToReachTarget;
            transform.position = Vector3.Lerp(startPosition, endPosition, t);

            if (flyingFX)
            {
                flyingFX.transform.RotateAround(flyingFX.transform.position, flyingFX.transform.right, waitingRotateSpeed * Time.deltaTime);
                flyingFX.transform.localScale = Vector3.Lerp(flyingFX.transform.localScale, endScale, fxScaleSpeed * Time.deltaTime);
            }

            var distance = Vector3.Distance(transform.position, endPosition);
            if(distance < 0.1f)
            {
                Impact();
            }         
        }
        else
        {
            flyingFX.transform.RotateAround(flyingFX.transform.position, flyingFX.transform.right, flyRotateSpeed * Time.deltaTime);
            transform.Translate(Vector3.forward * flySpeed * Time.deltaTime);
            endScale = Vector3.Lerp(endScale, new Vector3(maxScale, maxScale, maxScale), scalingSpeed * Time.deltaTime);

            if (scaleUp)
            {
                transform.localScale = endScale;

            }

            var distance2 = Vector3.Distance(startPosition, transform.position);
            if (distance2 > maxDistance)
            {
                CreateBlast();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Zombie" )
        {
            other.GetComponent<ZombieController>().TakeDamage(100);
        }

        if (other.tag == "Boss" )
        {
            other.GetComponent<Boss>().TakeDamage(100);
        }
    }

    private void Impact()
    {
        impactFX.transform.SetParent(null);
        impactFX.transform.localPosition = endScale;
        impactFX.SetActive(true);

        Collider[] objectsInRange = Physics.OverlapSphere(transform.position, endScale.x);

        foreach(Collider col in objectsInRange)
        {
            if(col.tag == "Zombie")
            {
                col.GetComponent<Boss>().TakeDamage(100);
            }
            if (col.tag == "Boss")
            {
                col.GetComponent<Boss>().TakeDamage(100);
            }
        }

        Destroy(impactFX, impactDestroyDelay);
        Destroy(gameObject);
    }

    private void CreateBlast()
    {
        hasBlasted = true;
        flyingFX.transform.SetParent(null);
        Destroy(flyingFX, timeToReachTarget);

        blastFX = Instantiate(flyingFX, transform.position, flyingFX.transform.rotation);
        blastFX.transform.SetParent(transform);
        blastFX.transform.localScale = new Vector3(minScale, minScale, minScale);

        startPosition = Player.position;
        endPosition = blastFX.transform.position;

        transform.position = startPosition;
        transform.localScale = new Vector3(minScale, minScale, minScale);
    }
}
