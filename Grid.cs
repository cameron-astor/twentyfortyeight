using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


public partial class Grid : Node2D
{

	[Signal]
	public delegate void ScoreUpdateEventHandler(int additionalPoints);

	[Signal]
	public delegate void GameOverEventHandler();

	private int DEBUG_MOVE_COUNTER = 0;

	private PackedScene sceneTile;
	private bool MOVEMENT_IN_PROGRESS = false;

	private Tile[,] grid;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Load the packed tile scene.
		sceneTile = ResourceLoader.Load<PackedScene>("res://Tile.tscn");

		// Initialize 4x4 grid.
		grid = new Tile[4, 4];
		
		// Create starting tiles.
		PopulateStartingTiles();
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	// Handle user input.
	// These actions must first be defined and mapped to keys in Project Settings -> Input Map.
    public override void _Input(InputEvent @event)
    {
		if (!MOVEMENT_IN_PROGRESS) 
		{
			bool moved = false;

			if(@event.IsActionPressed("up")) 
			{
				moved = MoveTiles("up");
			}

			if(@event.IsActionPressed("down")) 
			{
				moved = MoveTiles("down");
			}

			if(@event.IsActionPressed("left")) 
			{
				moved = MoveTiles("left");
			}

			if(@event.IsActionPressed("right")) 
			{
				moved = MoveTiles("right");
			}

			if (moved)
			{
				SpawnRandomTile();
				DEBUG_MOVE_COUNTER++;
				GD.Print(DEBUG_MOVE_COUNTER);
			}

		}

    }

	// Perform a single movement of tiles across the board.
	// Calculate new positions.
	// Calculate tile mergers.
	// Returns whether there was any movement on the grid.
	private bool MoveTiles(String direction) {

		bool isHorizontal = direction == "left" || direction == "right";
    	bool isReverse = direction == "up" || direction == "left";
		bool movementOccurred = false;


		Dictionary<Tile, Vector2> mergeCoords = new Dictionary<Tile, Vector2>(); // Keep track of coordinates for merge animations
		Dictionary<Tile, Vector2> originalPositions = new Dictionary<Tile, Vector2>(); // Dictionary to store the original positions of the tiles

		int pointsScored = 0;

		// We are abstracting rows and columns here to generalize the method.
		// i and j can both represent either rows or columns based on the direction pressed.
		// In the case of horizontal movement, we iterate through columns while holding row constant.
		// In the case of vertical movement, we iterate through rows while holding column constant.
		for (int i = 0; i < 4; i++) {
			Stack<Tile> tiles = new Stack<Tile>();

			// Iterate over either a row or a column based on whatever direction was pressed.
			// Keep track of active tiles encountered on a stack.
			for (int j = 0; j < 4; j++) {
				int x = isHorizontal ? (isReverse ? 3 - j : j) : i;
				int y = isHorizontal ? i : (isReverse ? 3 - j : j);

				if (grid[x, y] != null)
				{
					// Store the original position
                	originalPositions[grid[x, y]] = new Vector2(x, y);
					tiles.Push(grid[x, y]);
					grid[x, y] = null; // clear grid as we go
				}
			}

			// Process tiles
			int newIndex = isReverse ? 0 : 3; // Keep track of where to move tiles to in the grid

			while (tiles.Count > 0)
			{
				Tile current = tiles.Pop();
				Tile next = tiles.Count > 0 ? tiles.Peek() : null;
				Tile merged = null;

				// Check for merge
				if (next != null && current.GetValue() == next.GetValue())
				{

					movementOccurred = true; // If there is a merge, we know movement occurred.

					// Increase score
					pointsScored += current.GetValue() * 2;

					// Keep track of merged tiles information 
					merged = tiles.Pop(); // Remove the next tile (merging)
					current.SetValue(current.GetValue() * 2); // Double the value

					

				}

				// Set tile in new position
				if (isHorizontal)
				{
					grid[newIndex, i] = current; // Horizontal movements
					if (merged != null)
					{
						mergeCoords.Add(merged, ArrayToTileCoords(new Vector2(newIndex, i)));
					}
				}
				else
				{
					grid[i, newIndex] = current; // Vertical movement
					if (merged != null)
					{
						mergeCoords.Add(merged, ArrayToTileCoords(new Vector2(i, newIndex)));
					}
				}

				newIndex += isReverse ? 1 : -1;
			}

		}

		// After processing each tile, check if its position has changed
        foreach (Tile t in originalPositions.Keys) {
			Vector2 coords = originalPositions[t];
            if (grid[(int) coords.X, (int) coords.Y] != t) {
                movementOccurred = true;
                break; // No need to check further if any movement is detected
            }
        }

		// Eventually we will end up with the grid array reflecting the correct position 
		// but the actual tile nodes still with the old position property. It's then that we can,
		// as a final step, calculate the new location and play an animation to get the tiles there.
		MOVEMENT_IN_PROGRESS = true;

		for (int x = 0; x < 4; x++) 
		{
			for (int y = 0; y < 4; y++)
			{
				if (grid[x, y] != null) // if there is a tile in the grid square
				{
					Tile t = grid[x, y];
					Tween tween = t.CreateTween();
					tween.TweenProperty(t, // node
										"position", // property to animate
										ArrayToTileCoords(new Vector2(x, y)), // final value
										0.1f); // duration
				}
			}
		}

		// Deal with merged tiles animations
		foreach (Tile t in mergeCoords.Keys)
		{
			Vector2 coords = mergeCoords[t];
			Tween tween = t.CreateTween();
			// MOVEMENT_IN_PROGRESS = true;
			tween.TweenProperty(t, // node
								"position", // property to animate
								coords, // final value
								0.1f); // duration
			tween.TweenCallback(Callable.From(() => { t.QueueFree(); }));
		}

		MOVEMENT_IN_PROGRESS = false;
		
		// Send score update signal
		EmitSignal(SignalName.ScoreUpdate, pointsScored);

		if (CheckGameOver()) EmitSignal(SignalName.GameOver);

		// Return whether any actual movement occurred
		return movementOccurred;

	}

