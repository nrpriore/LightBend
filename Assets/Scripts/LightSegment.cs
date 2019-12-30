using UnityEngine;

// Holds the properties of a single Light Segment
public class LightSegment {

	// Public properties
	public Vector2 TilePosition { get{ return _tilePosition;} }
	public Vector2 StartSide { get{ return _startSide;} }
	public Vector2 Direction { get{ return _direction;} }

	#region private variables for properties
	private Vector2 _tilePosition;
	private Vector2 _startSide;
	private Vector2 _direction;
	#endregion


	public LightSegment(Vector2 tilePosition, Vector2 startSide, Vector2 direction) {
		_tilePosition = tilePosition;
		_startSide = startSide;
		_direction = direction;
	}

	// Is this segment the end of the path
	public bool IsEnd {
		get{ return 
			(_tilePosition.y == 0 && _startSide.y + _direction.y == -1) ||								// Bottom
			(_tilePosition.x == 0 && _startSide.x + _direction.x == -1) ||								// Left
			(_tilePosition.y == GridController.GridSize.y - 1 && _startSide.y + _direction.y == 1) ||	// Top
			(_tilePosition.x == GridController.GridSize.x - 1 && _startSide.x + _direction.x == 1)		// Right
		;}
	}
}
