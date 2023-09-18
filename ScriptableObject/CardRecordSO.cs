using UnityEngine;

[CreateAssetMenu(menuName = "CardRecord")]

// for each player record the answer card number & player's answer result 
public class CardRecordSO : ScriptableObject
{
    private string answerCardNumber = "";
    private string answerResult = "NA";
    public void AnswerCardUpdate(string index)
    {
        answerCardNumber = index;
    }

    public void ResultUpdate(string result)
    {
        answerResult = result;
    }

    public void CardRecordClear()
    {
        answerCardNumber = "";
        answerResult = "NA";
    }

    public string ReadAnswerCard()
    {
        return answerCardNumber;
    }

    public string ReadAnswerResult()
    {
        return answerResult;
    }
}
