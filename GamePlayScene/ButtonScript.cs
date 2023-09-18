using Photon.Pun;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;


// For unity button system in editor
public class ButtonScript : MonoBehaviourPun
{
    [SerializeField] AudioSource buttonSound;
    [SerializeField] MuteButtonSO muteButtonSO;

    private void Awake()
    {
        UpdateMuteSetting();
    }
    public  void ClipPlay()
    {
        buttonSound.Play();
    }
    public void AddClick()
    {

        Events.onCalcUpdate.Invoke("+" , true);
    }
    public void MinusClick()
    {
        Events.onCalcUpdate.Invoke("-", true);
    }

    public void MultiplyClick()
    {
        Events.onCalcUpdate.Invoke("*", true);
    }

    public void DivideClick()
    {
        Events.onCalcUpdate.Invoke("/", true);
    }

    public void ClearClick()
    {
        Events.onCalcClear.Invoke();
    }

    public void RemoveClick()
    {
        Events.onCalcRemove.Invoke();
    }

    public void OkClick()
    {
        Events.onCalcAnswer.Invoke();
    }

    public void NaClick()
    {
        Events.onLocalStateChange.Invoke(GameState.NaUpdateTime); // problem here
    }

    public void ReadyClick()
    {
        Events.onCheckAllPlayerDone.Invoke(PhotonNetwork.LocalPlayer.ActorNumber, GameState.NewRound);
    }

    public void NewGameClick()
    {
        Events.onCheckAllPlayerDone.Invoke(PhotonNetwork.LocalPlayer.ActorNumber, GameState.NewGame);
    }

    public void QuitClick() 
    {
        Application.Quit();    
    }
    public void RoomClick()
    {
        // PhotonNetwork.LeaveRoom();
        Hashtable leveGameUpdate = new Hashtable()
        {
            { "ReturningFromGame", true }
        };
        PhotonNetwork.SetPlayerCustomProperties(leveGameUpdate);
    }
    public void LobbyClick()
    {
        Hashtable leveGameUpdate = new Hashtable()
        {
            { "ReturningFromGame", false }
        };
        PhotonNetwork.SetPlayerCustomProperties(leveGameUpdate);
        PhotonNetwork.LeaveRoom();
    }

    public void OnClickMute()
    {
        muteButtonSO.MuteToggle();
        UpdateMuteSetting();
    }

    private void UpdateMuteSetting()
    {
        AudioSource[] audioSources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource source in audioSources)
        {
            source.mute = muteButtonSO.isMute;
        }
    }
}

