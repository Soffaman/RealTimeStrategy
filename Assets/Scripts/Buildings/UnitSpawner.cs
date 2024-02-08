using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
{
    [SerializeField] private Health _health = null;
    [SerializeField] private GameObject _unitPrefab = null;
    [SerializeField] private Transform _unitSpawnPoint = null;

    #region Server

    public override void OnStartServer()
    {
        _health.ServerOnDie += ServerHandleDie;

    }

    public override void OnStopServer()
    {
        _health.ServerOnDie -= ServerHandleDie;
    }

    [Server]
    private void ServerHandleDie()
    {
        NetworkServer.Destroy(gameObject);
    }

    [Command]
    private void CmdSpawnUnit()
    {
        GameObject unitInstance = Instantiate(
            _unitPrefab,
            _unitSpawnPoint.position,
            _unitSpawnPoint.rotation);

        NetworkServer.Spawn(unitInstance, connectionToClient);
    }

    #endregion

    #region Client

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button != PointerEventData.InputButton.Left) { return; }

        if (!isOwned) { return; }

        CmdSpawnUnit();
    }
    #endregion
}
