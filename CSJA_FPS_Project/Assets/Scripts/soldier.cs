using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class soldier : MonoBehaviour
{

    private Animator anim;
    private NavMeshAgent navMesh;

    public ParticleSystem fireEffect;

    private GameObject player;
    private PlayerHealth playerHealth;

    public float atkDistance = 10f; // distance for atk
    public float followDistance = 20f; // distance for follow
    public float atkProbality;

    public int damage = 20; //total de dano que causa
    public int health = 100; // total de vida que possui

    public Transform shootPoint;
    public float range = 100f;

    public float fireRate = 0.5f;
    private float fireTimer;

    private AudioSource audioSource;
    public AudioClip shootAudio;

    private bool isDead;


    void Start()
    {
        navMesh = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        player = GameObject.FindGameObjectWithTag("Player");
        playerHealth = player.GetComponent<PlayerHealth>();
    }


    void Update()
    {
        if (navMesh.enabled && !playerHealth.isDead)
        {
            float dist = Vector3.Distance(player.transform.position, transform.position);
            bool shoot = false;
            bool follow = (dist < followDistance);

            if (follow)
            {
                if (dist < atkDistance)
                {
                    shoot = true;
                    Fire();
                }

                navMesh.SetDestination(player.transform.position);                
                shootPoint.LookAt(player.transform);
                transform.LookAt(new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z));
                //navMesh.isStopped = false;
            }

            if (!follow || shoot)
            {
                navMesh.SetDestination(transform.position);
                //navMesh.isStopped = true;
            }

            anim.SetBool("shoot", shoot);
            anim.SetBool("run", follow);
        }

        if (fireTimer < fireRate)
        {
            fireTimer += Time.deltaTime;
        }
    }

    public void Fire()
    {
        if (fireTimer < fireRate)
            return;

        RaycastHit hit;

        if (Physics.Raycast(shootPoint.position, shootPoint.forward, out hit, range))
        {
            if (hit.transform.GetComponent<PlayerHealth>())
            {
                hit.transform.GetComponent<PlayerHealth>().ApplyDamage(damage);
            }
        }

        fireEffect.Play();
        PlayShootAudio();

        fireTimer = 0;
    }

    public void ApplyDamage(int damage)
    {
        health -= damage;

        if (health <= 0 && !isDead)
        {
            navMesh.enabled = false;
            anim.SetTrigger("die");
            isDead = true;
        }
    }

    public void PlayShootAudio()
    {
        audioSource.PlayOneShot(shootAudio);
    }
}
