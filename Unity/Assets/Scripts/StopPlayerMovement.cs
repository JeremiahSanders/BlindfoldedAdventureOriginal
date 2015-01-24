using UnityEngine;
using System.Collections;

public class StopPlayerMovement : MonoBehaviour {

    private void OnTriggerEnter(Collider col)
    {
        GameManager.instance.StopPlayerMovement();
    }
}
