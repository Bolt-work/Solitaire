namespace SolitaireCore.Cards;

public class Card
{
    // A = 1
    // J = 11
    // Q = 12
    // k = 13

    public CardId Id;
    public int Value;
    public CardColours Colour;
    public Suits Suit;
    public bool FacingUp;
    public bool IsPlaceholder;
    public Card(string id, int value, CardColours colour, Suits suit, bool facingUp)
    {
        Id = new CardId(id);
        Value = value;
        Colour = colour;
        Suit = suit;
        FacingUp = facingUp;
        IsPlaceholder = false;
    }

    public void SetToFaceUp() => FacingUp = true;
}
