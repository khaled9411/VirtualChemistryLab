using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScaleVisualizer : MonoBehaviour
{
    public static ScaleVisualizer Instance { get; private set; }

    [System.Serializable]
    public class ElementScale
    {
        public string elementSymbol;
        public Transform beam;
        public Transform leftPan;
        public Transform rightPan;
        public TextMeshProUGUI leftLabel;
        public TextMeshProUGUI rightLabel;
        public GameObject balancedGlow;
    }

    [Header("Scales")]
    public List<ElementScale> scales;

    [Header("Animation")]
    [Range(5f, 30f)] public float maxTilt = 18f;
    [Range(0.2f, 1f)] public float tiltTime = 0.5f;
    [Range(5f, 40f)] public float panOffset = 22f;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void SetupScale(EquationQuestion q)
    {
        var usedElements = new HashSet<string>();
        foreach (var mol in q.molecules)
            foreach (var el in mol.elements)
                usedElements.Add(el.symbol);

        foreach (var s in scales)
        {
            bool show = usedElements.Contains(s.elementSymbol);
            s.beam.gameObject.SetActive(show);

            if (show)
            {
                s.beam.DOLocalRotate(new Vector3(s.beam.eulerAngles.x, s.beam.eulerAngles.y, 0f), tiltTime * 0.5f).SetEase(Ease.OutElastic);
                ResetPans(s);
                if (s.leftLabel) s.leftLabel.text = "0";
                if (s.rightLabel) s.rightLabel.text = "0";
                if (s.balancedGlow) s.balancedGlow.SetActive(false);
            }
        }
    }

    public void UpdateScales(Dictionary<string, int> reactant,
                             Dictionary<string, int> product)
    {
        foreach (var s in scales)
        {
            if (!s.beam.gameObject.activeSelf) continue;

            int left = reactant.ContainsKey(s.elementSymbol) ? reactant[s.elementSymbol] : 0;
            int right = product.ContainsKey(s.elementSymbol) ? product[s.elementSymbol] : 0;

            if (s.leftLabel) s.leftLabel.text = left.ToString();
            if (s.rightLabel) s.rightLabel.text = right.ToString();

            float tiltZ = 0f;
            if (left != right && (left + right) > 0)
            {
                float diff = right - left;
                float norm = Mathf.Max(left, right);
                tiltZ = -(diff / norm) * maxTilt;
            }

            s.beam.DOLocalRotate(new Vector3(s.beam.eulerAngles.x, s.beam.eulerAngles.y, tiltZ), tiltTime).SetEase(Ease.OutElastic);

            float leftY = tiltZ * (panOffset / maxTilt);
            float rightY = -tiltZ * (panOffset / maxTilt);
            if (s.leftPan) s.leftPan.DOLocalMoveY(leftY, tiltTime).SetEase(Ease.OutElastic);
            if (s.rightPan) s.rightPan.DOLocalMoveY(rightY, tiltTime).SetEase(Ease.OutElastic);

            bool balanced = left == right && left > 0;
            if (s.balancedGlow) s.balancedGlow.SetActive(balanced);
        }
    }

    public void PlayBalancedAnimation()
    {
        foreach (var s in scales)
        {
            if (!s.beam.gameObject.activeSelf) continue;

            var seq = DOTween.Sequence();
            seq.Append(s.beam.DOLocalRotate(new Vector3(s.beam.eulerAngles.x, s.beam.eulerAngles.y, 0f), 0.35f).SetEase(Ease.OutBounce));
            ResetPans(s, 0.35f);
            seq.Append(s.beam.DOPunchRotation(new Vector3(0, 0, 6f), 0.55f, 6, 0.5f));
        }
    }

    public void PlayUnbalancedAnimation()
    {
        foreach (var s in scales)
        {
            if (!s.beam.gameObject.activeSelf) continue;
            s.beam.DOShakeRotation(0.4f, new Vector3(0, 0, 12f), 8, 45f);
        }
    }

    void ResetPans(ElementScale s, float dur = 0f)
    {
        if (s.leftPan)
        {
            if (dur > 0) s.leftPan.DOLocalMoveY(0f, dur);
            else s.leftPan.localPosition = Vector3.zero;
        }
        if (s.rightPan)
        {
            if (dur > 0) s.rightPan.DOLocalMoveY(0f, dur);
            else s.rightPan.localPosition = Vector3.zero;
        }
    }
}