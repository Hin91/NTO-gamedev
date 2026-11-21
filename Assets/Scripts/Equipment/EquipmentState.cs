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
                || component.IsBroken)
            {
                isOperational = false;
                return;
            }
        }
        isOperational = true;
    }
}