using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class RTSPlayer : NetworkBehaviour
{
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private LayerMask buildingBlockLayer = new LayerMask();
    [SerializeField] private Building[] _buildings = new Building[0];
    [SerializeField] private float buildingRangeLimit = 5f;

    [SyncVar(hook = nameof(ClientHandleResourcesUpdate))]
    private int _resources = 500;

    public event Action<int> ClientOnResourcesUpdated;

    private Color _teamColor = new Color();
    private List<Unit> _myUnits = new List<Unit>();
    private List<Building> _myBuildings = new List<Building>();

    public Transform GetCameraTransform()
    {
        return _cameraTransform;
    }

    public Color GetTeamColor()
    {
        return _teamColor;
    }

    public List<Unit> GetMyUnits()
    {
        return _myUnits;
    }

    public List<Building> GetMyBuildings()
    {
        return _myBuildings;
    }

    public int GetResources()
    {
        return _resources;
    }

    public bool CanPlaceBuilding(BoxCollider buildingCollider,Vector3 point)
    {
        if (Physics.CheckBox(
            point + buildingCollider.center,
            buildingCollider.size / 2,
            Quaternion.identity,
            buildingBlockLayer))
        {
            return false;
        }

        foreach (Building building in _myBuildings)
        {
            if ((point - building.transform.position).sqrMagnitude
                <= buildingRangeLimit * buildingRangeLimit)
            {
                return true;
            }
        }

        return false;
    }

    #region Server

    public override void OnStartServer()
    {
        Unit.ServerOnUnitSpawned += ServerHandleUnitSpawned;
        Unit.ServerOnUnitDespawned += ServerHandleUnitDespawned;
        Building.ServerOnBuildingSpawned += ServerHandleBuildingSpawned;
        Building.ServerOnBuildingDespawned += ServerHandleBuildingDespawned;
    }

    

    public override void OnStopServer()
    {
        Unit.ServerOnUnitSpawned -= ServerHandleUnitSpawned;
        Unit.ServerOnUnitDespawned -= ServerHandleUnitDespawned;
        Building.ServerOnBuildingSpawned -= ServerHandleBuildingSpawned;
        Building.ServerOnBuildingDespawned -= ServerHandleBuildingDespawned;
    }

    [Server]
    public void SetTeamColor(Color newTeamColor)
    {
        _teamColor = newTeamColor;
    }

    [Server]
    public void SetResources(int newRes)
    {
        _resources = newRes;
    }

    [Command]
    public void CmdTryPlaceBuilding(int buildingId, Vector3 point)
    {
        Building buildingToPlace = null;

        foreach(Building building in _buildings)
        {
            if(building.GetId() == buildingId)
            {
                buildingToPlace = building;
                break;
            }
        }

        if(buildingToPlace == null) { return; }

        if(_resources < buildingToPlace.GetPrice()) { return; }

        BoxCollider buildingCollider = buildingToPlace.GetComponent<BoxCollider>();

        if (!CanPlaceBuilding(buildingCollider, point)) { return; }

        GameObject buildingInstance = 
            Instantiate(buildingToPlace.gameObject, point, buildingToPlace.transform.rotation);

        NetworkServer.Spawn(buildingInstance, connectionToClient);

        SetResources(_resources - buildingToPlace.GetPrice());
    }


    private void ServerHandleBuildingSpawned(Building building)
    {
        if(building.connectionToClient.connectionId != connectionToClient.connectionId) { return; }

        _myBuildings.Add(building);
    }

    private void ServerHandleBuildingDespawned(Building building)
    {
        if (building.connectionToClient.connectionId != connectionToClient.connectionId) { return; }

        _myBuildings.Remove(building);
    }

    private void ServerHandleUnitSpawned(Unit unit)
    {
        if (unit.connectionToClient.connectionId != connectionToClient.connectionId) { return; }

        _myUnits.Add(unit);
    }

    private void ServerHandleUnitDespawned(Unit unit)
    {
        if (unit.connectionToClient.connectionId != connectionToClient.connectionId) { return; }

        _myUnits.Remove(unit);
    }

    #endregion

    #region Client

    public override void OnStartAuthority()
    {
        if (NetworkServer.active) { return; }
        Unit.AuthorityOnUnitSpawned += AuthorityHandleUnitSpawned;
        Unit.AuthorityOnUnitDespawned += AuthorityHandleUnitDespawned;
        Building.AuthorityOnBuildingSpawned += AuthorityHandleBuildingSpawned;
        Building.AuthorityOnBuildingDespawned += AuthorityHandleBuildingDespawned;

    }

    public override void OnStopClient()
    {
        if (!isClientOnly || !isOwned) { return; }
        Unit.AuthorityOnUnitSpawned -= AuthorityHandleUnitSpawned;
        Unit.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawned;
        Building.AuthorityOnBuildingSpawned -= AuthorityHandleBuildingSpawned;
        Building.AuthorityOnBuildingDespawned -= AuthorityHandleBuildingDespawned;
    }

    private void ClientHandleResourcesUpdate(int oldRes, int newRes)
    {
        ClientOnResourcesUpdated?.Invoke(newRes);
    }

    private void AuthorityHandleBuildingSpawned(Building building)
    {
        _myBuildings.Add(building);
    }
    private void AuthorityHandleBuildingDespawned(Building building)
    {
        _myBuildings.Remove(building);
    }

    private void AuthorityHandleUnitSpawned(Unit unit)
    {
        _myUnits.Add(unit);
    }

    private void AuthorityHandleUnitDespawned(Unit unit)
    {
        _myUnits.Remove(unit);
    }

    #endregion
}
