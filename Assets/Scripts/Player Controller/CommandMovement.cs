using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

/// <summary>
/// Command Pattern used for Movement of the Alive and Dead Players
/// </summary>

public abstract class CommandMovement
{
    public abstract void Execute(Actor actor, object param = null);
}

public class MoveActor : CommandMovement
{
    public override void Execute(Actor actor, object param = null)
    {
        // the actual calculation of the Vector3 happens in the InputHandlerAlive + InputHandlerDead scripts
        actor.cc.Move((Vector3)param);
    }
}

public class FlyDead : CommandMovement
{
    public override void Execute(Actor actor, object param = null)
    {
        // the actual calculation of the Vector3 happens in the InputHandlerAlive + InputHandlerDead scripts
        actor.transform.Translate((Vector3)param, Space.World);
    }
}

public class RotateActor : CommandMovement
{
    public override void Execute(Actor actor, object param = null)
    {
        // the actual calculation of the float happens in the InputHandlerAlive + InputHandlerDead scripts
        actor.body.transform.rotation = Quaternion.Euler(0f, (float)param, 0f);
    }
}
