
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Tilemap floorTilemap;
    [SerializeField] private Tilemap wallsTilemap;
    [SerializeField] private Rigidbody2D rb;

    [Header("Obstacles")]
    [SerializeField] private LayerMask obstacleLayers;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;

    private GridPathfinding pathfinding;

    private List<Vector3Int> currentPath;
    private int currentPathIndex;

    private EquipmentInteraction pendingInteraction;

    private void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        if (mainCamera == null ||
            floorTilemap == null ||
            wallsTilemap == null ||
            rb == null)
        {
            Debug.LogError(
                "PlayerMovement: не назначены необходимые компоненты."
            );

            enabled = false;
            return;
        }

        pathfinding = new GridPathfinding(
            floorTilemap,
            wallsTilemap,
            obstacleLayers
        );
    }

    private void Update()
    {
        if (Mouse.current == null ||
            !Mouse.current.leftButton.wasPressedThisFrame)
            return;

        Vector3 screenPosition =
            Mouse.current.position.ReadValue();

        screenPosition.z =
            -mainCamera.transform.position.z;

        Vector3 worldPosition =
            mainCamera.ScreenToWorldPoint(screenPosition);

        HandleClick(worldPosition);
    }

    private void HandleClick(Vector3 worldPosition)
    {
        Collider2D hit = Physics2D.OverlapPoint(
            worldPosition,
            obstacleLayers
        );

        if (hit != null)
        {
            EquipmentInteraction equipment =
                hit.GetComponentInParent<EquipmentInteraction>();

            if (equipment != null)
            {
                MoveToEquipment(equipment);
            }

            return;
        }

        Vector3Int targetCell =
            floorTilemap.WorldToCell(worldPosition);

        if (!pathfinding.IsWalkable(targetCell))
            return;

        pendingInteraction = null;

        MoveToCell(targetCell);
    }

    private void MoveToCell(Vector3Int targetCell)
    {
        Vector3Int startCell =
            floorTilemap.WorldToCell(transform.position);

        currentPath =
            pathfinding.FindPath(startCell, targetCell);

        currentPathIndex = 0;

        if (currentPath == null)
        {
            Debug.Log("Путь до цели не найден.");
        }
    }

    private void MoveToEquipment(
    EquipmentInteraction equipment)
    {
        currentPath = null;
        pendingInteraction = equipment;

        Collider2D equipmentCollider =
            equipment.GetComponentInChildren<Collider2D>();

        if (equipmentCollider == null)
        {
            Debug.LogError(
                "У оборудования отсутствует Collider2D."
            );

            pendingInteraction = null;
            return;
        }

        Vector3Int startCell =
            floorTilemap.WorldToCell(transform.position);

        Bounds equipmentBounds =
            equipmentCollider.bounds;

        currentPath =
            pathfinding.FindPathToAdjacentCell(
                startCell,
                equipmentBounds
            );

        currentPathIndex = 0;

        if (currentPath == null)
        {
            pendingInteraction = null;
            Debug.Log("Нет доступного пути к оборудованию.");
        }
        else if (currentPath.Count == 0)
        {
            OpenPendingInteraction();
        }
    }

    private bool IsAdjacent(
        Vector3Int playerCell,
        Vector3Int equipmentCell)
    {
        int distance =
            Mathf.Abs(playerCell.x - equipmentCell.x) +
            Mathf.Abs(playerCell.y - equipmentCell.y);

        return distance == 1;
    }

    private void FixedUpdate()
    {
        if (currentPath == null)
            return;

        Vector3 targetWorldPosition =
            floorTilemap.GetCellCenterWorld(
                currentPath[currentPathIndex]
            );

        Vector2 targetPosition =
            new Vector2(
                targetWorldPosition.x,
                targetWorldPosition.y
            );

        Vector2 nextPosition = Vector2.MoveTowards(
            rb.position,
            targetPosition,
            moveSpeed * Time.fixedDeltaTime
        );

        rb.MovePosition(nextPosition);

        if ((nextPosition - targetPosition).sqrMagnitude
            < 0.000001f)
        {
            currentPathIndex++;

            if (currentPathIndex >= currentPath.Count)
            {
                currentPath = null;
                OpenPendingInteraction();
            }
        }
    }

    private void OpenPendingInteraction()
    {
        if (pendingInteraction == null)
            return;

        EquipmentInteraction equipment =
            pendingInteraction;

        pendingInteraction = null;

        equipment.OpenInteraction();
    }
}
