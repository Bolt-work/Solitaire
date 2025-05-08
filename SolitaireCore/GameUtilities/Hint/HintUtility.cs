using SolitaireCore.Boards;
using SolitaireCore.CardPiles;
using SolitaireCore.Cards;

namespace SolitaireCore.GameUtilities.Hint;

public class HintUtility : IHintUtility
{

    public IList<CardId>? GetSuggested(Board gameBoard, CardManager cardManager, CardPileManager cardPileManager)
    {
        /*
         * check bottom most card 
            check foundation
            check stock card
            get new card
         */

        IList<CardPileId> foundationCardPileIds = gameBoard.GetAllPileIdsForFoundations();
        List<CardModel> foundationTopCards = foundationCardPileIds
                                .Select(cardPileId => cardPileManager.GetCardPileModelById(cardPileId).GetTopCardId())
                                .Select(cardManager.GetCardModelById)
                                .ToList();


        IList<CardPileId> tableauCardPileIds = gameBoard.GetAllPileIdsForColumns();
        IList<CardPileModel> tableauCardPile = cardPileManager.GetCardPileModelById(tableauCardPileIds);

        List<List<CardModel>> tableauAvailableCards = new();
        foreach (CardPileModel cardPileModel in tableauCardPile)
        {
            List<CardModel> cardModels = cardPileModel.Contents
                                            .Select(cardId => cardManager.GetCardModelById(cardId))
                                            .Where(cardModel => cardModel.FacingUp)
                                            //.Where(cardModel => !cardModel.IsPlaceholder)
                                            .ToList();

            tableauAvailableCards.Add(cardModels);
        }

        /// Check if bottom most card can be moved
        List<CardModel> tableauBottomAvailableCards = tableauAvailableCards
                                                        .Where(cardList => cardList.Any())
                                                        .Select(cardList => cardList.First())
                                                        .Where(cardModel => !cardModel.IsPlaceholder)
                                                        .ToList();

        List<CardModel> tableauTopAvailableCards = tableauAvailableCards
                                                        .Where(cardList => cardList.Any())
                                                        .Select(cardList => cardList.Last())
                                                        //.Where(cardModel => !cardModel.IsPlaceholder)
                                                        .ToList();


        /// Check if bottom most cards in tableau can be moved in tableau
        foreach (CardModel bottomCard in tableauBottomAvailableCards)
        {
            foreach (CardModel topCard in tableauTopAvailableCards)
            {
                // King and placeholder check
                if (topCard.IsPlaceholder && bottomCard.Value == 13)
                {
                    return new List<CardId> { bottomCard.Id, topCard.Id };
                }

                if (bottomCard.Value + 1 == topCard.Value && bottomCard.Colour != topCard.Colour)
                {
                    return new List<CardId> { bottomCard.Id, topCard.Id };
                }
            }
        }

        /// Check if cards can be moved to foundation
        foreach (CardModel topCard in tableauTopAvailableCards)
        {
            foreach (CardModel foundationCard in foundationTopCards)
            {
                if (topCard.Value == foundationCard.Value + 1 && (topCard.Suit == foundationCard.Suit || foundationCard.IsPlaceholder))
                {
                    return new List<CardId> { topCard.Id, foundationCard.Id };
                }
            }
        }

        /// If stock card check if card can be added
        CardPileId stockCardPileId = gameBoard.PileIdForStock;
        CardPileModel stockPile = cardPileManager.GetCardPileModelById(stockCardPileId);
        CardModel stockTopCard = cardManager.GetCardModelById(stockPile.GetTopCardId());

        if (!stockTopCard.IsPlaceholder)
        {
            // This is dup code !!!
            foreach (CardModel topCard in tableauTopAvailableCards)
            {
                // King and placeholder check
                if (topCard.IsPlaceholder && stockTopCard.Value == 13)
                {
                    return new List<CardId> { stockTopCard.Id, topCard.Id };
                }

                if (stockTopCard.Value + 1 == topCard.Value && stockTopCard.Colour != topCard.Colour)
                {
                    return new List<CardId> { stockTopCard.Id, topCard.Id };
                }
            }

            foreach (CardModel foundationCard in foundationTopCards)
            {
                if (stockTopCard.Value == foundationCard.Value + 1 && (foundationCard.IsPlaceholder || stockTopCard.Suit == foundationCard.Suit))
                {
                    return new List<CardId> { stockTopCard.Id, foundationCard.Id };
                }
            }
        }

        /// Check if any remaining else return none
        CardPileId remainingCardPileId = gameBoard.PileIdForRemaining;
        CardPileModel remainingPile = cardPileManager.GetCardPileModelById(remainingCardPileId);
        CardModel remainingTopCard = cardManager.GetCardModelById(remainingPile.GetTopCardId());

        if (!remainingTopCard.IsPlaceholder || !stockTopCard.IsPlaceholder)
        {
            return new List<CardId> { remainingTopCard.Id };
        }

        // No available moves
        return null;
    }
}
