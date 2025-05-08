namespace SolitaireCore.Cards;

public class CardManager
{
    // A = 1
    // J = 11
    // Q = 12
    // k = 13

    private static int MAXCARDVALUE = 13;
    private Dictionary<CardId, Card> CardStore = new Dictionary<CardId, Card>();

    public CardManager()
    {
        PopulateFullDeck();
    }

    private IList<Card> GetCardById(IList<CardId> ids)
    {
        return ids.Select(k => CardStore[k]).ToList();
    }

    public CardModel GetCardModelById(CardId id)
    {
        Card card = GetCardById(id);
        return new CardModel(card);
    }

    public IList<CardModel> GetCardModelById(IList<CardId> ids)
    {
        IList<Card> cards = GetCardById(ids);
        return cards.Select(x => new CardModel(x)).ToList();
    }

    private Card GetCardById(CardId id) => CardStore[id];

    private void PopulateDeckOfOneSuit(Suits suit, CardColours colour)
    {
        for (int value = 1; value <= MAXCARDVALUE; value++)
        {
            var id = CreateId(value, suit);
            var card = new Card(id, value, colour, suit, true);
            CardStore.Add(card.Id, card);
        }
    }

    private static string CreateId(int cardValue, Suits suit)
    {
        string noValue = ConvertCardValueToString(cardValue);
        string suitName = ConvertSuitToString(suit);

        return $"card_{suitName}_{noValue}";
    }

    private static string ConvertCardValueToString(int value)
    {
        if (value == 1)
            return "a";

        if (value == 10)
            return "10";

        if (value == 11)
            return "j";

        if (value == 12)
            return "q";

        if (value == 13)
            return "k";


        return "0" + value.ToString();
    }

    private static string ConvertSuitToString(Suits suit)
    {
        return suit switch
        {
            Suits.Clubs => "clubs",
            Suits.Diamonds => "diamonds",
            Suits.Hearts => "hearts",
            Suits.Spades => "spades",
            _ => throw new Exception($"No match for Suit {suit}")
        };
    }

    private void PopulateFullDeck()
    {
        PopulateDeckOfOneSuit(Suits.Spades, CardColours.Black);
        PopulateDeckOfOneSuit(Suits.Clubs, CardColours.Black);
        PopulateDeckOfOneSuit(Suits.Hearts, CardColours.Red);
        PopulateDeckOfOneSuit(Suits.Diamonds, CardColours.Red);
    }

    public CardId CreatePlaceholderCard(string cardPileId)
    {
        var placeholder = new PlaceholderCard(cardPileId);
        CardStore.Add(new CardId(cardPileId), placeholder);

        return placeholder.Id;
    }


    public List<CardId> GetRandomizedCardIds()
    {
        List<CardId> cardIds = CardStore.Values
            .Where(card => !card.IsPlaceholder)
            .Select(card => card.Id)
            .ToList();

        cardIds.Shuffle();
        return cardIds;
    }

    public IList<Card> GetAllCards() => CardStore.Values.ToList();
    public IList<CardModel> GetAllCardModels()
    {
        IList<Card> cards = GetAllCards();
        return cards.Select(x => new CardModel(x)).ToList();
    }

    public bool AreAllCardsFaceUp(IList<CardId> cardIds)
    {
        foreach (CardId cardId in cardIds)
        {
            Card card = CardStore[cardId];

            if (!card.FacingUp)
                return false;
        }

        return true;
    }

    public void SetCardFacing(CardId cardId, bool facingUp) 
    {
        Card card = GetCardById(cardId);
        card.FacingUp = facingUp;
    }

    public void FlipCardFaceUp(CardId cardId) => SetCardFacing(cardId, true);


    public void FlipAllCardsFaceDown()
    {
        foreach (var card in GetAllCards())
        {
            card.FacingUp = false;
        }
    }

    public void FlipCardsFaceDown(IList<CardId> cardIds)
    {
        foreach (CardId cardId in cardIds)
        {
            Card card = GetCardById(cardId);
            card.FacingUp = false;
        }
    }

    public IList<(CardId,bool)> GetCardsFacing(IList<CardId> cardIds) 
    {
        return cardIds
        .Select(cardId => (cardId, GetCardById(cardId).FacingUp))
        .ToList();
    }
}
