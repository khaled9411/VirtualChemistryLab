using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BalanceRow : MonoBehaviour
{
    public TextMeshProUGUI symbolText;
    public TextMeshProUGUI leftText;
    public TextMeshProUGUI rightText;
    public Image statusDot;

    public void Setup(string sym)
    {
        symbolText.text = sym;
        leftText.text = "0";
        rightText.text = "0";
        statusDot.color = new Color(0.75f, 0.75f, 0.75f);
    }

    public void UpdateCounts(int left, int right)
    {
        leftText.text = left.ToString();
        rightText.text = right.ToString();

        bool balanced = left == right && left > 0;
        Color target = balanced
            ? new Color(0.18f, 0.78f, 0.42f)
            : new Color(0.90f, 0.22f, 0.22f);

        statusDot.DOColor(target, 0.25f);

        if (balanced)
            statusDot.transform.DOPunchScale(Vector3.one * 0.5f, 0.3f, 5, 0.5f);
    }
}