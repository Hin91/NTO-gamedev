
using UnityEngine;

public class EquipmentInteraction : MonoBehaviour
{

    [SerializeField] private EquipmentDefinition definition;
    [SerializeField] private EquipmentInteractionUI interactionUI;

    public string DisplayName => definition.DisplayName;
    public EquipmentDefinition Definition => definition;
    public EquipmentState State { get; private set; }

    public void Initialize(EquipmentState state)
    {
        if (state == null)
        {
            Debug.LogError(
                $"Для {gameObject.name} передано пустое состояние оборудования."
            );
            return;
        }

        State = state;
    }

    public void OpenInteraction()
    {
        if (interactionUI == null)
        {
            Debug.LogError(
                $"Для {gameObject.name} не назначен EquipmentInteractionUI."
            );
            return;
        }

        if (State == null)
        {
            Debug.LogError(
                $"Для {gameObject.name} ещё не инициализировано состояние."
            );
            return;
        }

        interactionUI.Show(this);
    }
}
