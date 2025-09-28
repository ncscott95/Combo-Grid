using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewAttribute", menuName = "StatSystem/Attribute")]
public class Attribute : ScriptableObject
{
    public string AttributeName;
    public Sprite Icon;
    public Color Color;
    public float MaxValue;
    public float CurrentValue { get; protected set; }

    private List<AttributeUI> _uiElements = new();

    public void Initialize()
    {
        CurrentValue = MaxValue;
        UpdateUIElements();
    }

    public void ModifyCurrentValue(float amount)
    {
        CurrentValue = Mathf.Clamp(CurrentValue + amount, 0, MaxValue);
        UpdateUIElements();
    }

    public void TryRegisterUIElement(AttributeUI uiElement)
    {
        if (!_uiElements.Contains(uiElement))
        {
            _uiElements.Add(uiElement);
            uiElement.UpdateUI(CurrentValue / MaxValue);
        }
    }

    private void UpdateUIElements()
    {
        foreach (var uiElement in _uiElements)
        {
            uiElement.UpdateUI(CurrentValue / MaxValue);
        }
    }
}
