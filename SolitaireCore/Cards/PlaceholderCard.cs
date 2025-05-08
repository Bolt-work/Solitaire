using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolitaireCore.Cards;

public class PlaceholderCard : Card
{
    public PlaceholderCard(string cardPileId)
        : base(cardPileId, 0, CardColours.Black, Suits.Hearts, true)
    {
        IsPlaceholder = true;
    }
}