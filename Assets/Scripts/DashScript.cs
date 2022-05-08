using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashScript : MonoBehaviour
{
    public bool isDashing;

    private int dashAttempts;
    private float dashStartTime;
    public GameObject camera;

    [SerializeField] ParticleSystem forwardDashSystem;
    [SerializeField] ParticleSystem backwardDashSystem;
    [SerializeField] ParticleSystem rightDashSystem;
    [SerializeField] ParticleSystem leftDashSystem;

    float x;
    float z;

    FPSController playerController;

    // Start is called before the first frame update
    void Start()
    {
        playerController = GetComponent<FPSController>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleDash();
    }

    void HandleDash()
    {
        bool isTryingToDash = Input.GetKeyDown(KeyCode.LeftShift);

        if (playerController.AbillityArray[0])
        {
            if (isTryingToDash && !isDashing)
            {
                if (dashAttempts <= 200)
                {
                    OnStartDash();
                }
            }

            if (isDashing)
            {
                if (Time.time - dashStartTime <= 0.4f)
                {

                    //player is not giving input just dash for
                    x = Input.GetAxis("Horizontal") * 0.5f;
                    z = Input.GetAxis("Vertical") * 0.5f;

                    //new Vector3(x * speed, 0 , z * speed);
                    transform.position += camera.transform.forward * z + camera.transform.right * x;

                }
                else
                {
                    OnEndDash();
                }
            }
        }
        
    }

    void OnStartDash()
    {
        isDashing = true;
        dashStartTime = Time.time;
        dashAttempts += 1;
        PlayDashParticles();
    }

    void OnEndDash()
    {
        isDashing = false;
        dashStartTime = 0;
    }

    void PlayDashParticles()
    {
        var xDir = Input.GetAxis("Horizontal");
        var zDir = Input.GetAxis("Vertical");

        if(zDir > 0 && Mathf.Abs(xDir) <= zDir)
        {
            forwardDashSystem.Play();
            return;
        }

        if (zDir < 0 && Mathf.Abs(xDir) <= Mathf.Abs(zDir))
        {
            forwardDashSystem.Play();
            return;
        }

        if (zDir < 0 && Mathf.Abs(xDir) <= Mathf.Abs(zDir))
        {
            backwardDashSystem.Play();
            return;
        }

        if (xDir > 0)
        {
            rightDashSystem.Play();
            return;
        }

        if (xDir < 0)
        {
            leftDashSystem.Play();
            return;
        }

        forwardDashSystem.Play();

    }
}
