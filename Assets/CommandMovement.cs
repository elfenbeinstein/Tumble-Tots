using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CommandMovement
{
    public abstract void Execute();
}

public class Jumping : CommandMovement
{
    public override void Execute()
    {

    }
}

public class Dashing : CommandMovement
{
    public override void Execute()
    {

    }
}

public class DoNothing : CommandMovement
{
    public override void Execute()
    {

    }
}
