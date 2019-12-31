using UnityEngine;

// The most basic Token. Simply deflects the LightPath
public class Block : Token {
	
	// Override method determining how the LightPath is bent
	public override LightSegment BendPath(LightSegment prevSegment) {
		Vector2 prevEndSide = prevSegment.StartSide + prevSegment.Direction;

		return new LightSegment(
			prevSegment.TilePosition,
			prevEndSide,
			prevSegment.Direction + (-2 * prevEndSide)
		);
	}
}