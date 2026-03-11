using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MoleculeSlot : MonoBehaviour
{
    [Header("References")]
    public Button upButton;
    public Button downButton;
    public TextMeshProUGUI coeffText;
    public TextMeshProUGUI formulaText;
    public Image background;

    public MoleculeData Molecule { get; private set; }
    public int CurrentCoeff { get; private set; } = 1;

    private EquationUIBuilder builder;
    private const int MIN = 1, MAX = 8;

    public void Setup(MoleculeData mol, EquationUIBuilder b)
    {
        Molecule = mol;
        builder = b;
        CurrentCoeff = 1;

        formulaText.text = mol.formula;
        coeffText.text = "1";

        // Reset background
        background.color = new Color(0.95f, 0.95f, 0.98f);

        upButton.onClick.RemoveAllListeners();
        downButton.onClick.RemoveAllListeners();
        upButton.onClick.AddListener(Increment);
        downButton.onClick.AddListener(Decrement);
    }

    void Increment()
    {
        if (CurrentCoeff >= MAX) return;
        CurrentCoeff++;
        AnimateAndRefresh(+1);
    }

    void Decrement()
    {
        if (CurrentCoeff <= MIN) return;
        CurrentCoeff--;
        AnimateAndRefresh(-1);
    }

    void AnimateAndRefresh(int dir)
    {
        coeffText.text = CurrentCoeff.ToString();

        //coeffText.transform.DOKill(true);
        //coeffText.transform.localScale = Vector3.one;
        //coeffText.transform.DOPunchScale(Vector3.one * 0.35f * dir, 0.18f, 5, 0.5f);

        background.DOColor(new Color(0.95f, 0.95f, 0.98f), 0.1f);

        builder.RefreshBalancePanel();
    }

    public void SetHighlight(Color c)
    {
        background.DOKill();
        background.DOColor(c, 0.25f);
        transform.DOPunchScale(Vector3.one * 0.08f, 0.25f, 4, 0.5f);
    }

    public void ForceCoeff(int val)
    {
        CurrentCoeff = Mathf.Clamp(val, MIN, MAX);
        coeffText.text = CurrentCoeff.ToString();
    }
}
