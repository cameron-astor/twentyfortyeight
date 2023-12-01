using Godot;
using System;

public partial class Tile : Polygon2D
{

	[Export]
	private int value = 0;
	private Color tilecolor;
	public Vector2 targetPosition { get; set; } // Used in merging logic

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		PlaySpawnAnimation();

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

	private void PlaySpawnAnimation()
	{
		Scale = new Vector2(0, 0);
		Position = Position + new Vector2(50, 50);

		Tween scaleAnim = CreateTween();
		Tween positionAnim = CreateTween();

		scaleAnim.TweenProperty(this, // node
							"scale", // property to animate
							new Vector2(1, 1), // final value
							0.2f); // duration
		positionAnim.TweenProperty(this,
							"position",
							Position - new Vector2(50, 50),
							0.2f);
	}

	public void PlayMergeAnimation()
	{
		Scale = new Vector2(0, 0);

		Tween scaleAnim = CreateTween();
		Tween positionAnim = CreateTween();

		scaleAnim.TweenProperty(this, // node
							"scale", // property to animate
							new Vector2(1.5f, 1.5f), // final value
							1f); // duration
		positionAnim.TweenProperty(this,
							"position",
							Position - new Vector2(50, 50),
							0.2f);
	}

	private void UpdateColor()
	{
		
	}

}
