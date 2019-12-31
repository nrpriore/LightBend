using System.Linq;
using System.Collections.Generic;
using UnityEngine;

// Governs the creation and interaction of the LightPath
public class LightPath {

	// Private references
	private LevelController _level;
	private Transform _pathTransform;
	private List<LightSegment> _path;


	// Constructor ----------------------------------------------- //
	public LightPath(LevelController levelController) {
		_level = levelController;
		_pathTransform = GameObject.Find("Path").transform;
		_path = new List<LightSegment>();
	}
	

	// Public Methods -------------------------------------------- //
	// Builds the LightPath starting at the given starting LightSegment
	public void Build(LightSegment startSegment) {
		// Clear old path
		foreach(Transform child in _pathTransform) {
			MonoBehaviour.Destroy(child.gameObject);
		}
		_path.Clear();

		// Create new path
		LightSegment segment = startSegment;
		while(!IsSegmentAtBorder(segment)) {
			segment = GetNextSegment(segment);

			if(_path.Any(x => x.ID == segment.ID)) {
				break; // Stop building if we've somehow hit an infinite loop
			}
			if(segment.ID == _level.EndCoil.ID) {
				_level.CompleteLevel();
				break; // Stop building on Win Condition
			}

			_path.Add(segment);
		}
		if(IsSegmentAtBorder(segment)) {
			DrawPathOffScreen(segment);
		}

		// Actually create the GameObjects
		foreach(LightSegment ls in _path) {
			Vector2 quadrant = (2f * ls.StartSide) + ls.Direction;
			int angleDirection = (quadrant.x * quadrant.y > 0)? -1 : 1;
			MonoBehaviour.Instantiate(Resources.Load<GameObject>("Prefabs/Segment"), ls.TilePosition + ((Vector2.one * 0.25f) * quadrant), Quaternion.Euler(0, 0, angleDirection * 45), _pathTransform);
		}
	}


	// Private Methods ------------------------------------------- //
	// Generates the next LightSegment based on the given previous LightSegment. Interacts with Tokens
	private LightSegment GetNextSegment(LightSegment prevSegment) {
		Vector2 endSide = prevSegment.StartSide + prevSegment.Direction;

		Vector2 nextTile = prevSegment.TilePosition + endSide;
		Vector2 nextTileSide = endSide * -1;
		
		Token token = _level.Grid[(int)nextTile.x][(int)nextTile.y].Token;
		if(token != null) {
			return token.BendPath(prevSegment);
		}

		return new LightSegment(
			prevSegment.TilePosition + endSide,
			endSide * -1,
			prevSegment.Direction
		);
	}

	// Returns if the LightSegment is at a border Tile (stops LightPath building)
	private bool IsSegmentAtBorder(LightSegment segment) {
		return 
			(segment.TilePosition.y == 0 && segment.StartSide.y + segment.Direction.y == -1) ||						// Bottom
			(segment.TilePosition.x == 0 && segment.StartSide.x + segment.Direction.x == -1) ||						// Left
			(segment.TilePosition.y == _level.GridSize.y - 1 && segment.StartSide.y + segment.Direction.y == 1) ||	// Top
			(segment.TilePosition.x == _level.GridSize.x - 1 && segment.StartSide.x + segment.Direction.x == 1)		// Right
		;
	}

	// Extends the LightPath from the border Tile to the end of the screen for realistic effect
	private void DrawPathOffScreen(LightSegment prevSegment) {
		for(int i = 0; i < 10; i++) {
			Vector2 endSide = prevSegment.StartSide + prevSegment.Direction;

			Vector2 nextTile = prevSegment.TilePosition + endSide;
			Vector2 nextTileSide = endSide * -1;
		
			LightSegment nextSegment = new LightSegment(
				prevSegment.TilePosition + endSide,
				endSide * -1,
				prevSegment.Direction
			);

			_path.Add(nextSegment);
			prevSegment = nextSegment;
		}
	}
}
