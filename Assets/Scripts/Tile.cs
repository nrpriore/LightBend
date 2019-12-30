using UnityEngine;

public class Tile {

	public Token Token { get{ return _token;} }
	public bool Enabled { get{ return _enabled;} }

	private bool _enabled;
	private Token _token;

	public Tile(bool enabled) {
		_enabled = enabled;
		_token = null;
	}

	public void AssignToken(Token token) {
		_token = token;
	}
}
