using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boss : MonoBehaviour
{
    public int rutina;
    public float cronometro;
    public float time_rutinas;
    public Animator ani;
    public Quaternion angulo;
    public float grado;
    public GameObject target;
    public bool atacando;
    public RangoBoss rango;
    public float speed;
    public GameObject[] hit;
    public int hit_select;

    public Image healthbar;

    public ParticleSystem spitFire;


    // Lanzallamas
    public bool lanza_llamas;
    public List<GameObject> pool = new List<GameObject>();
    public GameObject fire;
    public GameObject cabeza;
    private float cronometro2;


    // Jump Attack

    public float jump_distance;
    public bool direction_skill;


    // Fireball 

    public GameObject fire_ball;
    public GameObject point;
    public List<GameObject> pool2 = new List<GameObject>();

    public int fase = 1;
    public float Hp_Min;
    public float Hp_Max;
    public Image barra;
    public AudioSource musica;
    public bool muerto;


    // Start is called before the first frame update
    void Start()
    {
        ani = GetComponent<Animator>();
        target = GameObject.Find("Character");
    }

    public void Comportamiento_Boss()
    {
        Debug.Log("Comportamiento_Boss");
        if (Vector3.Distance(transform.position, target.transform.position) < 30)
        {
            var lookPos = target.transform.position - transform.position;
            lookPos.y = 0;
            var rotation = Quaternion.LookRotation(lookPos);
            point.transform.LookAt(target.transform.position);
            //musica.enabled = true;

            if(Vector3.Distance(transform.position, target.transform.position) > 2 && !atacando)
            {
                Debug.Log("Accesing Switch Rutina" + rutina);
                switch (rutina)
                {                  
                    
                    case 0:
                        
                        Debug.Log(" trying to Walk case 0");
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

                        if(cronometro > time_rutinas)
                        {
                            rutina = Random.Range(0, 5);
                            cronometro = 0;
                            Debug.Log("Walking cronometro > time_rutinas");
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
                        if(fase == 3)
                        {
                                              
                            ani.SetBool("walk", false);
                            ani.SetBool("run", false);
                            ani.SetBool("attack", true);
                            ani.SetFloat("skills", 0.8f);
                            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, 2);
                            rango.GetComponent<CapsuleCollider>().enabled = false;

                        }
                        else
                        {
                            rutina = 0;
                            cronometro = 0;
                        }

                        break;

                    case 3:
                        
                        if(fase == 3)
                        {
                            jump_distance += 1 * Time.deltaTime;
                            ani.SetBool("walk", false);
                            ani.SetBool("run", false);
                            ani.SetBool("attack", true);
                            ani.SetFloat("skills", 0.6f);
                            hit_select = 3;
                            rango.GetComponent<CapsuleCollider>().enabled = false;

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
                            rutina = 0;
                            cronometro = 0;
                        }

                        break;

                    case 4:
                        
                        if (fase == 2)
                        {
                            ani.SetBool("walk", false);
                            ani.SetBool("run", false);
                            ani.SetBool("attack", true);
                            ani.SetFloat("skills", 1);
                            rango.GetComponent<CapsuleCollider>().enabled = false;
                            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, 0.5f);
                        }
                        else
                        {
                            rutina = 0;
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
            muerto = true;
            ani.SetTrigger("dead");
            Destroy(this.gameObject, 10f);
        }
    }

    public void Final_Ani()
    {
        Debug.Log("Final Ani ");
        rutina = 0;
        ani.SetBool("attack", false);
        atacando = false;
        rango.GetComponent<CapsuleCollider>().enabled = true;
        lanza_llamas = false;
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

    public GameObject GetBala()
    {
        for (int i = 0; i < pool.Count; i++)
        {
            if (!pool[i].activeInHierarchy)
            {
                pool[i].SetActive(true);
                return pool[i];
            }
        }

        GameObject obj = Instantiate(fire, cabeza.transform.position, cabeza.transform.rotation) as GameObject;
        pool.Add(obj);
        return obj;
    }

    public void LanzaLlamas_skill()
    {
        cronometro2 += 1 * Time.deltaTime;
        if(cronometro2 > 0.1f)
        {
            GameObject obj = GetBala();
            obj.transform.position = cabeza.transform.position;
            obj.transform.rotation = cabeza.transform.rotation;
            cronometro2 = 0;
        }
    }

    public void Start_fire()
    {
        spitFire.Play();
        lanza_llamas = true;       
    }
    public void Stop_Fire()
    {
        spitFire.Stop();
        lanza_llamas = false;
        
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
        obj.transform.position = cabeza.transform.position;
        obj.transform.rotation = cabeza.transform.rotation;
    }

    public void Vivo()
    {

        Debug.Log("Call Vivo" + rutina);
        if (Hp_Min < 500)
        {
            fase = 2;
            time_rutinas = 1;
        }

        Comportamiento_Boss();

        if (lanza_llamas)
        {
            LanzaLlamas_skill();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null)
        {
            target = GameObject.Find("Character");
        }

        healthbar.fillAmount = Hp_Min / Hp_Max;
                         
        if (Hp_Min > 0)
        {
            Vivo();
        }
        else
        {
            if (!muerto)
            {
                ani.SetTrigger("Dead");
                musica.enabled = false;
            }
        }
    }

}
