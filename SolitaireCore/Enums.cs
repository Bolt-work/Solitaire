using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolitaireCore
{
    public enum CardColours
    {
        Black = 0,
        Red = 1
    }

    public enum Suits
    {
        Spades,
        Clubs, 
        Hearts, 
        Diamonds
    }

    public enum BoardPlacement 
    {
        Foundation,
        Column,
        RemainingPile,
        StockPile
    }
}
