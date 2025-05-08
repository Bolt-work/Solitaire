using SolitaireCore.Cards;

namespace SolitaireCore.CardPiles;

public class CardPileManager
{
    private Dictionary<CardPileId, CardPile> PileStore = new Dictionary<CardPileId, CardPile>();
    public CardPileId CreatePile(string id, CardManager cardManager)
    {
        CardPile cardPile = new(id);
        var placeHolderId = cardManager.CreatePlaceholderCard(id);
        cardPile.AddCardToTop(placeHolderId);
        PileStore.Add(cardPile.Id, cardPile);
        return cardPile.Id;
    }

    private CardPile GetCardPileById(CardPileId id) => PileStore[id];
    public CardPileModel GetCardPileModelById(CardPileId id) => new CardPileModel(GetCardPileById(id));
    public IList<CardPileModel> GetCardPileModelById(IList<CardPileId> ids)
    {
        return ids
        .Select(id => PileStore[id])
        .Select(x => new CardPileModel(x))
        .ToList();
    }
    public void AddCardsToTopOfPileById(CardPileId id, IList<CardId> cardIds) => GetCardPileById(id).AddCardsToTop(cardIds);
    public void RemoveCardsFromPileById(CardPileId id, IList<CardId> cardIds) => GetCardPileById(id).RemoveCardById(cardIds);

    public CardPile? FindPileWithCard(CardId cardId)
    {
        foreach (var cardPile in PileStore.Values)
        {
            if (cardPile.ContainsCardById(cardId))
                return cardPile;
        }

        return null;
    }

    public void SetCardPileContents(CardPileId id, List<CardId> contents)
    {
        CardPile cardPile = GetCardPileById(id);
        cardPile.Contents = [.. contents];
    }

    public void ShuffleCardPileById(CardPileId id)
    {
        CardPile cardPile = GetCardPileById(id);
        cardPile.ShuffleCardPile();
    }

    public void MoveAllCardsToAnotherPile(CardPileId selectedCardPileId, CardPileId targetCardPileId)
    {
        CardPile selectedCardPile = GetCardPileById(selectedCardPileId);
        CardPile targetCardPile = GetCardPileById(targetCardPileId);

        targetCardPile.Add(selectedCardPile);
        selectedCardPile.RemoveAllCardButPlaceholder();
    }

    public void SwitchCardsInPileById(CardPileId cardPileId, int firstCardIndex, int secondCardIndex)
    {
        CardPile cardPile = GetCardPileById(cardPileId);

        CardId firstCard = cardPile.Contents[firstCardIndex];
        CardId secondCard = cardPile.Contents[secondCardIndex];

        cardPile.Contents[firstCardIndex] = secondCard;
        cardPile.Contents[secondCardIndex] = firstCard;
    }
}
