using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolitaireCore;

public class SolitaireException : Exception
{
    public SolitaireException(string message):
        base(message){}
}

public class BoardSizeInvalidException : SolitaireException
{
   public BoardSizeInvalidException(string message) :
        base(message){}
}

public class NoCardsInPileInvalidException : SolitaireException
{
    public NoCardsInPileInvalidException(string message) :
         base(message)
    { }
}