using SolitaireCore.CardPiles;
using SolitaireCore.Cards;

namespace SolitaireCore.Boards;

public class Board
{
    private Dictionary<string, CardPileId> _slots;

    private string _foundationKey = "F";
    private string _tableauKey = "T";
    private string _remainingKey = "R";
    private string _stockKey = "S";

    private string _cardPileTag = "cardPile:";

    public readonly int foundationNo = 4;
    public readonly int tableauNo = 7;
    public readonly int remainingNo = 1;
    public readonly int stockNo = 1;

    public Board(CardManager cardManager, CardPileManager cardPileManager)
    {
        _slots = new Dictionary<string, CardPileId>();
        Setup(cardManager, cardPileManager);
    }

    private void Setup(CardManager cardManager, CardPileManager cardPileManager)
    {
        PopulatePiles(foundationNo, _foundationKey, cardManager, cardPileManager);
        PopulatePiles(tableauNo, _tableauKey, cardManager, cardPileManager);
        PopulatePiles(remainingNo, _remainingKey, cardManager, cardPileManager);
        PopulatePiles(stockNo, _stockKey, cardManager, cardPileManager);
    }

    private void PopulatePiles(int size, string key, CardManager cardManager, CardPileManager cardPileManager)
    {
        for (int i = 0; i < size; i++)
        {
            string slotKey = key + i;
            string id = BuildCardPileIdName(i, key);
            _slots.Add(slotKey, cardPileManager.CreatePile(id, cardManager));
        }
    }

    public CardPileId GetPileIdForFoundation(int slotNo)
    {
        return _slots[_foundationKey + slotNo];
    }

    public IList<CardPileId> GetAllPileIdsForFoundations()
    {
        return Enumerable.Range(0, foundationNo)
                         .Select(i => _slots[_foundationKey + i])
                         .ToList();
    }

    public CardPileId PileIdForRemaining => _slots[_remainingKey + 0];
    public CardPileId PileIdForStock => _slots[_stockKey + 0];

    public CardPileId GetPileIdForColumn(int slotNo)
    {
        return _slots[_tableauKey + slotNo];
    }

    public IList<CardPileId> GetAllPileIdsForColumns()
    {
        return Enumerable.Range(0, tableauNo)
                         .Select(i => _slots[_tableauKey + i])
                         .ToList();
    }


    public string BuildCardPileIdName(int slotNo, string key)
    {
        return _cardPileTag + key + slotNo;
    }

    public (BoardPlacement, int) GetCardPilePositionFromId(CardPileId cardPileId)
    {
        string cardPileName = cardPileId.ToString();
        string nameWithOutTag = cardPileName.Substring(_cardPileTag.Length);
        string placementCharacter = nameWithOutTag[0].ToString();
        BoardPlacement boardPlacement = BoardPlacementFromCharacter(placementCharacter);
        int slotNo = int.Parse(nameWithOutTag[1].ToString());

        return (boardPlacement, slotNo);
    }

    private BoardPlacement BoardPlacementFromCharacter(string placementTag)
    {
        if (placementTag.Equals(_foundationKey))
            return BoardPlacement.Foundation;

        if (placementTag.Equals(_remainingKey))
            return BoardPlacement.RemainingPile;

        if (placementTag.Equals(_stockKey))
            return BoardPlacement.StockPile;

        if (placementTag.Equals(_tableauKey))
            return BoardPlacement.Column;

        throw new Exception($"Unknown placementTag : {placementTag}");
    }
}
