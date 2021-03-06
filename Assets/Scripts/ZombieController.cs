using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieController : MonoBehaviour
{
    public GameObject[] itemArray;
    public float health = 30f;
    public GameObject target;
    public float walkingSpeed;
    public float runningSpeed;
    public Animator anim;
    public NavMeshAgent agent;
    public GameObject ragdoll;
    public float damageAmount = 10;
    public enum STATE { IDLE, WANDER, ATTACK, CHASE, DEAD };
    public STATE state = STATE.IDLE;
    public AudioSource[] attacks;

    // Start is called before the first frame update
    public void Start()
    {
        anim = this.GetComponent<Animator>();
        agent = this.GetComponent<NavMeshAgent>();
    }

    public void TurnOffTriggers()
    {
        anim.SetBool("isWalking", false);
        anim.SetBool("isAttacking", false);
        anim.SetBool("isRunning", false);
        anim.SetBool("isDead", false);
    }

    public void PlayAttackAudio()
    {
        AudioSource audioSource = new AudioSource();
        int random = Random.Range(1, attacks.Length);
        audioSource = attacks[random];
        audioSource.Play();
        attacks[random] = attacks[0];
        attacks[0] = audioSource;
    }
    public void DamagePlayer()
    {
        if(target != null)
        {
            target.GetComponent<FPSController>().TakeHit(damageAmount);
            PlayAttackAudio();
        }      
    }

    public float DistanceToPlayer()
    {
        if (GameStats.gameOver) 
        {
            return Mathf.Infinity;
        }
            
        return Vector3.Distance(target.transform.position, this.transform.position);
    }

    public bool CanSeePlayer()
    {
        if (DistanceToPlayer() < 25)
        {
            return true;
        }
            
        return false;
    }

    public bool ForgetPlayer()
    {
        if (DistanceToPlayer() > 20)
        {
            return true;
        }
            
        return false;
    }

     public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0f)
        {
            KillZombie();
        }
    }

    public void KillZombie()
    {
       
        TurnOffTriggers();
        anim.SetBool("isDead", true);
        state = STATE.DEAD;

        Vector3 pos = new Vector3(this.transform.position.x,
                          Terrain.activeTerrain.SampleHeight(this.transform.position) + 1,
                          this.transform.position.z);

        //if(Random.Range(0,10) < 5)
        //{

        //}  
        GameObject item = Instantiate(itemArray[Random.Range(0, 2)], pos, this.transform.rotation);
        item.GetComponentInChildren<Billboard>()._camera = FindObjectOfType<Camera>();
    }
    
    public void Update()
    {
       
        if(target == null && GameStats.gameOver == false)
        {
            target = GameObject.FindWithTag("Player");
                return;
        }

        switch (state)
        {

            case STATE.IDLE:
                if (CanSeePlayer()) state = STATE.CHASE;
                else if (Random.Range(0, 3000) < 5)
                    state = STATE.WANDER;
                break;

            case STATE.WANDER:

                if (!agent.hasPath)
                {
                    float newX = this.transform.position.x + Random.Range(-5, 5);
                    float newZ = this.transform.position.z + Random.Range(-5, 5);
                    float newY = Terrain.activeTerrain.SampleHeight(new Vector3(newX, 0, newZ));
                    Vector3 dest = new Vector3(newX, newY, newZ);
                    agent.SetDestination(dest);
                    agent.stoppingDistance = 0;
                    TurnOffTriggers();
                    agent.speed = walkingSpeed;
                    anim.SetBool("isWalking", true);
                }

                if (CanSeePlayer()) state = STATE.CHASE;

                else if (Random.Range(0, 3000) < 5)
                {
                    state = STATE.IDLE;
                    TurnOffTriggers();
                    agent.ResetPath();
                }
                break;

            case STATE.CHASE:

                if (GameStats.gameOver)
                {
                    TurnOffTriggers();
                    state = STATE.WANDER;
                    return;
                }

                agent.SetDestination(target.transform.position);
                agent.stoppingDistance = 3;
                TurnOffTriggers();
                agent.speed = runningSpeed;
                anim.SetBool("isRunning", true);

                if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
                {
                    state = STATE.ATTACK;
                }

                if (ForgetPlayer())
                {
                    state = STATE.WANDER;
                    agent.ResetPath();
                }

                break;

            case STATE.ATTACK:

                if (GameStats.gameOver)
                {
                    TurnOffTriggers();
                    state = STATE.WANDER;
                    return;
                }

                TurnOffTriggers();
                anim.SetBool("isAttacking", true);
                this.transform.LookAt(target.transform.position);
                if (DistanceToPlayer() > agent.stoppingDistance + 2)
                {
                    state = STATE.CHASE;
                }
                    
                break;

            case STATE.DEAD:

                Destroy(agent);
                AudioSource[] sounds = this.GetComponents<AudioSource>();
                foreach(AudioSource s in sounds)
                {
                    s.volume = 0;
                }

                this.GetComponent<SinkingScript>().StartSink();
                break;

        }
    }
}
