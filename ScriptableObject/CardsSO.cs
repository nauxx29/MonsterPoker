using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cards")]

public class CardsSO : ScriptableObject
{
    public Sprite _sprite;
    public string _index { get; set; }  // Access only
    public string _suit { get; set; } // Access only

    /// <summary>
    /// Assign sprite name (which containsuit and index value) to the correspond variable for accesss
    /// </summary>
    private void Awake()
    {
        List<string> suitsList = new List<string> { "Spade", "Heart", "Diamond", "Club" };

        string spriteName = _sprite.name;

        foreach (string suit in suitsList)
        {
            if (spriteName.Contains(suit))
            {
                _suit = suit;
                _index = spriteName.Substring(suit.Length);
                if ( _index[0] == '0')
                {
                    _index = _index.Substring(1);
                }
                return;
            }
        }
    }
}
