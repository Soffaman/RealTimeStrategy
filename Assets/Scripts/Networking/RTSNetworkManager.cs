using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RTSNetworkManager : NetworkManager
{
    [SerializeField] private GameObject _unitSpawnerPrefab = null;
    [SerializeField] private GameOverHandler _gameOverHandlerPrefab = null;

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);

        GameObject unitSpawnerInstance = Instantiate(
            _unitSpawnerPrefab,
            conn.identity.transform.position,
            conn.identity.transform.rotation);

        NetworkServer.Spawn(unitSpawnerInstance, conn);
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        if (SceneManager.GetActiveScene().name.StartsWith("Scene_Map"))
        {
            GameOverHandler gameOverHandlerInstance = Instantiate(_gameOverHandlerPrefab);

            NetworkServer.Spawn(gameOverHandlerInstance.gameObject);
        }
    }
}
