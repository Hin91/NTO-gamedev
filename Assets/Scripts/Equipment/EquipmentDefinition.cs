using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(
    fileName="NewEquipment", 
    menuName="Game/Equipment/Equipment Definition")]
public class EquipmentDefinition : ScriptableObject
{
    [SerializeField] private string id;
    [SerializeField] private string displayName;
    [SerializeField] private List<ComponentSlotDefinition> slots = new();
    [SerializeField] private List<EquipmentOutputDefinition> outputs = new();
    [SerializeField] private List<StationParameterDefinition> inputParameters = new();

    public string Id => id;
    public string DisplayName => displayName;
    public List<ComponentSlotDefinition> Slots => slots;
    public IReadOnlyList<EquipmentOutputDefinition> Outputs => outputs;
    public IReadOnlyList<StationParameterDefinition> InputParameters => inputParameters;
}
