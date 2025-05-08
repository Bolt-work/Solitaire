using SolitaireCore.Boards;
using SolitaireCore.CardPiles;
using SolitaireCore.Cards;

namespace SolitaireCore.GameUtilities.WinDetection
{
    public interface IWinDetection
    {
        bool Detect(Board gameBoard, CardManager cardManager, CardPileManager cardPileManager);
    }
}