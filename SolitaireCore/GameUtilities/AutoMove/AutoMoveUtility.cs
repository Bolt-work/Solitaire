using SolitaireCore.Boards;
using SolitaireCore.CardPiles;
using SolitaireCore.Cards;

namespace SolitaireCore.GameUtilities.AutoMove;

public class AutoMoveUtility : IAutoMoveUtility
{
    public (CardId SelectedCard, CardId TargetCard)? CanMoveToFoundation(CardId cardId, Board gameBoard, CardManager cardManager, CardPileManager cardPileManager)
    {
        CardModel card = cardManager.GetCardModelById(cardId);

        for (int i = 0; i < gameBoard.foundationNo; i++)
        {
            CardPileId cardPileId = gameBoard.GetPileIdForFoundation(i);
            CardPileModel cardPile = cardPileManager.GetCardPileModelById(cardPileId);

            CardModel topCard = cardManager.GetCardModelById(cardPile.GetTopCardId());

            if (topCard.Value + 1 == card.Value && (topCard.IsPlaceholder || topCard.Suit == card.Suit))
            {
                return (cardId, topCard.Id);
            }
        }

        return null;
    }
}

