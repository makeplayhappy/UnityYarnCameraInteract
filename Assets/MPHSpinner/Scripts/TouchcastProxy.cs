using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// handles input touch and interaction with draggable and spawnable gameobjects
public class TouchcastProxy : MonoBehaviour{

    [Header("Main Scene Raycasts")]
    public LayerMask raycastLayermask;
    public float raycastDistance = 25f;

    public bool hasExitVelocity = true;
    public float impulseMultiplier = 0.25f;
    public float forceMagnitudeClamp = 20f;

    [Tooltip("This sets the plane dragged objects will be projected onto")]
    public Transform draggablePlaneTransform;

    private Plane draggablePlane;
    private Ray ray;
    private RaycastHit rayHitInfo = new RaycastHit();
    private Touch currentTouch;
    public Camera[] cameras;
    private Rect[] _cameraPixelRect;
    private TouchcastSettings[] _touchcastSettings;

    private int[] camIndex = new int[5];
    private Transform[] dragging = new Transform[5];
    private Vector2[] lastPositions = new Vector2[5];
    private Vector2[] lastVelocity = new Vector2[5];
    private Vector2[] lastDelta = new Vector2[5];

    private Vector3[] resetPosition = new Vector3[5];

    








    void Start(){
        _cameraPixelRect = new Rect[ cameras.Length ];
        _touchcastSettings = new TouchcastSettings[ cameras.Length ];

        //create camera to rect mappings
        for(int i = 0;i < cameras.Length;i++){
            _cameraPixelRect[i] = cameras[i].pixelRect;
            _touchcastSettings[i] = cameras[i].GetComponent<TouchcastSettings>();
        }

        if( draggablePlaneTransform != null){
        //for now I'll use the up vector as the normal
            draggablePlane = new Plane(draggablePlaneTransform.up, draggablePlaneTransform.position);
        }
        
    }

/*
TouchPhase ---
Began	A finger touched the screen.
Moved	A finger moved on the screen.
Stationary	A finger is touching the screen but hasn't moved.
Ended	A finger was lifted from the screen. This is the final phase of a touch.
Canceled	The system cancelled tracking for the touch.
*/

