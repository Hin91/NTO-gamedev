using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "RandomBreakdownSystem",
    menuName = "Game/Random Breakdown System"
)]
public class RandomBreakdownSystem : ScriptableObject
{
    [SerializeField] private float safeInterval = 5f;
    [SerializeField] private float chanceIncreasePerTick = 0.02f;
    [SerializeField] private float maxChance = 0.3f;

    public float SafeInterval => safeInterval;
    public float ChanceIncreasePerTick => chanceIncreasePerTick;
    public float MaxChance => maxChance;

    private const string FuseComponentId = "fuse";

    public bool TryBreakRandomComponent(
    List<EquipmentState> equipmentStates)
    {
        if (equipmentStates == null || equipmentStates.Count == 0)
            return false;

        List<EquipmentState> availableEquipment = new();

        foreach (EquipmentState equipment in equipmentStates)
        {
            if (equipment == null)
                continue;

            foreach (ComponentState component in equipment.Components)
            {
                if (component != null && !component.IsBroken)
                {
                    availableEquipment.Add(equipment);
                    break;
                }
            }
        }

        if (availableEquipment.Count == 0)
            return false;

        EquipmentState selectedEquipment =
            availableEquipment[UnityEngine.Random.Range(
                0, availableEquipment.Count)];

        List<ComponentState> availableComponents = new();

        foreach (ComponentState component in selectedEquipment.Components)
        {
            if (component != null && !component.IsBroken)
                availableComponents.Add(component);
        }

        if (availableComponents.Count == 0)
            return false;

        ComponentState selectedComponent =
            availableComponents[UnityEngine.Random.Range(
                0, availableComponents.Count)];

        ComponentState workingFuse = null;

        foreach (ComponentState component in availableComponents)
        {
            if (component.Definition.Id == FuseComponentId)
            {
                workingFuse = component;
                break;
            }
        }

        if (workingFuse != null &&
            selectedComponent.Definition.Id != FuseComponentId)
        {
            workingFuse.Break();
        }
        else
        {
            selectedComponent.Break();
        }

        selectedEquipment.CheckOperationalState();

        return true;
    }
}