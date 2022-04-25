using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System.Collections.Generic;

[TrackColor(0.9454092f, 0.9779412f, 0.3883002f)]
[TrackClipType(typeof(CameraGridClip))]
[TrackBindingType(typeof(Camera))]
public class CameraGridTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<CameraGridMixerBehaviour>.Create (graph, inputCount);
    }

    public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
    {
#if UNITY_EDITOR
       Camera trackBinding = director.GetGenericBinding(this) as Camera;
       if (trackBinding == null)
           return;

#endif
        base.GatherProperties(director, driver);
    }
}
