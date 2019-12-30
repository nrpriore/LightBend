using UnityEngine;

// Governs grid creation, manipulation, and deletion
public class GridController : MonoBehaviour {
	public static Vector2 GridSize;	// Temporary variable until better system is built

	// Private component references
	private GameController _game;
	private Transform _grid;


	// MonoBehaviour Methods ------------------------------------- //
	// Mimics a constructor. Call this when adding the component from GameController, don't put anything in Start()
	public GridController Initialize(GameController game) {
		_game = game;
		_grid = GameObject.Find("Grid").transform;

		return this;
	}


	// Public Methods -------------------------------------------- //
	// Creates a Tile map
	public void CreateGrid(Vector2 size) {
		GridSize = size;
		for(int x = 0; x < size.x; x++) {
			for(int y = 0; y < size.y; y++) {
				Instantiate(Resources.Load<GameObject>("Prefabs/Tile"), new Vector2(x, y), Quaternion.identity, _grid);
			}
		}
		Camera.main.transform.localPosition = new Vector3((size.x - 1) / 2f, (size.y - 1) / 2f, Camera.main.transform.localPosition.z);

		/*for(int i = 0; i < size.magnitude; i++) {
			//Instantiate(i % size.x, i / size.x)
		}*/
	}
}
