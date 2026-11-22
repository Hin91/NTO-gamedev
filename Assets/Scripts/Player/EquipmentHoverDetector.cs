
using UnityEngine;
using UnityEngine.InputSystem;

public class EquipmentHoverDetector : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask interactableLayers;

    private EquipmentHighlight currentEquipment;

    private void Update()
    {
        if (Mouse.current == null)
            return;

        Vector3 screenPosition =
            Mouse.current.position.ReadValue();

        screenPosition.z =
            -mainCamera.transform.position.z;

        Vector3 worldPosition =
            mainCamera.ScreenToWorldPoint(screenPosition);

        Collider2D hit = Physics2D.OverlapPoint(
            worldPosition,
            interactableLayers
        );

        EquipmentHighlight newEquipment = null;

        if (hit != null)
        {
            newEquipment =
                hit.GetComponentInParent<EquipmentHighlight>();
        }

        if (newEquipment == currentEquipment)
            return;

        if (currentEquipment != null)
            currentEquipment.SetHighlighted(false);

        currentEquipment = newEquipment;

        if (currentEquipment != null)
            currentEquipment.SetHighlighted(true);
    }

    private void OnDisable()
    {
        if (currentEquipment != null)
            currentEquipment.SetHighlighted(false);

        currentEquipment = null;
    }
}
