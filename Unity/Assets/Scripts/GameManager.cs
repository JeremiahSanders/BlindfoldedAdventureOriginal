using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    public static GameManager instance;
    public Text LivesText;
    public Text TileDisappearsText;
    public GameObject LevelPrefab;
    public GameObject PlayerPrefab;
    public GameObject FloorTilePrefab;
    public GameObject GameOverHUD;
    public GameObject YouWinHUD;
    public AudioClip RequestUp;
    public AudioClip RequestDown;
    public AudioClip RequestLeft;
    public AudioClip RequestRight;
    public int FreeTurns = 5;
    public int TurnsBetweenTileDrop = 3;
    public int Lives = 3;
    public int SecondsPerTileDrop = 3;
    private float tileCountdown = 0f;

    public AudioClip AdvisePositive;
    public AudioClip AdviseNegative;

    private Vector3 origin = new Vector3(0f,0f,0f);
    private GameObject currentPlayerObject;
    private GameObject currentLevelObject;
    private List<PlayerMovement> playerMovements;
    public int TurnNumber { get { return playerMovements == null ? 0 : playerMovements.Count + 1; } }
    private PlayerMovement currentMovementRequest;

    public bool PlayerCanSubmitMove
    {
        get
        {
            return currentMovementRequest != null
                   &&
                   (!currentMovementRequest.RequestDirection.HasValue
                    || (currentMovementRequest.Advice.HasValue && !currentMovementRequest.FinalDirection.HasValue)
                       )
                ;
        }
    }

    public TurnPhase CurrentPhase { get; private set; }

    public enum TurnPhase {
        ProposeAction = 1,
        Advise = 2,
        PerformAction = 3
    }

    //public void PlayerRight()
    //{
    //    ChangeDirection(MovementDirection.Right);
    //}
    //public void PlayerLeft()
    //{
    //    ChangeDirection(MovementDirection.Left);
    //}
    //public void PlayerDown()
    //{
    //    ChangeDirection(MovementDirection.Down);
    //}
    //public void PlayerUp()
    //{
    //    ChangeDirection(MovementDirection.Up);
    //}


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
        if (currentTileCollection == null) { return;}
        if (currentTileCollection.Count == 0) { return; }
        int tileToDrop = UnityEngine.Random.Range(0, currentTileCollection.Count);
        GameObject tileToDestroy = currentTileCollection.ElementAt(tileToDrop).Value.gameObject;
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
     
    //private void ChangeDirection(MovementDirection direction)
    //{
    //    LogCurrentRequest(currentMovementRequest);
    //    switch (CurrentPhase) {
    //        case TurnPhase.ProposeAction:
    //            if (currentMovementRequest != null && currentMovementRequest.RequestDirection == direction) {
    //                return;
    //            }
    //            if (currentMovementRequest == null) {
    //                currentMovementRequest = new PlayerMovement {RequestDirection = direction};
    //            }
    //            else {
    //                currentMovementRequest.RequestDirection = direction;
    //            }
    //            PlayCurrentRequestSound();
    //            CurrentPhase = TurnPhase.Advise;
    //            break;
    //        case TurnPhase.Advise:
    //            break;
    //        case TurnPhase.PerformAction:
    //            if (currentMovementRequest != null) {
    //                currentMovementRequest.FinalDirection = direction;
    //            }
    //            break;
    //        default:
    //            throw new ArgumentOutOfRangeException();
    //    }
    //}

    private void LogCurrentRequest(PlayerMovement playerMovement)
    {
        if (playerMovement == null) {
            Debug.Log("playerMovement is null");
            return;
        }
        string toPrint = "Initial Direction: ";
        if (playerMovement.RequestDirection.HasValue) {
            toPrint += playerMovement.RequestDirection.ToString();
        }
        if (playerMovement.Advice.HasValue) {
            toPrint += " advice: " + playerMovement.Advice.ToString();
        }
        if (playerMovement.FinalDirection.HasValue) {
            toPrint += " final: " + playerMovement.FinalDirection.ToString();
        }
        Debug.Log(toPrint);
    }

    //public void PlayerMove()
    //{
    //    if (currentMovementRequest == null || currentMovementRequest.FinalDirection == null) {
    //        return;
    //    }
    //    switch (currentMovementRequest.FinalDirection.Value) {
    //        case MovementDirection.Up:
    //            currentPlayerObject.transform.Translate(0f, 0f, 1f);
    //            break;
    //        case MovementDirection.Down:
    //            currentPlayerObject.transform.Translate(0f, 0f, -1f);
    //            break;
    //        case MovementDirection.Left:
    //            currentPlayerObject.transform.Translate(-1f, 0f, 0f);
    //            break;
    //        case MovementDirection.Right:
    //            currentPlayerObject.transform.Translate(1f, 0f, 0f);
    //            break;
    //        default:
    //            throw new ArgumentOutOfRangeException("currentMovementRequest.FinalDirection");
    //    }
    //    Rigidbody playerRigidbody = currentPlayerObject.GetComponent<Rigidbody>();
    //    RaycastHit hit;
    //    playerRigidbody.SweepTest(new Vector3(0f, -50f, 0f), out hit, 50f);
    //    Debug.Log("Distance to obj: " + hit.distance.ToString());
    //    if (hit.distance < 0.1f) {
    //        AdvanceTurn(currentMovementRequest);
    //    }
    //    else {
    //        // THERE'S NOTHING THERE!
    //        FallOff();
    //    }
    //    UpdateTileDisappearsText();
    //}

    private void UpdateTileDisappearsText()
    {
        if (TileDisappearsText != null) {
            int disappearsIn = NextTileDisappears;
            TileDisappearsText.text = disappearsIn == 0
                ? "Tile disappears this turn"
                : String.Format("Next tile disappears in {0:F1} seconds", tileCountdown);
        }
    }
    private int NextTileDisappears
    {
        get
        {
            if (TurnNumber == 0) return 0;
            if (TurnNumber < FreeTurns) return FreeTurns - TurnNumber;
            return ((TurnNumber - FreeTurns) % TurnsBetweenTileDrop);
        }
    }
    public void StopPlayerMovement()
    {
        //Rigidbody playerRigidbody = currentPlayerObject.GetComponent<Rigidbody>();
        //playerRigidbody.isKinematic = true;
    }
    private void ResetLevel()
    {
        if (currentPlayerObject != null) { Destroy(currentPlayerObject);}
        ResetPlayerObject();
        playerMovements.Clear();
        if (currentLevelObject != null) {
            Destroy(currentLevelObject);
        }
        if (currentTileCollection != null)
        {
            foreach (var o in currentTileCollection)
            {
                Destroy(o.Value);
            }
            currentTileCollection.Clear();
        }
        ResetLevelObject();
        tileCountdown = SecondsPerTileDrop;
        if (currentMovementRequest == null) {currentMovementRequest = new PlayerMovement();}
    }

    //private void AdvanceTurn(PlayerMovement currentMovement)
    //{
    //    if (currentMovement != null) {
    //        playerMovements.Add(currentMovement);
    //    }
    //    currentMovementRequest = new PlayerMovement();
    //    CurrentPhase = TurnPhase.ProposeAction;
    //    if (TurnNumber > 0 && NextTileDisappears == 0)
    //        DropTile();
    //}

    //private void FallOff()
    //{
    //    Debug.Log("Falloff was triggered");
    //    Rigidbody playerRigidbody = currentPlayerObject.GetComponent<Rigidbody>();
    //    playerRigidbody.isKinematic = false;
    //    playerMovements.Add(currentMovementRequest);
    //    currentMovementRequest = null;
    //}

    public void LoseLife()
    {
        Lives--;
        if (Lives > 0) {
            UpdateLivesText();
            ResetLevel();
        }
        else {
            CountdownActive = false;
            if (GameOverHUD != null)
                GameOverHUD.SetActive(true);
            LivesText.gameObject.SetActive(false);
        }
    }

    private void PlayCurrentRequestSound()
    {
        if (currentMovementRequest == null) {
            return;
        }
        //TODO: prevent playing while other sounds are playing
        switch (currentMovementRequest.RequestDirection) {
            case MovementDirection.Up:
                AudioSource.PlayClipAtPoint(RequestUp, origin);
                break;
            case MovementDirection.Down:
                AudioSource.PlayClipAtPoint(RequestDown, origin);
                break;
            case MovementDirection.Left:
                AudioSource.PlayClipAtPoint(RequestLeft, origin);
                break;
            case MovementDirection.Right:
                AudioSource.PlayClipAtPoint(RequestRight, origin);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void CancelDirection()
    {
        if (currentMovementRequest == null)return;
        if (currentMovementRequest.RequestDirection.HasValue && CurrentPhase == TurnPhase.ProposeAction)
            currentMovementRequest.RequestDirection = null;
        if (CurrentPhase == TurnPhase.PerformAction && currentMovementRequest.FinalDirection.HasValue)
            currentMovementRequest.FinalDirection = null;
    }
    public void Awake()
    {
        if (instance == null) instance = this;
        else{ if(instance != this)Destroy(gameObject);}
        Setup();
    }

    public void Setup()
    {
        if (playerMovements == null) playerMovements = new List<PlayerMovement>();
        ResetLevelObject();
        ResetPlayerObject();
        CountdownActive = true;
        UpdateLivesText();
        UpdateTileDisappearsText();

    }

    private void UpdateLivesText()
    {
       if (LivesText != null) {
            LivesText.text = String.Format("Lives: {0}", Lives);
        } 
    }

    private void ResetLevelObject()
    {
        currentTileCollection = GenerateLevel(5, 5);
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
        }
    }

    // Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
	{
        if (!CountdownActive) return;
	    tileCountdown -= Time.deltaTime;
        UpdateTileDisappearsText();
	    if (tileCountdown <= 0f) {
	        DropTile();
	        tileCountdown = (float) SecondsPerTileDrop;
	    }
	}

    private bool _countdownActive = true;

    private bool CountdownActive
    {
        get
        {
            return _countdownActive;
        }
        set
        {
            _countdownActive = value;
            if (_countdownActive)
                tileCountdown = SecondsPerTileDrop;
            TileDisappearsText.gameObject.SetActive(_countdownActive);
        }
    }

    private readonly Vector3 tileScale = new Vector3(1f, 1f, 1f);
    private Dictionary<XYPoint, GameObject> currentTileCollection;

    private Dictionary<XYPoint, GameObject> GenerateLevel(int width, int height)
    {
        float middleX = (float) width/2f;
        float middleY = (float) height/2f;
        Dictionary<XYPoint, GameObject> level = new Dictionary<XYPoint, GameObject>();
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                GameObject tile = Instantiate(FloorTilePrefab, origin, Quaternion.identity) as GameObject;
                float destinationX = (((float)x + 1f) < middleX) ? 0 - x : x - middleX;
                float destinationY = (((float)y + 1f) < middleY) ? 0 - y : y - middleY;
                Vector3 destination = new Vector3(destinationX, 0f, destinationY);
                tile.transform.Translate(destination, Space.World);
                tile.transform.localScale = tileScale;
                level[new XYPoint(x, y)] = tile;
            }
        }
        return level;
    }

    internal class XYPoint {
        public XYPoint(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; private set; }
        public int Y { get; private set; }
    }

    internal class PlayerMovement {
        public MovementDirection? RequestDirection { get; set; }
        public bool? Advice { get; set; }
        public MovementDirection? AdviceDirection { get; set; }
        public MovementDirection? FinalDirection { get; set; }
    }
        public enum MovementDirection { Up,Down,Left,Right}
}
