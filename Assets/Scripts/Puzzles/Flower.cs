using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flower : MonoBehaviour
{
    public GameObject managerParent;
    private FlowerManager manager;
    // Start is called before the first frame update
    void Start()
    {
        manager = managerParent.GetComponent<FlowerManager>();
    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("hi");
        manager.AddFlower();
        this.gameObject.SetActive(false);
    }
}
