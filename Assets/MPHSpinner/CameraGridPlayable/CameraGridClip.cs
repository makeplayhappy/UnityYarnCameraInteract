using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class CameraGridClip : PlayableAsset, ITimelineClipAsset
{
    public CameraGridBehaviour template = new CameraGridBehaviour ();

    public ClipCaps clipCaps
    {
        get { return ClipCaps.Blending; }
    }

    public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<CameraGridBehaviour>.Create (graph, template);
        return playable;    
    }
}
