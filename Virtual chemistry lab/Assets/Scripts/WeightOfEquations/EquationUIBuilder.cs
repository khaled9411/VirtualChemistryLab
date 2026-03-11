using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class EquationUIBuilder : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject moleculeSlotPrefab;
    public GameObject separatorPlusPrefab;
    public GameObject elementRowPrefab;

    [Header("Containers")]
    public Transform reactantsContainer;
    public Transform productsContainer;
    public Transform balancePanelContainer;

    // Runtime
    private List<MoleculeSlot> slots = new List<MoleculeSlot>();
    private Dictionary<string, BalanceRow> balanceRows = new Dictionary<string, BalanceRow>();
    private EquationQuestion currentQ;

    public void BuildEquation(EquationQuestion q)
    {
        currentQ = q;
        slots.Clear();
        balanceRows.Clear();

        ClearContainer(reactantsContainer);
        ClearContainer(productsContainer);
        ClearContainer(balancePanelContainer);

        bool firstReactant = true, firstProduct = true;

        foreach (var mol in q.molecules)
        {
            Transform container = mol.isReactant ? reactantsContainer : productsContainer;
            bool isFirst = mol.isReactant ? firstReactant : firstProduct;

            if (!isFirst)
            {
                var plus = Instantiate(separatorPlusPrefab, container);
                plus.GetComponentInChildren<TextMeshProUGUI>().text = "+";
            }

            if (mol.isReactant) firstReactant = false;
            else firstProduct = false;

            var slotGO = Instantiate(moleculeSlotPrefab, container);
            var slot = slotGO.GetComponent<MoleculeSlot>();
            slot.Setup(mol, this);
            slots.Add(slot);

            int i = slots.Count;
            slotGO.transform.localScale = Vector3.zero;
            slotGO.transform.DOScale(1f, 0.3f)
                  .SetDelay(i * 0.07f)
                  .SetEase(Ease.OutBack);
        }

        BuildBalanceRows(q);
        RefreshBalancePanel();
    }

    void BuildBalanceRows(EquationQuestion q)
    {
        var elements = new List<string>();
        foreach (var mol in q.molecules)
            foreach (var el in mol.elements)
                if (!elements.Contains(el.symbol))
                    elements.Add(el.symbol);

        foreach (var sym in elements)
        {
            var rowGO = Instantiate(elementRowPrefab, balancePanelContainer);
            var row = rowGO.GetComponent<BalanceRow>();
            row.Setup(sym);
            balanceRows[sym] = row;
        }
    }

    public void RefreshBalancePanel()
    {
        if (currentQ == null) return;

        var left = new Dictionary<string, int>();
        var right = new Dictionary<string, int>();

        foreach (var slot in slots)
        {
            var target = slot.Molecule.isReactant ? left : right;
            foreach (var el in slot.Molecule.elements)
            {
                if (!target.ContainsKey(el.symbol)) target[el.symbol] = 0;
                target[el.symbol] += slot.CurrentCoeff * el.atomsPerMolecule;
            }
        }

        foreach (var kv in balanceRows)
        {
            int l = left.ContainsKey(kv.Key) ? left[kv.Key] : 0;
            int r = right.ContainsKey(kv.Key) ? right[kv.Key] : 0;
            kv.Value.UpdateCounts(l, r);
        }

        ScaleVisualizer.Instance?.UpdateScales(left, right);
    }

    public bool ValidateAnswers()
    {
        bool allOk = true;
        foreach (var slot in slots)
        {
            bool ok = slot.CurrentCoeff == slot.Molecule.correctCoefficient;
            slot.SetHighlight(ok
                ? new Color(0.18f, 0.78f, 0.42f)
                : new Color(0.90f, 0.22f, 0.22f));
            if (!ok) allOk = false;
        }
        return allOk;
    }

    static void ClearContainer(Transform t)
    {
        foreach (Transform child in t) Destroy(child.gameObject);
    }
}