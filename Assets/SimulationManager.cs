using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    public  GameObject AntPrefab;

    public  GameObject CityPrefab;

    static public SimulationManager instance
    {
        get;
        private set;
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            throw new System.Exception("Tried to create multiple Simulation Managers");
    }

    private void Update()
    {
         if (!Input.GetKeyDown(KeyCode.Space))
            return;
         GameObject ant = Instantiate(AntPrefab, new Vector3(0, 0, 0), Quaternion.identity);
         Ant antComp = ant.GetComponent<Ant>();
         antComp.WalkPath(new Queue<City>(Citylist), 0.4);
    }


    private List<City> Citylist = new List<City>();

   public void addCity(City city)
    { this.Citylist.Add(city);}

    public void removeCity(City city)
    {
        this.Citylist.Remove(city);
    }


    public double GetDistance(City city1, City city2)
    {
        return Vector3.Distance(city1.transform.position, city2.transform.position);
    }


    

 
}
