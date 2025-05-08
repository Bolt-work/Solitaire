using Microsoft.Extensions.Logging;
using Microsoft.Maui.Layouts;
using SolitaireCore;
using SolitaireCore.Boards;
using SolitaireCore.CardPiles;
using SolitaireCore.Cards;
using SolitaireCore.GameEngines;
using SolitaireCore.GameUtilities.AutoMove;
using SolitaireCore.GameUtilities.CardPosition;
using SolitaireCore.GameUtilities.Hint;
using SolitaireCore.GameUtilities.Undo;
using static SolitaireCore.GameEngines.ClassicGame;

namespace SolitaireApp.Views
{
    public partial class GamePage : ContentPage
    {
        private readonly ILogger? _logger;

        private CardManager _cardManager = null!;
        private CardPileManager _cardPileManager = null!;
        private CardImageManager _cardImageManager = null!;
        private ClassicGame _game = null!;
        private AbsoluteLayout _absoluteLayout = null!;

        private CardImageId? _cardSelectedId = null;
        private Image _greenSelector;
        private Image _redSelector;
        private HighlightSelectorManager _selectorManager;


        public GamePage() : this(null) { }
        public GamePage(ILogger<GamePage>? logger)
        {
            _logger = logger;
            SetupGame();

            _selectorManager = new HighlightSelectorManager();
            _greenSelector = _selectorManager.CreateGreenSelector();
            _redSelector = _selectorManager.CreateRedSelector();


            InitializeComponent();

            FirstDraw();
        }

        private async void NewGame()
        {
            var result = await DisplayAlert("New Game", "Are you sure", "Confirm", "Cancel");
            if (result)
            {
                _absoluteLayout.Children.Clear();
                SetupGame();
                FirstDraw();
            }
        }

        private async void Hint()
        {
            IList<CardId>? hintCards = _game.Hint();

            if (hintCards is null)
            {
                await DisplayAlert("Hint", "no more", "Confirm", "Cancel");
                return;
            }

            _selectorManager.ClearYellowSelectors(_absoluteLayout);
            foreach (CardId cardId in hintCards)
            {
                CardPosition? cardPos = _game.GetCardPosition(cardId);

                if (cardPos is null)
                {
                    _logger?.LogError($"Hint: Card position not found :: {cardId}");
                    continue;
                }

                Image yellowSelector = _selectorManager.GetYellowSelector();
                AddCardColourSelector(yellowSelector, cardPos);
            }
        }

        private async void ExitApplication()
        {
            var result = await DisplayAlert("Exit Game", "Are you sure", "Confirm", "Cancel");

            if (result)
            {
                Application.Current.Quit();
            }
        }

        private void SetupGame()
        {
            _cardManager = new CardManager();
            _cardPileManager = new CardPileManager();

            Board board = BoardFactory.SwitchLowestValueBoard(_cardManager, _cardPileManager);

            _game = new ClassicGame(_cardManager, _cardPileManager, board, _logger);
            _game.RedrawUiHandlerEvent += OnRedrawUiEventHandler;

            _cardImageManager = new CardImageManager(_cardManager.GetAllCardModels());
            _cardImageManager.RegisterCardTappedEventHandlers(OnCardImageTapped);
        }



        private void FirstDraw()
        {
            _absoluteLayout = new AbsoluteLayout
            {
                BackgroundColor = Colors.DarkGreen
            };
            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += OnBackgroundTapped;
            _absoluteLayout.GestureRecognizers.Add(tapGesture);


            BottomBarControls bottomBar = CreateBottomBarControls();
            AbsoluteLayout.SetLayoutBounds(bottomBar, new Rect(0, 1, 1, 80));
            AbsoluteLayout.SetLayoutFlags(bottomBar, AbsoluteLayoutFlags.WidthProportional | AbsoluteLayoutFlags.YProportional);
            _absoluteLayout.Children.Add(bottomBar);

            Content = _absoluteLayout;

            var board = _game.GameBoard;

            var remainingPileId = board.PileIdForRemaining;
            CardPileModel remainingPile = _cardPileManager.GetCardPileModelById(remainingPileId);
            BoardRenderer.DrawRemainingCardPile(remainingPile, _cardImageManager, _absoluteLayout);

            var sockPileId = board.PileIdForStock;
            CardPileModel sockPile = _cardPileManager.GetCardPileModelById(sockPileId);
            BoardRenderer.DrawStockCardPile(sockPile, _cardImageManager, _absoluteLayout);


            for (int i = 0; i < board.foundationNo; i++)
            {
                var pileId = board.GetPileIdForFoundation(i);
                var cardPile = _cardPileManager.GetCardPileModelById(pileId);

                BoardRenderer.DrawFoundationCardPile(cardPile, i, _cardImageManager, _absoluteLayout);
            }

            for (int i = 0; i < board.tableauNo; i++)
            {
                var pileId = board.GetPileIdForColumn(i);
                var cardPile = _cardPileManager.GetCardPileModelById(pileId);

                BoardRenderer.DrawMainBoardCardPile(cardPile, i, _cardImageManager, _absoluteLayout);
            }
        }

