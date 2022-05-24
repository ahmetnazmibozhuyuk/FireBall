using UnityEngine;

//private variables: _privateVariable - Serialized private and local variables: privateVariable - public variables: PublicVariable.

//Joint creation and GUI related methods relocated to their respective classes. Considering the already small scale
// of the project, I decided to keep the Trigger and Collision methods in this class but placed them into a region.
[RequireComponent(typeof(JointCreator))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private GameObject[] blockPrefabs;
    [SerializeField]
    private Rigidbody playerRigidbody;
    [SerializeField]
    private PlayerFollower playerFollower;
    [SerializeField]
    private GameObject pointPrefab;
    [SerializeField]
    private GUIController guiController;

    [SerializeField]
    private float swingSpeed = 5f;

    private JointCreator _jointCreator;

    private readonly int _perfectScorePoint = 10;
    private readonly int _lowScorePoint = 5;

    private readonly float _perfectScoreDistance = 0.5f;

    private bool _gameOver = false;
    private bool _gameStarted = false;

    private bool _inputIsDown;
    private void Awake()
    {
        //Since JointCreator class is a required component; I chose to use GetComponent rather than serializing it in order to make inspector less crowded.
        //I choose to use Awake method for operations like GetComponent so that I have no problems using said components on the Start method.
        _jointCreator = GetComponent<JointCreator>();
    }

    private void Start()
    {
        // BlockCreator and playerFollower are initialized from their respective methods.
        BlockCreator.GetSingleton().Initialize(30, blockPrefabs, pointPrefab);
        _jointCreator.InitializeJoint(BlockCreator.GetSingleton().GetBlockPositionByIndex(0));
        playerFollower.SetPosition(transform);
    }
    private void Update()
    {
        //Score doesn't set properly since it always tend to update the score. Make a proper way to update the score as player advances

        //Update method is tied to the frame rate so it is the superior method to display visual things like the score. Scaled with Time.deltaTime rather
        //than Time.fixedDeltaTime because Time.deltaTime is the time between this Update tick and the previous one as opposed to Time.fixedDeltaTime
        //which is the time between last FixedUpdate and this one.
        //We could use a similar method to PlayerFollower's BlockUpdate to make the score update less frequently, but the timers precision is very high.
        if (playerRigidbody.velocity.z != 0)
        {
            guiController.SetScore(playerRigidbody.velocity.z * Time.deltaTime * 0.1f);
        }
    }
    private void FixedUpdate()
    {
        //Rigidbody operations like AddForce should be applied from FixedUpdate since it is directly tied to physics related operations.
        ForwardMovement();
    }

    private void ForwardMovement()
    {
        if (_inputIsDown && _jointCreator.HasJoint)
        {
            playerRigidbody.AddForce(Vector3.one * swingSpeed);
        }
    }
    //PointerDown() and PointerUp() methods won't be used outside this class so they are set as private.
    private void PointerDown()
    {
        _inputIsDown = true;
        _jointCreator.FindRelativePosForHingeJoint(BlockCreator.GetSingleton().GetRelativeBlock(transform.position.z));


        if (!_gameStarted)
        {
            guiController.StartGame();
            _gameStarted = true;
        }
    }
    private void PointerUp()
    {
        _inputIsDown = false;
        _jointCreator.BreakJoint();
    }
    #region Trigger And Collision Methods
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Block") && !_gameOver) // tag equals changed with compare tag
        {
            PointerUp();
            gameObject.SetActive(false);
            _gameOver = true;
            //GUI related methods, fields and properties have relocated to GUIController.
            guiController.GameOver();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        //Magic numbers removed.
        if (other.gameObject.CompareTag("Point")) // tag equals changed with compare tag
        {
            if (Vector3.Distance(transform.position, other.gameObject.transform.position) < _perfectScoreDistance)
            {
                guiController.SetScore(_perfectScorePoint);
            }
            else
            {
                guiController.SetScore(_lowScorePoint);
            }
            other.gameObject.SetActive(false);
        }
    }
    #endregion

}
