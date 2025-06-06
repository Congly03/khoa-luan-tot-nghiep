using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Zombie1 : MonoBehaviour
{
    [Header("Zombie Health and Damage")]
    private float zombieHealth = 100f;
    private float presentHealth;
    public float giveDamage = 5f;
    public HealthBar healthBar;

    [Header("Zombie Things")]
    public NavMeshAgent zombieAgent;
    public Transform LookPoint;
    public Camera AttackingRaycastArea;
    public Transform playerBody;
    public LayerMask PlayerLayer;

    [Header("Zombie Guarding Var")]
    public GameObject[] walkPoints;
    int currenZombiePosition = 0;
    public float zombieSpeed;
    float walkingpointRadius = 2;

    [Header("Zombie Attacking Var")]
    public float timeBtwAttack;
    bool previouslyAttack;

    [Header("Zombie Animation")]
    public Animator anim;

    [Header("Zombie mood/states")]
    public float visionRadius;
    public float attackingRadius;
    public bool playerInvisionRadius;
    public bool playerInattackingRadius;

    private void Awake()
    {
        presentHealth = zombieHealth;
        healthBar.GiveFullHealth(zombieHealth);
        zombieAgent = GetComponent<NavMeshAgent>();
    }
    private void Update()
    {
        playerInvisionRadius = Physics.CheckSphere(transform.position, visionRadius, PlayerLayer);
        playerInattackingRadius = Physics.CheckSphere(transform.position, attackingRadius, PlayerLayer);

        if(!playerInvisionRadius && !playerInattackingRadius) Guard();
        if(playerInvisionRadius && !playerInattackingRadius) Pursueplayer();
        if(playerInvisionRadius && playerInattackingRadius) AttackPlayer();
    }
    private void Guard()
    {
        if (Vector3.Distance(walkPoints[currenZombiePosition].transform.position, transform.position) < walkingpointRadius)
        {
            currenZombiePosition = Random.Range(0, walkPoints.Length);
            if (currenZombiePosition >= walkPoints.Length)
            {
                currenZombiePosition = 0;
            }
        }
        transform.position = Vector3.MoveTowards(transform.position, walkPoints[currenZombiePosition].transform.position, Time.deltaTime * zombieSpeed);
        // change zombie facing
        transform.LookAt(walkPoints[currenZombiePosition].transform.position);
    }
    private void Pursueplayer()
    {
        if (zombieAgent != null && zombieAgent.isActiveAndEnabled && zombieAgent.isOnNavMesh)
        {
            if(zombieAgent.SetDestination(playerBody.position))
            {
                // Animations
                anim.SetBool("Walking", false);
                anim.SetBool("Running", true);
                anim.SetBool("Attacking", false);
                anim.SetBool("Died", false);
            }
            else
            {
                anim.SetBool("Walking", false);
                anim.SetBool("Running", false);
                anim.SetBool("Attacking", false);
                anim.SetBool("Died", true);
            }
        }
        else
        {
            Debug.LogError("NavMeshAgent chưa sẵn sàng hoặc không nằm trên NavMesh!");
        }
    }
    private void AttackPlayer()
    {
        zombieAgent.SetDestination(transform.position);
        transform.LookAt(LookPoint);
       if(!previouslyAttack)
       {
         RaycastHit hitInfo;
         if (Physics.Raycast(AttackingRaycastArea.transform.position, AttackingRaycastArea.transform.forward, out hitInfo, attackingRadius))
         {
            Debug.Log("Attacking" + hitInfo.transform.name);
            PlayerScript playerBody = hitInfo.transform.GetComponent<PlayerScript>();

            if(playerBody != null)
            {
                playerBody.playerHitDamage(giveDamage);
            }
                anim.SetBool("Walking", false);
                anim.SetBool("Running", false);
                anim.SetBool("Attacking", true);
                anim.SetBool("Died", false);
         }
         previouslyAttack = true;
         Invoke(nameof(ActiveAttacking), timeBtwAttack);
       } 
    }
    private void ActiveAttacking()
    {
        previouslyAttack = false;
    }
    public void zombieHitDamage(float takeDamage)
    { 
        presentHealth -= takeDamage;
        healthBar.SetHealth(presentHealth);
        
        if(presentHealth <= 0)
        {
            anim.SetBool("Walking", false);
            anim.SetBool("Running", false);
            anim.SetBool("Attacking", false);
            anim.SetBool("Died", true);

            zomebieDie();
        }
    }
    private void zomebieDie()
    {
        zombieAgent.SetDestination(transform.position);
        zombieSpeed = 0f;
        attackingRadius = 0f;
        visionRadius = 0f;
        playerInattackingRadius = false;
        playerInvisionRadius = false;
        healthBar.gameObject.SetActive(false);
        Object.Destroy(gameObject, 5.0f);
    }
}
