using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpawnController : MonoBehaviour
{
    public GameObject zombiePrefab;
    public int number;
    public float spawnRadius;
    public bool spawnOnStart = true;
    // Start is called before the first frame update
    void Start()
    {
        if (spawnOnStart)
        {
            spawnAll();
        }
        
    }

    void spawnAll()
    {
        for (int i = 0; i < number; i++)
        {
            Vector3 random = this.transform.position + Random.insideUnitSphere * spawnRadius;

            NavMeshHit hit;

            //Making sure that spawn reaches NavMesh
            if (NavMesh.SamplePosition(random, out hit, 10.0f, NavMesh.AllAreas))
            {
                Instantiate(zombiePrefab, hit.position, Quaternion.identity);
            }
            else
            {
                i--;
            }

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!spawnOnStart && other.gameObject.tag == "Player")
        {
            spawnAll();
            Destroy(this.gameObject);
        }
        
    }

}
