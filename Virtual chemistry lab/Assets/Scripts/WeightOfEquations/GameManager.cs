using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject gamePanel;
    public GameObject resultPanel;

    [Header("Game UI")]
    public TextMeshProUGUI questionTitleText;
    public TextMeshProUGUI questionCounterText;
    public TextMeshProUGUI feedbackText;
    public CanvasGroup feedbackPanel;
    public Button checkButton;
    public Button nextButton;

    [Header("Result UI")]
    public TextMeshProUGUI resultTitleText;
    public TextMeshProUGUI resultSubText;
    public Transform starContainer;
    public GameObject starPrefab;
    public Button playAgainButton;
    public Button menuButton;

    [Header("Sub-systems")]
    public EquationUIBuilder equationBuilder;
    public ScaleVisualizer scaleVisualizer;

    private List<EquationQuestion> questions;
    private int currentIndex = 0;
    private int correctCount = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        DOTween.Init(false, true, LogBehaviour.ErrorsOnly);
    }

    void Start() => ShowMainMenu();
    public void ShowMainMenu()
    {
        SetPanel(mainMenuPanel);
    }

    public void StartGame()
    {
        questions = EquationDatabase.GetAllQuestions();
        currentIndex = 0;
        correctCount = 0;

        SetPanel(gamePanel);
        LoadQuestion(currentIndex);
    }


    void LoadQuestion(int index)
    {
        if (index >= questions.Count) { ShowResults(); return; }

        var q = questions[index];

        questionTitleText.text = q.title;
        questionCounterText.text = $"{index + 1} / {questions.Count}";

        feedbackPanel.alpha = 0f;
        feedbackPanel.interactable = false;
        checkButton.interactable = true;
        nextButton.gameObject.SetActive(false);

        equationBuilder.BuildEquation(q);
        scaleVisualizer.SetupScale(q);

        gamePanel.transform.localPosition = new Vector3(400f, 0f, 0f);
        gamePanel.transform.DOLocalMoveX(0f, 0.35f).SetEase(Ease.OutCubic);
    }

    public void CheckAnswer()
    {
        bool correct = equationBuilder.ValidateAnswers();

        if (correct)
        {
            correctCount++;
            ShowFeedback(true);
            scaleVisualizer.PlayBalancedAnimation();
            checkButton.interactable = false;
            nextButton.gameObject.SetActive(true);

            nextButton.transform.localScale = Vector3.zero;
            nextButton.transform.DOScale(1f, 0.4f).SetDelay(0.3f).SetEase(Ease.OutBack);
        }
        else
        {
            ShowFeedback(false);
            scaleVisualizer.PlayUnbalancedAnimation();

            equationBuilder.transform
                .DOShakePosition(0.35f, new Vector3(10f, 0f, 0f), 18)
                .SetEase(Ease.OutCubic);
        }
    }

    void ShowFeedback(bool correct)
    {
        feedbackText.text = correct ? "Correct!" : "Try Again";
        feedbackText.color = correct
            ? new Color(0.15f, 0.78f, 0.42f)
            : new Color(0.90f, 0.22f, 0.22f);

        feedbackPanel.DOKill();
        feedbackPanel.alpha = 1f;
        feedbackPanel.interactable = true;

        if (!correct)
        {
            DOVirtual.DelayedCall(1.5f, () =>
                feedbackPanel.DOFade(0f, 0.4f)
                    .OnComplete(() => feedbackPanel.interactable = false));
        }
    }

    public void GoToNextQuestion()
    {
        currentIndex++;
        LoadQuestion(currentIndex);
    }


    void ShowResults()
    {
        SetPanel(resultPanel);

        bool perfect = correctCount == questions.Count;
        resultTitleText.text = perfect ? "Well Done!" : "Keep Practicing!";
        resultSubText.text = $"You balanced {correctCount} out of {questions.Count} equations";

        int stars = correctCount >= 5 ? 3 : correctCount >= 3 ? 2 : correctCount >= 1 ? 1 : 0;
        foreach (Transform child in starContainer) Destroy(child.gameObject);
        for (int i = 0; i < 3; i++)
        {
            int idx = i;
            var star = Instantiate(starPrefab, starContainer);
            star.GetComponent<Image>().color = idx < stars
                ? new Color(1f, 0.82f, 0f)
                : new Color(0.75f, 0.75f, 0.75f);
            star.transform.localScale = Vector3.zero;
            star.transform.DOScale(1f, 0.45f)
                .SetDelay(0.2f + idx * 0.18f)
                .SetEase(Ease.OutBack);
        }

        // Animate result card
        resultPanel.transform.localScale = Vector3.one * 0.85f;
        resultPanel.transform.DOScale(1f, 0.4f).SetEase(Ease.OutBack);
    }

    public void RestartGame() => StartGame();

    void SetPanel(GameObject target)
    {
        mainMenuPanel.SetActive(target == mainMenuPanel);
        gamePanel.SetActive(target == gamePanel);
        resultPanel.SetActive(target == resultPanel);
    }

    public EquationQuestion CurrentQuestion => questions?[currentIndex];
}