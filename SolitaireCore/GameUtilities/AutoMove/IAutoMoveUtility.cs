using SolitaireCore.Boards;
using SolitaireCore.CardPiles;
using SolitaireCore.Cards;

namespace SolitaireCore.GameUtilities.AutoMove;

public interface IAutoMoveUtility
{
    (CardId SelectedCard, CardId TargetCard)? CanMoveToFoundation(CardId cardId, Board gameBoard, CardManager cardManager, CardPileManager cardPileManager);
}