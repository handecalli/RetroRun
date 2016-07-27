using UnityEngine;
using System.Collections;

public class CameraControllerOnline : MonoBehaviour {

	public float dampTime = 0.15f;
	public float toPointX = 0.5f;	
    private GameObject mainCamera;

	private Vector3 velocity = Vector3.zero;

	Camera cam;     

	void Start(){
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
		cam = mainCamera.GetComponent<Camera>();
        cam.enabled = true;
	}

	void FixedUpdate () 
	{
		if (mainCamera)
		{
			Vector3 point = cam.WorldToViewportPoint(transform.position);
			Vector3 delta = transform.position - cam.ViewportToWorldPoint(new Vector3(toPointX, 0.5f, point.z)); 
			Vector3 destination = mainCamera.transform.position + delta;

			mainCamera.transform.position = Vector3.SmoothDamp(mainCamera.transform.position, destination, ref velocity, dampTime);
		}
		
	}

}
