using UnityEngine;


/// <summary>
/// Script Attach on CardPrefab(GameObject) for storing cardrecordSO's data.
/// Include (1) index number (string) (2) isSelective (bool) (3) card's sprite (sprite)
/// </summary>
public class CardInfo : MonoBehaviour
{
    public string cardIndex = "";
    public bool isSelective = false;

    SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void CardIndexUpdate(string _index)
    {
        cardIndex = _index;
    }

    public void CardIsSelected()
    {
        isSelective  = !isSelective;
    }

    public void SpriteUpdate(Sprite _sprite)
    {
            spriteRenderer.sprite = _sprite;
    }
}
