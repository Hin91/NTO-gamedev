using UnityEngine;

[System.Serializable]
public class EquipmentOutputDefinition
{
    [SerializeField] private StationParameterDefinition parameter;
    [SerializeField] private bool valueWhenOperational;

    public StationParameterDefinition Parameter => parameter;
    public bool ValueWhenOperational => valueWhenOperational;
}
