using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class GridHoverHighlight : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Tilemap floorTilemap;
    [SerializeField] private Tilemap wallsTilemap;
    [SerializeField] private Tilemap highlightTilemap;
    [SerializeField] private TileBase highlightTile;

    private Vector3Int previousCell;
    private bool hasPreviousCell;

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

        Vector3Int cellPosition =
            floorTilemap.WorldToCell(worldPosition);

        if (hasPreviousCell &&
            cellPosition == previousCell)
            return;

        highlightTilemap.ClearAllTiles();

        previousCell = cellPosition;
        hasPreviousCell = true;

        bool hasFloor =
            floorTilemap.HasTile(cellPosition);

        bool hasWall =
            wallsTilemap.HasTile(cellPosition);

        if (hasFloor && !hasWall)
        {
            highlightTilemap.SetTile(
                cellPosition,
                highlightTile
            );
        }
    }

    private void OnDisable()
    {
        highlightTilemap.ClearAllTiles();
        hasPreviousCell = false;
    }
}
