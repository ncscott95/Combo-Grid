using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class AbilityGridUICell : MonoBehaviour
{
    [SerializeField] private List<Image> _actionImages = new() { null, null, null, null }; // Up, Left, Down, Right

    public Image CellImage { get; private set; }
    private AbilityGridUIManager _manager;

    private void Awake()
    {
        CellImage = GetComponent<Image>();
        CellImage.color = new(0.5f, 0.5f, 0.5f, 0.25f);
    }

    public void SetManager(AbilityGridUIManager manager) { _manager = manager; }

    public void UpdateUICell(AbilityGridCell linkedCell)
    {
        if (linkedCell != null)
        {
            CellImage.sprite = linkedCell.Skill.Icon;

            SetTransitionVisuals(linkedCell.HasUpAction, linkedCell, 0);
            SetTransitionVisuals(linkedCell.HasLeftAction, linkedCell, 1);
            SetTransitionVisuals(linkedCell.HasDownAction, linkedCell, 2);
            SetTransitionVisuals(linkedCell.HasRightAction, linkedCell, 3);
        }
        else
        {
            CellImage.sprite = null;
        }
    }

    public void EnterCell()
    {
        CellImage.color = Color.yellow;
    }

    public void ExitCell()
    {
        CellImage.color = new(0.5f, 0.5f, 0.5f, 0.25f);
    }

    public void IdleCell()
    {
        CellImage.color = Color.green;
    }

    public void RotateCell(bool clockwise)
    {
        float rotationAngle = clockwise ? -90f : 90f;
        CellImage.transform.Rotate(0, 0, rotationAngle);
        _actionImages = ListRotator.RotateList(_actionImages, clockwise, 1);

        // TODO: allow individual rotations when the player is editing the grid
        // _manager.UpdateGridUI();
    }

    private void SetTransitionVisuals(bool active, AbilityGridCell linkedCell, int index)
    {
        if (!active)
        {
            _actionImages[index].gameObject.SetActive(false);
            return;
        }

        var transition = linkedCell.Transitions[index];
        _actionImages[index].sprite = transition != null ? transition.Icon : null;
        _actionImages[index].color = transition != null ? transition.Color : Color.clear;
        _actionImages[index].gameObject.SetActive(true);
    }
}
