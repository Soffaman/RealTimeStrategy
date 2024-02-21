using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResourcesDisplay : MonoBehaviour
{
    [SerializeField] TMP_Text _resourcesText;

    private RTSPlayer _player;

    private void Start()
    {
        _player = NetworkClient.connection?.identity.GetComponent<RTSPlayer>();

        ClientHandleResourcesUpdated(_player.GetResources());

        _player.ClientOnResourcesUpdated += ClientHandleResourcesUpdated;
    }

    private void OnDestroy()
    {
        _player.ClientOnResourcesUpdated -= ClientHandleResourcesUpdated;
    }

    private void ClientHandleResourcesUpdated(int resources)
    {
        _resourcesText.text = $"Resources: {resources}";
    }
}
