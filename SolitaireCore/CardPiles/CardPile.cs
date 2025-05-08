using SolitaireCore.Cards;
using System;

namespace SolitaireCore.CardPiles;

public class CardPile
{
    public readonly CardPileId Id;
    public readonly CardId PlaceHolderCardId;
    public List<CardId> Contents;

    public CardPile(string id)
    {
        Id = new CardPileId(id);
        PlaceHolderCardId = new CardId(id);
        Contents = new List<CardId>();
        //Contents.Add(CardFactory.CreatePlaceholderCard(Id));
    }

    public void AddCardToTop(CardId card) => Contents.Add(card);
    public void AddCardsToTop(IList<CardId> cards) => Contents.AddRange(cards);
    public IList<CardId> GetAllCards() => Contents.Where(x => !x.Equals(PlaceHolderCardId)).ToList();
    public IList<CardId> GetAllCardsWithPlaceholder() => Contents;
    public int GetIndexOfCardById(CardId cardId) => Contents.FindIndex(cardId.Equals);
    public bool ContainsCardById(CardId cardId) => Contents.Contains(cardId);

    public IList<CardId> GetCardIdsOfCardsAboveCardById(CardId cardId)
    {
        var cardIndex = Contents.FindIndex(x => x == cardId);
        if (cardIndex == -1)
            return new List<CardId>();

        cardIndex++;

        if (cardIndex >= Contents.Count)
            return new List<CardId>();

        List<CardId> aboveCards = Contents.GetRange(cardIndex, Contents.Count - cardIndex);
        return aboveCards;
    }

    public IList<CardId> GetCardIdsOfCardsBelowCardById(CardId cardId)
    {
        var cardIndex = Contents.FindIndex(x => x == cardId);
        return Contents.GetRange(0, cardIndex);
    }

    public IList<CardId> RemoveCardById(CardId cardId) => RemoveCardById(new List<CardId> { cardId });
    public IList<CardId> RemoveCardById(IList<CardId> cardIds)
    {
        foreach (var cardId in cardIds)
        {
            Contents.Remove(cardId);
        }

        return cardIds.ToList();
    }

    public CardId? RemoveTopCard()
    {
        if (Contents.Count < 1)
            return null;

        CardId topCard = Contents[Contents.Count - 1];
        Contents.RemoveAt(Contents.Count - 1);
        return topCard;
    }

    public IList<CardId> RemoveNumberOfTopCards(CardPile cardPile, int cardNo, CardManager cardManager)
    {
        List<CardId> topCards = new List<CardId>();
        if (cardNo >= cardPile.Contents.Count)
        {
            topCards.Add(PlaceHolderCardId);
            return topCards;
        }

        List<CardId> returnedCards = Contents.GetRange(Contents.Count - cardNo, cardNo);
        Contents.RemoveRange(Contents.Count - cardNo, cardNo);

        return returnedCards;
    }

    public void RemoveAllCardButPlaceholder()
    {
        IList<CardId> cardIds = GetAllCards();
        RemoveCardById(cardIds);
    }

    public CardId GetTopCard()
    {
        return Contents[Contents.Count - 1];
    }

    public void Add(CardPile deck) => Add(deck.GetAllCards());

    public void Add(IList<CardId> cardIds)
    {
        Contents.AddRange(cardIds);
    }

    public void ShuffleCardPile()
    {
        Contents.Shuffle();

        var placeholder = Contents.Single(PlaceHolderCardId.Equals);
        Contents.Remove(placeholder);
        Contents.Insert(0, placeholder);
    }

    public bool IsEmpty() => Contents.Count < 1;
    public int Count() => Contents.Count;
}
