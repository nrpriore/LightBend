using UnityEngine;

// Entry point of the application
public class GameController : MonoBehaviour {

	// Public Component references
	public GridController GC { get{ return _gc;} }

	#region private variables for properties
	private GridController _gc;
	#endregion


	// MonoBehaviour Methods ------------------------------------- //
	// Entry point of the application
	void Awake() {
		AddComponents();

		_gc.CreateGrid(new Vector2(5,7));
	}


	// Private Methods ------------------------------------------- //
	// Adds relevant components at game start
	private void AddComponents() {
		_gc = gameObject.AddComponent<GridController>().Initialize(this);
	}
}
