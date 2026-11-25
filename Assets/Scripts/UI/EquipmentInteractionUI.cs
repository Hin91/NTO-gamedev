using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class EquipmentInteractionUI : MonoBehaviour
{
    [SerializeField] private GameObject interactionPanel;
    [SerializeField] private TMP_Text equipmentNameText;
    [SerializeField] private Transform componentsContainer;
    [SerializeField] private ComponentSlotUI componentSlotPrefab;
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private Transform inventoryItemsContainer;
    [SerializeField] private ComponentSlotUI inventoryItemPrefab;
    [SerializeField] private Button extractButton;
    [SerializeField] private Button installButton;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerInventory playerInventory;
    private EquipmentInteraction currentEquipment;
    public bool IsOpen { get; private set; }
    private ComponentSlotUI selectedSlot;
    private ComponentState selectedInventoryComponent;
    private void Awake()
    {
        interactionPanel.SetActive(false);
        extractButton.onClick.AddListener(ExtractSelectedComponent);
        installButton.onClick.AddListener(ShowInventory);
    }

    public void Show(EquipmentInteraction equipment)
    {
        if (equipment == null)
            return;

        currentEquipment = equipment;

        equipmentNameText.text = equipment.DisplayName;
        CreateComponentSlots(equipment);

        interactionPanel.SetActive(true);
        IsOpen = true;
        playerMovement?.StopMovement();
    }


    public void Close()
    {
        interactionPanel.SetActive(false);
        inventoryPanel.SetActive(false);

        IsOpen = false;
        selectedSlot = null;
        selectedInventoryComponent = null;
        currentEquipment = null;
    }


    private void CreateComponentSlots(EquipmentInteraction equipment)
    {
        Debug.Log("CreateComponentSlots вызван!");
        foreach (Transform child in componentsContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (ComponentSlotDefinition slot in equipment.Definition.Slots)
        {
            if (!equipment.State.TryGetComponent(slot.Id, out ComponentState componentState))
            {
                Debug.LogError($"Не найден компонент для слота {slot.Id}.");
                continue;
            }

            ComponentSlotUI slotUI = Instantiate(
                componentSlotPrefab,
                componentsContainer
            );

            slotUI.Initialize(slot, componentState, HandleSlotSelected);
        }
    }

    private void HandleSlotSelected(ComponentSlotUI clickedSlot)
    {
        if (selectedSlot != null)
            selectedSlot.SetSelected(false);

        selectedSlot = clickedSlot;

        selectedSlot.SetSelected(true);

        Debug.Log(
            $"Выбран слот: {selectedSlot.SlotDefinition.DisplayName}"
        );
    }
    private void HandleInventoryItemSelected(ComponentSlotUI item)
    {
        if (item == null)
            return;

        selectedInventoryComponent = item.ComponentState;
        InstallSelectedComponent(selectedInventoryComponent);
    }

    public void InstallSelectedComponent(ComponentState component)
    {
        if (currentEquipment == null ||
            playerInventory == null ||
            selectedSlot == null ||
            component == null)
        {
            return;
        }

        if (!playerInventory.TryRemoveComponent(component))
            return;

        if (!currentEquipment.State.TryInstallComponent(
            selectedSlot.SlotDefinition.Id,
            component))
        {
            playerInventory.TryAddComponent(component);
            Debug.LogWarning("Не удалось установить компонент.");
            return;
        }

        selectedSlot.SetComponentState(component);

        inventoryPanel.SetActive(false);
        selectedInventoryComponent = null;

        Debug.Log($"Компонент {component.Definition.DisplayName} установлен.");
    }


    public void ExtractSelectedComponent()
    {
        if (currentEquipment == null ||
            playerInventory == null ||
            selectedSlot == null)
        {
            return;
        }

        if (selectedSlot.ComponentState == null)
        {
            Debug.Log("Выбранный слот пуст.");
            return;
        }

        string slotId = selectedSlot.SlotDefinition.Id;

        if (!currentEquipment.State.TryExtractComponent(
            slotId, out ComponentState component))
        {
            Debug.LogWarning("Не удалось извлечь компонент.");
            return;
        }

        if (!playerInventory.TryAddComponent(component))
        {
            currentEquipment.State.TryInstallComponent(slotId, component);

            Debug.LogWarning("Не удалось добавить компонент в инвентарь.");
            return;
        }

        selectedSlot.SetComponentState(null);

        Debug.Log($"Компонент {component.Definition.DisplayName} извлечён.");
    }
    public void ShowInventory()
    {
        if (currentEquipment == null ||
            playerInventory == null ||
            selectedSlot == null)
        {
            return;
        }

        string requiredComponentId =
            selectedSlot.SlotDefinition.Component.Id;

        foreach (Transform child in inventoryItemsContainer)
            Destroy(child.gameObject);

        selectedInventoryComponent = null;

        foreach (ComponentState component in playerInventory.Components)
        {
            if (component == null ||
                component.Definition == null ||
                component.Definition.Id != requiredComponentId)
            {
                continue;
            }

            ComponentSlotUI item = Instantiate(
                inventoryItemPrefab,
                inventoryItemsContainer
            );

            item.InitializeInventoryItem(
                component,
                HandleInventoryItemSelected
            );
        }

        inventoryPanel.SetActive(true);
    }
}