        private BottomBarControls CreateBottomBarControls()
        {
            int scoreValue = Preferences.Get("ScoreValue", 0);

            var bottomBar = new BottomBarControls(scoreValue);
            bottomBar.HintClicked += (s, e) => Hint();
            bottomBar.NewGameClicked += (s, e) => NewGame();
            bottomBar.ExitClicked += (s, e) => ExitApplication();
            bottomBar.UndoClicked += (s, e) => _game.Undo();

            return bottomBar;
        }


        private void OnCardImageTapped(object sender, CardTappedEventArgs e)
        {
            if (e.CardId == null)
                return;

            _selectorManager.ClearYellowSelectors(_absoluteLayout);

            CardId cardId = new CardId(e.CardId.ToString());
            var cardPos = _game.GetCardPosition(cardId);

            if (cardPos is null)
            {
                _logger?.LogError($"OnCardImageTapped: Card position not found {cardId}");
                return;
            }

            // Single tap to flip remaining pile
            if (cardPos.BoardPlacement == BoardPlacement.RemainingPile)
            {
                _game.CardAction(cardId, cardId);
                _cardSelectedId = null;
                return;
            }

            // If card is double tapped try move card
            if (_cardSelectedId == cardId)
            {
                var result = _game.AutoMove(cardId);

                if (result)
                {
                    _cardSelectedId = null;
                    _absoluteLayout.Children.Remove(_greenSelector);
                    CheckForWin();
                    return;
                }
            }

            // No first card is selected
            if (_cardSelectedId is null)
            {
                _cardSelectedId = e.CardId;
                AddCardColourSelector(_greenSelector, cardPos);
            }
            else // Two cards selected
            {
                _absoluteLayout.Children.Remove(_greenSelector);
                var selected = new CardId(_cardSelectedId.ToString());
                var target = new CardId(e.CardId.ToString());
                var successful = _game.CardAction(selected, target);
                _cardSelectedId = null;
                CheckForWin();
            }
        }

        private async void CheckForWin()
        {
            if (_game.WinDetection())
            {
                int scoreValue = Preferences.Get("ScoreValue", 0);
                Preferences.Set("ScoreValue", scoreValue + 1);

                await DisplayAlert("Congratulations !!", $"Win Count :{scoreValue + 1}", "Confirm");
                SetupGame();
                FirstDraw();
            }
        }

        private void OnBackgroundTapped(object? sender, TappedEventArgs e)
        {
            _cardSelectedId = null;
            ClearCardSelectors();
        }

        #region Card Selectors
        private void AddCardColourSelector(Image cardSelectorImage, CardPosition cardPos)
        {
            if (cardPos.BoardPlacement == BoardPlacement.Column)
            {
                BoardRenderer.AddCardToBoardAbsoluteLayout(_absoluteLayout, cardSelectorImage, cardPos.PositionOfPile, cardPos.PositionInPile - 1);
            }
            else if (cardPos.BoardPlacement == BoardPlacement.Foundation)
            {
                BoardRenderer.AddCardToFoundationAbsoluteLayout(_absoluteLayout, cardSelectorImage, cardPos.PositionOfPile);
            }
            else if (cardPos.BoardPlacement == BoardPlacement.RemainingPile)
            {
                BoardRenderer.AddCardToFoundationAbsoluteLayout(_absoluteLayout, cardSelectorImage, 6);
            }
            else if (cardPos.BoardPlacement == BoardPlacement.StockPile)
            {
                BoardRenderer.AddCardToFoundationAbsoluteLayout(_absoluteLayout, cardSelectorImage, 5);
            }
        }

        private void ClearCardSelectors()
        {
            _absoluteLayout.Children.Remove(_greenSelector);
            _absoluteLayout.Children.Remove(_redSelector);
            _selectorManager.ClearYellowSelectors(_absoluteLayout);
        }

        #endregion

        private void OnRedrawUiEventHandler(object sender, RedrawUiEventArgs e)
        {
            foreach (CardPileId cardPileId in e.PileIds)
            {
                var (placement, slotNo) = _game.GetCardPilePositionFromId(cardPileId);
                CardPileModel cardPile = _cardPileManager.GetCardPileModelById(cardPileId);
                IList<CardId> cardIds = cardPile.GetCardsWithoutPlaceholder();
                IList<CardModel> cards = _cardManager.GetCardModelById(cardIds);
                _cardImageManager.UpdateCardImages(cards);
                BoardRenderer.ReDrawCardPile(cardPile, placement, slotNo, _cardImageManager, _absoluteLayout);
            }
        }

    }
}
