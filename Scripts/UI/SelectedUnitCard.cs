using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectedUnitCard : MonoBehaviour
{
    public Image image;
    public TextMeshPro tmp;
    public UnitController unitController;
    public SelectedUnitsController suc;
    public void SetCard(Sprite sprite, string text, UnitController uc, SelectedUnitsController suc){
        image.sprite = sprite;
        tmp.SetText(text);
        unitController = uc;
        this.suc = suc;
    }
}
