using UnityEngine;
using System.Collections;

public class CameraMovementScript : MonoBehaviour {
    
    private Vector3 normalPos;
    private Camera cam;

    public bool inBattle;
	// Use this for initialization
	void Start () {
        inBattle = false;
        normalPos = transform.position;
        cam = GetComponent<Camera>();
    }
	
	// Update is called once per frame
	void Update () {
        Vector3 target = normalPos;
        float orthoSize = 120f;
        if(inBattle) {
            orthoSize = 60f;
            target = GameMachine.gameMachine.PlayerPosition + Vector3.back * 10f;
        }
        transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * 2f);
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, orthoSize, Time.deltaTime);
    }
}