    private int getCameraIndexAtPixel(Vector2 pixelPosition){
        for(int i = 0;i < _cameraPixelRect.Length;i++){
            if(_cameraPixelRect[i].Contains(pixelPosition) ){
                return i;
            }
        }
        return -1;
    }
    //check each finger / touch and store save details into the arrays
    void Update(){

        if ( Input.touchCount > 0 ){

            for (int i = 0; i<Input.touchCount; i++){

                currentTouch = Input.GetTouch(i);
                Vector2 currentTouchPosition = currentTouch.position;

               // Debug.Log ("Touch TouchPhase. " + currentTouch.phase );

                switch( currentTouch.phase ){
                    case TouchPhase.Began:
                        //check no UI elements are getting clicked
                        if ( EventSystem.current != null && EventSystem.current.currentSelectedGameObject != null ){
#if UNITY_EDITOR                            
                            Debug.Log("UI Element blocking touchable raycast");
#endif                            
                            continue; //jump to next for
                        }

                        //camIndex[ currentTouch.fingerId ] = -1;
                        int t_camIndex = getCameraIndexAtPixel( currentTouchPosition );
                        camIndex[ currentTouch.fingerId ] = t_camIndex;

                        if( camIndex[ currentTouch.fingerId ] < 0){
#if UNITY_EDITOR                            
                            Debug.Log("No Camera found at " + currentTouchPosition);

#endif
                            continue; //jump to next for
                        }

                        ray = cameras[ t_camIndex ].ScreenPointToRay( currentTouchPosition );

                        if(Physics.Raycast(ray, out rayHitInfo, raycastDistance, raycastLayermask)){
                            //Debug.Log ("Touch TouchPhase Began Finger ID:" + currentTouch.fingerId);
                            //Transform hitTransform = rayHitInfo.collider.transform;
                            //Debug.Log ("Touched " + hitTransform.name + " tag: " + hitTransform.tag);


                            dragging[ currentTouch.fingerId ] = _touchcastSettings[t_camIndex].touchTransform;
                            dragging[ currentTouch.fingerId ].position = rayHitInfo.point;

                            resetPosition[ currentTouch.fingerId ] = _touchcastSettings[t_camIndex].startPosition;

                            setForDrag( dragging[ currentTouch.fingerId ] );

                            if( hasExitVelocity ){
                                lastVelocity[ currentTouch.fingerId ] = Vector2.zero;
                                lastPositions[ currentTouch.fingerId ] = currentTouchPosition;
                            }

                            lastDelta[ currentTouch.fingerId ] = currentTouch.deltaPosition;
/*
                            switch( hitTransform.tag ){
                                case "Draggable":

                                    draggableSetting[ currentTouch.fingerId ] = hitTransform.GetComponent<DraggableSetting>();                             

                                break;
                                case "Player":

                                    touchController[ currentTouch.fingerId ] = hitTransform.GetComponent<TouchController>();
                                    touchController[ currentTouch.fingerId ].SetTouchRay(ray);

                                break;
#if UNITY_EDITOR
                                default:

                                    Debug.Log( hitTransform.name + " on raycast layer without (Draggable or Player) tag: " + hitTransform.tag);

                                break;
#endif

                                

                            }
*/
                            
                            
                        }
                    break;
                    case TouchPhase.Moved:

                        if( dragging[ currentTouch.fingerId ] != null ){
                            //Debug.Log ("Dragging TouchPhase Moved Finger ID:" + currentTouch.fingerId);

                            
                            ray = cameras[ camIndex[ currentTouch.fingerId ] ].ScreenPointToRay( currentTouchPosition );

                            if(Physics.Raycast(ray, out rayHitInfo, raycastDistance, raycastLayermask)){

                                dragging[ currentTouch.fingerId ].position = rayHitInfo.point;

                            }

/*
                                switch( dragging[ currentTouch.fingerId ].tag ){

                                    case "Player":
                                        touchController[ currentTouch.fingerId ].SetTouchRay(ray);
                                    break;

                                    case "Draggable":

                                        float enter = 0.0f;
                                        if (draggablePlane.Raycast(ray, out enter)){
                                            //Get the point that is clicked
                                            Vector3 hitPoint = ray.GetPoint(enter);
                                            //Move your cube GameObject to the point where you clicked
                                            Vector3 offset = Vector3.zero;

                                            if( draggableSetting[ currentTouch.fingerId ] != null){

                                                hitPoint += draggableSetting[ currentTouch.fingerId ].dragOffset;

                                                if( draggableSetting[ currentTouch.fingerId ].dragTarget != null && draggableSetting[ currentTouch.fingerId ].dragTargetAxes != Vector3.zero){
                                                    //loop the Vector3 axes
                                                    for(int k=0;k<3;k++){
                                                        if( draggableSetting[ currentTouch.fingerId ].dragTargetAxes[k] != 0){
                                                            // get distance from target
                                                            float dist = hitPoint[k] - draggableSetting[ currentTouch.fingerId ].dragTarget.position[k];
                                                            // evaluate curve 
                                                            float tagettedPos = draggableSetting[ currentTouch.fingerId ].targetFalloff.Evaluate(dist);
                                                            //Debug.Log(k+ ": drag target dist:" + dist + " new pos : " + tagettedPos);
                                                            // force in axes to target
                                                            hitPoint[k] = tagettedPos;

                                                        }

                                                    }

                                                }
                                            }

                                            dragging[ currentTouch.fingerId ].position = constrainPosition(hitPoint);  
                                        }           

                                    break;

                                }
*/

                                
                                if( hasExitVelocity ){
                                    lastVelocity[ currentTouch.fingerId ] = (lastVelocity[ currentTouch.fingerId ] +  ( (currentTouchPosition - lastPositions[currentTouch.fingerId]) / Time.deltaTime ) ) * 0.5f;
                                    lastPositions[ currentTouch.fingerId ] = currentTouchPosition;
                                }

                                lastDelta[ currentTouch.fingerId ] = currentTouch.deltaPosition;

/*
                            }else{

                                Debug.Log("Err: No draggable plane setup");

                            }
*/                            
                        }
                        
                    break;
                    case TouchPhase.Stationary:

                        if( dragging[ currentTouch.fingerId ] != null ){

                            if( hasExitVelocity ){
                                lastVelocity[ currentTouch.fingerId ] = (lastVelocity[ currentTouch.fingerId ] +  ( (currentTouchPosition - lastPositions[currentTouch.fingerId]) / Time.deltaTime ) ) * 0.5f;
                                lastPositions[ currentTouch.fingerId ] = currentTouchPosition;
                            }
                            lastDelta[ currentTouch.fingerId ] = currentTouch.deltaPosition;
                        }
                        
                        /*
                        if( dragging[ currentTouch.fingerId ] != null ){
                            
                        }else if( isPressable ){
                            if(Physics.Raycast(ray, out rayHitInfo, raycastDistance, raycastLayermask)){
                                //Debug.Log ("Touch TouchPhase Began Finger ID:" + currentTouch.fingerId);
                                Transform hitTransform = rayHitInfo.collider.transform;
                                //Debug.Log ("Stationary on " + hitTransform.name + " tag: " + hitTransform.tag);

                                switch( hitTransform.tag ){
                                    case "Eventable":
                                        //Debug.Log("Stationary EVENT touch");
                                        InteractionEvent i_event = hitTransform.GetComponent<InteractionEvent>();
                                        i_event.touched();

                                    break;
                                }
                            }
                            
                        }
                        */
                    
                    break;

                    case TouchPhase.Ended:
                    case TouchPhase.Canceled:
                        if( dragging[ currentTouch.fingerId ] != null){

                            unsetForDrag( currentTouch.fingerId );

                        }
                        if( hasExitVelocity ){
                            lastVelocity[ currentTouch.fingerId ] = Vector2.zero;
                            lastPositions[ currentTouch.fingerId ] = Vector2.zero;
                        }
                        lastDelta[ currentTouch.fingerId ] = Vector2.zero;


                    break;
                    default:
                        if( dragging[ currentTouch.fingerId ] != null ){

                            if( hasExitVelocity ){
                                lastVelocity[ currentTouch.fingerId ] = (lastVelocity[ currentTouch.fingerId ] +  ( (currentTouchPosition - lastPositions[currentTouch.fingerId]) / Time.deltaTime ) ) * 0.5f;
                                lastPositions[ currentTouch.fingerId ] = currentTouchPosition;
                            }
                            lastDelta[ currentTouch.fingerId ] = currentTouch.deltaPosition;
                        }
                        /*
                        if( dragging[ currentTouch.fingerId ] != null){

                            //unsetForDrag( currentTouch.fingerId );

                            //switch isKinematic off here and then set force on next frame during the unsetDrag method
                            //if you try and set veolicty on same frame it fails to accept the new force (I guess because we're in update - after fixedUpdate)
                            Rigidbody rb = dragging[ currentTouch.fingerId ].gameObject.GetComponent<Rigidbody>();

                            if(rb != null && rb.isKinematic){
                                rb.isKinematic = false;
                            }
                            //Debug.Log ("Default Finger ID:" + currentTouch.fingerId);
                        }
                        */
                    break;

                }// end switch of touchPhase


                //do final touch post processing calculations




            } // end for ( Input.touchCount ) loop

        }else{ 
            // no touches
            //clean up array once all touches are un-touched
            for(int i = 0; i < dragging.Length; i++){
                if( dragging[ i ] != null){

                    unsetForDrag( i );
                    
                }
            }
        }
        
    }

