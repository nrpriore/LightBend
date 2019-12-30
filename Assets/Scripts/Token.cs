using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// Base class for placeable pieces that governs movement
public class Token : MonoBehaviour {
	private const float MAX_SNAP_DISTANCE = 0f; 	// Max distance at DragEnd for a valid snap (middle of 4 tokens = 0.7071f, 0 = infinite)
	private const float LERP_THRESHOLD = 0.001f;	// Distance at which Update() will stop Lerping, to stop infinite Lerping
	private const float LERP_SPEED = 30f;			// Speed factor of Lerp

	private bool _dragging;			// If the token is being dragged currently
	private Vector2 _offset;		// Offset vector so center of token doesn't snap to mousePosition on click
	private Vector2 _initialPos;	// Initial token position on grid
	private Vector2 _lerpTarget;	// Target vector to Lerp to in Update()


	// MonoBehaviour Methods ------------------------------------- //
	// Runs when Token is added to scene. Initializes variables
	void Start() {
		_dragging = false;
		_lerpTarget = transform.localPosition;
	}

	// Runs every frame. Lerps token if necessary
	void Update() {
		if(!_dragging) {
			if(((Vector2)transform.localPosition - _lerpTarget).magnitude > LERP_THRESHOLD) {
				transform.localPosition = Vector2.Lerp(transform.localPosition, _lerpTarget, Time.deltaTime * LERP_SPEED);
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
			(int)Mathf.Max(0, Mathf.Min(GridController.GridSize.x - 1, endPosition.x + 0.5f)),
			(int)Mathf.Max(0, Mathf.Min(GridController.GridSize.y - 1, endPosition.y + 0.5f))
		);
		float distanceToNearestTile = (nearestTilePosition - endPosition).magnitude;

		return (distanceToNearestTile <= MAX_SNAP_DISTANCE || MAX_SNAP_DISTANCE == 0)? nearestTilePosition : _initialPos;
	}
}
