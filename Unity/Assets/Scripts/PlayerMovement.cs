using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    // Use this for initialization
    private void Start() {}

    // Update is called once per frame
    private void Update()
    {
        float vert = Input.GetAxis("Vertical Player 1");
        float hor = Input.GetAxis("Horizontal Player 1");

        if (GameManager.instance.PlayerCanSubmitMove) {
            if (hor > 0) {
                GameManager.instance.PlayerRight();
            }
            else if (hor < 0) {
                GameManager.instance.PlayerLeft();
            }
            else if (vert > 0) {
                GameManager.instance.PlayerUp();
            }
            else if (vert < 0) {
                GameManager.instance.PlayerDown();
            }
        }

        if (Input.GetButton("Confirm Player 1")) {
            GameManager.instance.PlayerMove();
        }
        if (Input.GetButton("Cancel Player 1"))
        {
            GameManager.instance.CancelDirection();
        }
    }
}