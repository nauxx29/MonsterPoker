using UnityEngine;

[CreateAssetMenu(menuName = "MuteButton")]

// Record isMute or not
public class MuteButtonSO : ScriptableObject
{

    private bool isMute = false;
    private void OnEnable()
    {
        isMute = false;
    }
    public void MuteToggle()
    {
        isMute = !isMute;
    }

    public bool ReadIsMute()
    {
        return isMute;
    }
}


