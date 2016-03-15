using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {
    
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

        Vector3 target;
        int orthograSize = 120;
        if (inBattle)
        {
            target = GameMachine.gameMachine.PlayerPosition + Vector3.back * 10f; // playerPosition - Vec3(0,0 -10);
            orthograSize = 60;
        }
        else
        {
            target = normalPos;
        }
        transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * 2f);
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, orthograSize, Time.deltaTime);
	}
}
