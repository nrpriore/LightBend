using UnityEngine;

// Entry point of the application
public class GameController : MonoBehaviour {

	// Public Component references
	public LevelController LC { get{ return _lc;} }

	#region private variables for properties
	private LevelController _lc;
	#endregion


	// MonoBehaviour Methods ------------------------------------- //
	// Entry point of the application
	void Awake() {
		AddComponents();

		_lc.CreateGrid(new Vector2(7,9));
		_lc.CreateLevel(new LightSegment(new Vector2(0,5), new Vector2(0, 1), new Vector2(1, -1)));
	}


	// Private Methods ------------------------------------------- //
	// Adds relevant components at game start
	private void AddComponents() {
		_lc = gameObject.AddComponent<LevelController>().Initialize(this);
	}
}
