using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Command
{
    public abstract void Execute(Actor actor, bool keyDown = true);
}

public class MoveForward : Command
{
    public override void Execute(Actor actor, bool keyDown = true)
    {
        actor.transform.position += Vector3.forward * 10 * Time.deltaTime;
    }
}

public class MoveBack : Command
{
    public override void Execute(Actor actor, bool keyDown = true)
    {

    }
}

public class MoveRight : Command
{
    public override void Execute(Actor actor, bool keyDown = true)
    {

    }
}

public class MoveLeft : Command
{
    public override void Execute(Actor actor, bool keyDown = true)
    {

    }
}