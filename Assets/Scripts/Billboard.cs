using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Camera _camera;

    private void Start()
    {
        //GameObject item = Instantiate(itemArray[Random.Range(0, 3)], pos, this.transform.rotation);
        GetComponentInChildren<Billboard>()._camera = FindObjectOfType<Camera>();
    }
    private void LateUpdate()
    {
        transform.forward = _camera.transform.forward;
    }
}
