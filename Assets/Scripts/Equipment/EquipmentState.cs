using System;
using System.Collections.Generic;

public class EquipmentState
{
    public EquipmentDefinition Definition { get; }

    private readonly Dictionary<string, ComponentState> components = new();
    private bool isOperational=true;
    public bool IsOperational => isOperational;
    public IReadOnlyCollection<ComponentState> Components => components.Values;

    public EquipmentState(EquipmentDefinition definition)
    {
        if (definition == null)
            throw new ArgumentNullException(nameof(definition));

        Definition = definition;

        if (definition.Slots == null)
            throw new InvalidOperationException(
                $"У оборудования {definition.Id} отсутствует список слотов."
            );

        foreach (ComponentSlotDefinition slot in definition.Slots)
        {
            if (slot == null)
                throw new InvalidOperationException(
                    "Обнаружен пустой слот оборудования."
                );

            if (string.IsNullOrWhiteSpace(slot.Id))
                throw new InvalidOperationException(
                    "У слота оборудования отсутствует ID."
                );

            if (slot.Component == null)
                throw new InvalidOperationException(
                    $"У слота {slot.Id} не назначен компонент."
                );

            if (string.IsNullOrWhiteSpace(slot.Component.Id))
                throw new InvalidOperationException(
                    $"У компонента в слоте {slot.Id} отсутствует ID."
                );

            if (components.ContainsKey(slot.Id))
                throw new InvalidOperationException(
                    $"Повторяется ID слота: {slot.Id}."
                );

            components.Add(
                slot.Id,
                new ComponentState(slot.Component)
            );
        }
    }

    public bool TryGetComponent(string slotId, out ComponentState component)
    {
        if (string.IsNullOrWhiteSpace(slotId))
        {
            component = null;
            return false;
        }

        if (!components.TryGetValue(slotId, out component))
            return false;

        return component != null;
    }
    public void CheckOperationalState()
    {
        foreach (ComponentSlotDefinition slot in Definition.Slots)
        {
            if (!slot.RequiredForOperation)
                continue;

            if (!components.TryGetValue(slot.Id, out ComponentState component)
                || component == null
                || !component.IsInstalled
                || component.IsBroken)
            {
                isOperational = false;
                return;
            }
        }
        isOperational = true;
    }


    public bool TryExtractComponent(
        string slotId,
        out ComponentState component)
    {
        component = null;

        if (!components.TryGetValue(slotId, out ComponentState found))
            return false;

        if (found == null || !found.Extract())
            return false;

        components.Remove(slotId);
        component = found;

        CheckOperationalState();
        return true;
    }

    public bool TryInstallComponent(
        string slotId,
        ComponentState component)
    {
        if (component == null || component.IsInstalled)
            return false;

        ComponentSlotDefinition slot =
            Definition.Slots.Find(candidate => candidate.Id == slotId);

        if (slot == null)
            return false;

        if (slot.Component.Id != component.Definition.Id)
            return false;

        if (components.ContainsKey(slotId))
            return false;

        if (!component.Install())
            return false;

        components.Add(slotId, component);

        CheckOperationalState();
        return true;
    }


}