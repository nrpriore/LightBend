using UnityEngine;
using System.Linq;
using System.Collections.Generic;

// Factory class used to generate levels based on a given or random seed
public class LevelGenerator {
	private const int SEED_LENGTH = 8;

	// Private references
	private LevelController _level;


	// Constructor ----------------------------------------------- //
	public LevelGenerator(LevelController levelController) {
		_level = levelController;
	}


	// Public Methods -------------------------------------------- //
	// Creates a level based on a given or random seed
	public void CreateLevel(string seed = "") {
		// Use the given seed or generate a new one to initiate the Random class instance
		if(seed == "" || seed.Length != SEED_LENGTH) {
			Debug.Log("Null or invalid key. Must be "+SEED_LENGTH+" characters. Generating random seed.");
			seed = GenerateRandomSeed(SEED_LENGTH);
		}
		int[] key = new int[SEED_LENGTH];
		for(int i = 0; i < SEED_LENGTH; i++) {
			key[i] = (int)seed[i];
		}
		System.Random rand = new System.Random(key[0]);
		for(int i = 0; i < SEED_LENGTH; i++) {
			key[i] *= rand.Next(1, 8);
		}
		int randSeed = key[0];
		for(int i = 1; i < SEED_LENGTH - 1; i++) {
			randSeed += (key[i] * key[i+1]);
		}
		// Fun fact: randSeed ranges from 13,872 to 5,716,432

		rand = new System.Random(randSeed);
		// ------------------------------------------------------- //

		// Generate the starting level variables - GridSize and StartSegment
		int gridWidth = rand.Next(4, 7); // Lower bound is inclusive, upper bound is exclusive, silly C#
		int gridHeight = rand.Next(4, 9);
		Vector2 gridSize = new Vector2(gridWidth, gridHeight);

		bool startVertical = rand.Next(2) == 0;
		bool startPositive = rand.Next(2) == 0;		
		int xStart = (startVertical)? rand.Next(1, gridWidth) : ((startPositive)? gridWidth + 1 : 0);
		int yStart = (startVertical)? ((startPositive)? gridHeight + 1 : 0) : rand.Next(1, gridHeight);
		int xStartSide = 0;
		int yStartSide = 0;
		int xStartDirection = 0;
		int yStartDirection = 0;
		if(startVertical) {
			if(xStart == 1) {
				xStartSide = -1;
			}
			else if(xStart < gridWidth) {
				xStartSide = (rand.Next(2) == 0)? -1 : 1;
			}
			else {
				xStartSide = 1;
			}
			xStartDirection = -xStartSide;
			yStartDirection = (startPositive)? -1 : 1;
		}
		else {
			if(yStart == 1) {
				yStartSide = -1;
			}
			else if(yStart < gridHeight) {
				yStartSide = (rand.Next(2) == 0)? -1 : 1;
			}
			else {
				yStartSide = 1;
			}
			xStartDirection = (startPositive)? -1 : 1;
			yStartDirection = -yStartSide;
		}
		
		LightSegment startSegment = new LightSegment(
			new Vector2(xStart, yStart), 
			new Vector2(xStartSide, yStartSide), 
			new Vector2(xStartDirection, yStartDirection)
		);
		// ------------------------------------------------------- //

		// Add Tokens and set the win condition
		int numTokens = Mathf.Min(rand.Next(4, 9), gridWidth * gridHeight / 4);
		List<TokenType> tokens = new List<TokenType>();

		Tile[][] grid = new Tile[gridWidth + 2][];
		for(int x = 0; x < grid.Length; x++) {
			grid[x] = new Tile[gridHeight + 2];
			for(int y = 0; y < grid[x].Length; y++) {
				grid[x][y] = new Tile(!IsOutsideBorderTile(new Vector2(x, y), gridSize));
			}
		}

		LightSegment segment = GetNextSegment(startSegment, grid);
		List<Vector2> invalidTiles = new List<Vector2>() {segment.TilePosition};
		LightSegment nextSegment = null;
		LightSegment endCoil = null;
		int meep = 0; int maxMeep = 100;
		while(endCoil == null) {
			nextSegment = GetNextSegment(segment, grid);
			if(invalidTiles.Any(tile => tile.x == nextSegment.TilePosition.x && tile.y == nextSegment.TilePosition.y)) {
				if(IsTokenRequired(nextSegment, gridSize)) {
					Debug.Log("Ending path incorrectly at " + (int)nextSegment.TilePosition.x + ", " + (int)nextSegment.TilePosition.y);
					endCoil = GetNextSegment(nextSegment, grid);
					break;
				}

				Debug.Log("Skipping tile at " + (int)nextSegment.TilePosition.x + ", " + (int)nextSegment.TilePosition.y);
				segment = nextSegment;

				meep++; if(meep > maxMeep) { break;}
				continue;
			}
			if(IsOutsideBorderTile(nextSegment.TilePosition, gridSize)) {
				Debug.Log("Ending path incorrectly at " + (int)nextSegment.TilePosition.x + ", " + (int)nextSegment.TilePosition.y);
				endCoil = nextSegment;
				break;
			}
			
			if(IsTokenRequired(nextSegment, gridSize) && tokens.Count < numTokens) {
				Debug.Log("Adding Token at border " + (int)nextSegment.TilePosition.x + ", " + (int)nextSegment.TilePosition.y);
				TokenType newToken = new Block();
				grid[(int)nextSegment.TilePosition.x][(int)nextSegment.TilePosition.y].AssignTokenType(newToken);
				tokens.Add(newToken);
				segment = GetNextSegment(segment, grid);

				meep++; if(meep > maxMeep) { break;}
			}
			else if(rand.NextDouble() <= 0.5d && tokens.Count < numTokens) {
				Debug.Log("Adding Token at " + (int)nextSegment.TilePosition.x + ", " + (int)nextSegment.TilePosition.y);
				TokenType newToken = new Block();
				grid[(int)nextSegment.TilePosition.x][(int)nextSegment.TilePosition.y].AssignTokenType(newToken);
				tokens.Add(newToken);
				segment = GetNextSegment(segment, grid);

				meep++; if(meep > maxMeep) { break;}
			}
			else if(IsBorderTile(nextSegment.TilePosition, gridSize) && tokens.Count == numTokens) {
				Debug.Log("Ending path at " + (int)nextSegment.TilePosition.x + ", " + (int)nextSegment.TilePosition.y);
				endCoil = GetNextSegment(nextSegment, grid);
			}
			else {
				Debug.Log("Passing through at " + (int)nextSegment.TilePosition.x + ", " + (int)nextSegment.TilePosition.y);
				segment = nextSegment;

				meep++; if(meep > maxMeep) { break;}
			}

			invalidTiles.Add(nextSegment.TilePosition);
		}


		//endCoil = new LightSegment(new Vector2(2,0), new Vector2(0,1), new Vector2(-1,-1));
		_level.SetGrid(grid);
		_level.SetLevel(startSegment, endCoil);
		_level.InstantiateGrid();
		_level.InstantiateLevel();
	}


