using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class CameraGridBehaviour : PlayableBehaviour
{
    //Screen Layout Grid (should be the same for all currently active
    public Vector2 gridSize = new Vector2 (1, 1);

    //This cameras screen position in the grid
	public Rect gridPosition = new Rect(0, 0, 1, 1);

    //Margin: clockwise: x = left, y= top, z = right, w = bottom
	public Vector4 margin = new Vector4 (12, 12, 12, 12);
    

}
