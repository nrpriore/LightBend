using UnityEngine;

// Entry point of the application
public class GameController : MonoBehaviour {

	// Public Component references
	public LevelController LC { get{ return _lc;} }
	public GridController GC { get{ return _gc;} }

	#region private variables for properties
	private LevelController _lc;
	private GridController _gc;
	#endregion


	// MonoBehaviour Methods ------------------------------------- //
	// Entry point of the application
	void Awake() {
		AddComponents();

		_gc.CreateGrid(new Vector2(5,7));
		_lc.CreateLevel(new LightSegment(new Vector2(0,4), new Vector2(-1, 0), new Vector2(1, -1)));
	}


	// Private Methods ------------------------------------------- //
	// Adds relevant components at game start
	private void AddComponents() {
		_lc = gameObject.AddComponent<LevelController>().Initialize(this);
		_gc = gameObject.AddComponent<GridController>().Initialize(this);
	}
}
