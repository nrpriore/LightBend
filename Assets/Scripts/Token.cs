using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// Base class for placeable pieces that governs movement
public class Token : MonoBehaviour {
	private const float MAX_SNAP_DISTANCE = 0f; 	// Max distance at DragEnd for a valid snap (middle of 4 tokens = 0.7071f, 0 = infinite)
	private const float LERP_THRESHOLD = 0.001f;	// Distance at which Update() will stop Lerping, to stop infinite Lerping
	private const float LERP_SPEED = 30f;			// Speed factor of Lerp
	private const float REDRAW_DISTANCE = 0.3f;		// Distance at which LightPath is redrawn when Token is being dragged

	private LevelController _level;

	private bool _dragging;			// If the token is being dragged currently
	private Vector2 _offset;		// Offset vector so center of token doesn't snap to mousePosition on click
	private Vector2 _initialPos;	// Initial token position on grid
	private Vector2 _lerpTarget;	// Target vector to Lerp to in Update()

	private bool _onTile;			// Is the token currently attached to a tile (not mid drag) - triggers LightPath redraw

	public virtual LightSegment BendPath(LightSegment prevSegment) {
		throw new System.Exception("This should never be called. Call the derived class's BendPath()");
	}

	// MonoBehaviour Methods ------------------------------------- //
	// Mimics constructor. Initializes variables - don't use Start()
	public Token Initialize(Vector2 position, LevelController levelController) {
		_level = levelController;
		_dragging = false;

		transform.localPosition = _lerpTarget = _initialPos = position;
		_level.AssignTokenToTile(this, position);
		_onTile = true;

		return this;
	}

	// Runs every frame. Lerps token if necessary
	void Update() {
		if(!_dragging) {
			if(((Vector2)transform.localPosition - _lerpTarget).magnitude > LERP_THRESHOLD) {
				transform.localPosition = Vector2.Lerp(transform.localPosition, _lerpTarget, Time.deltaTime * LERP_SPEED);

				if(((Vector2)transform.localPosition - _lerpTarget).magnitude <= REDRAW_DISTANCE && !_onTile) {
					_level.AssignTokenToTile(this, _lerpTarget);
					_onTile = true;

					_level.BuildPath();
				}
			}
		}
	}


	// Mouse Methods for Editor ---------------------------------- //
	public void OnMouseDown() {
		_offset = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.localPosition;
		_initialPos = TargetTilePosition(transform.localPosition);
		_dragging = true;
	}
	public void OnMouseDrag() {
		Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		transform.localPosition = mousePos - _offset;

		if(_onTile) {
			if(Vector2.Distance(transform.localPosition, _initialPos) >= REDRAW_DISTANCE) {
				_level.AssignTokenToTile(null, _initialPos);
				_onTile = false;

				_level.BuildPath();
			}
		}
	}
	public void OnMouseUp() {
		_lerpTarget = TargetTilePosition(transform.localPosition);
		_dragging = false;
	}


	// Touch Methods for Mobile ---------------------------------- //



	// Private Supporting Methods -------------------------------- //
	// Returns nearest Tile position if endPosition is within MAX_SNAP_DISTANCE from it, otherwise the Initial position of the Token
	private Vector2 TargetTilePosition(Vector2 endPosition) {
		Vector2 nearestTilePosition = new Vector2(
			(int)Mathf.Max(0, Mathf.Min(LevelController.GridSize.x - 1, endPosition.x + 0.5f)),
			(int)Mathf.Max(0, Mathf.Min(LevelController.GridSize.y - 1, endPosition.y + 0.5f))
		);
		float distanceToNearestTile = (nearestTilePosition - endPosition).magnitude;

		return (distanceToNearestTile <= MAX_SNAP_DISTANCE || MAX_SNAP_DISTANCE == 0)? nearestTilePosition : _initialPos;
	}
}
