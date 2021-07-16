using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Glidders.Manager;

public class CommandManager : MonoBehaviour
{
    private MoveSignal moveSignal = default;
    private AttackSignal attackSignal = default;
    private DirecionSignal direcionSignal = default;

    public void SetMoveSignal(MoveSignal setSignal)
    {
        moveSignal = setSignal;
    }

    public void SetAttackSignal(AttackSignal setSignal)
    {
        attackSignal = setSignal;
    }

    public void SetDirectionSignal(DirecionSignal setSignal)
    {
        direcionSignal = setSignal;
    }


}
