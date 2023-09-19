using System;
using UnityEngine;

public static class Events
{
    // for AS
    public static readonly Evt onOnChangeAudio = new Evt();

    // for Photon
    public static readonly Evt<GameState> onMasterStateChange = new Evt<GameState>();
    public static readonly Evt<GameState> onLocalStateChange = new Evt<GameState>();

    // for game
    public static readonly Evt onDeal = new Evt();
    public static readonly Evt onCalc = new Evt();
    public static readonly Evt<string, bool> onCalcUpdate = new Evt<string, bool>();
    public static readonly Evt onCalcRemove = new Evt();
    public static readonly Evt onCalcClear = new Evt();
    public static readonly Evt onCalcAnswer = new Evt();
    public static readonly Evt onCalcResult = new Evt();
    public static readonly Evt onRemove = new Evt();
    public static readonly Evt onClear = new Evt();
    public static readonly Evt onAnswer = new Evt();

    // for game result
    public static readonly Evt<bool, bool> onClockCountDown = new Evt<bool, bool>();
    public static readonly Evt onCalcDoneChangeColor = new Evt();
    public static readonly Evt onTimeUpShow = new Evt();
    public static readonly Evt<bool> onWaiting = new Evt<bool>();
    public static readonly Evt onResultShow = new Evt();

    // for reset or next round
    public static readonly Evt onReset = new Evt();
    public static readonly Evt onResetCount = new Evt();  
    public static readonly Evt onEndResult = new Evt();
    public static readonly Evt onNewGame = new Evt();
    public static readonly Evt onNewGameDone = new Evt();
    public static readonly Evt<int, GameState> onCheckAllPlayerDone = new Evt<int, GameState>();
}

public class Evt
{
    private event Action _action = delegate { };

    public void Invoke()
    {
        _action.Invoke();
    }

    public void AddListener(Action listener)
    {
        _action += listener;
    }

    public void RemoveListener(Action listener)
    {
        _action -= listener;
    }
}

public class Evt<T>
{
    private event Action<T> _action = delegate { };

    public void Invoke(T param)
    {
        _action.Invoke(param);
    }

    public void AddListener(Action<T> listener)
    {
        _action += listener;
    }

    public void RemoveListener(Action<T> listener)
    {
        _action -= listener;
    }

}

public class Evt<T1, T2>
{
    private event Action<T1, T2> _action = delegate { };

    public void Invoke(T1 param1, T2 param2)
    {
        _action.Invoke(param1, param2);
    }

    public void AddListener(Action<T1, T2> listener)
    {
        _action += listener;
    }

    public void RemoveListener(Action<T1, T2> listener)
    {
        _action -= listener;
    }
}
