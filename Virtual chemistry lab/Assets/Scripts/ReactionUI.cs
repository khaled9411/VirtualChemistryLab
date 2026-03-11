using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections;

public class ReactionUI : MonoBehaviour
{
    public RectTransform equationPanel;

    public TextMeshProUGUI text2Mg;
    public TextMeshProUGUI textPlus1;
    public TextMeshProUGUI textO2;
    public TextMeshProUGUI text2MgO;
    public TextMeshProUGUI textHeatLight;

    public Slider progressBar;
    public TextMeshProUGUI progressText;

    public TextMeshProUGUI temperatureText;
    public Image thermometerFill;
    public Color coldColor = Color.blue;
    public Color hotColor = new Color(1f, 0.3f, 0f);

    public RectTransform completionPanel;
    public TextMeshProUGUI completionText;

    public RectTransform mgInfoPanel;
    public RectTransform o2InfoPanel;

    public float equationAppearDuration = 0.8f;
    public float elementHighlightDuration = 0.5f;
    public float arrowAnimDuration = 1f;

    private Color reactantColor = new Color(0.2f, 0.6f, 1f);
    private Color productColor = new Color(1f, 0.5f, 0.1f);
    private Color arrowColor = Color.white;
    private Color highlightColor = Color.yellow;

    private bool equationVisible = false;
    private Sequence arrowSequence;

    void Awake()
    {
        HideAll();
    }

    public void ShowEquation()
    {
        if (equationVisible) return;
        equationVisible = true;

        StartCoroutine(AnimateEquationAppearance());
    }

    IEnumerator AnimateEquationAppearance()
    {
        if (equationPanel != null)
        {
            equationPanel.gameObject.SetActive(true);
            equationPanel.localScale = Vector3.zero;
            equationPanel.DOScale(Vector3.one, equationAppearDuration)
                .SetEase(Ease.OutBack);
        }

        yield return new WaitForSeconds(0.3f);

        yield return AnimateTextAppear(text2Mg, "2Mg", reactantColor, 0.4f);

        yield return AnimateTextAppear(textPlus1, "+", Color.white, 0.2f);

        yield return AnimateTextAppear(textO2, "O2", reactantColor, 0.4f);

        yield return new WaitForSeconds(0.3f);

        yield return new WaitForSeconds(0.3f);

        yield return AnimateTextAppear(text2MgO, "2MgO", productColor, 0.5f);

        yield return AnimateTextAppear(textHeatLight, "+ Heat + Light",
            new Color(1f, 0.9f, 0.2f), 0.4f);

        ShowProgressBar();
    }

    public void UpdateProgress(float progress)
    {
        if (progressBar != null)
        {
            progressBar.DOValue(progress, 0.2f);
        }

        if (progressText != null)
        {
            int percent = Mathf.RoundToInt(progress * 100f);
            progressText.text = $": {percent}%";
        }

        HighlightEquationStep(progress);
    }

    public void UpdateTemperature(float temperature, float maxTemp = 3100f)
    {
        if (temperatureText != null)
        {
            temperatureText.text = $"{temperature:F0} C";

            float t = temperature / maxTemp;
            temperatureText.color = Color.Lerp(coldColor, hotColor, t);
        }

        if (thermometerFill != null)
        {
            float fillAmount = temperature / maxTemp;
            thermometerFill.DOFillAmount(fillAmount, 0.3f);
            thermometerFill.DOColor(Color.Lerp(coldColor, hotColor, fillAmount), 0.3f);
        }
    }

    void HighlightEquationStep(float progress)
    {
        if (progress < 0.3f)
        {
            FlashText(text2Mg);
            FlashText(textO2);
        }
        else if (progress < 0.7f)
        {

        }
        else
        {
            FlashText(text2MgO);
            FlashText(textHeatLight);
        }
    }

    public void ShowCompletionMessage()
    {
        StartCoroutine(ShowCompletionSequence());
    }

    IEnumerator ShowCompletionSequence()
    {
        arrowSequence?.Kill();

        TextMeshProUGUI[] allTexts = { text2Mg, textPlus1, textO2, text2MgO, textHeatLight };
        foreach (var t in allTexts)
        {
            if (t != null)
                t.DOColor(new Color(1f, 0.85f, 0.2f), 0.5f);
        }

        yield return new WaitForSeconds(0.8f);

        if (completionPanel != null)
        {
            completionPanel.gameObject.SetActive(true);
            completionPanel.localScale = Vector3.zero;

            if (completionText != null)
            {
                completionText.text =
                    "!\n" +
                    "(MgO)\n" +
                    "";
                completionText.color = new Color(0.2f, 1f, 0.4f);
            }

            completionPanel.DOScale(Vector3.one, 0.6f).SetEase(Ease.OutElastic);

            completionPanel.DOScale(Vector3.one * 1.05f, 0.5f)
                .SetDelay(0.8f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
        }

        StartCoroutine(CelebrationEffect());
    }

    IEnumerator CelebrationEffect()
    {
        for (int i = 0; i < 5; i++)
        {
            yield return new WaitForSeconds(0.15f);
            if (equationPanel != null)
            {
                equationPanel.DOShakePosition(0.3f, 5f, 20);
            }
        }
    }

    IEnumerator AnimateTextAppear(TextMeshProUGUI textComp, string content,
        Color color, float duration)
    {
        if (textComp == null) yield break;

        textComp.text = content;
        textComp.color = new Color(color.r, color.g, color.b, 0f);
        textComp.gameObject.SetActive(true);

        textComp.DOFade(1f, duration * 0.5f);

        textComp.transform.localScale = Vector3.one * 0.5f;
        textComp.transform.DOScale(Vector3.one, duration * 0.6f)
            .SetEase(Ease.OutBack);

        textComp.DOColor(Color.white, 0.1f)
            .SetLoops(2, LoopType.Yoyo)
            .OnComplete(() => textComp.color = color);

        yield return new WaitForSeconds(duration);
    }

    void ShowProgressBar()
    {
        if (progressBar != null)
        {
            progressBar.gameObject.SetActive(true);
            progressBar.value = 0f;
            progressBar.transform.localScale = Vector3.zero;
            progressBar.transform.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack);
        }

        if (progressText != null)
        {
            progressText.gameObject.SetActive(true);
        }
    }

    void FlashText(TextMeshProUGUI textComp)
    {
        if (textComp == null) return;
        textComp.DOColor(highlightColor, 0.1f)
            .SetLoops(2, LoopType.Yoyo);
    }

    public void HideAll()
    {
        equationVisible = false;

        if (equationPanel != null) equationPanel.gameObject.SetActive(false);
        if (completionPanel != null) completionPanel.gameObject.SetActive(false);

        TextMeshProUGUI[] texts = { text2Mg, textPlus1, textO2,text2MgO, textHeatLight };
        foreach (var t in texts)
            if (t != null) t.gameObject.SetActive(false);

        if (progressBar != null) progressBar.gameObject.SetActive(false);
        if (progressText != null) progressText.gameObject.SetActive(false);
    }
}