using UnityEngine;
using System.Collections.Generic;

public class GameTickSystem : MonoBehaviour
{
    [SerializeField, Min(0f)] private float safeInterval = 5f;
    [SerializeField, Range(0f, 1f)] private float chanceIncreasePerTick = 0.02f;
    [SerializeField, Range(0f, 1f)] private float maxChance = 0.3f;
    [SerializeField] private List<StationParameterDefinition> stationParameters = new();
    [SerializeField] private List<EquipmentDefinition> equipmentDefinitions = new();
    [SerializeField] private StationCalculationSystem calculationSystem;
    [SerializeField] private RandomBreakdownSystem breakdownSystem;
    private float safeTimer;
    private float currentBreakdownChance;
    private StationState stationState;
    private List<EquipmentState> equipmentStates = new();

    public float SafeInterval => safeInterval;
    public float ChanceIncreasePerTick => chanceIncreasePerTick;
    public float MaxChance => maxChance;
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

        UpdateRandomBreakdowns();

        calculationSystem.CalculateOutputs(equipmentStates, stationState);
        calculationSystem.CalculateFloatParameters(stationState);

        Debug.Log("Game Tick!");

        if (stationState.TryGetBool("oxygenGeneration", out bool oxygenGeneration))
            Debug.Log($"Oxygen generation: {oxygenGeneration}");

        if (stationState.TryGetFloat("oxygenAmount", out float oxygenAmount))
            Debug.Log($"Oxygen amount: {oxygenAmount}");
    }

    private void UpdateRandomBreakdowns()
    {
        if (breakdownSystem == null)
            return;

        if (safeTimer > 0f)
        {
            safeTimer = Mathf.Max(0f, safeTimer - TickInterval);
            return;
        }

        currentBreakdownChance = Mathf.Min(
            currentBreakdownChance + breakdownSystem.ChanceIncreasePerTick,
            breakdownSystem.MaxChance
        );

        if (UnityEngine.Random.value < currentBreakdownChance)
        {
            bool breakdownOccurred =
                breakdownSystem.TryBreakRandomComponent(equipmentStates);

            if (breakdownOccurred)
            {
                safeTimer = breakdownSystem.SafeInterval;
                currentBreakdownChance = 0f;

                Debug.Log("Random breakdown occurred!");
            }
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
