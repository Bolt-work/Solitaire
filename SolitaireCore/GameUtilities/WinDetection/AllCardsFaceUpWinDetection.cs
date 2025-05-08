using SolitaireCore.Boards;
using SolitaireCore.CardPiles;
using SolitaireCore.Cards;

namespace SolitaireCore.GameUtilities.WinDetection;

public class AllCardsFaceUpWinDetection : IWinDetection
{
    public bool Detect(Board gameBoard, CardManager cardManager, CardPileManager cardPileManager)
    {
        CardPileModel remainingPile = cardPileManager.GetCardPileModelById(gameBoard.PileIdForRemaining);
        if (remainingPile.GetCardsWithoutPlaceholder().Any())
            return false;

        CardPileModel stockPile = cardPileManager.GetCardPileModelById(gameBoard.PileIdForStock);
        if (stockPile.GetCardsWithoutPlaceholder().Any())
            return false;

        // All cards face up
        for (int i = 0; i < gameBoard.foundationNo; i++)
        {
            CardPileId cardPileId = gameBoard.GetPileIdForFoundation(i);
            CardPileModel cardPile = cardPileManager.GetCardPileModelById(cardPileId);

            if (!cardManager.AreAllCardsFaceUp(cardPile.GetCardsWithoutPlaceholder()))
            {
                return false;
            }
        }

        return true;
    }
}
