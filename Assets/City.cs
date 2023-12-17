using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class City : MonoBehaviour
{

    public Vector3 currentPosition;


    // Start is called before the first frame update
    static private int nextID;
    public int ID
    {
        private set; get;
    }

    private void Awake()
    {
        ID = nextID++;
    }
    void Start()
    {
        currentPosition = transform.position;
        SimulationManager.instance.addCity(this);
    }

    private void OnDestroy()
    {
        SimulationManager.instance.removeCity(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Transform GetTransform()
    {
        return transform;
    }
}
