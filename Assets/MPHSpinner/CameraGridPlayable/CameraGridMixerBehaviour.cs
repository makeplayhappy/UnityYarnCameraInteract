using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class CameraGridMixerBehaviour : PlayableBehaviour
{
    Rect m_DefaultFrameRect;

    Camera m_TrackBinding;
    bool m_FirstFrameHappened;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        m_TrackBinding = playerData as Camera;

        if (m_TrackBinding == null)
            return;

        if (!m_FirstFrameHappened)
        {
            m_DefaultFrameRect = m_TrackBinding.pixelRect;

            m_FirstFrameHappened = true;
        }

        int inputCount = playable.GetInputCount ();

        Rect blendedRect = Rect.zero;

        //generate the rects - then blend between float x, float y, float width, float height - to generate blended 

        List<Rect> blendableRects = new List<Rect>();
        List<float> blendableWeights = new List<float>();

        for (int i = 0; i < inputCount; i++){

            float inputWeight = playable.GetInputWeight(i);

            if( inputWeight > 0f){
                ScriptPlayable<CameraGridBehaviour> inputPlayable = (ScriptPlayable<CameraGridBehaviour>)playable.GetInput(i);
                CameraGridBehaviour input = inputPlayable.GetBehaviour ();

                blendableRects.Add( CamRectFromGrid( input.gridSize , input.gridPosition, input.margin) );
                blendableWeights.Add( inputWeight );


            }

        }

        if( blendableRects.Count > 1 ){

        
            float blendedX = blendableRects[0].x; 
            float blendedY = blendableRects[0].y; 
            float blendedWidth = blendableRects[0].width;
            float blendedHeight = blendableRects[0].height;

            for (int i = 1; i < blendableRects.Count; i++){
                blendedX = Mathf.Lerp( blendedX, blendableRects[i].x, blendableWeights[i]);
                blendedY = Mathf.Lerp( blendedX, blendableRects[i].y, blendableWeights[i]);
                blendedWidth = Mathf.Lerp( blendedWidth, blendableRects[i].width, blendableWeights[i]);
                blendedHeight = Mathf.Lerp( blendedHeight, blendableRects[i].height, blendableWeights[i]);
            }
            m_TrackBinding.pixelRect = new Rect(blendedX,blendedY,blendedWidth,blendedHeight);
            return;

        }else if( blendableRects.Count == 1){
            m_TrackBinding.pixelRect = blendableRects[0];
        }

    }

    private Rect CamRectFromGrid( Vector2 gridSize , Rect gridPosition, Vector4 margin){

        float top = -1.0f;
        float right = -1.0f;
        float bottom = -1.0f;
        float left = -1.0f;
        float width = 0.0f;
        float height = 0.0f;


        Rect _screenRect = new Rect( 0, 0, Screen.width, Screen.height );

        float unitWidth = _screenRect.width / (float)gridSize.x;
        float unitHeight = _screenRect.height / (float)gridSize.y;

        width = unitWidth * gridPosition.width;
        height = unitHeight * gridPosition.height;
        left = unitWidth * gridPosition.x;
        right = left + width;
        top = _screenRect.height - (unitHeight * gridPosition.y);
        bottom = top - height;

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
        width = Mathf.Max (1.0f, right - marginRight - (left + marginLeft));
        height = Mathf.Max (1.0f, top - marginTop - (bottom + marginBottom));

        Rect _frameRect = Rect.zero;
        _frameRect.x = left + Mathf.Min (marginLeft, width) + _screenRect.x;
        _frameRect.y = bottom + Mathf.Min (marginBottom, height) + _screenRect.y;
        _frameRect.width = width;
        _frameRect.height = height;
        return _frameRect;

        //_camera.pixelRect = _frameRect;
    }

    public override void OnPlayableDestroy (Playable playable)
    {
        m_FirstFrameHappened = false;

        if(m_TrackBinding == null)
            return;
        

        m_TrackBinding.pixelRect = m_DefaultFrameRect;

    }
}
