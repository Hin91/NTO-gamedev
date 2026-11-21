using UnityEngine;

[CreateAssetMenu (
    fileName = "NewComponent", 
    menuName = "Game/Components/Component Definition"
    )]

public class ComponentDefinition : ScriptableObject
{
    [SerializeField] private string id;
    [SerializeField] private string displayName;

    public string Id => id;
    public string DisplayName => displayName;
}
