using SolitaireCore.Boards;
using SolitaireCore.CardPiles;

namespace SolitaireCore.GameUtilities.CardPosition;

public class CardPositionUtility : ICardPositionUtility
{
    public CardPosition? GetCardPosition(CardId cardId, Board gameBoard, CardPileManager cardPileManager)
    {
        CardPile? cardPile = cardPileManager.FindPileWithCard(cardId);

        if (cardPile is null)
            return null;

        var (placement, slotNo) = gameBoard.GetCardPilePositionFromId(cardPile.Id);
        int cardPosition = cardPile.GetIndexOfCardById(cardId);
        IList<CardId> aboveCards = cardPile.GetCardIdsOfCardsAboveCardById(cardId);
        IList<CardId> belowCards = cardPile.GetCardIdsOfCardsBelowCardById(cardId);

        return new CardPosition(placement, cardPile.Id, slotNo, cardPosition, cardId, aboveCards, belowCards);
    }
}

public class CardPosition
{
    public CardPosition(BoardPlacement boardPlacement, CardPileId pileId, int positionOfPile, int positionInPile, CardId cardId, IList<CardId> aboveCards, IList<CardId> belowCards)
    {
        BoardPlacement = boardPlacement;
        PileId = pileId;
        PositionOfPile = positionOfPile;
        PositionInPile = positionInPile;
        CardId = cardId;
        AboveCards = aboveCards;
        BelowCards = belowCards;
    }

    public BoardPlacement BoardPlacement { get; set; }
    public CardPileId PileId { get; set; }
    public int PositionOfPile { get; set; }
    public int PositionInPile { get; set; }
    public CardId CardId { get; set; }
    public IList<CardId> AboveCards { get; set; }
    public IList<CardId> BelowCards { get; set; }
}
