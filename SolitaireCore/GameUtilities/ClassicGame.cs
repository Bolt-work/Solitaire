using SolitaireCore.Boards;
using SolitaireCore.CardPiles;
using SolitaireCore.Cards;
using SolitaireCore.GameUtilities.Undo;
using SolitaireCore.GameUtilities.WinDetection;
using SolitaireCore.GameUtilities.AutoMove;
using SolitaireCore.GameUtilities.Hint;
using SolitaireCore.GameUtilities.CardPosition;
using Microsoft.Extensions.Logging;

namespace SolitaireCore.GameEngines;

public class ClassicGame
{
    private readonly ILogger? _logger;
    public delegate void RedrawUiHandler(object sender, RedrawUiEventArgs e);

    public event RedrawUiHandler? RedrawUiHandlerEvent;

    private CardManager _cardManager;
    private CardPileManager _cardPileManager;
    private Board _gameBoard;

    private IUndoUtility _undoManager;
    private IAutoMoveUtility _autoMoveUtility;
    private IHintUtility _hintUtility;
    private IWinDetection _winDetection;
    private ICardPositionUtility _cardPositionUtility;



    public ClassicGame(CardManager cardManager, CardPileManager cardPileManager, Board gameBoard, ILogger? logger)
    {
        _cardManager = cardManager;
        _cardPileManager = cardPileManager;
        _gameBoard = gameBoard;
        _logger = logger;

        _undoManager = new UndoUtility();
        _autoMoveUtility = new AutoMoveUtility();
        _hintUtility = new HintUtility();
        _winDetection = new AllCardsFaceUpWinDetection();
        _cardPositionUtility = new CardPositionUtility();

    }

    public class RedrawUiEventArgs : EventArgs
    {
        public List<CardPileId> PileIds { get; set; } = new List<CardPileId>();
    }


    public Board GameBoard { get { return _gameBoard; } }

    public IList<CardId>? Hint()
    {
        return _hintUtility.GetSuggested(_gameBoard, _cardManager, _cardPileManager);
    }

    public void Undo()
    {
        IList<CardPileId> updatePileIds = _undoManager.Undo(_cardManager, _cardPileManager);
        CallRedrawPileEvent(updatePileIds);
    }

    private void RecordPileHistory(CardPileId selectedCardPileId, CardPileId targetCardPileId)
    {
        _undoManager.RecordPileHistory(selectedCardPileId, targetCardPileId, _cardManager, _cardPileManager);
    }

    public bool AutoMove(CardId cardId)
    {
        var result = _autoMoveUtility.CanMoveToFoundation(cardId, _gameBoard, _cardManager, _cardPileManager);

        if (result is null)
            return false;

        var (selectedCard, targetCard) = result.Value;
        return CardAction(selectedCard, targetCard);
    }

    public bool WinDetection()
    {
        return _winDetection.Detect(_gameBoard, _cardManager, _cardPileManager);
    }

    public (BoardPlacement, int) GetCardPilePositionFromId(CardPileId cardPileId) 
    {
        return _gameBoard.GetCardPilePositionFromId(cardPileId); 
    }

    public CardPosition? GetCardPosition(CardId cardId) 
    {
        return _cardPositionUtility.GetCardPosition(cardId, _gameBoard, _cardPileManager);
    }

    public bool CardAction(CardId selectedCardId, CardId targetCardId)
    {
        CardPosition? selectedCardPos = GetCardPosition(selectedCardId);
        CardPosition? targetCardPos = GetCardPosition(targetCardId);


        if (selectedCardPos is null || targetCardPos is null) 
        {
            _logger?.LogCritical($"CardAction: Card Position Not found: S:{selectedCardPos}, T{targetCardPos}");
            return false;
        }

        // Rules for double clicking remaining pile
        if (selectedCardPos.BoardPlacement == BoardPlacement.RemainingPile && targetCardPos.BoardPlacement == BoardPlacement.RemainingPile)
        {
            return RulesForRemainingPile(selectedCardPos, targetCardPos);
        }
        // Rules for cards moving to foundation piles
        else if (targetCardPos.BoardPlacement == BoardPlacement.Foundation)
        {
            return RulesForFoundationPiles(selectedCardPos, targetCardPos);
        }
        else // Rules for moving cards on Column
        {
            return RulesForColumnPiles(selectedCardPos, targetCardPos);
        }
    }


    private bool RulesForRemainingPile(CardPosition selectedCardPos, CardPosition targetCardPos)
    {
        CardPileModel remainingCards = _cardPileManager.GetCardPileModelById(selectedCardPos.PileId);

        if (remainingCards.Count() > 1)
        {
            CardPileModel stockPile = _cardPileManager.GetCardPileModelById(_gameBoard.PileIdForStock);
            CardPosition? stockCardPosition = GetCardPosition(stockPile.GetTopCardId());

            if (stockCardPosition is null)
            {
                _logger?.LogCritical($"RulesForRemainingPile: Stock Card Position not found");
                return false;
            }

            RecordPileHistory(selectedCardPos.PileId, stockCardPosition.PileId);
            MoveCards(selectedCardPos, stockCardPosition);
            FlipCardsFaceUp(selectedCardPos, targetCardPos);
            CallRedrawPileEvent(selectedCardPos.PileId, stockCardPosition.PileId);
            return true;
        }
        else // No Cards left in remaining pile
        {
            RecordPileHistory(_gameBoard.PileIdForStock, _gameBoard.PileIdForRemaining);
            _cardPileManager.MoveAllCardsToAnotherPile(_gameBoard.PileIdForStock, _gameBoard.PileIdForRemaining);

            IList<CardId> cardIds = _cardPileManager.GetCardPileModelById(_gameBoard.PileIdForRemaining).GetAllCards();
            _cardManager.FlipCardsFaceDown(cardIds);

            CallRedrawPileEvent(_gameBoard.PileIdForStock, _gameBoard.PileIdForRemaining);
            return true;
        }
    }

