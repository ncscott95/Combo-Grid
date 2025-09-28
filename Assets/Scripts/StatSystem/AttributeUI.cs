using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AttributeUI : MonoBehaviour
{
    [SerializeField] private Image _fillImage;
    [SerializeField] private Image _iconImage;
    [SerializeField] private TMP_Text _valueText;
    private Attribute _attribute;

    public void SetAttribute(Attribute attribute)
    {
        _attribute = attribute;
        Initialize();
    }

    private void Initialize()
    {
        if (_attribute != null)
        {
            _attribute.TryRegisterUIElement(this);

            if (_fillImage != null)
            {
                _fillImage.color = _attribute.Color;
            }

            if (_iconImage != null && _attribute.Icon != null)
            {
                _iconImage.sprite = _attribute.Icon;
                _iconImage.color = _attribute.Color;
            }

            if (_valueText != null)
            {
                _valueText.color = _attribute.Color;
            }
        }
    }

    public void UpdateUI(float fillAmount)
    {
        if (_fillImage != null)
        {
            _fillImage.fillAmount = fillAmount;
        }

        if (_valueText != null)
        {
            _valueText.text = Math.Round(_attribute.CurrentValue, 2).ToString();
        }
    }
}
