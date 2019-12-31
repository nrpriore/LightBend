using UnityEngine;

// Governs all level "events" (creation, win condition, UI interaction, etc)
public class LevelController : MonoBehaviour {

	// Public references
	public Tile[][] Grid { get{ return _grid;} }
	public Vector2 GridSize { get{ return _gridSize;} }
	public LightSegment StartCoil { get{ return _startCoil;} }
	public LightSegment EndCoil { get{ return _endCoil;} }

	// Private references
	private GameController _game;
	private Transform _gridTransform;
	private Transform _tokensTransform;
	private Tile[][] _grid;
	private Vector2 _gridSize;
	private LightPath _path;
	private LightSegment _startSegment;
	private LightSegment _startCoil;
	private LightSegment _endCoil;


	// MonoBehaviour Methods ------------------------------------- //
	// Mimics a constructor. Call this when adding the component from GameController, don't put anything in Start()
	public LevelController Initialize(GameController game) {
		_game = game;
		_gridTransform = GameObject.Find("Grid").transform;
		_tokensTransform = GameObject.Find("Tokens").transform;
		_path = new LightPath(this);

		return this;
	}


	// Public Methods -------------------------------------------- //
	// Creates a Tile map
	public void CreateGrid(Vector2 size) {
		_gridSize = size = size + (2 * Vector2.one);	// Creates the disabled border tiles
		_grid = new Tile[(int)size.x][];

		for(int x = 0; x < size.x; x++) {
			_grid[x] = new Tile[(int)size.y];

			for(int y = 0; y < size.y; y++) {
				bool enabled = !IsBorder(x, y);
				_grid[x][y] = new Tile(enabled);

				if(enabled) {
					Instantiate(Resources.Load<GameObject>("Prefabs/Tile"), new Vector2(x, y), Quaternion.identity, _gridTransform);
				}
			}
		}
		Camera.main.transform.localPosition = new Vector3((size.x - 1) / 2f, (size.y - 1) / 2f, Camera.main.transform.localPosition.z);
	}

	// Sets up the level. Adds components and builds the initial path
	public void CreateLevel(LightSegment startSegment) {
		Vector2 position = new Vector2(2,2);
		Instantiate(Resources.Load<GameObject>("Prefabs/Block"), _tokensTransform).GetComponent<Token>().Initialize(position, this);
		position = new Vector2(1,2);
		Instantiate(Resources.Load<GameObject>("Prefabs/Block"), _tokensTransform).GetComponent<Token>().Initialize(position, this);
		position = new Vector2(5,2);
		Instantiate(Resources.Load<GameObject>("Prefabs/Block"), _tokensTransform).GetComponent<Token>().Initialize(position, this);
		position = new Vector2(3,2);
		Instantiate(Resources.Load<GameObject>("Prefabs/Block"), _tokensTransform).GetComponent<Token>().Initialize(position, this);
		position = new Vector2(4,2);
		Instantiate(Resources.Load<GameObject>("Prefabs/Block"), _tokensTransform).GetComponent<Token>().Initialize(position, this);
		position = new Vector2(1,1);
		Instantiate(Resources.Load<GameObject>("Prefabs/Block"), _tokensTransform).GetComponent<Token>().Initialize(position, this);

		_startSegment = startSegment;
		_startCoil = new LightSegment(_startSegment.TilePosition, _startSegment.EndSide, _startSegment.Direction * -1);
		float angleDirection = (_startSegment.Direction.x * _startSegment.Direction.y > 0)? -1 : 1;
		Instantiate(Resources.Load<GameObject>("Prefabs/Coil"), CoilPosition(_startSegment.TilePosition), Quaternion.Euler(0,0,45 * angleDirection));

		_endCoil = new LightSegment(new Vector2(2,0), new Vector2(0,1), new Vector2(-1,-1));
		angleDirection = (_endCoil.Direction.x * _endCoil.Direction.y > 0)? -1 : 1;
		Instantiate(Resources.Load<GameObject>("Prefabs/Coil"), CoilPosition(_endCoil.TilePosition), Quaternion.Euler(0,0,45 * angleDirection));
		BuildPath();
	}

	// Assigns a Token to a Tile on the _grid
	public void AssignTokenToTile(Token token, Vector2 position) {
		_grid[(int)position.x][(int)position.y].AssignToken(token);
	}

	// Builds the LightPath on the screen
	public void BuildPath() {
		_path.Build(_startSegment);
	}

	// Beat Level event
	public void CompleteLevel() {
		Debug.Log("Level beaten");
	}


	// Private Methods ------------------------------------------- //
	// Returns if the x,y coordinate is one of the disabled border tiles
	private bool IsBorder(int x, int y) {
		return x == 0 || x == GridSize.x - 1 || y == 0 || y == GridSize.y - 1;
	}
	// Returns the placement of a Coil given a particular border tile
	private Vector2 CoilPosition(Vector2 tilePosition) {
		return new Vector2(
			Mathf.Max(0.5f, Mathf.Min(tilePosition.x, GridSize.x - 1.5f)),
			Mathf.Max(0.5f, Mathf.Min(tilePosition.y, GridSize.y - 1.5f))
		);
	}
}