	// Private Methods ------------------------------------------- //
	// Generates a random alphanumeric string of given length
	private string GenerateRandomSeed(int length) {
		string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

		int tempSeed = (int)(UnityEngine.Random.value * (int.MaxValue - 1));
		System.Random random = new System.Random(tempSeed);
		return new string(Enumerable.Range(1, length).Select(_ => chars[random.Next(chars.Length)]).ToArray());
	} 

	// Returns if we're at a border Tile
	private bool IsBorderTile(Vector2 tilePosition, Vector2 gridSize) {
		return tilePosition.x == 1 || tilePosition.x == gridSize.x || tilePosition.y == 1 || tilePosition.y == gridSize.y;
	}
	private bool IsTokenRequired(LightSegment segment, Vector2 gridSize) {
		return IsBorderTile(segment.TilePosition, gridSize) && 
			(segment.TilePosition.x + segment.EndSide.x == 0 ||
			segment.TilePosition.x + segment.EndSide.x > gridSize.x ||
			segment.TilePosition.y + segment.EndSide.y == 0 ||
			segment.TilePosition.y + segment.EndSide.y > gridSize.y
		);
	}
	// Returns if disabled outside-border Tile
	private bool IsOutsideBorderTile(Vector2 tilePosition, Vector2 gridSize) {
		return tilePosition.x == 0 || tilePosition.x == gridSize.x + 1 || tilePosition.y == 0 || tilePosition.y == gridSize.y + 1;
	}

	// Generates the next LightSegment based on the given previous LightSegment. MIMICS LightPath.GetNextSegment()
	private LightSegment GetNextSegment(LightSegment prevSegment, Tile[][] grid) {
		Vector2 nextTile = prevSegment.TilePosition + prevSegment.EndSide;
		Vector2 nextTileSide = prevSegment.EndSide * -1;
		
		TokenType tokenType = grid[(int)nextTile.x][(int)nextTile.y].TokenType;
		if(tokenType != null) {
			return tokenType.BendPath(prevSegment);
		}

		return new LightSegment(
			prevSegment.TilePosition + prevSegment.EndSide,
			prevSegment.EndSide * -1,
			prevSegment.Direction
		);
	}


	/*private class LevelData {
		public Tile[][] Grid;
		public LightSegment StartCoil;
		public LightSegment EndCoil;
		public LightPath LightPath;
	}*/
}
