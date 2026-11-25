
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private readonly List<ComponentState> components = new();

    public IReadOnlyList<ComponentState> Components => components;

    public bool TryAddComponent(ComponentState component)
    {
        if (component == null ||
            component.IsInstalled ||
            components.Contains(component))
        {
            return false;
        }

        components.Add(component);
        return true;
    }

    public bool TryRemoveComponent(ComponentState component)
    {
        if (component == null)
            return false;

        return components.Remove(component);
    }
}
