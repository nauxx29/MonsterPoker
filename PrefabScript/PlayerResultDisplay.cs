
using UnityEngine;
using TMPro;

// Attach on Player result prefab
public class PlayerResultDisplay : MonoBehaviour
{
    [SerializeField] TMP_Text _name;
    [SerializeField] TMP_Text answer;
    [SerializeField] TMP_Text time;
    [SerializeField] TMP_Text result;

    public void UpdateName(string playerName)
    {
        _name.text = playerName;
    }

    public void UpdateAnswer(string playerAswer)
    {
        answer.text = playerAswer;
    }

    public void UpdateTime(string playerTime) 
    {
        time.text = playerTime;
    }

    public void UpdateResult(bool isWin)
    {
        if (isWin)
        {
            result.text = "Win";
        } else { 
            result.text = ""; 
        }
    }
}