    private bool RulesForFoundationPiles(CardPosition selectedCardPos, CardPosition targetCardPos) 
    {
        CardModel selectedCard = _cardManager.GetCardModelById(selectedCardPos.CardId);
        CardModel targetCard = _cardManager.GetCardModelById(targetCardPos.CardId);

        // Card must be same suit unless target is placeholder
        if (selectedCard.Suit != targetCard.Suit && !targetCard.IsPlaceholder)
        {
            return false;
        }

        // Card to placed must be one value higher than bottom card
        if (selectedCard.Value != targetCard.Value + 1)
        {
            return false;
        }

        CardMoveValidMoveCard(selectedCardPos, targetCardPos);
        return true;
    }

    private bool RulesForColumnPiles(CardPosition selectedCardPos, CardPosition targetCardPos) 
    {
        CardModel selectedCard = _cardManager.GetCardModelById(selectedCardPos.CardId);
        CardModel targetCard = _cardManager.GetCardModelById(targetCardPos.CardId);

        // Cant move same card - should be a UI check
        if (selectedCardPos.CardId == targetCardPos.CardId)
            return false;

        // Placeholders cant be moved
        if (selectedCard.IsPlaceholder)
            return false;

        // Only kings on placeholder
        if (targetCard.IsPlaceholder && selectedCard.Value != 13)
            return false;

        // Card can not be moved to here
        if (targetCardPos.BoardPlacement == BoardPlacement.StockPile || targetCardPos.BoardPlacement == BoardPlacement.RemainingPile)
            return false;

        // Selected card must be face up unless its in remaining pile
        if (!selectedCard.FacingUp)
            return false;


        // All cards above selected card must be face up
        if (!_cardManager.AreAllCardsFaceUp(selectedCardPos.AboveCards))
            return false;

        // When moving to foundation, card cant be beneath another card
        if (targetCardPos.BoardPlacement == BoardPlacement.Foundation && targetCardPos.AboveCards.Any())
            return false;

        // Selected card must have different colour unless target is placeholder
        if (selectedCard.Colour == targetCard.Colour && !targetCard.IsPlaceholder)
            return false;

        // Selected must be one less than target card unless target is placeholder and selected card is king
        if (targetCard.IsPlaceholder)
        {
            if (selectedCard.Value != 13)
            {
                return false;
            }
        }
        else if (selectedCard.Value + 1 != targetCard.Value)
        {
            return false;
        }

        CardMoveValidMoveCard(selectedCardPos, targetCardPos);
        return true;
    }

    private void CardMoveValidMoveCard(CardPosition selectedCardPos, CardPosition targetCardPos) 
    {
        RecordPileHistory(selectedCardPos.PileId, targetCardPos.PileId);
        MoveCards(selectedCardPos, targetCardPos);
        FlipCardsFaceUp(selectedCardPos, targetCardPos);
        CallRedrawPileEvent(selectedCardPos.PileId, targetCardPos.PileId);
    }

    private void CallRedrawPileEvent(IList<CardPileId> pileIds) => CallRedrawPileEvent(pileIds.ToArray());
    private void CallRedrawPileEvent(params CardPileId[] pileIds) 
    {
        RedrawUiHandlerEvent?.Invoke(this, new RedrawUiEventArgs
        {
            PileIds = pileIds.ToList(),
        });
    }


    private void MoveCards(CardPosition selectedCardPos, CardPosition targetCardPos)
    {
        CardPileModel? selectedCardPile = _cardPileManager.GetCardPileModelById(selectedCardPos.PileId);
        CardPileModel? targetCardPile = _cardPileManager.GetCardPileModelById(targetCardPos.PileId);

        if (selectedCardPos is null || targetCardPos is null)
        {
            _logger?.LogCritical($"MoveCards: Card Position Not found: S:{selectedCardPos}, T{targetCardPos}");
            return;
        }


        List<CardId> moveList = new List<CardId> { selectedCardPos.CardId };
        moveList.AddRange(selectedCardPos.AboveCards);

        _cardPileManager.RemoveCardsFromPileById(selectedCardPile.Id, moveList);
        _cardPileManager.AddCardsToTopOfPileById(targetCardPile.Id, moveList);
    }

    private void FlipCardsFaceUp(CardPosition selectedCardPos, CardPosition targetCardPos)
    {
        if (selectedCardPos.BoardPlacement == BoardPlacement.RemainingPile && targetCardPos.BoardPlacement == BoardPlacement.RemainingPile)
        {
            _cardManager.FlipCardFaceUp(selectedCardPos.CardId);
        }
        else
        {
            CardId? cardBelow = selectedCardPos.BelowCards.LastOrDefault();

            if (cardBelow is null)
                return;

            _cardManager.FlipCardFaceUp(cardBelow);
        }
    }
}





