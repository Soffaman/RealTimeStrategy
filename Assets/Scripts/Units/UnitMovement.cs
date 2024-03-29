using Mirror;
using UnityEngine.AI;
using UnityEngine;
using UnityEngine.UIElements;


public class UnitMovement : NetworkBehaviour
{
    [SerializeField] private NavMeshAgent _agent = null;
    [SerializeField] private Targeter _targeter = null;
    [SerializeField] private float _chaseRange = 10f;

    #region Server

    public override void OnStartServer()
    {
        GameOverHandler.ServerOnGameOver += ServerHandleGameOver;
    }

    public override void OnStopServer()
    {
        GameOverHandler.ServerOnGameOver -= ServerHandleGameOver;
    }

    [ServerCallback]
    private void Update()
    {
        Targetable target = _targeter.GetTarget();

        if (target != null)
        {
            if ((target.transform.position - transform.position).sqrMagnitude > _chaseRange * _chaseRange)
            {
                _agent.SetDestination(target.transform.position);
            }
            else if (_agent.hasPath)
            {
                _agent.ResetPath();
            }

            return;
        }

        if (!_agent.hasPath) { return; }

        if (_agent.remainingDistance > _agent.stoppingDistance) { return; }

        _agent.ResetPath();
    }

    [Command]
    public void CmdMove(Vector3 position)
    {
        ServerMove(position);
    }

    [Server]
    public void ServerMove(Vector3 position)
    {
        _targeter.ClearTarget();

        if (!NavMesh.SamplePosition(position, out NavMeshHit hit, 1f, NavMesh.AllAreas)) { return; }

        _agent.SetDestination(hit.position);
    }

    [Server]
    private void ServerHandleGameOver()
    {
        _agent.ResetPath();
    }

    #endregion



}
