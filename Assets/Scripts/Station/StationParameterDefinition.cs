using UnityEngine;

public enum StationParameterType
{
    Boolean,
    Float
}

[CreateAssetMenu(
    fileName = "NewStationParameter", 
    menuName = "Game/Station/Parameter Definition")]
public class StationParameterDefinition : ScriptableObject
{
    [SerializeField] private string id;
    [SerializeField] private string displayName;
    [SerializeField] private StationParameterType valueType;
    [SerializeField] private bool defaultBoolValue;

    [SerializeField] private float defaultFloatValue;
    [SerializeField] private float minFloatValue;
    [SerializeField] private float maxFloatValue;

    public string Id => id;
    public string DisplayName => displayName;
    public StationParameterType ValueType => valueType;

    public bool DefaultBoolValue => defaultBoolValue;

    public float DefaultFloatValue => defaultFloatValue;
    public float MinFloatValue => minFloatValue;
    public float MaxFloatValue => maxFloatValue;
}
