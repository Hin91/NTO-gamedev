using UnityEngine;

[System.Serializable]
public class StationParameterCalculationDefinition
{
    [SerializeField] private StationParameterDefinition targetParameter;
    [SerializeField] private StationParameterDefinition conditionParameter;
    [SerializeField] private float baseChangePerTick;
    [SerializeField] private float additionalChangePerTick;

    public StationParameterDefinition TargetParameter => targetParameter;
    public StationParameterDefinition ConditionParameter => conditionParameter;
    public float BaseChangePerTick => baseChangePerTick;
    public float AdditionalChangePerTick => additionalChangePerTick;
}
