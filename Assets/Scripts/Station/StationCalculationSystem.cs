using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "CalculationSystem",
    menuName = "Game/Station/Calculation System"
)]
public class StationCalculationSystem : ScriptableObject
{
    [SerializeField] private List<StationParameterCalculationDefinition> parameterCalculations = new();

    public bool CanOperate(EquipmentState equipmentState, StationState stationState)
    {
        if (equipmentState == null) throw new ArgumentNullException(nameof(equipmentState));
        if (stationState == null) throw new ArgumentNullException(nameof(stationState));

        if (!equipmentState.IsOperational) return false;

        foreach (StationParameterDefinition input in equipmentState.Definition.InputParameters)
        {
            if (input == null || input.ValueType != StationParameterType.Boolean)
                return false;

            if (!stationState.TryGetBool(input.Id, out bool value) || !value)
                return false;
        }
        return true;
    }

    public void CalculateOutputs(IEnumerable<EquipmentState> equipmentStates, StationState stationState)
    {
        Dictionary<string, bool> calculatedOutputs = new();
        foreach (EquipmentState equipment in equipmentStates)
        {
            if (equipment == null || !CanOperate(equipment, stationState))
            {
                continue;
            }
            foreach (EquipmentOutputDefinition output in equipment.Definition.Outputs) {
                if (output == null || output.Parameter == null)
                    continue;
                if (calculatedOutputs.ContainsKey(output.Parameter.Id))
                {
                    calculatedOutputs[output.Parameter.Id] = calculatedOutputs[output.Parameter.Id] || output.ValueWhenOperational;
                    continue;
                }
                calculatedOutputs.Add(output.Parameter.Id, output.ValueWhenOperational);
            }
        }
        stationState.ResetBool();
        foreach (var pair in calculatedOutputs)
        {
            stationState.TrySetBool(pair.Key, pair.Value);
        }
    }

    public void CalculateFloatParameters(StationState stationState)
    {
        foreach (StationParameterCalculationDefinition calculation in parameterCalculations)
        {
            if (calculation == null ||
                calculation.TargetParameter == null ||
                calculation.ConditionParameter == null)
            {
                continue;
            }

            if (!stationState.TryGetFloat(calculation.TargetParameter.Id, out float curValue))
                continue;

            if (!stationState.TryGetBool(calculation.ConditionParameter.Id, out bool condition))
                continue;

            curValue += calculation.BaseChangePerTick;

            if (condition)
                curValue += calculation.AdditionalChangePerTick;

            stationState.TrySetFloat(calculation.TargetParameter.Id, curValue);
        }
    }
}
