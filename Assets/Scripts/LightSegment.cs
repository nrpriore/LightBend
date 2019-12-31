using UnityEngine;

// Holds the properties of a single Light Segment
public class LightSegment {

	// Public references
	public Vector2 TilePosition { get{ return _tilePosition;} }
	public Vector2 StartSide { get{ return _startSide;} }
	public Vector2 Direction { get{ return _direction;} }

	// Private references
	private Vector2 _tilePosition;
	private Vector2 _startSide;
	private Vector2 _direction;


	// Constructor ----------------------------------------------- //
	public LightSegment(Vector2 tilePosition, Vector2 startSide, Vector2 direction) {
		_tilePosition = tilePosition;
		_startSide = startSide;
		_direction = direction;
	}


	// Public Methods -------------------------------------------- //
	// Generates an ID to check for duplicate Segments
	public string ID {
		get{ return 
			_tilePosition.x.ToString() + _tilePosition.y.ToString() + 
			_startSide.x.ToString() + _startSide.y.ToString() + 
			_direction.x.ToString() + _direction.y.ToString();
		}
	}
}
