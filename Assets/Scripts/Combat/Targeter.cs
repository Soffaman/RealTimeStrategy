using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Targeter : NetworkBehaviour
{
    private Targetable _target;

    #region Server

    [Command]
    public void CmdSetTarger(GameObject targetGameObject)
    {
        if (!targetGameObject.TryGetComponent<Targetable>(out Targetable newTarget)) { return; }

        _target = newTarget;
    }

    [Server]
    public void ClearTarget()
    {
        _target = null;
    }

    #endregion

    #region Client



    #endregion
}
