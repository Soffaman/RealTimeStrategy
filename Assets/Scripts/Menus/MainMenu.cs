using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject _landingPagePanel = null;

    public void HostLobby()
    {
        _landingPagePanel.SetActive(false);

        NetworkManager.singleton.StartHost();
    }
}
