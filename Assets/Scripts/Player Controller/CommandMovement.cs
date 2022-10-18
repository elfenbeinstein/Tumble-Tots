using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CommandMovement
{
    public abstract void Execute(Actor actor, object param = null);
}

public class MoveActor : CommandMovement
{
    public override void Execute(Actor actor, object param = null)
    {
        actor.cc.Move((Vector3)param);
    }
}

public class RotateActor : CommandMovement
{
    public override void Execute(Actor actor, object param = null)
    {
        actor.cc.transform.rotation = Quaternion.Euler(0f, (float)param, 0f);
    }
}

public class DoNothing : CommandMovement
{
    public override void Execute(Actor actor, object param = null)
    {

    }
}
