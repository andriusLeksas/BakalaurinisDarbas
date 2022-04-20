using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangoBoss : MonoBehaviour
{

    public Animator ani;
    public Boss boss;
    public int melee;

    private void OnTriggerEnter(Collider coll)
    {
        if (coll.CompareTag("Player"))
        {
            melee = Random.Range(0, 4);

            switch (melee)
            {
                //golp1
                case 0:
                    ani.SetFloat("skills", 0);
                    boss.hit_select = 0;
                    break;
                //golp2
                case 1:
                    ani.SetFloat("skills", 0.2f);
                    boss.hit_select = 1;
                    break;
                //jump
                case 2:
                    ani.SetFloat("skills", 0.4f);
                    boss.hit_select = 2;
                    break;
                //fireBall
                case 3:
                    if(boss.fase == 2)
                    {
                        ani.SetFloat("skills", 1);
                    }
                    else
                    {
                        melee = 0;
                    }
                    break;
            }

            ani.SetBool("walk", false);
            ani.SetBool("run", false);
            ani.SetBool("attack", true);
            boss.atacando = true;
            GetComponent<CapsuleCollider>().enabled = false;
        }
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

}
