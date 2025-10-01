using System.Collections.Generic;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "NewAttribute", menuName = "StatSystem/Attribute")]
public class Attribute : ScriptableObject
{
    public string AttributeName;
    public Sprite Icon;
    public Color Color;
    public float MaxValue;
    public float RegenDelay;
    public float RegenRate;
    public float CurrentValue { get; protected set; }
    private Coroutine _regenCoroutine;

    private List<AttributeUI> _uiElements = new();

    public void Initialize()
    {
        CurrentValue = MaxValue;
        UpdateUIElements();
    }

    public void ModifyCurrentValue(float amount, bool triggerRegen)
    {
        CurrentValue = Mathf.Clamp(CurrentValue + amount, 0, MaxValue);
        if (RegenRate > 0 && triggerRegen && CurrentValue < MaxValue)
        {
            StartRegenCoroutine();
        }
        UpdateUIElements();
    }

    public void StartRegen()
    {
        if (RegenRate > 0 && CurrentValue < MaxValue)
        {
            StartRegenCoroutine();
        }
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

    private void StartRegenCoroutine()
    {
        if (_regenCoroutine != null)
        {
            CoroutineRunner.Instance.StopManagedCoroutine(_regenCoroutine);
        }

        _regenCoroutine = CoroutineRunner.Instance.StartManagedCoroutine(RegenCoroutine());
    }

    private IEnumerator RegenCoroutine()
    {
        yield return new WaitForSeconds(RegenDelay);

        while (CurrentValue < MaxValue)
        {
            yield return new WaitForEndOfFrame();
            ModifyCurrentValue(RegenRate * Time.deltaTime, false);
        }

        _regenCoroutine = null;
    }
}
