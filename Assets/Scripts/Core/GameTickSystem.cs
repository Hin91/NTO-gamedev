using UnityEngine;
using System.Collections.Generic;

public class GameTickSystem : MonoBehaviour
{
    [SerializeField] private List<StationParameterDefinition> stationParameters = new();
    [SerializeField] private List<EquipmentDefinition> equipmentDefinitions = new();
    [SerializeField] private StationCalculationSystem calculationSystem;
    private StationState stationState;
    private List<EquipmentState> equipmentStates = new();

    private const float TickInterval = 1f;
    private float timer = 0f;
    
    private void Update()
    {
        timer += Time.deltaTime;

        while (timer >= TickInterval)
        {
            timer -= TickInterval;
            Tick();
        }
    }

    private void Tick()
    {
        foreach (EquipmentState equipment in equipmentStates)
        {
            equipment.CheckOperationalState();
        }

        calculationSystem.CalculateOutputs(equipmentStates, stationState);
        calculationSystem.CalculateFloatParameters(stationState);

        Debug.Log("Game Tick!");

        if (stationState.TryGetBool("oxygenGeneration", out bool oxygenGeneration))
        {
            Debug.Log($"Oxygen generation: {oxygenGeneration}");
        }

        if (stationState.TryGetFloat("oxygenAmount", out float oxygenAmount))
        {
            Debug.Log($"Oxygen amount: {oxygenAmount}");
        }
    }

    private void InitializeGame()
    {
        stationState = new StationState(stationParameters);

        equipmentStates = new List<EquipmentState>();

        foreach (EquipmentDefinition definition in equipmentDefinitions)
        {
            equipmentStates.Add(new EquipmentState(definition));
        }
    }
    private void Start()
    {
        InitializeGame();
    }
}
