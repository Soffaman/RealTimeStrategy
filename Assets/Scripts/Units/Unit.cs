using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

public class Unit : NetworkBehaviour
{
    [SerializeField] private UnitMovement _unitMovement = null;
    [SerializeField] private UnityEvent _onSelected = null;
    [SerializeField] private UnityEvent _onDeselected = null;

    public UnitMovement GetUnitMovement()
    {
        return _unitMovement;
    } 

    #region Client

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
