using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode, RequireComponent(typeof(Camera))]
public class CameraGridEditor : MonoBehaviour
{
	[Header("Screen Layout Grid (should be the same for all currently active")]
    public Vector2 gridSize = new Vector2 (1, 1);

    [Header("This cameras screen position in the grid")]
	public Rect gridPosition = new Rect(0, 0, 1, 1);

    [Header("Margin: clockwise: x = left, y= top, z = right, w = bottom")]
	public Vector4 margin = new Vector4 (12, 12, 12, 12);

    [HideInInspector]
	public Camera _camera;

	[HideInInspector]
	private Rect _screenRect;
    public Rect _frameRect;

	#if UNITY_EDITOR
    void Start(){
        _camera = GetComponent<Camera> ();
        _screenRect = new Rect( 0, 0, Screen.width, Screen.height );

    }


	void Update () {
		if(UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode) {
			this.enabled = false;
		} else {
			Refresh();
		}
	}
	#endif



    private void Refresh(){

        float Top = -1.0f;
        float Right = -1.0f;
        float Bottom = -1.0f;
        float Left = -1.0f;
        float Width = 0.0f;
        float Height = 0.0f;


        _screenRect = new Rect( 0, 0, Screen.width, Screen.height );

        float unitWidth = _screenRect.width / (float)gridSize.x;
        float unitHeight = _screenRect.height / (float)gridSize.y;

        Width = unitWidth * gridPosition.width;
        Height = unitHeight * gridPosition.height;
        Left = unitWidth * gridPosition.x;
        Right = Left + Width;
        Top = _screenRect.height - (unitHeight * gridPosition.y);
        Bottom = Top - Height;

        float marginLeft = 0.0f;
        float marginRight = 0.0f;
        float marginTop = 0.0f;
        float marginBottom = 0.0f;

        //clockwise: x = left, y= top, z = right, w = bottom  
        marginLeft = margin.x * 2;
        marginTop = margin.y * 2;
        marginRight = margin.z * 2;
        marginBottom = margin.w * 2;
        

        // make sure width and height aren't less than 1 and that margin aren't larger than width or height
        Width = Mathf.Max (1.0f, Right - marginRight - (Left + marginLeft));
        Height = Mathf.Max (1.0f, Top - marginTop - (Bottom + marginBottom));

        _frameRect.x = Left + Mathf.Min (marginLeft, Width) + _screenRect.x;
        _frameRect.y = Bottom + Mathf.Min (marginBottom, Height) + _screenRect.y;
        _frameRect.width = Width;
        _frameRect.height = Height;

        _camera.pixelRect = _frameRect;
    }


}