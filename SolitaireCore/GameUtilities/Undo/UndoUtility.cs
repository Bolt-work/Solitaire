using SolitaireCore.CardPiles;
using SolitaireCore.Cards;
using SolitaireCore.GameUtilities.Undo;

namespace SolitaireCore.GameUtilities.Undo;

public class UndoUtility : IUndoUtility
{
    private IList<PileState> _pileStateHistory = new List<PileState>();

    public IList<CardPileId> Undo(CardManager cardManager, CardPileManager cardPileManager)
    {
        if (!_pileStateHistory.Any())
            return new List<CardPileId>();

        PileState lastMove = _pileStateHistory.Last();
        _pileStateHistory.Remove(lastMove);


        cardPileManager.SetCardPileContents(lastMove.selectedCardPileId, lastMove.selectedCardPileContent.ToList());
        cardPileManager.SetCardPileContents(lastMove.targetCardPileId, lastMove.targetCardPileContent.ToList());

        foreach (var (id, facing) in lastMove.CardsFacing)
        {
            cardManager.SetCardFacing(id, facing);
        }

        return new List<CardPileId> { lastMove.selectedCardPileId, lastMove.targetCardPileId };
    }

    public void RecordPileHistory(CardPileId selectedCardPileId, CardPileId targetCardPileId, CardManager cardManager, CardPileManager cardPileManager)
    {
        var selectedCardPile = cardPileManager.GetCardPileModelById(selectedCardPileId);
        var targetCardPile = cardPileManager.GetCardPileModelById(targetCardPileId);


        List<CardId> cardIds = selectedCardPile.Contents
                                    .Concat(targetCardPile.Contents)
                                    .ToList();

        var record = new PileState(selectedCardPile.Id,
            [.. selectedCardPile.Contents],
            targetCardPile.Id,
            [.. targetCardPile.Contents],
            cardManager.GetCardsFacing(cardIds));

        _pileStateHistory.Add(record);
    }


    private record PileState(CardPileId selectedCardPileId,
            IList<CardId> selectedCardPileContent,
            CardPileId targetCardPileId,
            IList<CardId> targetCardPileContent,
            IList<(CardId, bool)> CardsFacing);
}
