using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AttributeObject : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI NameText;
    public TextMeshProUGUI ValueText;
    public Button Button;

    [Header("Attribute")]
    public Attribute attribute;

    #region Initialization

    public void Init(Attribute attribute)
    {
        NameText.text = attribute.Name + ":";
        ValueText.text = attribute.Level.ToString();
        this.attribute = attribute;
        Button.onClick.AddListener(() => IncreaseAttribute());
        
        SkillManager.instance.Attributes.Add(attribute);
    }

    private void IncreaseAttribute()
    {
        attribute.LevelUp();
        UpdateText();
    }

    #endregion

    #region UpdateText

    public void UpdateText()
    {
        NameText.text = attribute.Name+":";
        ValueText.text = attribute.Level.ToString();

    }

    #endregion
}
