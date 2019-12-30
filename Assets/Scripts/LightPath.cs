using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightPath {

	private List<LightSegment> _path;


	public LightPath() {
		_path = new List<LightSegment>();
	}
	
	public void Build(LightSegment startSegment) {
		_path.Clear();
		LightSegment segment = startSegment;
		_path.Add(segment);
		while(!segment.IsEnd) {
			segment = GetNextSegment(segment);
			_path.Add(segment);
		}

		foreach(LightSegment ls in _path) {
			//Debug.Log(test.TilePosition);
		}
	}


	private LightSegment GetNextSegment(LightSegment prevSegment) {
		Vector2 endSide = prevSegment.StartSide + prevSegment.Direction;
		return new LightSegment(
			prevSegment.TilePosition + endSide,
			endSide * -1,
			prevSegment.Direction
		);
	}
}
