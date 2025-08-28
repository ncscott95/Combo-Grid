using System.Collections;
using AbilitySystem;
using UnityEngine;

public class AbilityGridManager : Singleton<AbilityGridManager>
{
    [SerializeField] private AbilityGridCell _emptyCellPrefab;
    [SerializeField] private AbilityGridCell _debugCellPrefab;

    public int GridWidth { get; private set; } = 3;
    public int GridHeight { get; private set; } = 3;
    public AbilityGridCell[,] CellGrid { get; private set; }
    public AbilityGridCell CurrentCell { get; private set; }
    private bool _canAct = true;

    public override void Awake()
    {
        base.Awake();

        InitializeGrid();
    }

    void Start()
    {
        MoveCell(CellGrid[0, 0]);
    }

    public void MoveCell(AbilityGridCell newCell)
    {
        if (!_canAct) return;

        if (CurrentCell != null) CurrentCell.ExitCell();
        CurrentCell = newCell;
        CurrentCell.EnterCell();
        StartCoroutine(CooldownCoroutine(CurrentCell));
    }

    public void InitializeGrid()
    {
        CellGrid = new AbilityGridCell[GridWidth, GridHeight];
        for (int y = 0; y < GridHeight; y++)
        {
            for (int x = 0; x < GridWidth; x++)
            {
                // AbilityGridCell cell = Instantiate(_emptyCellPrefab);
                AbilityGridCell cell = Instantiate(_debugCellPrefab);
                CellGrid[x, y] = cell;
            }
        }

        // Set up neighbors and input actions as before
        for (int y = 0; y < GridHeight; y++)
        {
            for (int x = 0; x < GridWidth; x++)
            {
                AbilityGridCell cell = CellGrid[x, y];
                cell.GridPosition = new Vector2Int(x, y);

                // Remove previous bindings if any (assuming we have stored delegates, otherwise this is a no-op)
                cell.LeftAction.started -= ctx => MoveCell(null);
                cell.RightAction.started -= ctx => MoveCell(null);
                cell.UpAction.started -= ctx => MoveCell(null);
                cell.DownAction.started -= ctx => MoveCell(null);

                // Left neighbor
                if (x > 0)
                {
                    AbilityGridCell leftCell = CellGrid[x - 1, y];
                    cell.LeftAction.started += ctx => MoveCell(leftCell);
                }
                // Right neighbor
                if (x < GridWidth - 1)
                {
                    AbilityGridCell rightCell = CellGrid[x + 1, y];
                    cell.RightAction.started += ctx => MoveCell(rightCell);
                }
                // Up neighbor
                if (y < GridHeight - 1)
                {
                    AbilityGridCell upCell = CellGrid[x, y + 1];
                    cell.UpAction.started += ctx => MoveCell(upCell);
                }
                // Down neighbor
                if (y > 0)
                {
                    AbilityGridCell downCell = CellGrid[x, y - 1];
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
        CurrentCell.IdleCell();
    }
}
