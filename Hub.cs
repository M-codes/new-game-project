using Godot;

public partial class hub : CanvasLayer
{
    [Signal]
    public delegate void StartGameEventHandler();

	public override void _Ready()
	{
		// Connect the button's Pressed event
		var startButton = GetNode<Button>("StartButton");
		startButton.Pressed += OnStartButtonPressed;

		var messageTimer = GetNode<Timer>("MessageTimer");
		messageTimer.Timeout += OnMessageTimerTimeout;

		GD.Print(" hub._Ready() connected StartButton.Pressed");
	}

    public override void _Process(double delta)
    {
        // Detect "start_game" action (Enter key)
        if (Input.IsActionJustPressed("start_game"))
        {
            var startButton = GetNode<Button>("StartButton");
            if (startButton.Visible)
            {
                OnStartButtonPressed();
            }
        }
    }


    public void ShowMessage(string text)
    {
        var message = GetNode<Label>("Message");
        message.Text = text;
        message.Show();
        GetNode<Timer>("MessageTimer").Start();
    }

    async public void ShowGameOver()
    {
        ShowMessage("Game Over");
        var messageTimer = GetNode<Timer>("MessageTimer");
        await ToSignal(messageTimer, Timer.SignalName.Timeout);

        var message = GetNode<Label>("Message");
        message.Text = "Dodge the Creeps!";
        message.Show();

        await ToSignal(GetTree().CreateTimer(1.0), SceneTreeTimer.SignalName.Timeout);
        GetNode<Button>("StartButton").Show();
    }

    public void UpdateScore(int score)
    {
        GetNode<Label>("ScoreLabel").Text = score.ToString();
    }

    private void OnStartButtonPressed()
    {
        GD.Print(" Start button pressed — emitting StartGame signal");
        GetNode<Button>("StartButton").Hide();
        EmitSignal(SignalName.StartGame);
    }

    private void OnMessageTimerTimeout()
    {
        GetNode<Label>("Message").Hide();
    }
}
