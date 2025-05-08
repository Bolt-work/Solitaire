using SolitaireCore.CardPiles;
using SolitaireCore.Cards;

namespace SolitaireCore.Boards;

public static class BoardFactory
{
    public static Board ClassicBoard(CardManager cardManager, CardPileManager cardPileManager)
    {
        var cardIds = cardManager.GetRandomizedCardIds();
        Board gameBoard = new Board(cardManager, cardPileManager);

        for (int i = 0; i < gameBoard.tableauNo; i++)
        {
            var pileId = gameBoard.GetPileIdForColumn(i);

            var cardsToAdd = cardIds.GetRange(0, i + 1);
            cardIds.RemoveRange(0, i + 1);

            cardPileManager.AddCardsToTopOfPileById(pileId, cardsToAdd);
        }

        CardPileId remainingPileId = gameBoard.PileIdForRemaining;
        cardPileManager.AddCardsToTopOfPileById(remainingPileId, cardIds);

        FlipTopCards(gameBoard, cardManager, cardPileManager);

        return gameBoard;
    }

    public static Board EasyAcesBoard(CardManager cardManager, CardPileManager cardPileManager) 
    {
        Board gameBoard = ClassicBoard(cardManager, cardPileManager);

        for (int i = 0; i < gameBoard.tableauNo; i++) 
        {
            var index = 1;

            CardPileId pileId = gameBoard.GetPileIdForColumn(i);
            CardPileModel cardPile = cardPileManager.GetCardPileModelById(pileId);

            CardId cardId = cardPile.Contents[index];
            CardModel card = cardManager.GetCardModelById(cardId);

            if (card.Value != 1) 
                continue;

            if (cardPile.Contents.Count > 2) 
            {
                cardPileManager.SwitchCardsInPileById(pileId, index, index + 1);
            }
        }

        cardManager.FlipAllCardsFaceDown();
        FlipTopCards(gameBoard, cardManager, cardPileManager);

        return gameBoard;
    }

    public static Board SwitchLowestValueBoard(CardManager cardManager, CardPileManager cardPileManager)
    {
        Board gameBoard = ClassicBoard(cardManager, cardPileManager);

        for (int i = 0; i < gameBoard.tableauNo; i++)
        {
            var index = 1;

            CardPileId pileId = gameBoard.GetPileIdForColumn(i);
            CardPileModel cardPile = cardPileManager.GetCardPileModelById(pileId);

            CardId cardId = cardPile.Contents[index];
            CardModel card = cardManager.GetCardModelById(cardId);

            if (cardPile.Contents.Count > 2)
            {
                CardId switchCardId = cardPile.Contents[index + 1];
                CardModel switchCard = cardManager.GetCardModelById(switchCardId);
                
                if (switchCard.Value > card.Value)
                    cardPileManager.SwitchCardsInPileById(pileId, index, index + 1);
            }
        }

        cardManager.FlipAllCardsFaceDown();
        FlipTopCards(gameBoard, cardManager, cardPileManager);

        return gameBoard;
    }

    private static void FlipTopCards(Board gameBoard, CardManager cardManager, CardPileManager cardPileManager)
    {
        for (int i = 0; i < gameBoard.tableauNo; i++)
        {
            var pileId = gameBoard.GetPileIdForColumn(i);
            var cardPile = cardPileManager.GetCardPileModelById(pileId);

            var topCardId = cardPile.GetTopCardId();
            cardManager.FlipCardFaceUp(topCardId);
        }
    }
}