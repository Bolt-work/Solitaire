using SolitaireCore;
using SolitaireCore.Cards;
using static SolitaireApp.CardImage;

namespace SolitaireApp;

public class CardImageManager
{
    private IDictionary<CardImageId, CardImage> _cardImages;

    public CardImageManager(IList<CardModel> cards)
    {
        _cardImages = CreateCardImages(cards);
    }

    private IDictionary<CardImageId, CardImage> CreateCardImages(IList<CardModel> cards)
    {
        Dictionary<CardImageId, CardImage> cardImages = new();
        foreach (CardModel card in cards)
        {
            if (card.IsPlaceholder)
            {
                cardImages.Add(new CardImageId(card.Id), ConstructCardPlaceholder(card.Id.ToString()));
            }
            else
            {
                CardImage cardImage = new CardImage
                {
                    CardId = new CardImageId(card.Id),
                    AutomationId = card.Id.ToString(),
                    Source = CreateSource(card),
                    Aspect = Aspect.AspectFit
                };

                cardImages.Add(new CardImageId(card.Id), cardImage);
            }
        }

        return cardImages;
    }

    private string CreateSource(CardModel card)
    {
        if (card.FacingUp)
            return $"{card.Id}.png";

        return "card_back.png";
    }
    private CardPlaceholderImage ConstructCardPlaceholder(string cardPileId)
    {
        return new CardPlaceholderImage
        {
            CardId = new CardImageId(cardPileId),
            AutomationId = cardPileId,
            Source = "card_placeholder.png",
            Aspect = Aspect.AspectFit
        };
    }

    public void UpdateCardImages(IList<CardModel> cards)
    {
        foreach (CardModel card in cards)
        {
            CardImageId cardImageId = new CardImageId(card.Id);
            CardImage? cardImage = GetCardImageById(cardImageId);

            if (cardImage is null)
                throw new Exception($"Card id not found in CardImage list id : {card.Id}");

            cardImage.Source = CreateSource(card);
        }
    }

    public CardImage? GetCardImageById(CoreId id) => _cardImages[new CardImageId(id.ToString())];

    public void RegisterCardTappedEventHandlers(CardTappedHandler eventHandler)
    {
        foreach (CardImage card in _cardImages.Values)
        {
            card.CardTappedEvent += eventHandler;
        }
    }
}
