using System.Collections;
using UnityEngine;
using DG.Tweening;

public class AtomParticleEffect : MonoBehaviour
{
    [Header("Particle Settings")]
    public GameObject atomBubblePrefab;
    public int particleCount = 12;
    public float spawnRadius = 150f;
    public float flyHeight = 400f;
    public float duration = 1.2f;

    [Header("Colors")]
    public Color[] elementColors = new Color[]
    {
        new Color(0.3f, 0.6f, 1f),
        new Color(0.9f, 0.9f, 0.9f),
        new Color(1f,   0.4f, 0.2f),
        new Color(0.4f, 0.9f, 0.4f),
    };

    public void PlayCorrectEffect(Vector2 screenCenter)
    {
        StartCoroutine(SpawnParticles(screenCenter));
    }

    IEnumerator SpawnParticles(Vector2 center)
    {
        string[] symbols = { "N", "H", "O", "C", "Fe", "Al", "Cl" };

        for (int i = 0; i < particleCount; i++)
        {
            var go = Instantiate(atomBubblePrefab, transform);
            var rt = go.GetComponent<RectTransform>();
            var img = go.GetComponent<UnityEngine.UI.Image>();
            var txt = go.GetComponentInChildren<TMPro.TextMeshProUGUI>();

            Vector2 startPos = center + Random.insideUnitCircle * spawnRadius;
            rt.anchoredPosition = startPos;
            rt.localScale = Vector3.zero;

            int colorIdx = Random.Range(0, elementColors.Length);
            img.color = elementColors[colorIdx];
            if (txt) txt.text = symbols[Random.Range(0, symbols.Length)];

            var seq = DOTween.Sequence();
            seq.Append(rt.DOScale(1f, 0.15f).SetEase(Ease.OutBack));
            seq.Join(rt.DOAnchorPosY(startPos.y + flyHeight + Random.Range(-80f, 80f), duration)
                       .SetEase(Ease.OutCubic));
            seq.Join(rt.DOAnchorPosX(startPos.x + Random.Range(-100f, 100f), duration));
            seq.Join(img.DOFade(0f, duration).SetDelay(duration * 0.5f));
            seq.OnComplete(() => Destroy(go));

            yield return new WaitForSeconds(0.05f);
        }
    }
}