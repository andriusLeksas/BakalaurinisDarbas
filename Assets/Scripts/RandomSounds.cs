using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSounds : MonoBehaviour
{
    public AudioSource sound;
    public bool sound3D = true;
    public float randomMin;
    public float randomMax;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("PlaySound", Random.Range(randomMin, randomMax));
    }

    // Update is called once per frame
    void PlaySound()
    {
        GameObject newSound = new GameObject();
        AudioSource newAs = newSound.AddComponent<AudioSource>();
        newAs.clip = sound.clip;
        
        if (sound3D)
        {
            newAs.spatialBlend = 1;
            newAs.maxDistance = sound.maxDistance;
            newSound.transform.SetParent(this.transform);
            newSound.transform.localPosition = Vector3.zero;
        }
      
        newAs.Play();
        
        Invoke("PlaySound", Random.Range(randomMin, randomMax));
        Destroy(newSound, sound.clip.length);
    }
     
}