    private Vector2 ArrayToTileCoords(Vector2 arrayCoords) {
		return new Vector2(arrayCoords.X * 115 + 15, arrayCoords.Y * 115 + 15);
	}

	// Sets up the 2048 game board by randomly spawning two tiles, each with a value of 2.
	private void PopulateStartingTiles() {
		// Randomly spawn two tiles
		Random rand = new Random();

		Vector2 tile1coords = new Vector2(rand.Next(0, 4), rand.Next(0, 4));
		Vector2 tile2coords = new Vector2(rand.Next(0, 4), rand.Next(0, 4));

		// Make sure the tiles don't spawn at the same spot.
		// Keep spawning until they don't.
		while (tile1coords.X == tile2coords.X && tile1coords.Y == tile2coords.Y) {
			tile1coords = new Vector2(rand.Next(0, 4), rand.Next(0, 4));
			tile2coords = new Vector2(rand.Next(0, 4), rand.Next(0, 4));
		}

		Tile t1 = sceneTile.Instantiate() as Tile;
		t1.Position = ArrayToTileCoords(tile1coords);
		t1.SetValue(2);
		AddChild(t1);

		Tile t2 = sceneTile.Instantiate() as Tile;
		t2.Position = ArrayToTileCoords(tile2coords);
		t2.SetValue(2);
		AddChild(t2);

		// Add the tiles to the grid.
		grid[(int) tile1coords.X, (int) tile1coords.Y] = t1;
		grid[(int) tile2coords.X, (int) tile2coords.Y] = t2;

	}

	// Spawns a new tile randomly in any available space.
	private void SpawnRandomTile() {
		// Find available spaces
		List<Vector2I> spaces = new List<Vector2I>();

		for (int x = 0; x < 4; x++) 
		{
			for (int y = 0; y < 4; y++)
			{
				if (grid[x, y] == null) 
				{
					spaces.Add(new Vector2I(x, y));
				}
			}
		}

		if (spaces.Count > 0)
		{
			Random r = new Random();
			int selection = r.Next(0, spaces.Count);
			SpawnTile(spaces[selection].X, spaces[selection].Y);
		}
	}

	private void SpawnTile(int x, int y) {
		Random r = new Random();

		Tile newTile = sceneTile.Instantiate() as Tile;
		newTile.Position = ArrayToTileCoords(new Vector2(x, y));

		int spawn4 = r.Next(0, 10);
		int value = spawn4 > 7 ? 4 : 2;
		newTile.SetValue(value);

		grid[x, y] = newTile;
		AddChild(newTile);
	}

	// Check whether the game is over.
	// 2048 ends when the board is full and there are no more possible moves to be made
	private bool CheckGameOver()
	{

		for (int x = 0; x < 4; x++)
		{
			for (int y = 0; y < 4; y++)
			{
				if (grid[x, y] == null)
				{
					return false; // Game is not over yet.
				} else { // If there is a tile, calculate adjacencies and possible moves
					// Get valid adjacencies for the current grid square
					var adjacentPositions = new (int, int)[] // Array of ValueTuple
					{
						(x + 1, y),
						(x - 1, y),
						(x, y + 1),
						(x, y - 1)
					};

					foreach (var p in adjacentPositions) // Check for out of bounds pairs
					{
						if (p.Item1 >= 0 && p.Item1 < 4 && p.Item2 >= 0 && p.Item2 < 4) // If an adjacent position is valid
						{

							if (grid[p.Item1, p.Item2] == null) return false; // There is a null tile, game is not over.
							if (grid[x, y].GetValue() == grid[p.Item1, p.Item2].GetValue()) return false; // There is a move, game is not over.
							
						}
					}

				}
			}
		}

		return true;
	}

	// Reset for game restart.
	public void Reset()
	{
		for (int x = 0; x < 4; x++) 
		{
			for (int y = 0; y < 4; y++)
			{
				if (grid[x, y] != null)
				{
					grid[x, y].QueueFree();
					grid[x, y] = null;
				}
			}
		}

		PopulateStartingTiles();
	}

}

