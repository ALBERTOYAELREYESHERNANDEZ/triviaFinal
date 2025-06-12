using System;
using System.Collections.Generic;

[Serializable]
public class QuizData
{
    public List<Question> questions;
}

[Serializable]
public class Question
{
    public string questionText;
    public List<TextAnswer> textAnswers;
    public bool hasTime;
    public QuestionTimerData questionTimerData;
}

[Serializable]
public class TextAnswer
{
    public bool isCorrect;
    public string answerValue;
    public string explanation; // ¡Este campo es necesario!
}

[Serializable]
public class QuestionTimerData
{
    public string timerType;
    public int timerValue;
}
