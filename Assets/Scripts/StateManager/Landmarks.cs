using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;


/// <summary>
/// This exists just so that RoamCmdr doesn't have to keep track of all of the landmark locations
/// 
/// 
/// 
/// </summary>
public class Landmarks : MonoBehaviour
{
    [SerializeField] private GameObject _lamb;
    public Vector3 Lamb { get { return _lamb.transform.position; } }
    [SerializeField] private GameObject _cave; // set to trigger cyl (1) 
    public Vector3 Cave { get { return _cave.transform.position + new Vector3(1.5f, 0f, 3f); } } 
    [SerializeField] private GameObject _mush1;
    public Vector3 Mush1 { get { return _mush1.transform.position; } }
    [SerializeField] private GameObject _mush2;
    public Vector3 Mush2 { get { return _mush2.transform.position; } }
    [SerializeField] private GameObject _mush3;
    public Vector3 Mush3 { get { return _mush3.transform.position; } }
    [SerializeField] private GameObject _cobweb;
    public Vector3 Cobweb { get { return _cobweb.transform.position; } }
    [SerializeField] private GameObject _demoFlower;
    public Vector3 DemoFlower { get { return _demoFlower.transform.position; } }
    [SerializeField] private GameObject _hiddenFlower;
    public Vector3 HiddenFlower { get { return _hiddenFlower.transform.position; } }
    [SerializeField] private GameObject _flowerPot; //(RC should actually know abt flowerPot)
    public Vector3 FlowerPot { get { return _flowerPot.transform.position; } }
    //[SerializeField] private GameObject _whatevElse;



    // Start is called before the first frame update
    
}
