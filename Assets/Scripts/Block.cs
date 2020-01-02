using UnityEngine;

// The most basic Token. Simply deflects the LightPath
public class Block : TokenType {

	public readonly int ID = 0;
	
	// Override method determining how the LightPath is bent
	public override LightSegment BendPath(LightSegment prevSegment) {
		return new LightSegment(
			prevSegment.TilePosition,
			prevSegment.EndSide,
			prevSegment.Direction + (-2 * prevSegment.EndSide)
		);
	}
}