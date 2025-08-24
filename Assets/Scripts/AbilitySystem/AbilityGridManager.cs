using System.Collections;
using AbilitySystem;
using UnityEngine;

public class AbilityGridManager : MonoBehaviour
{
    [SerializeField] private GameObject _cellPrefab;
    [SerializeField] private int _gridWidth = 8;
    [SerializeField] private int _gridHeight = 8;
    private AbilityGridCell _currentCell;
    private AbilityGridCell[,] _cellGrid;
    private bool _canAct = true;

    void Start()
    {
        InitializeGrid();
        _currentCell = _cellGrid[0, 0];
        _currentCell.EnterCell();
    }

    public void MoveCell(AbilityGridCell newCell)
    {
        if (!_canAct) return;

        _currentCell.ExitCell();
        _currentCell = newCell;
        _currentCell.EnterCell();
        StartCoroutine(CooldownCoroutine(_currentCell));
    }

    public void InitializeGrid()
    {
        _cellGrid = new AbilityGridCell[_gridWidth, _gridHeight];
        for (int y = 0; y < _gridHeight; y++)
        {
            for (int x = 0; x < _gridWidth; x++)
            {
                AbilityGridCell cell = Instantiate(_cellPrefab, transform).GetComponent<AbilityGridCell>();
                cell.DEBUGName = $"Cell ({x},{y})";
                _cellGrid[x, y] = cell;
            }
        }

        // Set up neighbors and input actions as before
        for (int y = 0; y < _gridHeight; y++)
        {
            for (int x = 0; x < _gridWidth; x++)
            {
                AbilityGridCell cell = _cellGrid[x, y];
                cell.DEBUGName = $"Cell ({x},{y})";

                // Remove previous bindings if any (assuming we have stored delegates, otherwise this is a no-op)
                cell.LeftAction.started -= ctx => MoveCell(null);
                cell.RightAction.started -= ctx => MoveCell(null);
                cell.UpAction.started -= ctx => MoveCell(null);
                cell.DownAction.started -= ctx => MoveCell(null);

                // Left neighbor
                if (x > 0)
                {
                    AbilityGridCell leftCell = _cellGrid[x - 1, y];
                    cell.LeftAction.started += ctx => MoveCell(leftCell);
                }
                // Right neighbor
                if (x < _gridWidth - 1)
                {
                    AbilityGridCell rightCell = _cellGrid[x + 1, y];
                    cell.RightAction.started += ctx => MoveCell(rightCell);
                }
                // Up neighbor
                if (y < _gridHeight - 1)
                {
                    AbilityGridCell upCell = _cellGrid[x, y + 1];
                    cell.UpAction.started += ctx => MoveCell(upCell);
                }
                // Down neighbor
                if (y > 0)
                {
                    AbilityGridCell downCell = _cellGrid[x, y - 1];
                    cell.DownAction.started += ctx => MoveCell(downCell);
                }
            }
        }
    }

    private IEnumerator CooldownCoroutine(AbilityGridCell cell)
    {
        _canAct = false;
        float elapsed = 0f;
        while (elapsed < cell.Cooldown)
        {
            elapsed += Time.deltaTime;
            yield return elapsed / cell.Cooldown;
        }
        _canAct = true;
    }
}
