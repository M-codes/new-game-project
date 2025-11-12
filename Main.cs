using Godot;

public partial class Main : Node
{
	[Export]
	public PackedScene MobScene { get; set; }

	private int _score;

	public override void _Ready()
	{
		GD.Print("Main._Ready() called");

		var hud = GetNodeOrNull<hub>("hub");
		if (hud == null)
		{
			GD.PrintErr("Could not find 'hub' node under Main!");
			GD.Print("Listing children of Main:");
			foreach (Node child in GetChildren())
				GD.Print(" - ", child.Name);
			return;
		}

		GD.Print("Found 'hub' node, connecting StartGame signal...");
		hud.Connect(hub.SignalName.StartGame, new Callable(this, nameof(NewGame)));
		GD.Print("Signal connected successfully.");

		GetNode<Timer>("StartTimer").Timeout += OnStartTimerTimeout;
		GetNode<Timer>("MobTimer").Timeout += OnMobTimerTimeout;
		GetNode<Timer>("ScoreTimer").Timeout += OnScoreTimerTimeout;

		GD.Print(" Timers connected successfully");
	}

	public void GameOver()
	{
		GD.Print("GameOver() called");

		GetNode<Timer>("MobTimer").Stop();
		GetNode<Timer>("ScoreTimer").Stop();

		var h = GetNodeOrNull<hub>("hub");
		if (h == null)
		{
			GD.PrintErr("hub node missing during GameOver!");
			return;
		}

		h.ShowGameOver();
		h.UpdateScore(_score);

		GD.Print("Stopping music and playing death sound");
		GetNode<AudioStreamPlayer>("Music").Stop();
		GetNode<AudioStreamPlayer>("DeathSound").Play();
	}

	public void NewGame()
	{
		GD.Print("NewGame() called");
		_score = 0;

		var player = GetNodeOrNull<Player>("Player");
		if (player == null)
		{
			GD.PrintErr("Player node not found!");
			return;
		}

		var startPosition = GetNodeOrNull<Marker2D>("StartPosition");
		if (startPosition == null)
		{
			GD.PrintErr("StartPosition node not found!");
			return;
		}

		player.Start(startPosition.Position);
		GD.Print("Player started at: ", startPosition.Position);

		GetNode<Timer>("StartTimer").Start();
		GD.Print("StartTimer started");

		var h = GetNodeOrNull<hub>("hub");
		if (h == null)
		{
			GD.PrintErr("hub node missing during NewGame!");
			return;
		}

		h.UpdateScore(_score);
		h.ShowMessage("Get Ready!");
		GD.Print(" HUD message shown, score reset to 0");

		GetTree().CallGroup("mobs", Node.MethodName.QueueFree);
		GD.Print(" Cleared existing mobs");

		GetNode<AudioStreamPlayer>("Music").Play();
		GD.Print(" Music started");
	}

	private void OnMobTimerTimeout()
	{
		GD.Print("OnMobTimerTimeout() - spawning mob");

		if (MobScene == null)
		{
			GD.PrintErr("MobScene not assigned in the inspector!");
			return;
		}

		Mob mob = MobScene.Instantiate<Mob>();
		var mobSpawnLocation = GetNodeOrNull<PathFollow2D>("MobPath/MobSpawnLocation");

		if (mobSpawnLocation == null)
		{
			GD.PrintErr(" MobSpawnLocation not found!");
			return;
		}

		mobSpawnLocation.ProgressRatio = GD.Randf();

		float direction = mobSpawnLocation.Rotation + Mathf.Pi / 2;
		mob.Position = mobSpawnLocation.Position;
		direction += (float)GD.RandRange(-Mathf.Pi / 4, Mathf.Pi / 4);
		mob.Rotation = direction;

		var velocity = new Vector2((float)GD.RandRange(150.0, 250.0), 0);
		mob.LinearVelocity = velocity.Rotated(direction);

		AddChild(mob);
		GD.Print($" Mob spawned at {mob.Position} with direction {direction}");
	}

	private void OnScoreTimerTimeout()
	{
		_score++;
		GD.Print(" Score incremented to ", _score);
		var hud = GetNodeOrNull<hub>("hub");
		if (hud != null)
			hud.UpdateScore(_score);
	}

	private void OnStartTimerTimeout()
	{
		GD.Print(" OnStartTimerTimeout() - starting Mob and Score timers");
		GetNode<Timer>("MobTimer").Start();
		GetNode<Timer>("ScoreTimer").Start();
	}
}
