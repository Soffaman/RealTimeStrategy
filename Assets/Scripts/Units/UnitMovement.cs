using Mirror;
using UnityEngine.AI;
using UnityEngine;


public class UnitMovement : NetworkBehaviour
{
    [SerializeField] private NavMeshAgent _agent = null;

    #region Server

    [ServerCallback]
    private void Update()
    {
        if (!_agent.hasPath) { return; }

        if (_agent.remainingDistance > _agent.stoppingDistance) { return; }

        _agent.ResetPath();
    }

    [Command]
    public void CmdMove(Vector3 position)
    {
        if (!NavMesh.SamplePosition(position, out NavMeshHit hit, 1f, NavMesh.AllAreas)) { return; }

        _agent.SetDestination(hit.position);
    }

    #endregion

    

}
