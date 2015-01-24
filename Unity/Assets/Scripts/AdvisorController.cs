using UnityEngine;
using System.Collections;

public class AdvisorController : MonoBehaviour {

    public int MovementSpeed = 1;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
    void Update()
    {
        float vert = Input.GetAxis("Vertical Player 1") * MovementSpeed;
        float hor = Input.GetAxis("Horizontal Player 1") * MovementSpeed;
        //vert *= Time.deltaTime;
        //hor *= Time.deltaTime;
        //transform.Translate(hor, 0, vert);
        //transform.Rotate(0, rotation, 0);

        if (hor > 0) { GameManager.instance.AdviseRight(); }
        if (hor < 0) { GameManager.instance.AdviseLeft(); }
        if (vert > 0) { GameManager.instance.AdviseUp(); }
        if (vert < 0) { GameManager.instance.AdviseDown(); }
	
	}
}
