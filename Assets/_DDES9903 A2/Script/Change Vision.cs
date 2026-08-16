using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class BookStudyQuiz : MonoBehaviour
{
    [Header("3D Study Panel Transform")]
    [SerializeField] private Vector3 panelPosition = new Vector3(0f, 2f, 0f);
    [SerializeField] private Vector3 panelRotation = new Vector3(0f, 0f, 0f);
    [SerializeField] private float panelScale = 0.0025f;

    private readonly string[] questions =
    {
        "1 + 2 = ?",
        "5 - 2 = ?",
        "3 + 4 = ?",
        "9 - 5 = ?",
        "2 + 6 = ?",
        "10 - 3 = ?",
        "4 + 5 = ?",
        "8 - 6 = ?",
        "1 + 8 = ?",
        "10 - 5 = ?"
    };

    private readonly int[] answers =
    {
        3,
        3,
        7,
        4,
        8,
        7,
        9,
        2,
        9,
        5
    };

    private GameObject studyCanvas;

    private TMP_Text titleText;
    private TMP_Text questionText;
    private TMP_Text resultText;
    private TMP_Text studyTimeText;
    private TMP_Text exitHintText;

    private TMP_InputField answerInput;
    private Button submitButton;

    private int currentQuestion = 0;
    private int studyMinutes = 0;

    private bool uiCreated = false;
    private bool quizFinished = false;
    private bool panelOpen = false;

    private void Update()
    {
        if (panelOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseStudyQuiz();
        }
    }

    // =========================================================
    // EZPZ On Primary Interact 选择这个方法
    // =========================================================
    public void OpenStudyQuiz()
    {
        if (!uiCreated)
        {
            CreateStudyUI();
            uiCreated = true;
            StartQuiz();
        }
        else
        {
            studyCanvas.SetActive(true);

            if (!quizFinished)
            {
                answerInput.ActivateInputField();
            }
        }

        panelOpen = true;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void CreateStudyUI()
    {
        // =====================================================
        // WORLD SPACE CANVAS
        // =====================================================

        studyCanvas = new GameObject("Study Quiz Panel");

        Canvas canvas = studyCanvas.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.sortingOrder = 100;

        RectTransform canvasRect =
            studyCanvas.GetComponent<RectTransform>();

        canvasRect.sizeDelta =
            new Vector2(800f, 700f);

        studyCanvas.transform.position =
            panelPosition;

        studyCanvas.transform.rotation =
            Quaternion.Euler(panelRotation);

        studyCanvas.transform.localScale =
            Vector3.one * panelScale;

        studyCanvas.AddComponent<GraphicRaycaster>();

        CreateEventSystemIfNeeded();

        // =====================================================
        // PANEL BACKGROUND
        // =====================================================

        GameObject panel = CreateImage(
            "Panel",
            studyCanvas.transform,
            new Color(0.95f, 0.95f, 0.95f, 1f)
        );

        RectTransform panelRect =
            panel.GetComponent<RectTransform>();

        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        // =====================================================
        // TITLE
        // =====================================================

        titleText = CreateText(
            "Title",
            panel.transform,
            "Study Time",
            48,
            new Vector2(0f, 275f),
            new Vector2(700f, 70f)
        );

        titleText.fontStyle = FontStyles.Bold;

        // =====================================================
        // QUESTION
        // =====================================================

        questionText = CreateText(
            "Question",
            panel.transform,
            "",
            48,
            new Vector2(0f, 150f),
            new Vector2(700f, 150f)
        );

        // =====================================================
        // INPUT FIELD
        // =====================================================

        answerInput = CreateInputField(
            panel.transform,
            new Vector2(0f, 30f)
        );

        answerInput.onSubmit.AddListener(
            delegate
            {
                SubmitAnswer();
            }
        );

        // =====================================================
        // SUBMIT BUTTON
        // =====================================================

        submitButton = CreateButton(
            "SubmitButton",
            panel.transform,
            "Submit",
            new Vector2(0f, -80f),
            new Vector2(260f, 70f)
        );

        submitButton.onClick.AddListener(SubmitAnswer);

        // =====================================================
        // RESULT
        // =====================================================

        resultText = CreateText(
            "ResultText",
            panel.transform,
            "",
            30,
            new Vector2(0f, -175f),
            new Vector2(700f, 70f)
        );

        // =====================================================
        // STUDY TIME
        // =====================================================

        studyTimeText = CreateText(
            "StudyTimeText",
            panel.transform,
            "Today's Study Time: 0 minutes",
            30,
            new Vector2(0f, -245f),
            new Vector2(700f, 60f)
        );

        studyTimeText.fontStyle = FontStyles.Bold;

        // =====================================================
        // ESC HINT
        // =====================================================

        exitHintText = CreateText(
            "ExitHintText",
            panel.transform,
            "Press ESC to exit",
            24,
            new Vector2(0f, -310f),
            new Vector2(700f, 40f)
        );

        exitHintText.color =
            new Color(0.35f, 0.35f, 0.35f, 1f);
    }

    private void StartQuiz()
    {
        currentQuestion = 0;
        studyMinutes = 0;
        quizFinished = false;

        titleText.text = "Study Time";
        resultText.text = "";

        answerInput.gameObject.SetActive(true);
        submitButton.gameObject.SetActive(true);

        UpdateStudyTime();
        ShowQuestion();
    }

    private void ShowQuestion()
    {
        questionText.text =
            $"Question {currentQuestion + 1} / {questions.Length}\n\n" +
            questions[currentQuestion];

        answerInput.text = "";
        answerInput.ActivateInputField();
    }

    public void SubmitAnswer()
    {
        if (quizFinished)
            return;

        if (string.IsNullOrWhiteSpace(answerInput.text))
        {
            resultText.text =
                "Please enter your answer.";

            answerInput.ActivateInputField();
            return;
        }

        int playerAnswer;

        if (!int.TryParse(answerInput.text, out playerAnswer))
        {
            resultText.text =
                "Please enter a number.";

            answerInput.text = "";
            answerInput.ActivateInputField();
            return;
        }

        if (playerAnswer == answers[currentQuestion])
        {
            studyMinutes += 30;
            currentQuestion++;

            UpdateStudyTime();

            if (currentQuestion >= questions.Length)
            {
                FinishQuiz();
                return;
            }

            resultText.text =
                "Correct! +30 minutes";

            ShowQuestion();
        }
        else
        {
            resultText.text =
                "Incorrect. Please try again.";

            answerInput.text = "";
            answerInput.ActivateInputField();
        }
    }

    private void UpdateStudyTime()
    {
        studyTimeText.text =
            $"Today's Study Time: {studyMinutes} minutes";
    }

    private void FinishQuiz()
    {
        quizFinished = true;

        titleText.text =
            "Study Complete!";

        questionText.text =
            "All 10 questions answered correctly!";

        resultText.text =
            "Great job!";

        studyTimeText.text =
            "Today's Study Time: 300 minutes";

        answerInput.gameObject.SetActive(false);
        submitButton.gameObject.SetActive(false);
    }

    public void CloseStudyQuiz()
    {
        if (studyCanvas != null)
        {
            studyCanvas.SetActive(false);
        }

        panelOpen = false;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // =========================================================
    // CREATE TEXT
    // =========================================================

    private TMP_Text CreateText(
        string objectName,
        Transform parent,
        string content,
        float fontSize,
        Vector2 position,
        Vector2 size)
    {
        GameObject obj =
            new GameObject(objectName);

        obj.transform.SetParent(parent, false);

        RectTransform rect =
            obj.AddComponent<RectTransform>();

        rect.sizeDelta = size;
        rect.anchoredPosition = position;

        TextMeshProUGUI text =
            obj.AddComponent<TextMeshProUGUI>();

        text.text = content;
        text.fontSize = fontSize;
        text.color = Color.black;
        text.alignment = TextAlignmentOptions.Center;

        return text;
    }

    // =========================================================
    // CREATE INPUT FIELD
    // =========================================================

    private TMP_InputField CreateInputField(
        Transform parent,
        Vector2 position)
    {
        GameObject inputObject = CreateImage(
            "AnswerInput",
            parent,
            Color.white
        );

        RectTransform inputRect =
            inputObject.GetComponent<RectTransform>();

        inputRect.sizeDelta =
            new Vector2(350f, 70f);

        inputRect.anchoredPosition =
            position;

        TMP_InputField input =
            inputObject.AddComponent<TMP_InputField>();

        input.contentType =
            TMP_InputField.ContentType.IntegerNumber;

        GameObject textArea =
            new GameObject("Text Area");

        textArea.transform.SetParent(
            inputObject.transform,
            false
        );

        RectTransform areaRect =
            textArea.AddComponent<RectTransform>();

        areaRect.anchorMin = Vector2.zero;
        areaRect.anchorMax = Vector2.one;
        areaRect.offsetMin = new Vector2(15f, 5f);
        areaRect.offsetMax = new Vector2(-15f, -5f);

        // Input text
        GameObject textObject =
            new GameObject("Text");

        textObject.transform.SetParent(
            textArea.transform,
            false
        );

        RectTransform textRect =
            textObject.AddComponent<RectTransform>();

        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        TextMeshProUGUI inputText =
            textObject.AddComponent<TextMeshProUGUI>();

        inputText.fontSize = 32f;
        inputText.color = Color.black;
        inputText.alignment =
            TextAlignmentOptions.Center;

        // Placeholder
        GameObject placeholderObject =
            new GameObject("Placeholder");

        placeholderObject.transform.SetParent(
            textArea.transform,
            false
        );

        RectTransform placeholderRect =
            placeholderObject.AddComponent<RectTransform>();

        placeholderRect.anchorMin = Vector2.zero;
        placeholderRect.anchorMax = Vector2.one;
        placeholderRect.offsetMin = Vector2.zero;
        placeholderRect.offsetMax = Vector2.zero;

        TextMeshProUGUI placeholder =
            placeholderObject.AddComponent<TextMeshProUGUI>();

        placeholder.text =
            "Enter your answer";

        placeholder.fontSize = 26f;
        placeholder.color = Color.gray;
        placeholder.alignment =
            TextAlignmentOptions.Center;

        input.textViewport = areaRect;
        input.textComponent = inputText;
        input.placeholder = placeholder;

        return input;
    }

    // =========================================================
    // CREATE BUTTON
    // =========================================================

    private Button CreateButton(
        string objectName,
        Transform parent,
        string label,
        Vector2 position,
        Vector2 size)
    {
        GameObject buttonObject = CreateImage(
            objectName,
            parent,
            new Color(0.2f, 0.55f, 0.95f, 1f)
        );

        RectTransform rect =
            buttonObject.GetComponent<RectTransform>();

        rect.sizeDelta = size;
        rect.anchoredPosition = position;

        Button button =
            buttonObject.AddComponent<Button>();

        TMP_Text buttonText = CreateText(
            "Text",
            buttonObject.transform,
            label,
            28,
            Vector2.zero,
            size
        );

        buttonText.color = Color.white;
        buttonText.fontStyle = FontStyles.Bold;

        return button;
    }

    // =========================================================
    // CREATE IMAGE
    // =========================================================

    private GameObject CreateImage(
        string objectName,
        Transform parent,
        Color color)
    {
        GameObject obj =
            new GameObject(objectName);

        obj.transform.SetParent(parent, false);

        obj.AddComponent<RectTransform>();

        Image image =
            obj.AddComponent<Image>();

        image.color = color;

        return obj;
    }

    // =========================================================
    // EVENT SYSTEM
    // =========================================================

    private void CreateEventSystemIfNeeded()
    {
        EventSystem existing =
            FindObjectOfType<EventSystem>();

        if (existing != null)
            return;

        GameObject eventObject =
            new GameObject("EventSystem");

        eventObject.AddComponent<EventSystem>();
        eventObject.AddComponent<StandaloneInputModule>();
    }
}