using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public float damageAmount = 10f;
    public float attackRange = 2f;
    private Transform player;
    private Character playerCharacter;
    private NavMeshAgent agent;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerCharacter = player.GetComponent<Character>();
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (playerCharacter.IsAlive())
        {
            agent.SetDestination(player.position);

            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if (distanceToPlayer <= attackRange)
            {
                AttackPlayer();
            }
        }
    }

    private void AttackPlayer()
    {
        playerCharacter.TakeDamage(damageAmount);
    }
}
