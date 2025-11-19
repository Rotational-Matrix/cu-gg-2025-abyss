using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerAnimationManager : MonoBehaviour
{
    public GameObject playerObject;
    private SpriteRenderer sr;
    //private Rigidbody rb;
    public Sprite nSprite; //back-facing
    public Sprite sSprite; //front-facing
    private LeashManager lm;
    public float flipEpsilon = 0.025f; //minimum inputvelocity to signal a sprite flip
    private bool frontFacing;
    public GameObject cave;
    public GameObject camera;
    private CameraFollow cameraFollow;
    public GameObject anchor;
    public GameObject light;
    private bool inCave;
    public Vector3 defaultPosition = new Vector3(2.25f, 0.75f, 0f);
    public Vector3 defaultAnchorPosition = new Vector3(1.6f, 0, 4.04f);
    public static event Action<PlayerAnimationManager> raiseCaveDialogue;
    public static event Action<PlayerAnimationManager> pause;
    // Start is called before the first frame update
    void Start()
    {
        inCave = false;
        //rb = playerObject.GetComponent<Rigidbody>();
        GameObject spriteContainer = playerObject.transform.GetChild(0).gameObject;
        sr = spriteContainer.GetComponent<SpriteRenderer>();
        lm = playerObject.GetComponent<LeashManager>();
        Cylinder.onCylinderEntrance += Teleport;
        cameraFollow = camera.GetComponent<CameraFollow>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //if (StateManager.GetDialogueStatus()) return;
        Vector3 input = lm.InputVelocity();
        float x = input.x;
        float z = input.z;
        if (z * z > flipEpsilon)
        {
            if (z > 0)
            {
                sr.sprite = nSprite;
                frontFacing = true;
            }
            else
            {
                sr.sprite = sSprite;
                frontFacing = false;
            }
        }
        //this uses sprite flips but can be made to use different sprites with no effort
        if (x * x > flipEpsilon)
        {
            if (x > 0) sr.flipX = !frontFacing;
            else sr.flipX = frontFacing;
        }
    }
    public void Teleport(Cylinder cylinder)
    {
        Vector3 newPos = inCave ? defaultPosition : cave.transform.position;
        Debug.Log("Moving to " + newPos.x + ", " + newPos.y + ", " + newPos.z);
        Vector3 newAnchorPos = newPos + (defaultAnchorPosition - defaultPosition);
        newAnchorPos.y = 0f;
        anchor.transform.position = newAnchorPos;
        Vector3 newPlayerPos = newPos;
        newPlayerPos.y = 0.625f; //hardcoded appropriate y-value for ground contact
        playerObject.transform.position = newPlayerPos;
        cameraFollow.FastMove(playerObject);
        light.transform.position = newPlayerPos;
        inCave = !inCave;
        if (inCave)
        {
            Debug.Log("Cave dialogue should appear");
            if (raiseCaveDialogue != null) raiseCaveDialogue(this);
        }
    }
}
