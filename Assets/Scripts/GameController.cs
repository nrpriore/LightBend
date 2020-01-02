using UnityEngine;

// Entry point of the application
public class GameController : MonoBehaviour {

	// Public references
	public LevelController LC { get{ return _lc;} }
	public LevelGenerator LG { get{ return _lg;} }

	// Private references
	private LevelController _lc;
	private LevelGenerator _lg;


	// MonoBehaviour Methods ------------------------------------- //
	// Entry point of the application
	void Awake() {
		AddComponents();

		_lg.CreateLevel("dickdick");
	}


	// Private Methods ------------------------------------------- //
	// Adds relevant components at game start
	private void AddComponents() {
		_lc = gameObject.AddComponent<LevelController>().Initialize(this);
		_lg = new LevelGenerator(_lc);
	}
}
