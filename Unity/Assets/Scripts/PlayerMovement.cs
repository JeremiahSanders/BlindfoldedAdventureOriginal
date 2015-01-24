using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    public int MovementSpeed = 1;
    // Use this for initialization
    private void Start() {}
    // Update is called once per frame
    private bool isMoving = false;
    private void Update()
    {
        if (isMoving) return;
        isMoving = true;
        float vert = Input.GetAxis("Vertical Player 1") * MovementSpeed;
        float hor = Input.GetAxis("Horizontal Player 1") * MovementSpeed;
        vert *= Time.deltaTime;
        hor *= Time.deltaTime;
        //transform.Translate(hor, 0, vert);
        //transform.Rotate(0, rotation, 0);

        if (hor > 0) { GameManager.instance.PlayerRight(); }
        if (hor < 0) { GameManager.instance.PlayerLeft(); }
        if (vert > 0) { GameManager.instance.PlayerUp(); }
        if (vert < 0) { GameManager.instance.PlayerDown(); }

        if (Input.GetButton("Fire1"))
        {
            GameManager.instance.PlayerMove();
        }
        isMoving = false;
    }
}