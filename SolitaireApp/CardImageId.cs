using SolitaireCore;

namespace SolitaireApp;

public class CardImageId : CoreId
{
    public CardImageId(string value) : base(value)
    {
    }

    public CardImageId(CardId value) : base(value.ToString())
    {
    }
}
