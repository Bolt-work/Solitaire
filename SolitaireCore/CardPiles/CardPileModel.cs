using System.Collections.ObjectModel;

namespace SolitaireCore.CardPiles;

public class CardPileModel
{
    public readonly CardPileId Id;
    public readonly CardId PlaceHolderCardId;
    public readonly IList<CardId> Contents;

    public CardPileModel(CardPileId id, CardId placeHolderCardId, List<CardId> contents)
    {
        Id = id;
        PlaceHolderCardId = placeHolderCardId;
        Contents = new ReadOnlyCollection<CardId>(contents);
    }

    public CardPileModel(CardPile cardPile)
        : this(cardPile.Id, cardPile.PlaceHolderCardId, cardPile.Contents) { }

    public IList<CardId> GetCardsWithoutPlaceholder() => Contents.Where(x => !x.Equals(PlaceHolderCardId)).ToList();
    public IList<CardId> GetCardsWithPlaceholder() => Contents;

    public CardId GetTopCardId()
    {
        return Contents[Contents.Count - 1];
    }

    public IList<CardId> GetAllCards() => Contents.Where(x => !x.Equals(PlaceHolderCardId)).ToList();
    public int Count() => Contents.Count;
}
