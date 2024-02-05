using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UnitBase : NetworkBehaviour
{
    [SerializeField] private Health _health = null;

    public static event Action<UnitBase> ServerOnBaseSpawned;
    public static event Action<UnitBase> ServerOnBaseDespawned;


    #region Server

    public override void OnStartServer()
    {
        _health.ServerOnDie += ServerHandleDie;
        ServerOnBaseSpawned?.Invoke(this);
    }

    public override void OnStopServer()
    {
        ServerOnBaseDespawned?.Invoke(this);
        _health.ServerOnDie -= ServerHandleDie;
    }

    [Server]
    private void ServerHandleDie()
    {
        NetworkServer.Destroy(gameObject);
    }

    #endregion

    #region Client

    #endregion
}
