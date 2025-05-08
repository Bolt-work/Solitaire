using SolitaireCore.Boards;
using SolitaireCore.CardPiles;
using SolitaireCore.Cards;

namespace SolitaireCore.GameUtilities.WinDetection;

public class AllCardsInFoundationsWinDetection : IWinDetection
{
    public bool Detect(Board gameBoard, CardManager cardManager, CardPileManager cardPileManager)
    {
        for (int i = 0; i < gameBoard.foundationNo; i++)
        {
            CardPileId cardPileId = gameBoard.GetPileIdForFoundation(i);
            CardPileModel cardPile = cardPileManager.GetCardPileModelById(cardPileId);

            CardModel topCard = cardManager.GetCardModelById(cardPile.GetTopCardId());

            if (topCard.Value != 13)
                return false;
        }

        return true;
    }
}
