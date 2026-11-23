
using TMPro;
using UnityEngine;

public class EquipmentInteractionUI : MonoBehaviour
{
    [SerializeField] private GameObject interactionPanel;
    [SerializeField] private TMP_Text equipmentNameText;

    private void Awake()
    {
        interactionPanel.SetActive(false);
    }

    public void Show(EquipmentInteraction equipment)
    {
        if (equipment == null)
            return;

        equipmentNameText.text = equipment.DisplayName;
        interactionPanel.SetActive(true);
    }

    public void Close()
    {
        interactionPanel.SetActive(false);
    }
}
