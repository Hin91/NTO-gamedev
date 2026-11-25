
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ComponentSlotUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text componentNameText;
    [SerializeField] private TMP_Text componentStatusText;
    [SerializeField] private GameObject selectionHighlight;

    private ComponentSlotDefinition slotDefinition;
    private ComponentState componentState;
    private Action<ComponentSlotUI> onSelected;
    private bool isInventoryItem;
    public ComponentSlotDefinition SlotDefinition => slotDefinition;
    public ComponentState ComponentState => componentState;

    private void Awake()
    {
        if (button == null)
            button = GetComponent<Button>();

        if (selectionHighlight != null)
            selectionHighlight.SetActive(false);
    }

    public void Initialize(
        ComponentSlotDefinition definition,
        ComponentState state,
        Action<ComponentSlotUI> selectionCallback)
    {
        if (definition == null)
        {
            Debug.LogError("Не удалось инициализировать ComponentSlotUI.");
            return;
        }

        isInventoryItem = false;
        slotDefinition = definition;
        componentState = state;
        onSelected = selectionCallback;

        componentNameText.text = definition.DisplayName;

        UpdateStatus();

        if (button != null)
        {
            button.onClick.RemoveListener(HandleClick);
            button.onClick.AddListener(HandleClick);
        }
        else
        {
            Debug.LogError("ComponentSlotUI: не назначен Button.");
        }
    }

    public void InitializeInventoryItem(
    ComponentState state,
    Action<ComponentSlotUI> selectionCallback)
    {
        if (state == null)
        {
            Debug.LogError("Не удалось инициализировать ячейку инвентаря.");
            return;
        }
        isInventoryItem = true;
        componentState = state;
        onSelected = selectionCallback;

        componentNameText.text = state.Definition.DisplayName;
        UpdateStatus();

        if (button != null)
        {
            button.onClick.RemoveListener(HandleClick);
            button.onClick.AddListener(HandleClick);
        }
    }

    private void HandleClick()
    {
        onSelected?.Invoke(this);
    }

    public void SetSelected(bool selected)
    {
        if (selectionHighlight != null)
            selectionHighlight.SetActive(selected);
    }

    public void SetComponentState(ComponentState state)
    {
        componentState = state;
        UpdateStatus();
    }


    public void UpdateStatus()
    {
        if (componentState == null)
        {
            componentStatusText.text = "Пусто";
            return;
        }

        if (!isInventoryItem && !componentState.IsInstalled)
        {
            componentStatusText.text = "Пусто";
            return;
        }

        componentStatusText.text = componentState.IsBroken
            ? "Сломан"
            : "Исправен";
    }

}
