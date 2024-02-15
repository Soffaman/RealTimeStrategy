using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

public class Unit : NetworkBehaviour
{
    [SerializeField] private int _resourceCost = 10;
    [SerializeField] private Health _health = null;
    [SerializeField] private UnitMovement _unitMovement = null;
    [SerializeField] private Targeter _targeter = null;
    [SerializeField] private UnityEvent _onSelected = null;
    [SerializeField] private UnityEvent _onDeselected = null;

    public static event Action<Unit> ServerOnUnitSpawned;
    public static event Action<Unit> ServerOnUnitDespawned;

    public static event Action<Unit> AuthorityOnUnitSpawned;
    public static event Action<Unit> AuthorityOnUnitDespawned;


    public int GetResourceCost() 
    { 
        return _resourceCost; 
    }
    public UnitMovement GetUnitMovement()
    {
        return _unitMovement;
    }

    public Targeter GetTargeter()
    {
        return _targeter;
    }

    #region Server

    public override void OnStartServer()
    {
        _health.ServerOnDie += ServerHandleDie;
        ServerOnUnitSpawned?.Invoke(this);
    }

    public override void OnStopServer()
    {
        _health.ServerOnDie -= ServerHandleDie;
        ServerOnUnitDespawned?.Invoke(this);
    }

    [Server]
    private void ServerHandleDie()
    {
        NetworkServer.Destroy(gameObject);
    }

    #endregion

    #region Client

    public override void OnStartAuthority()
    {
        AuthorityOnUnitSpawned?.Invoke(this);
    }

    public override void OnStopClient()
    {
        if (!isOwned) { return; }

        AuthorityOnUnitDespawned?.Invoke(this);
    }

    [Client]
    public void Select()
    {
        if (!isOwned) { return; }
        _onSelected?.Invoke();
    }

    [Client]
    public void Deselect()
    {
        if (!isOwned) { return; }
        _onDeselected?.Invoke();
    }

    #endregion
}
