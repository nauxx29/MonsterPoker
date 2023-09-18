using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class ResultDisplay : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_Text playerName;
    [SerializeField] TMP_Text otherPlayerName;
    [SerializeField] Image playerIcon;
    [SerializeField] Image otherPlayerIcon;
    [SerializeField] Sprite[] avatars;
    
    [Space]
    
    [SerializeField] GameObject resultPanel;
    [SerializeField] GameObject timeUpPanel;
    [SerializeField] GameObject waitingPanel;
    [SerializeField] GameObject endResultPanel;
    [SerializeField] GameObject playerLeftPanel;
    [SerializeField] TimeRecordSO _timeRecordSO;
    [SerializeField] CardRecordSO _cardRecordSO;
    
    [Space]
    
    [SerializeField] PlayerResultDisplay playerResultPrefab;
    [SerializeField] FinalResultPrefab finalResultPrefab;
    [SerializeField] Transform playerResultPanel;
    [SerializeField] Transform playerEndResultPanel;

    List<GameObject> resultPointer = new List<GameObject>();
    List<GameObject> endResulrPointer = new List<GameObject>();
    int playerCount = 0;

    // Photon
    PhotonView photonView;
    public enum GameResult { Win, Lose, Tie }
    public enum AnswerState { Correct, Wrong, NA }
    private void Awake()
    {
        Events.onOnChangeAudio.Invoke();
        photonView = GetComponent<PhotonView>();
        if ( photonView == null ) { Debug.Log("ResultDiaspley PV is null"); }
    }
    public override void OnEnable()
    {
        base.OnEnable();
        Events.onResultShow.AddListener(ResultShow);
        Events.onTimeUpShow.AddListener(TimeUpShow);
        Events.onWaiting.AddListener(Waiting);
        Events.onReset.AddListener(Reset);
        Events.onEndResult.AddListener(EndResult);
        Events.onNewGame.AddListener(NewGame);
        Events.onCalcDoneChangeColor.AddListener(CalcDoneChangeColor);
        playerName.text = PhotonNetwork.LocalPlayer.NickName;
        otherPlayerName.text = PhotonNetwork.PlayerListOthers[0].NickName;
        playerIcon.sprite = avatars[(int)PhotonNetwork.LocalPlayer.CustomProperties["playerAvatar"]];
        otherPlayerIcon.sprite = avatars[(int)PhotonNetwork.PlayerListOthers[0].CustomProperties["playerAvatar"]];
    }


    public override void OnDisable()
    {
        base.OnDisable();
        Events.onResultShow.RemoveListener(ResultShow);
        Events.onTimeUpShow.RemoveListener(TimeUpShow);
        Events.onWaiting.RemoveListener(Waiting);
        Events.onReset.RemoveListener(Reset);
        Events.onEndResult.RemoveListener(EndResult);
        Events.onNewGame.RemoveListener(NewGame);
        Events.onCalcDoneChangeColor.RemoveListener(CalcDoneChangeColor);
    }

    void ResultShow()
    {
        StorePlayerProperties();
    }

    void CalcDoneChangeColor()
    {
        photonView.RPC("RPC_ChangeColoe", RpcTarget.Others);
    }

    [PunRPC]
    private void RPC_ChangeColoe()
    {
        otherPlayerName.color = Color.yellow;
    }

    void StorePlayerProperties()
    {
        AnswerState playerAnswer = CheckResult();
        string timeForDisplay = TimeRecordText(_timeRecordSO.ReadTimeRecord());
        Hashtable customProperties = new Hashtable()
        {
            {"GameAnswer", playerAnswer },
            {"GameTimeForDisplay", timeForDisplay },
            {"GameTime", _timeRecordSO.ReadTimeRecord()}
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);
    }
    
    // Properties Update need sometime so wait for called back
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey("Score") && changedProps.ContainsKey("GameWin") && changedProps.ContainsKey("GameAnswer"))
        {
            Debug.Log(changedProps);
            playerCount++;
            if (playerCount == PhotonNetwork.PlayerList.Length)
            {
                playerCount = 0;
                Events.onCheckAllPlayerDone.Invoke(PhotonNetwork.LocalPlayer.ActorNumber, GameState.Deal);
            }
        }
        else if (changedProps.ContainsKey("GameAnswer"))
        {
            playerCount++;
            if (playerCount >= PhotonNetwork.PlayerList.Length)
            {
                playerCount = 0;
                if (PhotonNetwork.IsMasterClient)
                {
                    UpdateGameResult();
                }
            }
        }
        else if (changedProps.ContainsKey("GameWin"))
        {
            playerCount++;
            if (playerCount >= PhotonNetwork.PlayerList.Length)
            {
                playerCount = 0; 
                AssignUI();
            }    
        } 
        else if (changedProps.ContainsKey("ReturningFromGame") && targetPlayer == PhotonNetwork.LocalPlayer)
        {
            Events.onOnChangeAudio.Invoke();
            SceneManager.LoadScene("Lobby");
        }
    }
    
    public override void OnPlayerLeftRoom(Player player)
    {
            // show player left room panel
            resultPanel.SetActive(false);
            waitingPanel.SetActive(false);
            endResultPanel.SetActive(false);
            playerLeftPanel.SetActive(true);
    } 

    private string TimeRecordText(float timeRecord)
    {
        int min = Mathf.FloorToInt(timeRecord / 60);
        int sec = Mathf.FloorToInt(timeRecord % 60);
        string gameTimeResult = min.ToString() + ":" + sec.ToString();
        return gameTimeResult;
    }

    AnswerState CheckResult()
    {
        AnswerState resultCheck;

        if (_cardRecordSO.ReadAnswerResult() == "NA")
        {
            resultCheck = AnswerState.NA;
        }
        else if (_cardRecordSO.ReadAnswerResult() == _cardRecordSO.ReadAnswerCard())
        {
            resultCheck = AnswerState.Correct;
        }
        else 
        {
            resultCheck = AnswerState.Wrong;
        }
        return resultCheck;
    }

    // Logic only for 2 player 
    void UpdateGameResult()
    {
        for (int i = 0; i < PhotonNetwork.PlayerList.Length - 1; i++)
        {
            Player player1 = PhotonNetwork.PlayerList[i];
            Player player2 = PhotonNetwork.PlayerList[i + 1];
            AnswerState player1AnswerTMP = (AnswerState)player1.CustomProperties["GameAnswer"];
            AnswerState player2AnswerTMP = (AnswerState)player2.CustomProperties["GameAnswer"];
            float player1TimeTMP = (float)player1.CustomProperties["GameTime"];
            float player2TimeTMP = (float)player2.CustomProperties["GameTime"];

            if (player1AnswerTMP == AnswerState.Correct && player1TimeTMP < player2TimeTMP)
            {
                PlayerWin(player1, player2, false);
            }
            else if (player1AnswerTMP == AnswerState.NA && player2AnswerTMP == AnswerState.NA && player1TimeTMP < player2TimeTMP)
            {
                PlayerWin(player1, player2, false);
            }
            else if (player1AnswerTMP == AnswerState.NA && player2AnswerTMP == AnswerState.Wrong)
            {
                PlayerWin(player1, player2, false);
            }
            else if (player1AnswerTMP == AnswerState.Wrong && player2AnswerTMP == AnswerState.Wrong)
            {
                PlayerWin(player1, player2, true);
            }
            else 
            {
                PlayerWin(player2, player1, false);
            }
        }
    }

    /// <summary>
    ///  
    /// </summary>
    void PlayerWin(Player player1, Player player2, bool isTie)
    {
        Hashtable updateWin = new Hashtable
        {
            { "GameWin", GameResult.Win }
        };
        Hashtable updateLose = new Hashtable
        {
            { "GameWin", GameResult.Lose }
        };
        Hashtable updateTie = new Hashtable
        {
            { "GameWin", GameResult.Tie }
        };

        if (isTie)
        {
            player1.SetCustomProperties(updateTie);
            player2.SetCustomProperties(updateTie);
        }
        else
        {
            player1.SetCustomProperties(updateWin);
            UpdateScore(player1);

            player2.SetCustomProperties(updateLose);
        }
    }

    void UpdateScore(Player winner)
    {
        int score = (int)winner.CustomProperties["Score"];
        score++;
        Hashtable scoreHash = new Hashtable();
        scoreHash.Add("Score", score);
        winner.SetCustomProperties(scoreHash);
    }
    void AssignUI()
    {
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            PlayerResultDisplay playerResultUI = Instantiate(playerResultPrefab, playerResultPanel);
            resultPointer.Add(playerResultUI.gameObject);

            playerResultUI.UpdateName(PhotonNetwork.PlayerList[i].NickName);

            string timeTMP = (string)PhotonNetwork.PlayerList[i].CustomProperties["GameTimeForDisplay"];
            playerResultUI.UpdateTime(timeTMP);

            AnswerState answerTMP = (AnswerState)PhotonNetwork.PlayerList[i].CustomProperties["GameAnswer"];
            playerResultUI.UpdateAnswer(answerTMP.ToString());

            GameResult resultTMP = (GameResult)PhotonNetwork.PlayerList[i].CustomProperties["GameWin"];
            if (resultTMP == GameResult.Win)
            {
                playerResultUI.UpdateResult(true);
            }
            else { playerResultUI.UpdateResult(false); }
        }

        waitingPanel.SetActive(false);
        resultPanel.SetActive(true);
    }
    private void TimeUpShow()
    {
        timeUpPanel.SetActive(true);
        _cardRecordSO.ResultUpdate("-1");
        Events.onCheckAllPlayerDone.Invoke(PhotonNetwork.LocalPlayer.ActorNumber, GameState.ShowResult);
    }

    private void Waiting(bool displayUI)
    {
        waitingPanel.SetActive(displayUI);
    }

    private void Reset()
    {
        resultPanel.SetActive(false);
        timeUpPanel.SetActive(false);
        waitingPanel.SetActive(false);
        _cardRecordSO.CardRecordClear();
        otherPlayerName.color = Color.white;

        foreach (GameObject result in resultPointer)
        {
            Destroy(result);
        }
        resultPointer.Clear();

        Events.onResetCount.Invoke();
    }

    private void EndResult()
    {
        int[] scoreTMP = new int[PhotonNetwork.PlayerList.Length];
        FinalResultPrefab[] UIHolder = new FinalResultPrefab[PhotonNetwork.PlayerList.Length];
        
        for ( int i = 0; i < PhotonNetwork.PlayerList.Length ; i++ )
        {
            FinalResultPrefab playerEndResultUI = Instantiate(finalResultPrefab, playerEndResultPanel);
            UIHolder[i] = playerEndResultUI;
            endResulrPointer.Add(playerEndResultUI.gameObject);

            string playerName = PhotonNetwork.PlayerList[i].NickName;
            playerEndResultUI.UpdatePlayerName(playerName);
            
            int score = (int)PhotonNetwork.PlayerList[i].CustomProperties["Score"];
            playerEndResultUI.UpdateScore(score);
            Debug.Log(playerName + " score is " + score);
            
            scoreTMP[i] = score;
        }

        if (scoreTMP[0] > scoreTMP[1])
        {
            UIHolder[0].UpdateIsWinner(true);
            UIHolder[1].UpdateIsWinner(false);
        }
        else if (scoreTMP[0] < scoreTMP[1])
        {
            UIHolder[0].UpdateIsWinner(false);
            UIHolder[1].UpdateIsWinner(true);
        }
        Waiting(false);
        endResultPanel.SetActive(true);
    }

    private void NewGame()
    {
        foreach (GameObject endResult in endResulrPointer)
        {
            Destroy(endResult);
        }
        endResultPanel.SetActive(false);

        Hashtable initialProperties = new Hashtable() // default value
        {
            { "GameTime", -1f },
            { "GameAnswer", "defult"},
            { "GameTimeForDisplay", "defult"},
            { "GameWin", "defult" },
            { "Score", 0 },
        };
        PhotonNetwork.SetPlayerCustomProperties(initialProperties);

        Debug.Log(PhotonNetwork.LocalPlayer.NickName + "NewGameUPdate");
    }
}

