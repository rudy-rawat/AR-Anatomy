using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class QuizUI : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI questionText;
    public Button[] optionButtons;        // 4 buttons for A, B, C, D
    public TextMeshProUGUI[] optionTexts; // 4 texts for options
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI feedbackText;
    public Button nextButton;
    public Button closeButton;
    public Button cancelButton;
    public GameObject resultPanel;
    public TextMeshProUGUI resultText;

    private QuizQuestion[] selectedQuestions;
    private int currentQuestionIndex = 0;
    private int score = 0;
    private bool answered = false;

    private void Start()
    {
        if (nextButton != null)
        {
            nextButton.onClick.AddListener(NextQuestion);
            nextButton.gameObject.SetActive(false);
        }

        if (closeButton != null)
            closeButton.onClick.AddListener(ClosePanel);

        if (cancelButton != null)
            cancelButton.onClick.AddListener(ClosePanel);

        // Setup option buttons
        for (int i = 0; i < optionButtons.Length; i++)
        {
            int index = i; // Capture for closure
            optionButtons[i].onClick.AddListener(() => OnAnswerSelected(index));
        }

        if (resultPanel != null)
            resultPanel.SetActive(false);
    }

    public void StartQuiz(string organType)
    {
        Debug.Log($"StartQuiz called with organType: '{organType}'");

        if (string.IsNullOrEmpty(organType))
        {
            Debug.LogWarning("No organ type provided!");
            if (questionText != null)
                questionText.text = "No organ detected. Please scan an organ first.";
            return;
        }

        if (OrganRegistry.Instance == null)
        {
            Debug.LogError("OrganRegistry.Instance is null!");
            if (questionText != null)
                questionText.text = "Error: Organ Registry not found.";
            return;
        }

        OrganVariant variant = OrganRegistry.Instance.GetOrganVariant(organType);
        if (variant != null)
        {
            Debug.Log($"Found variant for {organType}");

            if (variant.quizQuestions == null || variant.quizQuestions.Length == 0)
            {
                Debug.LogWarning($"No quiz questions configured for: {organType}");
                if (questionText != null)
                    questionText.text = $"No quiz questions available for {variant.organName}.\n\nPlease add questions in OrganRegistry.";
                return;
            }

            Debug.Log($"Found {variant.quizQuestions.Length} questions");

            // Select 5 random questions from available questions
            selectedQuestions = SelectRandomQuestions(variant.quizQuestions, 5);
            currentQuestionIndex = 0;
            score = 0;

            if (resultPanel != null)
                resultPanel.SetActive(false);

            ShowQuestion();
        }
        else
        {
            Debug.LogWarning($"No organ variant found for: {organType}");
            if (questionText != null)
                questionText.text = $"Organ '{organType}' not found in registry.";
        }
    }

    private QuizQuestion[] SelectRandomQuestions(QuizQuestion[] allQuestions, int count)
    {
        // If we have fewer questions than requested, return all
        if (allQuestions.Length <= count)
            return allQuestions;

        // Randomly select questions
        List<QuizQuestion> shuffled = allQuestions.OrderBy(x => Random.value).ToList();
        return shuffled.Take(count).ToArray();
    }

    private void ShowQuestion()
    {
        if (currentQuestionIndex >= selectedQuestions.Length)
        {
            ShowResults();
            return;
        }

        answered = false;
        QuizQuestion question = selectedQuestions[currentQuestionIndex];

        // Display question
        if (questionText != null)
            questionText.text = $"Q{currentQuestionIndex + 1}: {question.question}";

        // Display options
        for (int i = 0; i < optionButtons.Length && i < question.options.Length; i++)
        {
            if (optionTexts[i] != null)
                optionTexts[i].text = question.options[i];

            if (optionButtons[i] != null)
            {
                optionButtons[i].interactable = true;
                optionButtons[i].GetComponent<Image>().color = Color.white;
            }
        }

        // Update score display
        if (scoreText != null)
            scoreText.text = $"Score: {score}/{selectedQuestions.Length}";

        // Clear feedback
        if (feedbackText != null)
            feedbackText.text = "";

        // Hide next button
        if (nextButton != null)
            nextButton.gameObject.SetActive(false);
    }

    private void OnAnswerSelected(int selectedIndex)
    {
        if (answered) return;

        answered = true;
        QuizQuestion question = selectedQuestions[currentQuestionIndex];
        bool correct = question.IsCorrect(selectedIndex);

        if (correct)
        {
            score++;
            if (feedbackText != null)
            {
                feedbackText.text = "✓ Correct!";
                feedbackText.color = Color.green;
            }

            // Highlight correct answer in green
            if (optionButtons[selectedIndex] != null)
                optionButtons[selectedIndex].GetComponent<Image>().color = Color.green;
        }
        else
        {
            if (feedbackText != null)
            {
                feedbackText.text = "✗ Wrong! Correct answer: " + question.options[question.correctAnswerIndex];
                feedbackText.color = Color.red;
            }

            // Highlight wrong answer in red
            if (optionButtons[selectedIndex] != null)
                optionButtons[selectedIndex].GetComponent<Image>().color = Color.red;

            // Show correct answer in green
            if (optionButtons[question.correctAnswerIndex] != null)
                optionButtons[question.correctAnswerIndex].GetComponent<Image>().color = Color.green;
        }

        // Disable all buttons
        foreach (var btn in optionButtons)
        {
            if (btn != null)
                btn.interactable = false;
        }

        // Update score
        if (scoreText != null)
            scoreText.text = $"Score: {score}/{selectedQuestions.Length}";

        // Show next button
        if (nextButton != null)
            nextButton.gameObject.SetActive(true);
    }

    private void NextQuestion()
    {
        currentQuestionIndex++;
        ShowQuestion();
    }

    private void ShowResults()
    {
        if (resultPanel != null)
        {
            resultPanel.SetActive(true);

            float percentage = (score / (float)selectedQuestions.Length) * 100;
            string grade = GetGrade(percentage);

            if (resultText != null)
            {
                resultText.text = $"Quiz Complete!\n\n" +
                                 $"Score: {score}/{selectedQuestions.Length}\n" +
                                 $"Percentage: {percentage:F0}%\n" +
                                 $"Grade: {grade}";
            }
        }

        // Hide question UI
        if (questionText != null)
            questionText.text = "";

        foreach (var btn in optionButtons)
        {
            if (btn != null)
                btn.gameObject.SetActive(false);
        }

        if (nextButton != null)
            nextButton.gameObject.SetActive(false);
    }

    private string GetGrade(float percentage)
    {
        if (percentage >= 90) return "A+ Excellent!";
        if (percentage >= 80) return "A Good Job!";
        if (percentage >= 70) return "B Well Done!";
        if (percentage >= 60) return "C Fair";
        if (percentage >= 50) return "D Pass";
        return "F Needs Improvement";
    }

    private void ClosePanel()
    {
        gameObject.SetActive(false);

        // Reset for next time
        if (resultPanel != null)
            resultPanel.SetActive(false);

        foreach (var btn in optionButtons)
        {
            if (btn != null)
                btn.gameObject.SetActive(true);
        }
    }
}