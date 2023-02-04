using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPathfinding : MonoBehaviour {
    [SerializeField] private NavMeshAgent navMeshAgent;
    
    private bool pathfind = true;
    
    public NavMeshAgent NavMeshAgent { get => navMeshAgent; }
    public bool Pathfind { get => pathfind; set => pathfind = value; }
    
    private Vector3 GetClosestPlayer() {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Length <= 0) return transform.position;
        return players.OrderBy((d) => (d.transform.position - transform.position).sqrMagnitude).First().transform.position;
    }

    private void Update() {
        if (navMeshAgent.isOnNavMesh) {
            navMeshAgent.isStopped = !pathfind;
            navMeshAgent.SetDestination(GetClosestPlayer());
        }
    }
}
