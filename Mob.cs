using Godot;

public partial class Mob : RigidBody2D
{
	public override void _Ready()
	{
		var animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		string[] mobTypes = animatedSprite2D.SpriteFrames.GetAnimationNames();
		animatedSprite2D.Play(mobTypes[GD.Randi() % mobTypes.Length]);
	}

	private void OnVisibleOnScreenNotifier2DScreenExited()
	{
		QueueFree();
	}

	private void OnMobBodyEntered(Node2D body)
	{
		if (body is Player player)
		{
			GD.Print("Mob collided with Player — emitting Hit");
			player.EmitSignal(Player.SignalName.Hit);
		}
	}
}
