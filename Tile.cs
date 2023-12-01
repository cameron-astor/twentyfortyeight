using Godot;
using System;

public partial class Tile : Polygon2D
{

	[Export]
	private int value = 0;
	public Vector2 targetPosition { get; set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Set the tile's value
		Label label = GetNode<Label>("Label");
		label.Text = value.ToString();

		// Set the tile's color based on its value
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	public int GetValue() {
		return value;
	}

	public void SetValue(int newValue) {
		value = newValue;
		Label label = GetNode<Label>("Label");
		label.Text = value.ToString();
	}

}
