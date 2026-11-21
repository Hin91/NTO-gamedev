using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Rendering.CameraUI;

public class StationCalculationSystem
{
    public bool canOperate(EquipmentState equipmentState, StationState stationState)
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

    public void calculateOutputs(IEnumerable<EquipmentState> equipmentStates, StationState statiomState)
    {
        Dictionary<string, bool> calculatedOutputs = new();
        foreach (EquipmentState equipment in equipmentStates)
        {
            if (equipment == null || !canOperate(equipment, statiomState))
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
    }
}
