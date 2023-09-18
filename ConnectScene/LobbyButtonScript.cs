using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyButtonScript : MonoBehaviour
{
    [SerializeField] AudioSource buttonAS;
    [SerializeField] MuteButtonSO muteButtonSO;

    [Space]
    
    [SerializeField] List<Sprite> tutorSprites = new List<Sprite>();
    [SerializeField] GameObject tutorPanel;
    [SerializeField] Image panelImage;
    [SerializeField] TMP_Text nextBtnText;
    
    [Space]
    
    [SerializeField] TMP_InputField roomNameInputField;

    int spriteIndex;

    private void Awake()
    {
        UpdateMuteSetting();
    }

    // Clicke tutor btn
    public void OnClickTutor()
    {
        nextBtnText.text = "Next";
        spriteIndex = 0;
        panelImage.sprite = tutorSprites[spriteIndex];
        tutorPanel.SetActive(true);
    }

    public void OnClickNext()
    {
        if (spriteIndex < tutorSprites.Count - 1)
        {
            spriteIndex++;
            panelImage.sprite = tutorSprites[spriteIndex];
            if (spriteIndex == tutorSprites.Count - 1)
            {
                nextBtnText.text = "End";
            }   
        }
        else
        {
            tutorPanel.SetActive(false);
        }
    }
    public void OnButtonClickSound()
    {
        buttonAS.mute = muteButtonSO.ReadIsMute();
        buttonAS.Play();
    }
    public void OnClickQuit()
    {
        Application.Quit();
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
            source.mute = muteButtonSO.ReadIsMute();
        }
    }

    public void OnClickCreate() // auto join created room
    {
        if (roomNameInputField.text.Length >= 1)
        {
            RoomOptions roomOptions = new RoomOptions();
            PhotonNetwork.CreateRoom(roomNameInputField.text, new RoomOptions() { MaxPlayers = 2, BroadcastPropsChangeToAll = true });
            // BroadcastPropsChangeToAll = true recieve other player change properties info 
        }
    }
    public void OnClickLeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void OnClickStartButton()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;
        PhotonNetwork.LoadLevel("CardGame");
    }

    public void OnCLickeRandom()
    {
        PhotonNetwork.JoinRandomRoom();
    }
}
