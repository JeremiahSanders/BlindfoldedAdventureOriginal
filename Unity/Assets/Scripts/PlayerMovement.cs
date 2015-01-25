using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    public float Acceleration = 1f;

    // Use this for initialization
    private void Start() {}

    // Update is called once per frame
    private void Update()
    {
        float vert = Input.GetAxis("Vertical") * Acceleration * Time.deltaTime;
        float hor = Input.GetAxis("Horizontal") * Acceleration * Time.deltaTime;

        Rigidbody rigidbody = GetComponent<Rigidbody>();
        if (rigidbody == null) return;
        Vector3 motion = new Vector3(hor, 0f, vert);
        //Debug.Log(String.Format("Moving: {0}H {1}V", hor, vert));
        rigidbody.AddForce(motion);

        //transform.Translate(hor, 0f, vert);

        //if (GameManager.instance.PlayerCanSubmitMove) {
        //    if (hor > 0) {
        //        GameManager.instance.PlayerRight();
        //    }
        //    else if (hor < 0) {
        //        GameManager.instance.PlayerLeft();
        //    }
        //    else if (vert > 0) {
        //        GameManager.instance.PlayerUp();
        //    }
        //    else if (vert < 0) {
        //        GameManager.instance.PlayerDown();
        //    }
        //}

        //if (Input.GetButton("Confirm Player 1")) {
        //    GameManager.instance.PlayerMove();
        //}
        //if (Input.GetButton("Cancel Player 1"))
        //{
        //    GameManager.instance.CancelDirection();
        //}
    }
}