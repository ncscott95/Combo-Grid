using UnityEngine;

public class AbilityGridUIManager : MonoBehaviour
{
    private AbilityGridUICell[,] _uiCellGrid;
    private bool _isInitialized = false;

    void OnEnable()
    {
        if (!_isInitialized)
        {
            InitializeGrid();
            _isInitialized = true;
        }
        UpdateGridUI();
    }

    private void InitializeGrid()
    {
        AbilityGridUICell[] _uiCells = GetComponentsInChildren<AbilityGridUICell>();

        int width = AbilityGridManager.Instance.GridWidth;
        int height = AbilityGridManager.Instance.GridHeight;
        _uiCellGrid = new AbilityGridUICell[width, height];

        if (_uiCells.Length != width * height)
        {
            Debug.LogWarning("UI Cell count does not match grid dimensions.");
        }

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                _uiCellGrid[x, y] = _uiCells[x + y * width];
                AbilityGridManager.Instance.CellGrid[x, y].SetUICell(_uiCellGrid[x, y]);
            }
        }
    }

    private void UpdateGridUI()
    {
        for (int y = 0; y < _uiCellGrid.GetLength(1); y++)
        {
            for (int x = 0; x < _uiCellGrid.GetLength(0); x++)
            {
                _uiCellGrid[x, y].UpdateUICell(AbilityGridManager.Instance.CellGrid[x, y]);
            }
        }
    }
}
