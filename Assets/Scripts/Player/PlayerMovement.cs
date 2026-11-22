
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

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;

    private GridPathfinding pathfinding;

    private List<Vector3Int> currentPath;
    private int currentPathIndex;

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
            wallsTilemap
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

        Vector3Int targetCell =
            floorTilemap.WorldToCell(worldPosition);

        if (!pathfinding.IsWalkable(targetCell))
            return;

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

        if ((nextPosition - targetPosition).sqrMagnitude < 0.000001f)
        {
            currentPathIndex++;

            if (currentPathIndex >= currentPath.Count)
                currentPath = null;
        }
    }
}
