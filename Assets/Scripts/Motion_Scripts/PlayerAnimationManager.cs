using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    // Start is called before the first frame update
    void Start()
    {
        //rb = playerObject.GetComponent<Rigidbody>();
        GameObject spriteContainer = playerObject.transform.GetChild(0).gameObject;
        sr = spriteContainer.GetComponent<SpriteRenderer>();
        lm = playerObject.GetComponent<LeashManager>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 input = lm.InputVelocity();
        float x = input.x;
        float z = input.z;
        if (z * z > flipEpsilon)
        {
            if (z > 0) {
                sr.sprite = nSprite;
                frontFacing = true;
            }
            else {
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
}
