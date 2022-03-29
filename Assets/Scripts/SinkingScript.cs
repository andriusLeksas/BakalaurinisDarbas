using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinkingScript : MonoBehaviour
{
    float destroyHeight;
    public float delay = 10;
    // Start is called before the first frame update
    void Start()
    {
        if(this.gameObject.tag == "Ragdoll")
        {
            Invoke("StartSink", 5);
        }
        
    }

    public void StartSink()
    {
        destroyHeight = Terrain.activeTerrain.SampleHeight(this.transform.position) - 5;
        Collider[] collList = this.transform.GetComponentsInChildren<Collider>();
        foreach(Collider c in collList)
        {
            Destroy(c);
        }

        InvokeRepeating("SinkIntoGround", delay, 0.2f);
    }

    void SinkIntoGround()
    {
        this.transform.Translate(0, -0.001f, 0);
        if(this.transform.position.y < destroyHeight)
        {
            Destroy(this.gameObject);
        }
    }
}
