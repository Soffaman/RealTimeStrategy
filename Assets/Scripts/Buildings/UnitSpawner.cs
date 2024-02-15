using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
{
    [SerializeField] private Health _health = null;
    [SerializeField] private Unit _unitPrefab = null;
    [SerializeField] private Transform _unitSpawnPoint = null;
    [SerializeField] private TMP_Text _remainingUnitsText = null;
    [SerializeField] private Image _unitProgressImage = null;
    [SerializeField] private int _maxUnitQueue = 5;
    [SerializeField] private float _spawnMoveRange = 7f;
    [SerializeField] private float _unitSpawnDuration = 5f;

    [SyncVar(hook = nameof(ClientHandleQueuedUnitsUpdated))]
    private int _queuedUnits;
    [SyncVar]
    private float _unitTimer;

    private float progressImageVelocity;

    private void Update()
    {
        if (isServer)
        {
            ProduceUnits();
        }

        if(isClient)
        {
            UpdateTimerDisplay();
        }
    }

    #region Server

    public override void OnStartServer()
    {
        _health.ServerOnDie += ServerHandleDie;

    }

    public override void OnStopServer()
    {
        _health.ServerOnDie -= ServerHandleDie;
    }

    [Server]
    private void ProduceUnits()
    {
        if(_queuedUnits == 0)
        {
            return;
        }

        _unitTimer += Time.deltaTime;

        if(_unitTimer < _unitSpawnDuration) 
        { 
            return; 
        }

        GameObject unitInstance = Instantiate(
            _unitPrefab.gameObject,
            _unitSpawnPoint.position,
            _unitSpawnPoint.rotation);

        NetworkServer.Spawn(unitInstance, connectionToClient);

        Vector3 spawnOffset = Random.insideUnitSphere * _spawnMoveRange;
        spawnOffset.y = _unitSpawnPoint.position.y;

        UnitMovement unitMovement = unitInstance.GetComponent<UnitMovement>();
        unitMovement.ServerMove(_unitSpawnPoint.position + spawnOffset);

        _queuedUnits--;
        _unitTimer = 0f;
    }

    [Server]
    private void ServerHandleDie()
    {
        NetworkServer.Destroy(gameObject);
    }

    [Command]
    private void CmdSpawnUnit()
    {
        if (_queuedUnits == _maxUnitQueue) { return; }

        RTSPlayer player = connectionToClient.identity.GetComponent<RTSPlayer>();

        if (player.GetResources() < _unitPrefab.GetResourceCost()) { return; }

        _queuedUnits++;

        player.SetResources(player.GetResources() - _unitPrefab.GetResourceCost());
    }

    #endregion

    #region Client

    private void UpdateTimerDisplay()
    {
        float newProgress = _unitTimer / _unitSpawnDuration;

        if(newProgress > _unitProgressImage.fillAmount)
        {
            _unitProgressImage.fillAmount = newProgress;
        }
        else
        {
            _unitProgressImage.fillAmount = Mathf.SmoothDamp(
                _unitProgressImage.fillAmount,
                newProgress,
                ref progressImageVelocity,
                0.1f
                );
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button != PointerEventData.InputButton.Left) { return; }

        if (!isOwned) { return; }

        CmdSpawnUnit();
    }

    private void ClientHandleQueuedUnitsUpdated(int oldUnits, int newUnits)
    {
        _remainingUnitsText.text = newUnits.ToString();
    }

    #endregion
}
