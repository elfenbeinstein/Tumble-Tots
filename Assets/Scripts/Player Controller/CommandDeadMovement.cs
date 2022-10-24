using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CommandDeadMovement
{
    public abstract void Execute(Actor actor, object param = null);
}

public class MoveDead : CommandDeadMovement
{
    public override void Execute(Actor actor, object param = null)
    {
        actor.transform.Translate((Vector3)param, Space.World);
    }
}
