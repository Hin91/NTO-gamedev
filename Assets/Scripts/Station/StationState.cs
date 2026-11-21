
using System;
using System.Collections.Generic;

public class StationState
{
    private readonly Dictionary<string, StationParameterDefinition> definitions = new();

    private readonly Dictionary<string, bool> boolValues = new();
    private readonly Dictionary<string, float> floatValues = new();

    public StationState(IEnumerable<StationParameterDefinition> parameterDefinitions)
    {
        if (parameterDefinitions == null)
            throw new ArgumentNullException(nameof(parameterDefinitions));

        foreach (StationParameterDefinition definition in parameterDefinitions)
        {
            if (definition == null)
                throw new InvalidOperationException(
                    "Обнаружено пустое определение параметра станции."
                );

            if (string.IsNullOrWhiteSpace(definition.Id))
                throw new InvalidOperationException(
                    "У параметра станции отсутствует ID."
                );

            if (definitions.ContainsKey(definition.Id))
                throw new InvalidOperationException(
                    $"Повторяется ID параметра станции: {definition.Id}."
                );

            definitions.Add(definition.Id, definition);

            switch (definition.ValueType)
            {
                case StationParameterType.Boolean:
                    boolValues.Add(
                        definition.Id,
                        definition.DefaultBoolValue
                    );
                    break;

                case StationParameterType.Float:
                    if (definition.MinFloatValue > definition.MaxFloatValue)
                        throw new InvalidOperationException(
                            $"Некорректный диапазон параметра {definition.Id}."
                        );

                    floatValues.Add(
                        definition.Id,
                        Math.Clamp(
                            definition.DefaultFloatValue,
                            definition.MinFloatValue,
                            definition.MaxFloatValue
                        )
                    );
                    break;

                default:
                    throw new InvalidOperationException(
                        $"Неизвестный тип параметра {definition.Id}."
                    );
            }
        }
    }

    public bool TryGetBool(string id, out bool value)
    {
        value = false;

        if (!definitions.TryGetValue(id, out StationParameterDefinition definition)
            || definition.ValueType != StationParameterType.Boolean)
            return false;

        return boolValues.TryGetValue(id, out value);
    }

    public bool TrySetBool(string id, bool value)
    {
        if (!definitions.TryGetValue(id, out StationParameterDefinition definition)
            || definition.ValueType != StationParameterType.Boolean)
            return false;

        boolValues[id] = value;
        return true;
    }

    public bool TryGetFloat(string id, out float value)
    {
        value = 0f;

        if (!definitions.TryGetValue(id, out StationParameterDefinition definition)
            || definition.ValueType != StationParameterType.Float)
            return false;

        return floatValues.TryGetValue(id, out value);
    }

    public bool TrySetFloat(string id, float value)
    {
        if (!definitions.TryGetValue(id, out StationParameterDefinition definition)
            || definition.ValueType != StationParameterType.Float)
            return false;

        floatValues[id] = Math.Clamp(
            value,
            definition.MinFloatValue,
            definition.MaxFloatValue
        );

        return true;
    }

    public void ResetBool()
    {
        List<string> keys = new List<string>(boolValues.Keys);

        foreach (string id in keys)
        {
            boolValues[id] = false;
        }
    }
}
