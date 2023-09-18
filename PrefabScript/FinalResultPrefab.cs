
using UnityEngine;
using TMPro;
using UnityEngine.UI;

// Attach on FinalResult prefab script, for updating the data on each instantiate items
public class FinalResultPrefab : MonoBehaviour
{
    [SerializeField] TMP_Text playerName;
    [SerializeField] TMP_Text score;

    public void UpdatePlayerName(string _playerName)
    {
        playerName.text = _playerName;
    }

    public void UpdateScore(int _score)
    {
        score.text = _score.ToString();
    }

    public void UpdateIsWinner(bool isWin)
    {
        Image isWinner = GetComponentInChildren<Image>();
        isWinner.enabled = isWin;
    }
}
