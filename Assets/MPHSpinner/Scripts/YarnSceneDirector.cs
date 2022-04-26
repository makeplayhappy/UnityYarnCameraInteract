using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Yarn.Unity;

public class YarnSceneDirector : MonoBehaviour {
    public PlayableDirector[] timelines;
    private DialogueRunner dialogueRunner; // utility object that serves lines of dialogue
    private FadeLayer fadeLayer; // black overlay used to fade in/out of scenes

    // when this scene conductor object is created
    // (in our example, this happens when the scene is created)
    private void Awake() {
        // get handles of utility objects in the scene that we need
        dialogueRunner = FindObjectOfType<Yarn.Unity.DialogueRunner>();
        dialogueRunner.AddCommandHandler<string>("playTimeline", PlayTimeline);
#if UNITY_EDITOR
        for(int i = 0;i < timelines.Length;i++){
            Debug.Log("Timeline " + i + ": " + timelines[i].name );
        }
        Debug.Log("public class YarnSceneDirector created.");
#endif
    }

    private void PlayTimeline(string timelineName){
        PlayableDirector timeline = getTimelineByName(timelineName);

        if( timeline != null){
            CheckAndStopPlayingTimelines();
            timeline.Play();
        }
        
    }

    private void CheckAndStopPlayingTimelines(){
        for(int i = 0;i < timelines.Length;i++){
            if( timelines[i].state == PlayState.Playing && timelines[i].playableGraph.IsValid() && timelines[i].playableGraph.IsPlaying() ){
                timelines[i].Stop();
            }
        }
    }

    private PlayableDirector getTimelineByName(string searchName){
        for(int i = 0;i < timelines.Length;i++){
            if( timelines[i].name == searchName){
                return timelines[i];
            }
        }
        return null;
    }

}