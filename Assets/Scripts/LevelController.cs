using UnityEngine;

// Governs all level "events" (creation, win condition, UI interaction, etc)
public class LevelController : MonoBehaviour {

	// Public references
	public Tile[][] Grid 				{ get{ return _grid;} }
	public Vector2 GridSize 			{ get{ return _gridSize;} }
	public LightPath Path 				{ get{ return _path;} }
	public LightSegment StartSegment 	{ get{ return _startSegment;} }
	public LightSegment StartCoil 		{ get{ return _startCoil;} }
	public LightSegment EndCoil 		{ get{ return _endCoil;} }

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
	public void SetGrid(Tile[][] grid) {
		_gridSize = new Vector2(grid.Length, grid[0].Length);
		_grid = grid;
	}
	public void InstantiateGrid() {
		if(_grid.Length > 0) {
			int width = _grid.Length;
			int height = _grid[0].Length;

			for(int x = 0; x < width; x++) {
				for(int y = 0; y < height; y++) {
					if(_grid[x][y].Enabled) {
						Instantiate(Resources.Load<GameObject>("Prefabs/Tile"), new Vector2(x, y), Quaternion.identity, _gridTransform);
					}
				}
			}
			Camera.main.transform.localPosition = new Vector3((width - 1) / 2f, (height - 1) / 2f, Camera.main.transform.localPosition.z);
			Camera.main.orthographicSize = width / Camera.main.aspect / 2f;
		}
		else {
			throw new System.Exception("Don't call this before _grid has been set in SetGrid()");
		}
	}

	// Sets up the level. Adds components and builds the initial path
	public void SetLevel(LightSegment start = null, LightSegment end = null) {
		if(start != null) {
			_startSegment = start;
			_startCoil = new LightSegment(_startSegment.TilePosition, _startSegment.EndSide, _startSegment.Direction * -1);
		} 
		
		if(end != null) {
			_endCoil = end;
		}
	}
	public void InstantiateLevel() {
		for(int x = 1; x < _gridSize.x; x++) {
			for(int y = 0; y < _gridSize.y; y++) {
				if(_grid[x][y].TokenType != null) {
					Instantiate(Resources.Load<GameObject>("Prefabs/Block"), _tokensTransform).GetComponent<Token>().Initialize(_grid[x][y].TokenType, new Vector2(x, y), this);
				}
			}
		}

		/*Vector2 position;
		position = new Vector2(4,2);
		Instantiate(Resources.Load<GameObject>("Prefabs/Block"), _tokensTransform).GetComponent<Token>().Initialize(new Block(), position, this);
		position = new Vector2(1,1);
		Instantiate(Resources.Load<GameObject>("Prefabs/Block"), _tokensTransform).GetComponent<Token>().Initialize(new Block(), position, this);
		position = new Vector2(2,4);
		Instantiate(Resources.Load<GameObject>("Prefabs/Block"), _tokensTransform).GetComponent<Token>().Initialize(new Block(), position, this);*/

		float angleDirection = (_startCoil.Direction.x * _startCoil.Direction.y > 0)? -1 : 1;
		Instantiate(Resources.Load<GameObject>("Prefabs/Coil"), CoilPosition(_startCoil), Quaternion.Euler(0,0,45 * angleDirection));

		angleDirection = (_endCoil.Direction.x * _endCoil.Direction.y > 0)? -1 : 1;
		Instantiate(Resources.Load<GameObject>("Prefabs/Coil"), CoilPosition(_endCoil), Quaternion.Euler(0,0,45 * angleDirection));

		_path.Build();
	}

	// Assigns a Token to a Tile on the _grid
	public void AssignTokenToTile(Token token, Vector2 position) {
		_grid[(int)position.x][(int)position.y].AssignToken(token);
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
	private Vector2 CoilPosition(LightSegment coil) {
		return new Vector2(
			coil.TilePosition.x + 0.5f * coil.StartSide.x,
			coil.TilePosition.y + 0.5f * coil.StartSide.y
		);
	}
}
