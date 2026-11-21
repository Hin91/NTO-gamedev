using UnityEngine;

[System.Serializable]
public class ComponentSlotDefinition
{
    [SerializeField] private string id;
    [SerializeField] private string displayName;
    [SerializeField] private ComponentDefinition component;
    [SerializeField] private bool requiredForOperation;

    public string Id => id;
    public string DisplayName => displayName;
    public ComponentDefinition Component => component;
    public bool RequiredForOperation => requiredForOperation;
}