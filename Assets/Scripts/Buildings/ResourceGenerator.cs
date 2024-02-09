using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceGenerator : NetworkBehaviour
{
    [SerializeField] private Health _health = null;
    [SerializeField] private int _resourcesPerInterval = 10;
    [SerializeField] private float _interval = 2f;

    private float _timer;
    private RTSPlayer _player;

    public override void OnStartServer()
    {
        _timer = _interval;
        _player = connectionToClient.identity.GetComponent<RTSPlayer>();

        _health.ServerOnDie += ServerHandleDie;
        GameOverHandler.ServerOnGameOver += ServerHandleGameOver;
    }

    public override void OnStopServer()
    {
        _health.ServerOnDie -= ServerHandleDie;
        GameOverHandler.ServerOnGameOver -= ServerHandleGameOver;
    }

    [Server]
    private void Update()
    {
        _timer -= Time.deltaTime;

        if(_timer <= 0)
        {
            _player.SetResources(_player.GetResources() + _resourcesPerInterval);

            _timer += _interval;
        }
    }


    private void ServerHandleGameOver()
    {
        enabled = false;
    }

    private void ServerHandleDie()
    {
        NetworkServer.Destroy(gameObject);
    }

    
}
