using Photon.Pun;
using UnityEngine;

// Update time result to the timeRecordSO
public class ClockScript : MonoBehaviourPunCallbacks
{
    [SerializeField] TimeRecordSO _timeRecordSO;
    [SerializeField] Animator _anim;
    
    float gameTime = 180f;
    bool isPlayingAnim = false;
    bool isActive = false;

    public override void OnEnable()
    {
        Events.onClockCountDown.AddListener(HandleClockEvent);
        Events.onReset.AddListener(Reset);
    }

    private void OnDisable()
    {
        Events.onClockCountDown.RemoveListener(HandleClockEvent);
        Events.onReset.RemoveListener(Reset);
    }

    private void Update()
    {
        if (isActive)
        {
            ClockCountDown();
        }
    }

    private void HandleClockEvent(bool shouldKeepCounting,  bool isNA)
    {
        isActive = shouldKeepCounting;

        if (!shouldKeepCounting)
        {
            gameTime = 180f - gameTime;
            _timeRecordSO.TimeRecordUpdate(gameTime); 
            _anim.Play("New State");
            Events.onCalcDoneChangeColor.Invoke();
            if (isNA)
            {
                Events.onCheckAllPlayerDone.Invoke(PhotonNetwork.LocalPlayer.ActorNumber, GameState.ShowResult);
            }
        }
    }
    private void ClockCountDown()
    {
        if (isPlayingAnim == false)
        {
            _anim.Play("TimeClock");
            isPlayingAnim = true;
        }

        if (gameTime > 0)
        {
            gameTime -= Time.deltaTime;
        }
        else
        {
            Events.onMasterStateChange.Invoke(GameState.TimeUp);
            isActive = false;  // Optionally stop the clock after time is up
        }
    }
    private void Reset()
    {
        gameTime = 180f;
        _anim.Play("New State");
        isPlayingAnim = false;
        isActive = false;
        Events.onResetCount.Invoke();
    }
}
