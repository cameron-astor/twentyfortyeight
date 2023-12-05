using Godot;
using System;

public partial class Game : Node
{


	private Score score;
	private Grid grid;
	private Node2D overlay;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		score = GetNode<Control>("Score") as Score;
		grid = GetNode<Node2D>("Grid") as Grid;
		overlay = GetNode<Node2D>("GameOverOverlay");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void GameOver()
	{
		overlay.Visible = true;
	}

	public void Restart()
	{

		grid.Reset();
		score.Reset();
		overlay.Visible = false;

	}
}
