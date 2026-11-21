using UnityEngine;

public class ComponentState
{
    public ComponentDefinition Definition { get; }
    public bool IsBroken { get; private set; }

    public ComponentState (ComponentDefinition defenition)
    {
        Definition = defenition;
        IsBroken = false;
    }

    public void Break()
    {
        IsBroken = true;
    }

    public void Repair()
    {
        IsBroken = false;
    }
}
