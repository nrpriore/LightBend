using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class LightPath {

	private LevelController _level;
	private Transform _pathTransform;
	private List<LightSegment> _path;


	public LightPath(LevelController levelController) {
		_level = levelController;
		_pathTransform = GameObject.Find("Path").transform;
		_path = new List<LightSegment>();
	}
	
	public void Build(LightSegment startSegment) {
		foreach(Transform child in _pathTransform) {
			MonoBehaviour.Destroy(child.gameObject);
		}

		_path.Clear();
		LightSegment segment = startSegment;
		//_path.Add(segment);
		while(!segment.IsEnd) {
			segment = GetNextSegment(segment);

			if(_path.Any(x => x.ID == segment.ID)) {
				break;
			}

			_path.Add(segment);
		}
		if(segment.IsEnd) {
			DrawPathOffScreen(segment);
		}

		foreach(LightSegment ls in _path) {
			Vector2 quadrant = (2f * ls.StartSide) + ls.Direction;
			int angleDirection = (quadrant.x * quadrant.y > 0)? -1 : 1;
			MonoBehaviour.Instantiate(Resources.Load<GameObject>("Prefabs/Segment"), ls.TilePosition + ((Vector2.one * 0.25f) * quadrant), Quaternion.Euler(0, 0, angleDirection * 45), _pathTransform);
		}
	}


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
