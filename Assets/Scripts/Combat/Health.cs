using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : NetworkBehaviour
{
    [SerializeField] private int _maxHealth = 100;

    [SyncVar]
    private int _currentHealt;

    public event Action ServerOnDie;

    #region Server

    public override void OnStartServer()
    {
        _currentHealt = _maxHealth;
    }

    [Server]
    public void DealDamage(int damageAmount)
    {
        if(_currentHealt == 0) { return; }

        _currentHealt = Mathf.Max(_currentHealt - damageAmount, 0);

        if(_currentHealt != 0) { return; }
        
        ServerOnDie?.Invoke();

        Debug.Log("We Died");
    }

    #endregion

    #region Client

    #endregion

}
