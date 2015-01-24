using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
    public static GameManager instance;
    public GameObject FloorTilePrefab;
    public GameObject PlayerPrefab;
    public AudioClip RequestUp;
    public AudioClip RequestDown;
    public AudioClip RequestLeft;
    public AudioClip RequestRight;
    public int FreeTurns = 5;
    public int TurnsBetweenTileDrop = 3;
    public int Lives = 3;

    public AudioClip AdvisePositive;
    public AudioClip AdviseNegative;

    private Vector3 origin = new Vector3(0f,0f,0f);
    private GameObject currentPlayerObject;
    private GameObject currentLevelObject;
    private List<PlayerMovement> playerMovements;
    public int TurnNumber { get { return playerMovements == null ? 0 : playerMovements.Count + 1; } }
    private PlayerMovement currentMovementRequest;

    public TurnPhase CurrentPhase { get; private set; }

    public enum TurnPhase {
        ProposeAction = 1,
        Advise = 2,
        PerformAction = 3
    }

    public void PlayerRight()
    {
        ChangeDirection(PlayerMovement.MovementDirection.Right);
    }
    public void PlayerLeft()
    {
        ChangeDirection(PlayerMovement.MovementDirection.Left);
    }
    public void PlayerDown()
    {
        ChangeDirection(PlayerMovement.MovementDirection.Down);
    }
    public void PlayerUp()
    {
        ChangeDirection(PlayerMovement.MovementDirection.Up);
    }


    public void AdviseRight()
    {
        //ChangeDirection(PlayerMovement.MovementDirection.Right);
    }
    public void AdviseLeft()
    {
        //ChangeDirection(PlayerMovement.MovementDirection.Left);
    }
    public void AdviseDown()
    {
        //ChangeDirection(PlayerMovement.MovementDirection.Down);
    }
    public void AdviseUp()
    {
        //ChangeDirection(PlayerMovement.MovementDirection.Up);
    }

    private void DropTile()
    {
        if (currentLevelObject == null) { return;}
        Transform[] childTransforms = currentLevelObject.GetComponentsInChildren<Transform>();
        if (childTransforms == null || childTransforms.Length == 0) { return; }
        int tileToDrop = UnityEngine.Random.Range(0, childTransforms.Length);
        GameObject tileToDestroy = childTransforms[tileToDrop].gameObject;
        Destroy(tileToDestroy);
    }

    public void AdviseYes()
    {
        DoAdvise(true);
    }

    public void AdviseNo()
    {
        DoAdvise(false);
    }

    private void DoAdvise(bool doIt)
    {
        if (CurrentPhase != TurnPhase.Advise) {
            return;
        }
        if (currentMovementRequest.Advice.HasValue) { return;}
        AudioSource.PlayClipAtPoint(doIt ? AdvisePositive : AdviseNegative, origin);
        currentMovementRequest.Advice = doIt;
        CurrentPhase = TurnPhase.PerformAction;
    }


    private void ChangeDirection(PlayerMovement.MovementDirection direction)
    {
        switch (CurrentPhase) {
            case TurnPhase.ProposeAction:
                if (currentMovementRequest != null && currentMovementRequest.RequestDirection == direction) {
                    return;
                }
                if (currentMovementRequest == null) {
                    currentMovementRequest = new PlayerMovement {RequestDirection = direction};
                }
                else {
                    currentMovementRequest.RequestDirection = direction; 
                }
                PlayCurrentRequestSound();
                CurrentPhase = TurnPhase.Advise;
                break;
            case TurnPhase.Advise:
                break;
            case TurnPhase.PerformAction:
                currentMovementRequest.FinalDirection = direction;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void PlayerMove()
    {
        if (currentMovementRequest == null || currentMovementRequest.FinalDirection == null) {
            return;
        }
        Rigidbody playerRigidbody = currentPlayerObject.GetComponent<Rigidbody>();
        //float multiplier = 2f;
        switch (currentMovementRequest.FinalDirection.Value) {
            case PlayerMovement.MovementDirection.Up:
                currentPlayerObject.transform.Translate(0f, 0f, 1f);
                //playerRigidbody.AddForce(0f, 0f, 1f*multiplier); 
                break;
            case PlayerMovement.MovementDirection.Down:
                currentPlayerObject.transform.Translate(0f, 0f, -1f);
                //playerRigidbody.AddForce(0f, 0f, -1f*multiplier);
                break;
            case PlayerMovement.MovementDirection.Left:
                currentPlayerObject.transform.Translate(-1f, 0f, 0f);
                //playerRigidbody.AddForce(-1f*multiplier, 0f, 0f);
                break;
            case PlayerMovement.MovementDirection.Right:
                currentPlayerObject.transform.Translate(1f, 0f, 0f);
                //playerRigidbody.AddForce(1f*multiplier, 0f, 0f);
                break;
            default:
                throw new ArgumentOutOfRangeException("currentMovementRequest.FinalDirection");
        }
        RaycastHit hit;
        playerRigidbody.SweepTest(new Vector3(0f, -50f, 0f), out hit, 50f);
        if (hit.distance == 0f) {
            // THERE'S NOTHING THERE!
            playerRigidbody.isKinematic = false;
        }
        else {
            AdvanceTurn(currentMovementRequest);
        }
    }

    private void ResetLevel()
    {
        if (currentPlayerObject != null) { Destroy(currentPlayerObject);}
        ResetPlayerObject();
        playerMovements.Clear();
        if (currentLevelObject != null) { Destroy(currentLevelObject);}
        ResetLevelObject();
    }

    private void AdvanceTurn(PlayerMovement currentMovement)
    {
        if (currentMovement != null) {
            playerMovements.Add(currentMovement);
        }
        currentMovementRequest = new PlayerMovement();
        CurrentPhase = TurnPhase.ProposeAction;
        int currentTurn = TurnNumber;
        if (currentTurn <= FreeTurns) { return;}
        currentTurn -= FreeTurns;
        if (currentTurn > 0 && currentTurn % TurnsBetweenTileDrop == 0)
            DropTile();
    }

    public void LoseLife()
    {
        Lives--;
        ResetLevel();
    }

    private void PlayCurrentRequestSound()
    {
        if (currentMovementRequest == null) {
            return;
        }
        //TODO: prevent playing while other sounds are playing
        switch (currentMovementRequest.RequestDirection) {
            case PlayerMovement.MovementDirection.Up:
                AudioSource.PlayClipAtPoint(RequestUp, origin);
                break;
            case PlayerMovement.MovementDirection.Down:
                AudioSource.PlayClipAtPoint(RequestDown, origin);
                break;
            case PlayerMovement.MovementDirection.Left:
                AudioSource.PlayClipAtPoint(RequestLeft, origin);
                break;
            case PlayerMovement.MovementDirection.Right:
                AudioSource.PlayClipAtPoint(RequestRight, origin);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void Awake()
    {
        if (instance == null) instance = this;
        else{ if(instance != this)Destroy(gameObject);}
        if (playerMovements == null) playerMovements = new List<PlayerMovement>();
        Setup();
    }

    public void Setup()
    {
        ResetLevelObject();
        ResetPlayerObject();
        AdvanceTurn(null);
    }

    private void ResetLevelObject()
    {
        currentLevelObject =
            Instantiate(FloorTilePrefab,
                //new Vector3(FloorTilePrefab.renderer.bounds.size.x/2f, 0f, FloorTilePrefab.renderer.bounds.size.z/2f),
                new Vector3(0f, 0f, 0f),
                Quaternion.identity) as GameObject;
        if (currentLevelObject == null) {
            return;
        }
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
        float xLoc = -1f*width/2f;
        float zLoc = -1f*height/2f;
        float depth = maxy - miny;
        float yLoc = 0f;
        if (depth > 0f) {
            yLoc = 0f - (depth/2f);
        }
        if (depth < 0f) {
            yLoc = depth/2f;
        }
        currentLevelObject.transform.Translate(new Vector3(xLoc, yLoc, zLoc));
    }

    private void ResetPlayerObject()
    {
        currentPlayerObject = Instantiate(PlayerPrefab, origin, Quaternion.identity) as GameObject;
        if (currentPlayerObject != null) {
            Renderer playerRenderer = currentPlayerObject.GetComponent<Renderer>();
            if (playerRenderer != null) {
                float playerDepth = playerRenderer.bounds.max.y - playerRenderer.bounds.min.y;
                currentPlayerObject.transform.Translate(new Vector3(0f, playerDepth + 2f, 0f));
            }
            Rigidbody playerRigidbody = currentPlayerObject.GetComponent<Rigidbody>();
            if (playerRigidbody != null) {
                playerRigidbody.isKinematic = true;
            }
        }
    }

    // Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    internal class PlayerMovement {
        public enum MovementDirection { Up,Down,Left,Right}
        public MovementDirection? RequestDirection { get; set; }
        public bool? Advice { get; set; }
        public MovementDirection? AdviceDirection { get; set; }
        public MovementDirection? FinalDirection { get; set; }
    }
}
