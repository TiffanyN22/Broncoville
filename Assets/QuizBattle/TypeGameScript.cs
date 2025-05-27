using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TypeGameScript : MonoBehaviour
{
    [SerializeField] public TextAsset TypeGameQuestions;
    [SerializeField] public TMP_Text QuestionCardText;
    [SerializeField] public TMP_Text AnswerFieldInputText;
    [SerializeField] public TMP_InputField AnswerField;
    [SerializeField] public TMP_Text CoinsText;
    [SerializeField] public TMP_Text QuestionCountText;
    private List<string[]> qnaList;

    private string answerSubmission;
    private string currentAnswer;
    private string currentQuestion;
    private int currentQnaIndex;

    public string gameMode;
    private int coinsGained;
    private int numQsAnswered;
    private int numQsCorrectlyAnswered;

    private bool displayingResultMode;


    // Start is called before the first frame update
    void Start()
    {
        gameMode = "sequence"; // sequence or random
        currentQnaIndex = 0;
        coinsGained = 0;
        qnaList = TextFileToQuestions(TypeGameQuestions);
        QuestionCardText.text = qnaList[0][0];
        answerSubmission = "";

        currentQuestion = qnaList[currentQnaIndex][0];
        currentAnswer = qnaList[currentQnaIndex][1];
    }

    // Update is called once per frame
    void Update()
    {
        // using the enter key instead of the submit button
        if (Input.GetKeyDown(KeyCode.Return))
        {
            submitButtonPress();
        }
    }

    private void nextQuestion()
    {
        // change qna index
        if(gameMode == "sequence")
        {
            currentQnaIndex = (currentQnaIndex + 1) % qnaList.Count;
        } else if(gameMode == "random")
        {
            currentQnaIndex = Random.Range(0, qnaList.Count);
        }
        else
        {
            currentQnaIndex = 0;
        }

        // change question and answer
        currentQuestion = qnaList[currentQnaIndex][0];
        currentAnswer = qnaList[currentQnaIndex][1];

        // change question card text
        QuestionCardText.text = currentQuestion;

    }

    private void updateNumberText()
    {
        // update coin count
        CoinsText.text = "coins: " + coinsGained;
        // update question count
        QuestionCountText.text = numQsCorrectlyAnswered + "/" + numQsAnswered + " correct";
    }

    private void ProcessSubmission()
    {
        if( answerSubmission.Trim().ToLower().Contains(currentAnswer.Trim().ToLower()) ) // submission is correct
        {
            coinsGained += 5;
            numQsCorrectlyAnswered++;
        } 
        else // submission is incorrect
        {
        }

        numQsAnswered++;
    }

    public void displayAnswerResult()
    {
        if (answerSubmission.Trim().ToLower().Contains(currentAnswer.Trim().ToLower()))
        {
            QuestionCardText.text = "correct!";
            Debug.Log("submission is correct!");
        }
        else
        {
            QuestionCardText.text = "incorrect. answer:\n" + currentAnswer;
            Debug.Log("submission: " + answerSubmission + ".");
            Debug.Log("submission is incorrect. answer: " + currentAnswer + ".");
        }
    }

    // BUTTONS
    public void submitButtonPress()
    {
        // save submission text
        answerSubmission = AnswerFieldInputText.text;

        if (!displayingResultMode)
        {
            displayingResultMode = true;
            displayAnswerResult();
            Debug.Log("displayed answer result");
        }
        else
        {
            displayingResultMode = false;

            // change qna
            ProcessSubmission();
            updateNumberText();
            AnswerField.text = "";

            AnswerField.ActivateInputField(); // keep input field selected (it auto clicks off otherwise)
            nextQuestion();
        }
    }

    // QUESTION AND ANSWER GENERATION
    private string[] GenerateQnA(string textLine) {
        string qBeginDelim = "{";
        string aEndDelim = "}";
        string qnaSeparator = "~";

        if (!textLine.Contains(qBeginDelim) || !textLine.Contains(aEndDelim) || !textLine.Contains(qnaSeparator)) {
            return new string[] { "~invalid question~", "~invalid answer~" };
        }
        else
        {
            string qnaSet = textLine.Substring(textLine.IndexOf(qBeginDelim) + 1, textLine.Length - 3);

            int qBeginIndex = textLine.IndexOf(qBeginDelim);
            int aEndIndex = textLine.IndexOf(aEndDelim);
            int qnaSepIndex = textLine.IndexOf(qnaSeparator);

            string question = qnaSet.Substring(qBeginIndex, qnaSepIndex - qBeginIndex - 1);
            string answer = qnaSet.Substring(qnaSepIndex, aEndIndex - qnaSepIndex - 1);

            return new string[] { question, answer};
        }
    }

    private List<string[]> TextFileToQuestions(TextAsset myTextFile)
    {
        string[] textLines = myTextFile.text.Split("\n");
        List<string[]> myQnaList = new List<string[]>();

        for (int i = 0; i < textLines.Length; i++)
        {
            string[] possibleQnA = GenerateQnA(textLines[i]);
            if (possibleQnA[0] != "~invalid question~")
            {
                myQnaList.Add(possibleQnA);
            }
        }

        /*
        for (int i = 0; i < myQnaList.Count; i++)
        {
            Debug.Log("q: " + myQnaList[i][0] + " a: " + myQnaList[i][1]);
        }
        */

        return myQnaList;
    }
}
