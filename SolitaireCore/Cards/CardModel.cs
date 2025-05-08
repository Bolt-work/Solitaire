namespace SolitaireCore.Cards;

public class CardModel
{
    public readonly CardId Id;
    public readonly int Value;
    public readonly CardColours Colour;
    public readonly Suits Suit;
    public readonly bool FacingUp;
    public readonly bool IsPlaceholder;
    public CardModel(CardId id, int value, CardColours colour, Suits suit, bool facingUp, bool isPlaceholder)
    {
        Id = id;
        Value = value;
        Colour = colour;
        Suit = suit;
        FacingUp = facingUp;
        IsPlaceholder = isPlaceholder;
    }

    public CardModel(Card card)
        : this(card.Id,
            card.Value,
            card.Colour,
            card.Suit,
            card.FacingUp,
            card.IsPlaceholder)
    { }
}