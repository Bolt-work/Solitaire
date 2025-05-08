using Microsoft.Maui.Layouts;
using SolitaireCore;
using SolitaireCore.CardPiles;

namespace SolitaireApp;

public static class BoardRenderer
{
    private static int _cardWidth = (int)(72 / 1.3);
    private static int _cardHeight = (int)(72 / 1.3);//96
    private static double _cardDrawOverlap = 1.8;

    public static void DrawFoundationCardPile(CardPileModel cardPile, int foundationBoardSlot, CardImageManager cardImageManager, Layout layout)
    {
        CardImage? cardImage = GetTopCardImageFromPile(cardPile, cardImageManager);

        if (cardImage != null)
            AddCardToFoundationAbsoluteLayout(layout, cardImage, foundationBoardSlot);
    }

    public static void DrawRemainingCardPile(CardPileModel cardPile, CardImageManager cardImageManager, Layout layout)
    {
        CardImage? cardImage = GetTopCardImageFromPile(cardPile, cardImageManager);

        if (cardImage != null)
            AddCardToFoundationAbsoluteLayout(layout, cardImage, 6);
    }

    public static void DrawStockCardPile(CardPileModel cardPile, CardImageManager cardImageManager, Layout layout)
    {
        CardImage? cardImage = GetTopCardImageFromPile(cardPile, cardImageManager);

        if (cardImage != null)
            AddCardToFoundationAbsoluteLayout(layout, cardImage, 5);
    }

    public static void DrawMainBoardCardPile(CardPileModel cardPile, int boardSlot, CardImageManager cardImageManager, Layout layout)
    {
        CardImage? placeholderImage = cardImageManager.GetCardImageById(cardPile.PlaceHolderCardId);

        if (placeholderImage != null)
            AddCardToBoardAbsoluteLayout(layout, placeholderImage, boardSlot, 0);

        IList<CardId> cardsInPile = cardPile.GetCardsWithoutPlaceholder();

        for (int cardPosition = 0; cardPosition < cardsInPile.Count; cardPosition++)
        {
            CardId cardId = cardsInPile[cardPosition];
            CardImage? cardImage = cardImageManager.GetCardImageById(cardId);

            if (cardImage != null)
                AddCardToBoardAbsoluteLayout(layout, cardImage, boardSlot, cardPosition);
        }
    }

    private static CardImage? GetTopCardImageFromPile(CardPileModel cardPile, CardImageManager cardImageManager)
    {
        CardId? topCardId = cardPile.GetTopCardId();

        if (topCardId is null)
            return null;

        return cardImageManager.GetCardImageById(topCardId);
    }

    public static void ReDrawCardPile(CardPileModel cardPile, BoardPlacement placement, int slotNo, CardImageManager cardImageManager, Layout layout)
    {
        switch (placement)
        {
            case BoardPlacement.Foundation:
                DrawFoundationCardPile(cardPile, slotNo, cardImageManager, layout);
                break;

            case BoardPlacement.Column:
                DrawMainBoardCardPile(cardPile, slotNo, cardImageManager, layout);
                break;

            case BoardPlacement.RemainingPile:
                DrawRemainingCardPile(cardPile, cardImageManager, layout);
                break;

            case BoardPlacement.StockPile:
                DrawStockCardPile(cardPile, cardImageManager, layout);
                break;

            default:
                throw new Exception($"CardPile with Id not found id : {cardPile.Id}");
        }
    }

    public static void AddCardToFoundationAbsoluteLayout(Layout layout, Image cardImage, int boardSlot)
    {

        var x = (boardSlot * _cardWidth);

        if (boardSlot == 5)
        {
            // could add a 3 card spread
            x = (boardSlot * _cardWidth) - 5;
        }

        var y = 20;
        AddCardToAbsoluteLayout(layout, cardImage, x, y);
    }

    public static void AddCardToBoardAbsoluteLayout(Layout layout, Image cardImage, int boardSlot, int cardPosition)
    {
        var x = boardSlot * _cardWidth;
        int y = (_cardHeight * 2) + (int)(cardPosition * (_cardHeight / _cardDrawOverlap));
        AddCardToAbsoluteLayout(layout, cardImage, x, y);
    }

    public static void AddCardToAbsoluteLayout(Layout layout, Image cardImage, int x, int y)
    {
        cardImage.Aspect = Aspect.Fill;
        AbsoluteLayout.SetLayoutBounds(cardImage, new Rect(x, y, _cardWidth, _cardHeight));
        AbsoluteLayout.SetLayoutFlags(cardImage, AbsoluteLayoutFlags.None);
        layout.Children.Remove(cardImage);
        layout.Children.Add(cardImage);
    }
}
