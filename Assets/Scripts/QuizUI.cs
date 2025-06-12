using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class QuizUI : MonoBehaviour
{
    public QuizLoader quizLoader;

    public TMP_Text questionText;
    public Button[] answerButtons;
    public TMP_Text timerText;
    public TMP_Text questionCounterText;
    public TMP_Text livesText;
    public TMP_Text feedbackText;

    private int currentQuestionIndex = 0;
    private float currentTimer = 0f;
    private int lives = 3;
    private bool answered = false;
    private Coroutine timerCoroutine;

    private const float questionTime = 30f; // 30 segundos por pregunta

    void OnEnable()
    {
        QuizLoader.OnQuizDataLoaded += ShowQuestion;
    }

    void OnDisable()
    {
        QuizLoader.OnQuizDataLoaded -= ShowQuestion;
    }

    void Start()
    {
        if (quizLoader.quizData != null && quizLoader.quizData.questions.Count > 0)
        {
            currentQuestionIndex = 0;
            ShowQuestion();
        }

        UpdateLivesUI();
    }

    void ShowQuestion()
    {
        if (quizLoader.quizData == null || quizLoader.quizData.questions.Count == 0)
        {
            Debug.LogError("No hay preguntas cargadas.");
            return;
        }

        var question = quizLoader.quizData.questions[currentQuestionIndex];
        questionText.text = question.questionText;
        questionCounterText.text = $"PREGUNTA: {currentQuestionIndex + 1}/{quizLoader.quizData.questions.Count}";
        feedbackText.text = "";

        for (int i = 0; i < answerButtons.Length; i++)
        {
            if (i < question.textAnswers.Count)
            {
                answerButtons[i].gameObject.SetActive(true);
                TMP_Text btnText = answerButtons[i].GetComponentInChildren<TMP_Text>();
                if (btnText != null)
                {
                    btnText.text = question.textAnswers[i].answerValue;
                }

                int index = i;
                answerButtons[i].onClick.RemoveAllListeners();
                answerButtons[i].onClick.AddListener(() => OnAnswerSelected(index));
            }
            else
            {
                answerButtons[i].gameObject.SetActive(false);
            }
        }

        answered = false;
        currentTimer = questionTime;

        if (timerCoroutine != null)
            StopCoroutine(timerCoroutine);

        timerCoroutine = StartCoroutine(TimerCoroutine());
    }

    void OnAnswerSelected(int index)
    {
        if (answered) return;

        var question = quizLoader.quizData.questions[currentQuestionIndex];
        var selectedAnswer = question.textAnswers[index];

        if (selectedAnswer.isCorrect)
        {
            answered = true;
            feedbackText.text = "Pregunta correcta";

            if (currentQuestionIndex == quizLoader.quizData.questions.Count - 1)
            {
                // Última pregunta correcta
                if (timerCoroutine != null)
                    StopCoroutine(timerCoroutine);

                StartCoroutine(EndQuizWithDelay(1.5f));
            }
            else
            {
                StartCoroutine(NextQuestionAfterDelay(1.5f));
            }
        }
        else
        {
            lives--;
            UpdateLivesUI();

            if (lives <= 0)
            {
                CheckGameOver();
                return;
            }

            feedbackText.text = selectedAnswer.explanation;
            // Permitir seguir respondiendo (answered sigue false)
        }
    }

    IEnumerator TimerCoroutine()
    {
        while (currentTimer > 0f)
        {
            currentTimer -= Time.deltaTime;
            timerText.text = Mathf.Ceil(currentTimer).ToString();
            yield return null;
        }

        lives--;
        UpdateLivesUI();

        if (lives <= 0)
        {
            CheckGameOver();
            yield break;
        }

        feedbackText.text = "No respondiste a tiempo.";
        answered = true;

        yield return new WaitForSeconds(1.5f);

        currentQuestionIndex++;

        if (currentQuestionIndex < quizLoader.quizData.questions.Count)
        {
            ShowQuestion();
        }
        else
        {
            EndQuiz();
        }
    }

    IEnumerator NextQuestionAfterDelay(float delay)
    {
        if (delay > 0f)
            yield return new WaitForSeconds(delay);

        currentQuestionIndex++;

        if (currentQuestionIndex < quizLoader.quizData.questions.Count)
        {
            ShowQuestion();
        }
        else
        {
            EndQuiz();
        }
    }

    IEnumerator EndQuizWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        EndQuiz();
    }

    void UpdateLivesUI()
    {
        livesText.text = $"VIDAS: {lives}/3";
    }

    void CheckGameOver()
    {
        Debug.Log("Game Over");
        StopAllCoroutines();
        questionText.text = "GAME OVER";
        feedbackText.text = "";
        timerText.text = "";
        foreach (var btn in answerButtons)
            btn.gameObject.SetActive(false);
    }

    void EndQuiz()
    {
        Debug.Log("Fin del quiz");
        StopAllCoroutines();
        questionText.text = "Fin del quiz";
        feedbackText.text = "";
        timerText.text = "";
        foreach (var btn in answerButtons)
            btn.gameObject.SetActive(false);
    }
}
