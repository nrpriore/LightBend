using UnityEngine;

// Class used to represent a potential space to place a Token
public class Tile {

	// Public references
	public bool Enabled { get{ return _enabled;} }
	public Token Token { get{ return _token;} }
	public TokenType TokenType { get{ return _tokenType;} }

	// Private references
	private bool _enabled;
	private Token _token;
	private TokenType _tokenType;


	// Constructor ----------------------------------------------- //
	public Tile(bool enabled) {
		_enabled = enabled;
		_token = null;
		_tokenType = null;
	}


	// Public Methods -------------------------------------------- //
	// Assigns a Token to this Tile
	public void AssignToken(Token token) {
		_token = token;
	}
	// Assigns a TokenType to this Tile, used by Level Generation, so we know what TokenType to assign on instantiation
	public void AssignTokenType(TokenType tokenType) {
		_tokenType = tokenType;
	}
}
