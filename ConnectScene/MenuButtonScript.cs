using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtonScript : MonoBehaviourPunCallbacks
{
    [SerializeField] AudioSource buttonAS;
    [SerializeField] MuteButtonSO muteButtonSO;
    [SerializeField] GameObject menuPanel;
    [SerializeField] GameObject connectPanel;
    [SerializeField] TMP_InputField _inputField;
    [SerializeField] TMP_Text buttonText;

    private void Awake()
    {
        UpdateMuteSetting();
    }
    public void ReadyOnClick()
    {
        menuPanel.SetActive(false);
        connectPanel.SetActive(true);
        //SceneManager.LoadScene(SceneName.ConnectScene.ToString());
    }

    public void QuitOnClick()
    {
        Application.Quit();
    }
    public void OnClickConnect()
    {
        if (_inputField.text.Length >= 1)
        {
            PhotonNetwork.NickName = _inputField.text;
            buttonText.text = "Connecting ...";
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.ConnectUsingSettings();
        }
    }
    public void OnButtonClickSound()
    {
        buttonAS.mute = muteButtonSO.ReadIsMute();
        buttonAS.Play();
    }

    // not evertime will cll especially when you are leave the game doen't mean you are reconnect to masterserver
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        SceneManager.LoadScene("Lobby");
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

}
