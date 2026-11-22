
using UnityEngine;

public class EquipmentHighlight : MonoBehaviour
{
    [SerializeField] private SpriteRenderer mainRenderer;
    [SerializeField] private SpriteRenderer outlineRenderer;

    private void Awake()
    {
        if (mainRenderer == null)
            mainRenderer = GetComponent<SpriteRenderer>();

        outlineRenderer.sprite = mainRenderer.sprite;
        outlineRenderer.sortingLayerID =
            mainRenderer.sortingLayerID;
        outlineRenderer.sortingOrder =
            mainRenderer.sortingOrder - 1;

        outlineRenderer.enabled = false;
    }

    public void SetHighlighted(bool highlighted)
    {
        outlineRenderer.enabled = highlighted;
    }
}
