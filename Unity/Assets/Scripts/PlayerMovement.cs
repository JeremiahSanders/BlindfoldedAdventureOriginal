using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    public int MovementSpeed = 1;
    // Use this for initialization
    private void Start() {}
    // Update is called once per frame
    private void Update()
    {
        float vert = Input.GetAxis("Vertical") * MovementSpeed;
        float hor = Input.GetAxis("Horizontal") * MovementSpeed;
        vert *= Time.deltaTime;
        hor *= Time.deltaTime;
        transform.Translate(hor, 0, vert);
        //transform.Rotate(0, rotation, 0);
    }
}