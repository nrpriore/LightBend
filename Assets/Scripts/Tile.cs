using UnityEngine;

// Class used to represent a potential space to place a Token
public class Tile {

	// Public references
	public Token Token { get{ return _token;} }
	public bool Enabled { get{ return _enabled;} }

	// Private references
	private bool _enabled;
	private Token _token;


	// Constructor ----------------------------------------------- //
	public Tile(bool enabled) {
		_enabled = enabled;
		_token = null;
	}


	// Public Methods -------------------------------------------- //
	// Assigns a Token to this Tile
	public void AssignToken(Token token) {
		_token = token;
	}
}
