using System.Collections;
using UnityEngine;

public class JointCreator : MonoBehaviour
{
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

    private void Start()
    {
        InitializeJoint(new Vector3(0, 5, 0));
    }
    private void InitializeJoint(Vector3 blockPosition)
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
    private IEnumerator Co_CreateJoint(Vector3 blockPosition)
    {
        while (_lineLength<1)
        {
            _lineLength = Mathf.Lerp(0, 1,_timeElapsed);
            _timeElapsed += Time.deltaTime*ropeSpeed;
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
        _addingNewJoint = false;

        if (hJoint == null) return;

        lRenderer.enabled = false;
        HasJoint = false;
        Destroy(hJoint);
        hJoint = null;
    }
}
