using System.Collections;
using UnityEngine;
using UnityEngine.AI;


public enum FishState
{
    Patrol,         //random position
    Investigate,    //move towards lure
    Flee,           //away from lure
    Bite            //interact with lure
}

public class FishLogic : MonoBehaviour
{
    public FishTypeData fishType;
    public NavMeshAgent agent;
    public Transform lure;
    public Lure lureLogic;
    public LayerMask lureLayer;
    public float patrolRadius = 10f;
    public float lureDetectionRadius = 5f; 
    public float immediateFleeRadius = 3f; 
    public float investigationChance = 0.5f;
    public float jigMultiplier = 1.5f;
    public float reelMovePenalty = 0.5f;
    public float minWaitTime = 1f; 
    public float maxWaitTime = 3f; 
    public bool hasDetectedLure;

    public FishState currentState;
    private Vector3 patrolTarget;

   

    void Start()
    {
        lure = FindFirstObjectByType<Lure>().transform;
        lureLogic = lure.gameObject.GetComponent<Lure>();
        ChangeState(FishState.Patrol); // Start in the Patrol state
    }

    void Update()
    {
        switch (currentState)
        {
            case FishState.Investigate:
                InvestigateLure();
                break;
            case FishState.Flee:
                Flee();
                break;
            
        }
    }

    private void ChangeState(FishState newState)
    {
        // Change  state
        currentState = newState;
        //Debug.Log(currentState);

        
        if (agent != null)
            agent.ResetPath();

        // Start coroutine
        switch (newState)
        {
            case FishState.Patrol:
                StartCoroutine(PatrolRoutine());
                break;
            case FishState.Flee:
                SetFleeTarget(lure.position);
                break;
            case FishState.Investigate:
                agent.SetDestination(lure.position);
                break;
            case FishState.Bite:
                BiteLure();
                break;
        }
    }

    private IEnumerator PatrolRoutine()
    {
        while (currentState == FishState.Patrol)
        {
            SetNewPatrolTarget();
            agent.SetDestination(patrolTarget);

            // Move to the patrol target 
            while (!agent.pathPending && agent.remainingDistance > agent.stoppingDistance)
            {
                //will always run away from lure first time it is seen
                if (!hasDetectedLure && Vector3.Distance(transform.position, lure.position) <= immediateFleeRadius)
                {
                    Debug.Log("lure detected!");
                    hasDetectedLure = true;
                    ChangeState(FishState.Flee);
                    yield break; //flee!
                }
                yield return null;
            }

            // wait random amount of time 
            float waitTime = Random.Range(minWaitTime, maxWaitTime);
            yield return new WaitForSeconds(waitTime);

            // lure check
            CheckForLure();
        }
    }

    private void CheckForLure()
    {
        Collider[] lures = Physics.OverlapSphere(transform.position, lureDetectionRadius, lureLayer);
        if (lures.Length > 0)
        {
            float chance = investigationChance;
            if (Input.GetKey(KeyCode.S)) chance *= jigMultiplier;
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) chance *= reelMovePenalty;

            if (Random.value < chance)
            {
                ChangeState(FishState.Investigate);
            }
            else
            {
                ChangeState(FishState.Flee);
            }
        }
    }

    private void InvestigateLure()
    {
        if (Vector3.Distance(transform.position, lure.position) <= agent.stoppingDistance)
        {
            ChangeState(FishState.Bite);
        }
        else
        {
            agent.SetDestination(lure.position);
        }
    }

    private void Flee()
    {
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            ChangeState(FishState.Patrol); 
        }
    }

    private void BiteLure()
    {
        agent.enabled = false;
        transform.SetParent(lure.transform);
        Debug.Log("biting");
        // bite logic
        StartCoroutine(lureLogic.FishBite(gameObject));

    }

    private void SetNewPatrolTarget()
    {
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += transform.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, 1))
        {
            patrolTarget = hit.position;
        }
    }

    private void SetFleeTarget(Vector3 lurePosition)
    {
        Vector3 fleeDirection = (transform.position - lurePosition).normalized * patrolRadius;
        Vector3 fleeTarget = transform.position + fleeDirection;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(fleeTarget, out hit, patrolRadius, 1))
        {
            agent.SetDestination(hit.position);
        }
    }
    public void OnLureCast()
    {
        hasDetectedLure = false;
    }
}