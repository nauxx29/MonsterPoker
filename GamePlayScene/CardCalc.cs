using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Update player gameplay logic 

public class CardCalc : MonoBehaviour
{
    const int CARDNUMBER = 5;
    const int SYMBOLNUMBER = 4;

    [SerializeField] Camera mainCam;
    [SerializeField] CardRecordSO _cardRecordSO;
    [SerializeField] Text calcText;
    [SerializeField] Text _errorText;

    Button[] buttonGroup;
    bool isButtonActive = false;
    bool isScriptActive = false;
    
    int resultStackLength;
    int pre_resultStackLength = 0;
    Stack<string> resultStack = new Stack<string>();
    Stack<CardInfo> cardInfoStack = new Stack<CardInfo>(); // Push here if selected and pop when remove from resultStack
    
    float resultAnswer = 0;
    string errorText;
    
    private void OnEnable()
    {
        mainCam = Camera.main; // detect ray
        buttonGroup = FindObjectsByType<Button>(FindObjectsSortMode.None); 

        Events.onCalc.AddListener(Clac);
        Events.onCalcUpdate.AddListener(CardUpdateResult);
        Events.onCalcRemove.AddListener(CalcRemove);
        Events.onCalcClear.AddListener(CalcClear);
        Events.onCalcAnswer.AddListener(CalcAnswer);
        Events.onCalcResult.AddListener(CalcResult);
        Events.onReset.AddListener(Reset);
    }
    
    private void OnDisable()
    {
        Events.onCalc.RemoveListener(Clac);
        Events.onCalcUpdate.RemoveListener(CardUpdateResult);
        Events.onCalcRemove.RemoveListener(CalcRemove);
        Events.onCalcClear.RemoveListener(CalcClear);
        Events.onCalcAnswer.RemoveListener(CalcAnswer);
        Events.onCalcResult.RemoveListener(CalcResult);
        Events.onReset.RemoveListener(Reset);
    }
    void Clac()
    {
        if (isButtonActive == false)
        {
            ButtonSwitch(); 
        }
        isScriptActive = true; // start the Update()
    }

    /// <summary> turn on/off calc UI button's *interactable* </summary>
    private void ButtonSwitch()
    {
        foreach (var button in buttonGroup)
        {
            button.interactable = true;
        }
        isButtonActive = !isButtonActive;
    }

    /// <summary> if mouse press => MouseDetect(); if calc result update => ChangeDetect(); </summary>
    private void Update()
    {
        if (isScriptActive) 
        {
            MouseDetect(); 
            ChangeDetect(); 
        }
    }

