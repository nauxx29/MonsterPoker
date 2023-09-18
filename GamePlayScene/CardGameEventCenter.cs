using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class CardGameEventCenter_Pho : MonoBehaviour
{
    const int RESET_FUNCTION_COUNT = 4;
    [SerializeField] const int GAME_TOTAL = 3;

    int gameRound = 0;
    PhotonView photonView;
    List<int> playerDoneId = new List<int>();
    GameState nowState = GameState.Waiting;
    int resetFuncCompleteCount = 0;
    void Awake()
    {
        
        photonView = GetComponent<PhotonView>();
        if (photonView == null)
        Debug.LogError("CardGameEventCenter_Pho PV is null");
    }
    private void OnEnable()
    {
        Events.onMasterStateChange.AddListener(HandleMasterStateChange);
        Events.onLocalStateChange.AddListener(LocalStateChange);
        Events.onCheckAllPlayerDone.AddListener(CheckAllPlayerDone);
        Events.onResetCount.AddListener(ResetCount);
        MasterStateChange(GameState.Deal);
    }

    private void OnDisable()
    {
        Events.onMasterStateChange.RemoveListener(HandleMasterStateChange);
        Events.onLocalStateChange.RemoveListener(LocalStateChange);
        Events.onCheckAllPlayerDone.RemoveListener(CheckAllPlayerDone);
        Events.onResetCount.RemoveListener(ResetCount);
    }
    void HandleMasterStateChange(GameState state)
    {
        if (state != nowState)
        {
            Debug.Log("Now State : " + state.ToString());
            nowState = state;
        }

        switch (state)
        {
            case GameState.Deal:
                if (PhotonNetwork.IsMasterClient)
                {
                    if (gameRound == GAME_TOTAL)
                    {
                        gameRound = 1;
                    }
                    else
                    {
                        gameRound++;
                    }
                }
                Events.onDeal.Invoke();
                break;
            case GameState.Calc:
                Events.onCalc.Invoke();
                Events.onClockCountDown.Invoke(true);
                break;
            case GameState.UpdateTime:
                Events.onClockCountDown.Invoke(false);
                break;
            case GameState.NaUpdateTime:
                Events.onClockCountDown.Invoke(false);
                break;
            case GameState.ShowResult:
                // show result
                Events.onResultShow.Invoke();
                break;
            case GameState.TimeUp:
                // Time UP
                Events.onTimeUpShow.Invoke();
                break;
            case GameState.Waiting:
                Events.onWaiting.Invoke(true);
                break;
            case GameState.NewRound:
                Events.onReset.Invoke();
                break;
            case GameState.CheckRound:
                CheckRound();
                break;
            case GameState.EndGameResult:
                Events.onEndResult.Invoke();
                break;
            case GameState.NewGame:
                Events.onNewGame.Invoke();
                break;
        }
    }

    /// <summary>
    /// Make MasterClient sync everyplayer to the <paramref name="newState"/>
    /// </summary>
    void MasterStateChange(GameState newState)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("RPC_MasterStateChange", RpcTarget.AllBuffered, newState);
        }
    }

    [PunRPC]
    private void RPC_MasterStateChange(GameState newState)
    {
        HandleMasterStateChange(newState);
        Events.onWaiting.Invoke(false);
    }


    private void LocalStateChange(GameState newState)
    {
        HandleMasterStateChange(newState);
    }

    /// <summary>
    /// when one script's Reset() complete resetFuncCompleteCount++
    /// when resetFuncCompleteCount == total Reset() => it means localplayer resetcomplete
    /// update 
    /// </summary>
    private void ResetCount()
    {
        resetFuncCompleteCount++;

        if (resetFuncCompleteCount == RESET_FUNCTION_COUNT)
        {
            resetFuncCompleteCount = 0;
            CheckAllPlayerDone(PhotonNetwork.LocalPlayer.ActorNumber, GameState.CheckRound);
        }
    }

    private void CheckRound()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (gameRound == GAME_TOTAL)
            {
                MasterStateChange(GameState.EndGameResult);
            }
            else
            {
                MasterStateChange(GameState.Deal);
            }
        }
    }
    private void CheckAllPlayerDone(int id, GameState state)
    {
        Events.onLocalStateChange.Invoke(GameState.Waiting);
        photonView.RPC("RPC_CheckAllPlayerDone", RpcTarget.MasterClient, id, state);
    }
    /// <summary>
    /// If player have done sth, Pass player's <paramref name="id"/> ;
    /// once all the player in PlayerList are done,
    /// MasterClient sync all the player to the next <paramref name="state"/>.
    /// </summary>

    [PunRPC]
    private void RPC_CheckAllPlayerDone(int id, GameState state)
    { 
        if (!playerDoneId.Contains(id))
        {
            playerDoneId.Add(id);
            if(state == GameState.Deal)
            {
                Debug.Log(id);
            }

            if (playerDoneId.Count == PhotonNetwork.PlayerList.Length)
            {
                playerDoneId.Clear();
                MasterStateChange(state);
            }
        }
        else
        {
            Debug.Log("Player already contain in the donelist");
        }
    }

}
