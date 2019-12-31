using UnityEngine;
using System.Linq;

// Factory class used to generate levels based on a given or random seed
public class LevelGenerator {

	// Private references
	private LevelController _level;


	// Constructor ----------------------------------------------- //
	public LevelGenerator(LevelController levelController) {
		_level = levelController;
	}


	// Public Methods -------------------------------------------- //
	// Creates a level based on a given or random seed
	public void CreateLevel(string seed = "") {
		if(seed == "") {
			seed = GenerateRandomSeed(8);
		}
		
		_level.CreateGrid(new Vector2(5,7));
		_level.CreateLevel(new LightSegment(new Vector2(4,0), new Vector2(1, 0), new Vector2(-1, 1)));
	}


	// Private Methods ------------------------------------------- //
	// Generates a random alphanumeric string of given length
	private string GenerateRandomSeed(int length) {
		string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

		int tempSeed = (int)(UnityEngine.Random.value * (int.MaxValue - 1));
		System.Random random = new System.Random(tempSeed);
		return new string(Enumerable.Range(1, length).Select(_ => chars[random.Next(chars.Length)]).ToArray());
	} 
}
