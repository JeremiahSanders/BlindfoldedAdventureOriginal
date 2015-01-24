using UnityEngine;
using System.Collections;

public class DeathZone : MonoBehaviour {

    private void OnTriggerEnter(Collider col)
    {
        GameManager.instance.LoseLife();
    }
}
