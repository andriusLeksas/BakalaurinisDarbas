using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boss : MonoBehaviour
{
    public int routine;
    public float cronometro;
    public float timeRoutines;
    public Animator ani;
    public Quaternion angle;
    public float degree;
    public GameObject target;
    public bool atacking;
    public BossSkills range;
    public float speed;
    public GameObject[] hit;
    public int hit_select;

    public Image healthbar;

    public ParticleSystem spitFire;


    public bool flamethrower;
    public List<GameObject> pool = new List<GameObject>();
    public GameObject fire;
    public GameObject head;
    private float cronometro2;


    public float jump_distance;
    public bool direction_skill;


    public GameObject fire_ball;
    public GameObject point;
    public List<GameObject> pool2 = new List<GameObject>();

    public int faze = 1;
    public float Hp_Min;
    public float Hp_Max;
    public Image image;
    public AudioSource music;
    public bool death;
    public float visionDistance = 30;
    
    public void Start()
    {
        ani = GetComponent<Animator>();
        target = GameObject.Find("Character");
    }

    public void BossBehaviour()
    {
        //Debug.Log("BossBehaviour");
        if (Vector3.Distance(transform.position, target.transform.position) < visionDistance)
        {
            var lookPos = target.transform.position - transform.position;
            lookPos.y = 0;
            var rotation = Quaternion.LookRotation(lookPos);
            point.transform.LookAt(target.transform.position);
            //music.enabled = true;

            if(Vector3.Distance(transform.position, target.transform.position) > 2 && !atacking)
            {
                //Debug.Log("Accesing Switch Rutina" + routine);
                switch (routine)
                {                  
                    
                    case 0:
                        
                        //Debug.Log(" trying to Walk case 0");
                        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, 2);
                        ani.SetBool("walk", true);
                        ani.SetBool("run", false);

                        if(transform.rotation == rotation)
                        {
                            transform.Translate(Vector3.forward * speed * Time.deltaTime);
                        }
                        else
                        {
                            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, 2);
                            if (transform.rotation == rotation)
                            {
                                transform.Translate(Vector3.forward * speed * Time.deltaTime);
                            }

                            return;

                        }

                        ani.SetBool("attack", false);

                        cronometro += 1 * Time.deltaTime;

                        if(cronometro > timeRoutines)
                        {
                            routine = Random.Range(0, 5);
                            cronometro = 0;
                            Debug.Log("Walking cronometro > timeRoutines");
                        }

                        break;

                    case 1:
                        
                        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, 2);
                        ani.SetBool("walk", false);
                        ani.SetBool("run", true);

                        if (transform.rotation == rotation)
                        {
                            transform.Translate(Vector3.forward * speed * 2 * Time.deltaTime);
                        }

                        ani.SetBool("attack", false);

                        break;

                    case 2:
                        if(faze == 3)
                        {
                                              
                            ani.SetBool("walk", false);
                            ani.SetBool("run", false);
                            ani.SetBool("attack", true);
                            ani.SetFloat("skills", 0.8f);
                            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, 2);
                            range.GetComponent<CapsuleCollider>().enabled = false;

                        }
                        else
                        {
                            routine = 0;
                            cronometro = 0;
                        }

                        break;

                    case 3:
                        
                        if(faze == 3)
                        {
                            jump_distance += 1 * Time.deltaTime;
                            ani.SetBool("walk", false);
                            ani.SetBool("run", false);
                            ani.SetBool("attack", true);
                            ani.SetFloat("skills", 0.6f);
                            hit_select = 3;
                            range.GetComponent<CapsuleCollider>().enabled = false;

                            if (direction_skill)
                            {
                                if(jump_distance < 1f)
                                {
                                    transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, 2);
                                }

                                transform.Translate(Vector3.forward * 12 * Time.deltaTime);
                            }                          

                        }
                        else
                        {
                            routine = 0;
                            cronometro = 0;
                        }

                        break;

                    case 4:
                        
                        if (faze == 2)
                        {
                            ani.SetBool("walk", false);
                            ani.SetBool("run", false);
                            ani.SetBool("attack", true);
                            ani.SetFloat("skills", 1);
                            range.GetComponent<CapsuleCollider>().enabled = false;
                            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, 0.5f);
                        }
                        else
                        {
                            routine = 0;
                            cronometro = 0;
                        }

                        break;

                    default:
                        break;
                }
            }
          
        }
              
    }

    public void TakeDamage(float amount)
    {
        Hp_Min -= amount;

        if (Hp_Min <= 0f)
        {
            death = true;
            ani.SetTrigger("dead");
            Destroy(this.gameObject, 10f);
        }
    }

    public void Final_Ani()
    {
        //Debug.Log("Final Ani ");
        routine = 0;
        ani.SetBool("attack", false);
        atacking = false;
        range.GetComponent<CapsuleCollider>().enabled = true;
        flamethrower = false;
        jump_distance = 0;
        direction_skill = false;
    }

    public void Direction_Attack_Start()
    {

        direction_skill = true;

    }

    public void Direction_Attack_Final()
    {

        direction_skill = false;

    }

    public void ColliderWeaponTrue()
    {

        hit[hit_select].GetComponent<SphereCollider>().enabled = true;

    }

    public void ColliderWeaponFalse()
    {
        hit[hit_select].GetComponent<SphereCollider>().enabled = false;
    }

    public GameObject GetObjects()
    {
        for (int i = 0; i < pool.Count; i++)
        {
            if (!pool[i].activeInHierarchy)
            {
                pool[i].SetActive(true);
                return pool[i];
            }
        }

        GameObject obj = Instantiate(fire, head.transform.position, head.transform.rotation) as GameObject;
        pool.Add(obj);
        return obj;
    }

    public void FlamethrowerSkill()
    {
        cronometro2 += 1 * Time.deltaTime;
        if(cronometro2 > 0.1f)
        {
            GameObject obj = GetObjects();
            obj.transform.position = head.transform.position;
            obj.transform.rotation = head.transform.rotation;
            cronometro2 = 0;
        }
    }

    public void Start_fire()
    {

        spitFire.Play();
        flamethrower = true;   
        
    }
    public void Stop_Fire()
    {

        spitFire.Stop();
        flamethrower = false;
        
    }

    public GameObject Get_Fire_Ball()
    {
        for (int i = 0; i < pool2.Count; i++)
        {
            if (!pool2[i].activeInHierarchy)
            {
                pool2[i].SetActive(true);
                return pool2[i];
            }
        }

        GameObject obj = Instantiate(fire_ball, point.transform.position, point.transform.rotation) as GameObject;
        pool2.Add(obj);
        return obj;
    }

    public void Fire_Ball_Skill()
    {

        GameObject obj = Get_Fire_Ball();
        obj.transform.position = head.transform.position;
        obj.transform.rotation = head.transform.rotation;

    }

    public void Alive()
    {

        if (Hp_Min < 500)
        {
            faze = 2;
            timeRoutines = 1;
        }

        BossBehaviour();

        if (flamethrower)
        {
            FlamethrowerSkill();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null)
        {
            target = GameObject.Find("Character");
        }
        else
        {
            healthbar.fillAmount = Hp_Min / Hp_Max;

            if (Hp_Min > 0)
            {

                Alive();

            }
            else
            {
                if (!death)
                {

                    ani.SetTrigger("Dead");
                    music.enabled = false;

                }
            }
        }
      
    }

}
