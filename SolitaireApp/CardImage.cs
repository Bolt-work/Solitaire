using SolitaireCore;

namespace SolitaireApp;

public class CardImage : Image
{
    public delegate void CardTappedHandler(object sender, CardTappedEventArgs e);

    public event CardTappedHandler? CardTappedEvent;

    public required  CardImageId CardId { get; set; }

    public CardImage() : base()
    {
        var tapRecognizer = new TapGestureRecognizer();
        tapRecognizer.Tapped += OnImageTapped;
        GestureRecognizers.Add(tapRecognizer);
    }

    protected virtual void OnImageTapped(object? sender, TappedEventArgs e)
    {
        CardTappedEvent?.Invoke(this, new CardTappedEventArgs { CardId = CardId });
    }

}

public class CardTappedEventArgs : EventArgs
{
    public CardImageId? CardId { get; set; } = null;
    public CardPileId? CardPileId { get; set; } = null;
}
