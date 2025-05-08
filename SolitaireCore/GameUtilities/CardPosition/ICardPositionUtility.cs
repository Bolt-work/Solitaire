using SolitaireCore.Boards;
using SolitaireCore.CardPiles;

namespace SolitaireCore.GameUtilities.CardPosition
{
    public interface ICardPositionUtility
    {
         CardPosition? GetCardPosition(CardId cardId, Board gameBoard, CardPileManager cardPileManager);
    }
}