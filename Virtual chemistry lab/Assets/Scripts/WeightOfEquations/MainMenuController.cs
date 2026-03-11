using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class MainMenuController : MonoBehaviour
{
    [Header("UI References")]
    public GameObject menuPanel;

    [Header("Title Animation")]
    public TextMeshProUGUI titleText;
    public CanvasGroup menuCanvasGroup;

    void Start()
    {
        menuCanvasGroup.alpha = 0f;
        menuCanvasGroup.DOFade(1f, 0.8f);

        if (titleText)
            titleText.transform.DOPunchScale(Vector3.one * 0.05f, 1f, 3, 0.5f)
                .SetLoops(-1, LoopType.Restart);
    }
}