    /// Detect if raycast hit on object's collider.
    private void MouseDetect()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition); // from camera to mousePosition
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            if (hit.collider != null)
            {
                GameObject hitCard = hit.collider.gameObject;
                AddHitCard(hitCard);
            }
        }
    }

    /// <summary>
    /// 1. Detect if the <paramref name="card"/> has been selected before.
    /// 2. If not, push to <c>cardInfoStack</c> and mark it as selected.
    /// </summary>
    /// <param name="card"> the card that raycast hit</param>
    private void AddHitCard(GameObject card)
    {
        CardInfo cardInfo = card.GetComponent<CardInfo>();
        if (cardInfo.isSelective)
        {
            errorText = "Already Use it";
            ErrorDisplayChanege(errorText);
        }
        else
        {
            int pre_resltStack = resultStack.Count;
            CardUpdateResult(cardInfo.cardIndex, false);

            if (resultStack.Count == pre_resltStack + 1)
            {
                cardInfoStack.Push(cardInfo);
                cardInfo.isSelective = true;
            }
            else if (resultStack.Count != pre_resltStack)
            {
                Debug.Log("AddHitCard() Update went wrong");
            }
        }
    }

    private void ChangeDetect()
    {
        resultStackLength = resultStack.Count;

        if (pre_resultStackLength != resultStackLength)
        {
            CalcDisplayChange();
            pre_resultStackLength = resultStackLength;
        }
    }

    // Show any changes on the display.
    private void CalcDisplayChange()
    {
        string displayresult = "";
        List<String> resultList = TurnStackToList(resultStack);

        foreach (string s in resultList)
        {
            displayresult += s;
        }
        calcText.text = displayresult;
    }

    private void ErrorDisplayChanege(string input)
    {
        _errorText.text = input;
    }
    

    // Check every input from user and update them to the resultStack.
    private void CardUpdateResult(string inputValue, bool inputIsSign)
    {
        if (resultStack.Count > 0 && resultStack.Count < (CARDNUMBER + SYMBOLNUMBER) && (IsCalcSign(resultStack.Peek()) != inputIsSign))
        {
            resultStack.Push(inputValue);
        } 
        else if (resultStack.Count == 0 && !inputIsSign)
        {
            resultStack.Push(inputValue);
        }
        else
        {
            errorText = "Rule: Number followed by an operator and then another number";
            ErrorDisplayChanege(errorText);
        }
    }
    private bool IsCalcSign(string inputvalue)
    {
        if (inputvalue == "+" || inputvalue == "-" || inputvalue == "*" || inputvalue == "/")
        {
            return true;
        }
        return false;
    }

    // Use stack's .Pop() make sure LIFO like a calc 
    private void CalcRemove()
    {
        if (resultStack.Count > 0)
        {
            if (!IsCalcSign(resultStack.Pop()))
            {
                DiseletiveCardInfo();
            }
        }
        else
        {
            errorText = "Stack is Empty";
            ErrorDisplayChanege(errorText);
        }
    }

    public void CalcClear()
    {
        resultStack.Clear();
        foreach (CardInfo cardInfo in  cardInfoStack)
        {
            cardInfo.isSelective = false;
        }
        cardInfoStack.Clear();
    }

    public void CalcAnswer()
    {
        if (resultStack.Count == CARDNUMBER + SYMBOLNUMBER)
        {
            Events.onLocalStateChange.Invoke(GameState.UpdateTime); // stop and update time first then 
            CalcResult();
        }
        else
        {
            errorText = "Not Enough Input";
            ErrorDisplayChanege(errorText);
        }
    }

    /// <summary> calc the resultStack FIFO and regardless of the symbol </summary>
    private void CalcResult()
    {
        List<String> resultList = TurnStackToList(resultStack);
        
            resultAnswer = float.Parse(resultList[0]); // Initialize the answer

        for (int i = 0; i < resultList.Count; i++)
        {
            if (IsCalcSign(resultList[i]))
            {
                switch (resultList[i])
                {
                    case "+":
                        resultAnswer += float.Parse(resultList[i + 1]);
                        break;
                    case "-":
                        resultAnswer -= float.Parse(resultList[i + 1]);
                        break;
                    case "*":
                        resultAnswer *= float.Parse(resultList[i + 1]);
                        break;
                    case "/":
                        resultAnswer /= float.Parse(resultList[i + 1]);
                        break;
                }
            }
        }

        _cardRecordSO.ResultUpdate(resultAnswer.ToString());
        Events.onCheckAllPlayerDone.Invoke(PhotonNetwork.LocalPlayer.ActorNumber, GameState.ShowResult);
    }
    private void DiseletiveCardInfo()
    {
        CardInfo cardInfoTMP = cardInfoStack.Pop();
        cardInfoTMP.isSelective = false;
    }
    private List<String> TurnStackToList(Stack<String> stack)
    {
        List<string> resultList = new List<string>(stack);
        resultList.Reverse();
        return resultList;
    }

    private void Reset()
    {
        CalcClear(); 
        isScriptActive = false;
        ButtonSwitch();
        errorText = string.Empty;
        ErrorDisplayChanege(errorText);
        resultAnswer = 0;
        _cardRecordSO.CardRecordClear();
        Events.onResetCount.Invoke();
    }
}
