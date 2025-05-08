using SolitaireCore.CardPiles;
using SolitaireCore.Cards;

namespace SolitaireCore.GameUtilities.Undo;

public interface IUndoUtility
{
    IList<CardPileId> Undo(CardManager cardManager, CardPileManager cardPileManager);
    void RecordPileHistory(CardPileId selectedCardPileId, CardPileId targetCardPileId, CardManager cardManager, CardPileManager cardPileManager);
}