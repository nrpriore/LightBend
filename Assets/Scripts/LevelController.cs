using UnityEngine;

// Governs all level "events" (creation, win condition, UI interaction, etc)
public class LevelController : MonoBehaviour {

	// Private component references
	private GameController _game;
	private LightPath _path;

	// MonoBehaviour Methods ------------------------------------- //
	// Mimics a constructor. Call this when adding the component from GameController, don't put anything in Start()
	public LevelController Initialize(GameController game) {
		_game = game;
		_path = new LightPath();

		return this;
	}


	// Public Methods -------------------------------------------- //
	// Sets up the level. Adds components and builds the initial path
	public void CreateLevel(LightSegment startSegment) {
		_path.Build(startSegment);
	}
}
