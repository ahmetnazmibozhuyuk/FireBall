using System.Collections;
using UnityEngine;

public class JointCreator : MonoBehaviour
{
    public bool HasJoint { get; private set; }

    [SerializeField]
    private LineRenderer lRenderer;
    private HingeJoint hJoint;


    private float _lineLength;

    private float _timeElapsed;

    private bool _addingNewJoint;
    private void Awake()
    {

    }
    private void Start()
    {
        //FindRelativePosForHingeJoint(new Vector3(0, 10, 0));
        InitializeJoint(new Vector3(0, 10, 0));
    }
    private void InitializeJoint(Vector3 blockPosition)
    {
        hJoint = GetComponent<HingeJoint>();
        hJoint.anchor = blockPosition - transform.position;
        lRenderer.enabled = true;
        lRenderer.SetPosition(1,blockPosition - transform.position);
        HasJoint = true;
    }
    public void FindRelativePosForHingeJoint(Vector3 blockPosition)
    {
        //Update the block position on this line in a proper way to Find Relative position for our blockPosition
        if (hJoint != null) return;


        //lRenderer.SetPosition(1, hJoint.anchor);
        lRenderer.enabled = true;
        _addingNewJoint = true;
        StartCoroutine(Co_CreateJoint(Quaternion.Inverse(transform.rotation) * (blockPosition - transform.position)));



    }
    private IEnumerator Co_CreateJoint(Vector3 pos)
    {

        while (_lineLength<1)
        {
            _lineLength = Mathf.Lerp(0, 1,_timeElapsed);
            Debug.Log(_lineLength);
            _timeElapsed += Time.deltaTime*3f;
            lRenderer.SetPosition(1, pos * _lineLength);
            if (!_addingNewJoint) yield break;
            yield return null;
        }
        hJoint = gameObject.AddComponent<HingeJoint>();
        hJoint.anchor = pos;
        HasJoint = true;
        _lineLength = 0;
        _timeElapsed = 0;
        _addingNewJoint = false;
        yield break;
    }
    public void BreakJoint()
    {
        _addingNewJoint = false;

        if (hJoint == null) return;

        lRenderer.enabled = false;
        HasJoint = false;
        Destroy(hJoint);
        hJoint = null;
    }
}
