using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : NetworkBehaviour
{
    [SerializeField] private int _maxHealth = 100;

    [SyncVar(hook = nameof(HandleHealthUpdated))]
    private int _currentHealt;

    public event Action ServerOnDie;

    public event Action<int, int> ClientOnHealthUpdated;

    #region Server

    public override void OnStartServer()
    {
        _currentHealt = _maxHealth;

        UnitBase.ServerOnPlayerDie += ServerHandlePlayerDie;
    }

    public override void OnStopServer()
    {
        UnitBase.ServerOnPlayerDie -= ServerHandlePlayerDie;
    }

    [Server]
    private void ServerHandlePlayerDie(int connectionId)
    {
        if (connectionToClient.connectionId != connectionId) { return; }


        DealDamage(_currentHealt);
    }

    [Server]
    public void DealDamage(int damageAmount)
    {
        if (_currentHealt == 0) { return; }

        _currentHealt = Mathf.Max(_currentHealt - damageAmount, 0);

        if (_currentHealt != 0) { return; }

        ServerOnDie?.Invoke();

        Debug.Log("We Died");
    }

    

    #endregion

    #region Client

    private void HandleHealthUpdated(int oldHealth, int newHealth)
    {
        ClientOnHealthUpdated?.Invoke(newHealth, _maxHealth);
    }

    #endregion

}
