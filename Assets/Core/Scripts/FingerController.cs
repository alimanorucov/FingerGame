using UnityEngine;
using System.Collections.Generic;

public class FingerController : MonoBehaviour
{
    [System.Serializable]
    public class Finger
    {
        public string name;
        public List<Transform> joints = new List<Transform>();
        public KeyCode toggleKey;
        public float closeAngle = 30f;

        public bool isClosed = false;
        private List<Quaternion> openRotations = new List<Quaternion>();

        public void Initialize()
        {
            openRotations.Clear();
            foreach (var joint in joints)
            {
                if (joint != null)
                    openRotations.Add(joint.localRotation);
            }
        }

        public void Toggle()
        {
            isClosed = !isClosed;
        }

        public void UpdateFinger(float speed)
        {
            if (joints.Count == 0) return;

            float t = Mathf.Clamp01(speed * Time.deltaTime);

            for (int i = 0; i < joints.Count; i++)
            {
                if (joints[i] == null) continue;

                Quaternion targetRotation;

                if (isClosed)
                {
                    float multiplier = 1f + (i * 0.2f);
                    targetRotation = openRotations[i] * Quaternion.Euler(closeAngle * multiplier, 0, 0);
                }
                else
                {
                    targetRotation = openRotations[i];
                }

                joints[i].localRotation = Quaternion.Slerp(joints[i].localRotation, targetRotation, t);
            }
        }
    }

    [Header("Finger Settings")]
    public Finger leftThumb = new Finger() { name = "Left Thumb", toggleKey = KeyCode.Q };
    public Finger leftIndex = new Finger() { name = "Left Index", toggleKey = KeyCode.W };
    public Finger leftMiddle = new Finger() { name = "Left Middle", toggleKey = KeyCode.E };
    public Finger leftRing = new Finger() { name = "Left Ring", toggleKey = KeyCode.R };
    public Finger leftPinky = new Finger() { name = "Left Pinky", toggleKey = KeyCode.T };

    public Finger rightThumb = new Finger() { name = "Right Thumb", toggleKey = KeyCode.Z };
    public Finger rightIndex = new Finger() { name = "Right Index", toggleKey = KeyCode.X };
    public Finger rightMiddle = new Finger() { name = "Right Middle", toggleKey = KeyCode.C };
    public Finger rightRing = new Finger() { name = "Right Ring", toggleKey = KeyCode.V };
    public Finger rightPinky = new Finger() { name = "Right Pinky", toggleKey = KeyCode.B };

    [Header("Animation Settings")]
    [Range(1f, 50f)]
    public float animationSpeed = 12f;

    void Start()
    {
        InitializeAllFingers();
    }

    void Update()
    {
        HandleInput();
    }

    void LateUpdate()
    {
        UpdateAllFingers();
    }

    void InitializeAllFingers()
    {
        leftThumb.Initialize();
        leftIndex.Initialize();
        leftMiddle.Initialize();
        leftRing.Initialize();
        leftPinky.Initialize();

        rightThumb.Initialize();
        rightIndex.Initialize();
        rightMiddle.Initialize();
        rightRing.Initialize();
        rightPinky.Initialize();
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(leftThumb.toggleKey)) leftThumb.Toggle();
        if (Input.GetKeyDown(leftIndex.toggleKey)) leftIndex.Toggle();
        if (Input.GetKeyDown(leftMiddle.toggleKey)) leftMiddle.Toggle();
        if (Input.GetKeyDown(leftRing.toggleKey)) leftRing.Toggle();
        if (Input.GetKeyDown(leftPinky.toggleKey)) leftPinky.Toggle();

        if (Input.GetKeyDown(rightThumb.toggleKey)) rightThumb.Toggle();
        if (Input.GetKeyDown(rightIndex.toggleKey)) rightIndex.Toggle();
        if (Input.GetKeyDown(rightMiddle.toggleKey)) rightMiddle.Toggle();
        if (Input.GetKeyDown(rightRing.toggleKey)) rightRing.Toggle();
        if (Input.GetKeyDown(rightPinky.toggleKey)) rightPinky.Toggle();
    }

    void UpdateAllFingers()
    {
        leftThumb.UpdateFinger(animationSpeed);
        leftIndex.UpdateFinger(animationSpeed);
        leftMiddle.UpdateFinger(animationSpeed);
        leftRing.UpdateFinger(animationSpeed);
        leftPinky.UpdateFinger(animationSpeed);

        rightThumb.UpdateFinger(animationSpeed);
        rightIndex.UpdateFinger(animationSpeed);
        rightMiddle.UpdateFinger(animationSpeed);
        rightRing.UpdateFinger(animationSpeed);
        rightPinky.UpdateFinger(animationSpeed);
    }

    public bool[] GetHandState()
    {
        bool[] state = new bool[10];

        // Sol el
        state[0] = leftThumb.isClosed;
        state[1] = leftIndex.isClosed;
        state[2] = leftMiddle.isClosed;
        state[3] = leftRing.isClosed;
        state[4] = leftPinky.isClosed;

        // Sað el
        state[5] = rightThumb.isClosed;
        state[6] = rightIndex.isClosed;
        state[7] = rightMiddle.isClosed;
        state[8] = rightRing.isClosed;
        state[9] = rightPinky.isClosed;

        return state;
    }
}