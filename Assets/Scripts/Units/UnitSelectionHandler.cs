using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitSelectionHandler : MonoBehaviour
{

    [SerializeField] private LayerMask _layerMask = new LayerMask();
    private Camera _mainCamera;

    public List<Unit> SelectedUnits { get; } = new List<Unit>();

    private void Start()
    {
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            foreach (Unit selectedUnit in SelectedUnits)
            {
                selectedUnit.Deselect();
            }

            SelectedUnits.Clear();
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            ClearSelectionArea();
        }
    }

    private void ClearSelectionArea()
    {
        Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _layerMask)) { return; }

        if(!hit.collider.TryGetComponent<Unit>(out Unit unit)) { return; }

        if(!unit.isOwned) { return; }

        SelectedUnits.Add(unit);

        foreach (Unit selectedUnit in SelectedUnits)
        {
            selectedUnit.Select();
        }
    }
}
