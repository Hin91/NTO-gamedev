
public class ComponentState
{
    public ComponentDefinition Definition { get; }
    public bool IsBroken { get; private set; }
    public bool IsInstalled { get; private set; }

    public ComponentState(ComponentDefinition definition)
    {
        Definition = definition;
        IsBroken = false;
        IsInstalled = true;
    }

    public void Break()
    {
        IsBroken = true;
    }

    public void Repair()
    {
        IsBroken = false;
    }

    public bool Extract()
    {
        if (!IsInstalled)
            return false;

        IsInstalled = false;
        return true;
    }

    public bool Install()
    {
        if (IsInstalled)
            return false;

        IsInstalled = true;
        return true;
    }
}
