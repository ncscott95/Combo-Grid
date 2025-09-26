using System.Collections.Generic;
using AbilitySystem;
using UnityEngine;

public class AbilityGridManager : Singleton<AbilityGridManager>
{
    [SerializeField] private AbilityGridCell _emptyCellPrefab;
    [SerializeField] private List<AbilityGridCell> _debugCellPrefabs;

    public int GridWidth { get; private set; } = 3;
    public int GridHeight { get; private set; } = 3;
    public AbilityGridCell[,] CellGrid { get; private set; }
    public AbilityGridCell CurrentCell { get; private set; }

    public override void Awake()
    {
        base.Awake();

        InitializeGrid();
        List<int> test = new() { 0, 1, 2, 3 };
        test = ListRotator.RotateList(test, true, 1);
        Debug.Log(string.Join(", ", test));
    }

    void Start()
    {
        for (int y = 0; y < GridHeight; y++)
        {
            for (int x = 0; x < GridWidth; x++)
            {
                int rotations = Random.Range(0, 4);
                for (int i = 0; i < rotations; i++) CellGrid[x, y].RotateCell(true);
            }
        }
        MoveCell(CellGrid[0, 0]);
    }

    public void MoveCell(AbilityGridCell newCell)
    {
        if (!PlayerControllerBase.Instance.SkillSequencer.CanStartSkill) return;

        if (CurrentCell != null) CurrentCell.ExitCell();
        CurrentCell = newCell;
        CurrentCell.EnterCell();
    }

    public void InitializeGrid()
    {
        // Create grid and instantiate cells
        CellGrid = new AbilityGridCell[GridWidth, GridHeight];
        for (int y = 0; y < GridHeight; y++)
        {
            for (int x = 0; x < GridWidth; x++)
            {
                // AbilityGridCell cell = Instantiate(_emptyCellPrefab);
                AbilityGridCell cell = Instantiate(_debugCellPrefabs[Random.Range(0, _debugCellPrefabs.Count)]);
                CellGrid[x, y] = cell;
                cell.GridPosition = new Vector2Int(x, y);
            }
        }

        // Assign cell neighbors
        for (int y = 0; y < GridHeight; y++)
        {
            for (int x = 0; x < GridWidth; x++)
            {
                AbilityGridCell cell = CellGrid[x, y];
                cell.GridPosition = new Vector2Int(x, y);

                AbilityGridCell[] neighbors = new AbilityGridCell[4];

                if (y < GridHeight - 1) neighbors[0] = CellGrid[x, y + 1]; // Up neighbor
                if (x > 0) neighbors[1] = CellGrid[x - 1, y]; // Left neighbor
                if (y > 0) neighbors[2] = CellGrid[x, y - 1]; // Down neighbor
                if (x < GridWidth - 1) neighbors[3] = CellGrid[x + 1, y]; // Right neighbor

                cell.InitializeActions(neighbors);
            }
        }
    }

    // TODO: enter waiting state when combo finishes
    // private IEnumerator EndComboCheckCoroutine()
    // {
    //     while (PlayerControllerBase.Instance.SkillSequencer.CurrentPhase != SkillSequencer.SkillPhase.Inactive)
    //     {
    //         yield return null;
    //     }

    //     CurrentCell.ExitCell();
    //     CurrentCell = null;
    // }
}
