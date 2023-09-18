using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using Hashtable = ExitGames.Client.Photon.Hashtable; //photon version HashTable

// Attach on player prefab
public class PlayerItem : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_Text playerName;
    [SerializeField] Image playerBackground;
    [SerializeField] Image playerAvatar;
    [SerializeField] Sprite[] avatars;
    [SerializeField] Color highlightColor;

    [Space]

    [SerializeField] GameObject rightArrow;
    [SerializeField] GameObject leftArrow;


    Hashtable playerProperties = new Hashtable(); 
    Player player;
    private void Start()
    {
        playerBackground = GetComponent<Image>();
    }

    public void SetPlayerInfo(Player _player)
    {
        playerName.text = _player.NickName;
        player = _player;
        UpdatePlayerItem(player);
    }

    public void ApplyLocalChaneges()
    {
        playerBackground.color = highlightColor;
        rightArrow.SetActive(true);
        leftArrow.SetActive(true);
    }

    public void OnClickLeftArrow()
    {
        if (player == PhotonNetwork.LocalPlayer)
        {
            if ((int)playerProperties["playerAvatar"] == 0)
            {
                playerProperties["playerAvatar"] = avatars.Length - 1;
            }
            else
            {
                playerProperties["playerAvatar"] = (int)playerProperties["playerAvatar"] - 1;
            }
            PhotonNetwork.SetPlayerCustomProperties(playerProperties); // make the local change up to server
        }
        
    }

    public void OnClickRightArrow()
    {
        if (player == PhotonNetwork.LocalPlayer)
        {
            if ((int)playerProperties["playerAvatar"] == avatars.Length - 1)
            {
                playerProperties["playerAvatar"] = 0;
            }
            else
            {
                playerProperties["playerAvatar"] = (int)playerProperties["playerAvatar"] + 1;
            }
            PhotonNetwork.SetPlayerCustomProperties(playerProperties); // make the local chage up to server
        }
    }
    
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable _hash)
    {
        if (player == targetPlayer)
        {
            UpdatePlayerItem(targetPlayer);
        }
    }

    void UpdatePlayerItem(Player player)
    {
        if (player.CustomProperties.ContainsKey("playerAvatar"))
        {
            playerAvatar.sprite = avatars[(int)player.CustomProperties["playerAvatar"]];
            playerProperties["playerAvatar"] = (int)player.CustomProperties["playerAvatar"];
        }
        else
        {
            playerProperties["playerAvatar"] = 0;
        }
    }
}
