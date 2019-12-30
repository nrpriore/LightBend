using UnityEngine;

public class Block : Token {
	

	public override LightSegment BendPath(LightSegment prevSegment) {
		Vector2 prevEndSide = prevSegment.StartSide + prevSegment.Direction;

		return new LightSegment(
			prevSegment.TilePosition,
			prevEndSide,
			prevSegment.Direction + (-2 * prevEndSide)
		);
	}
}