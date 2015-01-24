using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
    public static GameManager instance;
    public GameObject FloorTilePrefab;
    public GameObject PlayerPrefab;

    private GameObject currentPlayerObject;
    private GameObject currentLevelObject;
    private List<PlayerMovement> playerMovements;

    private PlayerMovement currentMovementRequest;



    private void Awake()
    {
        if (instance == null) instance = this;
        else{ if(instance != this)Destroy(gameObject);}
        if (playerMovements == null) playerMovements = new List<PlayerMovement>();
        Setup();
    }

    public void Setup()
    {
        currentPlayerObject = Instantiate(PlayerPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity) as GameObject;
        if (currentPlayerObject != null) {
            Renderer playerRenderer = currentPlayerObject.GetComponent<Renderer>();
            if (playerRenderer != null) {
                float playerDepth = playerRenderer.bounds.max.y - playerRenderer.bounds.min.y;
                currentPlayerObject.transform.Translate(new Vector3(0f, playerDepth + 2f, 0f));
            }
        }

        currentLevelObject = 
            Instantiate(FloorTilePrefab,
                //new Vector3(FloorTilePrefab.renderer.bounds.size.x/2f, 0f, FloorTilePrefab.renderer.bounds.size.z/2f),
                new Vector3(0f,0f,0f), 
                Quaternion.identity) as GameObject;
        if (currentLevelObject == null) {return;}
        Renderer[] renderers = currentLevelObject.GetComponentsInChildren<Renderer>();
        float minx = 0;
        float maxx = 0;
        float minz = 0;
        float maxz = 0;
        float miny = 0f;
        float maxy = 0f;
        foreach (Renderer r in renderers) {
            minx = Math.Min(r.bounds.center.x, minx);
            maxx = Math.Max(r.bounds.center.x, maxx);
            minz = Math.Min(r.bounds.center.z, minz);
            maxz = Math.Max(r.bounds.center.z, maxz);
            miny = Math.Min(r.bounds.min.y, miny);
            maxy = Math.Max(r.bounds.max.y, maxy);
        }
        float width = maxx - minx;
        float height = maxz - minz;
        float xLoc = -1f * width / 2f;
        float zLoc = -1f * height / 2f;
        float depth = maxy - miny;
        float yLoc = 0f;
        if (depth > 0f) {
            yLoc = 0f - (depth/2f); }
        if (depth < 0f) {
            yLoc = depth/2f;
        }
        currentLevelObject.transform.Translate(new Vector3(xLoc,yLoc,zLoc)); 
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    internal class PlayerMovement {
        public enum MovementDirection { Up,Down,Left,Right}
        public MovementDirection Direction { get; set; }
    }
}
