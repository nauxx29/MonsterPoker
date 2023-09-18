using UnityEngine;

[CreateAssetMenu(menuName = "TimeRecord")]

// for each player record their game time
public class TimeRecordSO : ScriptableObject
{
    private float timeRecord; // it should be private
    public void TimeRecordUpdate(float record)
    {
        timeRecord = record;
    }

    public void TimeRecordClear()
    {
        timeRecord = 0f;
    }

    public string TimeRecordText()
    {
        timeRecord = 180f - timeRecord;
        int min = Mathf.FloorToInt(timeRecord / 60);
        int sec = Mathf.FloorToInt(timeRecord % 60);
        string gameTimeResult = min.ToString() + ":" + sec.ToString();
        return gameTimeResult;
    }

    public string TimeRecordTrans(float time)
    {
        time = 180f - time;
        int min = Mathf.FloorToInt(time / 60);
        int sec = Mathf.FloorToInt(time % 60);
        string gameTimeResult = min.ToString() + ":" + sec.ToString();
        return gameTimeResult;
    }

    public float ReadTimeRecord()
    {
        return timeRecord;
    }
}
