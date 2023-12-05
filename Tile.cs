using Godot;
using System;
using System.Collections;

public partial class Tile : Polygon2D
{

	[Export]
	private int value = 0;
	private Color tilecolor;

	private Label label;
	private int currentDigits = 1;
	public Vector2 targetPosition { get; set; } // Used in merging logic

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

		PlaySpawnAnimation();
		UpdateLabel(); 	// Set the tile's value
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
		UpdateLabel();
		UpdateColor();
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

	// Sets the tile color based on the current value
	private void UpdateColor()
	{

		GD.Print(GetInstanceId());
		switch (value)
		{
			case(2): // Color definitions
				tilecolor = new Color("eee3da");
				break;
			case(4):
				tilecolor = new Color("eddfc8");
				break;
			case(8):
				tilecolor = new Color("f2b178");
				break;
			case(16):
				tilecolor = new Color("f59562");
				break;
			case(32):
				tilecolor = new Color("f57c5f");
				break;
			case(64):
				tilecolor = new Color("f65e3a");
				break;
			case(128):
				tilecolor = new Color("edcf73");
				break;
			case(256):
				tilecolor = new Color("edcc61");
				break;
			case(512):
				tilecolor = new Color("edc750");
				break;
			case(1024):
				tilecolor = new Color("edc53e");
				break;
			case(2048):
				tilecolor = new Color("edc22d");
				break;
		}
		
		Color = tilecolor;
		// if (value > 4) label.AddThemeColorOverride("font_color", new Color("ffffff"));
	}

	private void UpdateLabel()
	{
		label = GetNode<Label>("Label");
		label.Text = value.ToString();		

		// Adjust for number of digits
		switch (value.ToString().Length)
		{
			// case 1: do nothing
			case 2:
				if (currentDigits < 2) {
					label.Position = new Vector2(label.Position.X - 10, label.Position.Y);
					currentDigits = 2;
					break;
				}
				break;	
			case 3:
				if (currentDigits < 3) {
					label.Position = new Vector2(label.Position.X - 10, label.Position.Y);
					currentDigits = 3;
					break;
				}
				break;			

			case 4:
				if (currentDigits < 4) {
					label.Position = new Vector2(label.Position.X - 10, label.Position.Y);
					currentDigits = 4;
					break;
				}
				break;		

			default:
				break;

		}
	}

}