    public bool isBeingDragged(Transform transformToCheck){
        if( System.Array.IndexOf(dragging, transformToCheck) == -1 ){
            return false;
        }
        return true;
    }

    private void unsetForDrag(int fingerId){

        if( dragging[ fingerId ] != null){

            if( hasExitVelocity ){
/*
                switch( dragging[ currentTouch.fingerId ].tag ){

                    case "Player":
                        if( lastVelocity[fingerId ] != Vector2.zero ){
                            // draggablePlaneTransform.up
                            touchController[ currentTouch.fingerId ].AddExitVelocity( lastVelocity[fingerId ] );
                        }


                    break;

                    case "Draggable":

                        Transform tr = dragging[ fingerId ];
                        Rigidbody rb = tr.gameObject.GetComponent<Rigidbody>();

                        if(rb != null ){//&& rb.isKinematic){
                            //rb.isKinematic = false;
                            //set velocity
                            if( lastVelocity[fingerId ] != Vector2.zero ){
                                //rb.velocity = lastVelocity[fingerId ];
                                //Debug.Log( lastVelocity[fingerId ].magnitude);
                                Vector3 forceToAdd = lastVelocity[fingerId ] * impulseMultiplier;
                                if( forceToAdd.sqrMagnitude > forceMagnitudeClamp * forceMagnitudeClamp ){
                                    forceToAdd = Vector3.ClampMagnitude( forceToAdd * impulseMultiplier , forceMagnitudeClamp);
                                }
                                rb.AddForce( forceToAdd, ForceMode.VelocityChange);
                                lastPositions[fingerId] = tr.position;
                            }
                        }
                    break;

                }
                */
            }

            dragging[ fingerId ].position = resetPosition[ fingerId ];

            lastDelta[ fingerId ] = Vector2.zero;
            dragging[ fingerId ] = null;
        }
    }
/*
    //if constraints is defined and has 2 elements [0] => min and [1] => max
    //this will clamp the positon to the constraitn positions
    private Vector3 constrainPosition(Vector3 touchPosition){
        if( constraints.Length == 2){
            touchPosition.x = Mathf.Clamp( touchPosition.x, constraints[0].x, constraints[1].x );
            touchPosition.y = Mathf.Clamp( touchPosition.y, constraints[0].y, constraints[1].y );
            touchPosition.z = Mathf.Clamp( touchPosition.z, constraints[0].z, constraints[1].z );
        }
        return touchPosition;
    }
*/

    private void setForDrag(Transform transform){

        Rigidbody rb = transform.gameObject.GetComponent<Rigidbody>();
        if(rb != null && !rb.isKinematic){
            rb.isKinematic = true;
        }

    }
/*
    private Camera checkIsValidMainCamera( Camera cam){
        
        return cam;
    }
*/

#if UNITY_EDITOR
    void OnDrawGizmosSelected(){
        // Draw a semitransparent blue cube at the transforms position
        if( draggablePlaneTransform != null){
            Matrix4x4 default_matrix = Gizmos.matrix; 
            Gizmos.color = new Color(0, 0, 1, 0.4f);
            Gizmos.matrix = draggablePlaneTransform.localToWorldMatrix;
            Gizmos.DrawCube(draggablePlaneTransform.position, new Vector3(1f, 0.02f, 1f));
            Gizmos.matrix = default_matrix;
        }
/*
        if( constraints.Length == 2){
            Gizmos.color = new Color(0, 1, 0, 0.4f);
            Gizmos.DrawCube( (constraints[1] + constraints[0])*0.5f,  (constraints[1] - constraints[0]) );
        }
        */
    }
#endif 
}
