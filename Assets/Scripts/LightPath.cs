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
	public void Build() {
		// Clear old path
		foreach(Transform child in _pathTransform) {
			MonoBehaviour.Destroy(child.gameObject);
		}

		// Create new path
		if(BuildAndTestPath(out _path)) {
			_level.CompleteLevel();
		}
		else if(_path.Count > 0) {
			if(IsSegmentAtBorder(_path[_path.Count - 1])) {
				DrawPathOffScreen(_path[_path.Count - 1]);
			}
		}

		// Actually create the GameObjects
		foreach(LightSegment ls in _path) {
			Vector2 quadrant = (2f * ls.StartSide) + ls.Direction;
			int angleDirection = (quadrant.x * quadrant.y > 0)? -1 : 1;
			MonoBehaviour.Instantiate(Resources.Load<GameObject>("Prefabs/Segment"), ls.TilePosition + ((Vector2.one * 0.25f) * quadrant), Quaternion.Euler(0, 0, angleDirection * 45), _pathTransform);
		}
	}

	// Generates the List<LightSegment> _path, returns true if path beats the level
	public bool BuildAndTestPath(out List<LightSegment> lightPath) {
		List<LightSegment> path = new List<LightSegment>();
		bool success = false;

		LightSegment segment = _level.StartSegment;
		while(!IsSegmentAtBorder(segment)) {
			segment = GetNextSegment(segment);

			if(segment.ID == _level.EndCoil.ID) {
				success = true;
				break; // Stop building on Win Condition
			}
			else if(segment.CollisionID == _level.StartCoil.CollisionID || segment.CollisionID == _level.EndCoil.CollisionID) {
				break; // Stop building if the LightPath hits a coil at the wrong direction
			}
			else if(path.Any(x => x.ID == segment.ID)) {
				break; // Stop building if we've somehow hit an infinite loop
			}

			path.Add(segment);
		}

		lightPath = path;
		return success;
	}

	// Private Methods ------------------------------------------- //
	// Generates the next LightSegment based on the given previous LightSegment. Interacts with Tokens
	private LightSegment GetNextSegment(LightSegment prevSegment) {
		Vector2 nextTile = prevSegment.TilePosition + prevSegment.EndSide;
		Vector2 nextTileSide = prevSegment.EndSide * -1;
		
		Token token = _level.Grid[(int)nextTile.x][(int)nextTile.y].Token;
		if(token != null) {
			return token.Type.BendPath(prevSegment);
		}

		return new LightSegment(
			prevSegment.TilePosition + prevSegment.EndSide,
			prevSegment.EndSide * -1,
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
			Vector2 nextTile = prevSegment.TilePosition + prevSegment.EndSide;
			Vector2 nextTileSide = prevSegment.EndSide * -1;
		
			LightSegment nextSegment = new LightSegment(
				prevSegment.TilePosition + prevSegment.EndSide,
				prevSegment.EndSide * -1,
				prevSegment.Direction
			);

			_path.Add(nextSegment);
			prevSegment = nextSegment;
		}
	}
}
