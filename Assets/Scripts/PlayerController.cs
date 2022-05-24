using UnityEngine;


//In this section, you have to edit OnPointerDown and OnPointerUp sections to make the game behave in a proper way using hJoint
//Hint: You may want to Destroy and recreate the hinge Joint on the object. For a beautiful gameplay experience, joint would created after a little while (0.2 seconds f.e.) to create mechanical challege for the player
//And also create fixed update to make score calculated real time properly.
//Update FindRelativePosForHingeJoint to calculate the position for you rope to connect dynamically
//You may add up new functions into this class to make it look more understandable and cosmetically great.
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

    private bool gameOver = false;
    private bool gameStarted = false;

    private bool _inputIsDown;
    private void Awake()
    {
        //Since this class is a required component; I chose to use GetComponent rather than serializing it in order to make inspector less crowded.
        _jointCreator = GetComponent<JointCreator>();
    }
    private void Start()
    {
        BlockCreator.GetSingleton().Initialize(30, blockPrefabs, pointPrefab);
        playerFollower.SetPosition(transform);
    }

    private void ForwardMovement()
    {
        if (_inputIsDown && _jointCreator.HasJoint)
        {
            playerRigidbody.AddForce(Vector3.one * swingSpeed);
        }
    }
    public void PointerDown()
    {
        _inputIsDown = true;
        _jointCreator.FindRelativePosForHingeJoint(BlockCreator.GetSingleton().GetRelativeBlock(transform.position.z));


        if (!gameStarted)
        {
            guiController.StartGame();
        }
    }

    public void PointerUp()
    {
        _inputIsDown = false;
        _jointCreator.BreakJoint();
    }
    #region Trigger And Collision Methods
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals("Block") && !gameOver)
        {
            PointerUp(); //Finishes the game here to stoping holding behaviour

            Destroy(gameObject);
            gameOver = true;

            //If you know a more modular way to update UI, change the code below

            guiController.GameOver();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Point")) // tag equals changed with compare tag
        {
            if (Vector3.Distance(transform.position, other.gameObject.transform.position) < .5f)
            {
                guiController.SetScore(10);
            }
            else
            {
                guiController.SetScore(5);
            }
            other.gameObject.SetActive(false);
        }
    }
    #endregion
    private void FixedUpdate()
    {
        ForwardMovement();
        //Score doesn't set properly since it always tend to update the score. Make a proper way to update the score as player advances
        if (playerRigidbody.velocity.z > 0)
        {
            guiController.SetScore(playerRigidbody.velocity.z * Time.fixedDeltaTime * 0.1f);
        }
    }

}
