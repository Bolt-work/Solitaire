namespace SolitaireApp.Views;

public class BottomBarControls : Frame
{
    public event EventHandler? ExitClicked;
    public event EventHandler? NewGameClicked;
    public event EventHandler? HintClicked;
    public event EventHandler? UndoClicked;

    public BottomBarControls(int score)
    {
        BackgroundColor = Colors.DarkOliveGreen;
        CornerRadius = 20;
        Padding = 10;
        WidthRequest = 1000;
        HeightRequest = 60;

        var exitButton = new Button
        {
            Text = "Exit",
            BackgroundColor = Colors.Black,
            TextColor = Colors.White
        };
        exitButton.Clicked += (s, e) => ExitClicked?.Invoke(this, e);

        var newGameButton = new Button
        {
            Text = "New Game",
            BackgroundColor = Colors.Black,
            TextColor = Colors.White
        };
        newGameButton.Clicked += (s, e) => NewGameClicked?.Invoke(this, e);

        var hintButton = new Button
        {
            Text = "Hint",
            BackgroundColor = Colors.Black,
            TextColor = Colors.White
        };
        hintButton.Clicked += (s, e) => HintClicked?.Invoke(this, e);

        var undoButton = new Button
        {
            Text = "Undo",
            BackgroundColor = Colors.Black,
            TextColor = Colors.White
        };
        undoButton.Clicked += (s, e) => UndoClicked?.Invoke(this, e);

        var scoreLbl = new Label
        {
            Text = score.ToString(),
            TextColor = Colors.White,
            FontSize = 22
        };

        var spacer = new BoxView
        {
            WidthRequest = 50, // Adjust spacing as needed
            BackgroundColor = Colors.Transparent // Invisible
        };

        var buttonLayout = new HorizontalStackLayout
        {
            HorizontalOptions = LayoutOptions.Center,
            Spacing = 20,
            Children = { exitButton, newGameButton, scoreLbl, undoButton, hintButton }
        };

        Content = buttonLayout;
    }
}
