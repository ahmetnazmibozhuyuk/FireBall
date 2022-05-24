using System.Collections;
using UnityEngine;

//This class is responsible for setting the line renderer and HingeJoint position. I decided to relocate the Joint related methods to a seperate
//class to make the joint creating system more modular. This way both the JointCreator and the PlayerController is less crowded and chaotic and
//can be reused much easier.


public class JointCreator : MonoBehaviour
{
    //Rather than making a public field, HasJoint is a public property that can't be changed from outside the class.
    //This property is to prevent player from applying force to the ball when the joint connection is not created.
    public bool HasJoint { get; private set; }

    [SerializeField]
    private LineRenderer lRenderer;

    [SerializeField]
    private HingeJoint hJoint;

    [SerializeField]
    private float ropeSpeed = 8f;

    private float _lineLength;
    private float _timeElapsed;

    private bool _addingNewJoint;

    //Joint initialization method is seperated from regular Joint creation method because it is created instantly.
    public void InitializeJoint(Vector3 blockPosition)
    {
        hJoint.anchor = blockPosition - transform.position;
        lRenderer.enabled = true;
        lRenderer.SetPosition(1,blockPosition - transform.position);
        HasJoint = true;
    }
    public void FindRelativePosForHingeJoint(Vector3 blockPosition)
    {
        if (hJoint != null) return;

        lRenderer.enabled = true;
        _addingNewJoint = true;
        StartCoroutine(Co_CreateJoint(blockPosition));
    }
    //Joint is created only when the line is completed using this coroutine. BreakJoint can interrupt and reset this operation so the line is recreated properly.
    private IEnumerator Co_CreateJoint(Vector3 blockPosition)
    {
        while (_lineLength<1)
        {
            _lineLength = Mathf.Lerp(0, 1,_timeElapsed);
            _timeElapsed += Time.deltaTime*ropeSpeed;
            //Quaternion.Inverse operation is applied to 
            lRenderer.SetPosition(1, Quaternion.Inverse(transform.rotation) * (blockPosition - transform.position) * _lineLength);
            if (!_addingNewJoint)
            {
                _lineLength = 0;
                _timeElapsed = 0;
                lRenderer.enabled = false;
                yield break;
            }
            yield return null;
        }
        hJoint = gameObject.AddComponent<HingeJoint>();
        hJoint.anchor = Quaternion.Inverse(transform.rotation) * (blockPosition - transform.position);
        HasJoint = true;
        _lineLength = 0;
        _timeElapsed = 0;
        _addingNewJoint = false;
        yield break;
    }
    public void BreakJoint()
    {
        _addingNewJoint = false; // This variable allows us to stop the coroutine even though if the _hJoint is null.

        if (hJoint == null) return;

        lRenderer.enabled = false;
        HasJoint = false;
        Destroy(hJoint);
        hJoint = null;
    }
}
