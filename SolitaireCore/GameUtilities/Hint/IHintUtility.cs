using SolitaireCore.Boards;
using SolitaireCore.CardPiles;
using SolitaireCore.Cards;

namespace SolitaireCore.GameUtilities.Hint
{
    public interface IHintUtility
    {
        public IList<CardId>? GetSuggested(Board gameBoard, CardManager cardManager, CardPileManager cardPileManager);
    }
}