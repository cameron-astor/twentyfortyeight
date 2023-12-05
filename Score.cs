using Godot;
using System;

public partial class Score : Control
{
	// Called when the node enters the scene tree for the first time.
	private Label valueLabel;

	public override void _Ready()
	{
		valueLabel = GetNode<Label>("Panel/ScoreValueLabel");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void AddToScore(int additionalValue)
	{
		int currentValue = int.Parse(valueLabel.Text);
		int newValue = currentValue + additionalValue;
		valueLabel.Text = "" + newValue;
	}

	// Reset score for game restart.
	public void Reset()
	{
		valueLabel.Text = "" + 0;
	}
}
