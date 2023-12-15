using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    public  GameObject AntPrefab;

    public  GameObject CityPrefab;

    public TextMeshProUGUI Besttext;

    public double MinPheremone = 0.1;

    public double MaxPheremone = 1;

    private LineRenderer _lr;
    private LineRenderer lineRenderer
    { 
        get
        {
            if (_lr == null)
                _lr = GetComponent<LineRenderer>();
            return _lr;
        }
        
    }
    static public SimulationManager instance
    {
        get;
        private set;
    }

    public double AntSpeed;
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
         StartCoroutine(RunSimulation());
    }


    private List<City> LiveCityList = new List<City>();

   public void addCity(City city)
    { this.LiveCityList.Add(city);}

    public void removeCity(City city)
    {
        this.LiveCityList.Remove(city);
    }


    public double GetDistance(City city1, City city2)
    {
        return Vector3.Distance(city1.transform.position, city2.transform.position);
    }

    Dictionary<(int, int), double> Phermones = new();


    private double GetPhermone(City city1, City city2)
    {
       (int, int) key = (System.Math.Min(city1.ID, city2.ID), System.Math.Max(city1.ID, city2.ID));
        if (Phermones.ContainsKey(key))
            return Phermones[key];

        Phermones[key] = MaxPheremone;
        return MaxPheremone;
    }

    private void SetPhermone(City city1, City city2, double value)
    {
        value = System.Math.Min(value, MaxPheremone);
        value = System.Math.Max(value, MinPheremone);
        if (city1.ID > city2.ID)
            Phermones[(city2.ID, city1.ID)] = value;
        else
            Phermones[(city1.ID, city2.ID)] = value;
    }

    private void DecayPhermones()
    {
        Dictionary<(int, int), double> newPhermones = new Dictionary<(int, int), double>();
        foreach(KeyValuePair<(int, int), double> entry in Phermones)
        {
            newPhermones[entry.Key] = System.Math.Max( entry.Value * PhermoneDecayRate, MinPheremone);
        }
        Phermones = newPhermones;
    }

    public double TimeBetweenIterations;
    public int NumberofAnts;
    public double PhermoneDecayRate;

    public double DistanceWeight;
    public double PhermoneWeight;
    public double PhermoneDepositRate;
    public double PheromeDepositExponent;


    public enum DepositMode
    {
        Standard,
        BestIniterationOnly,
        GlobalBestOnly
    }

    public DepositMode depositMode;

    private IEnumerator RunSimulation()
    {
        Debug.Log("Starting Simulation");
        while(true)
        {
            StartCoroutine(RunSingleIteration(NumberofAnts,  LiveCityList.ToArray()));
            yield return new WaitForSeconds((float)TimeBetweenIterations);
        }
    }

        List<City> BestSoFar;
        double bestDistanceSoFar = double.MaxValue;
    private IEnumerator RunSingleIteration(int numberofants, City[] cities)
    {
        Ant[] ants = new Ant[numberofants];
        yield return StartCoroutine(InitialiseAnts(ants, 4));
        List<City>[] Routes = new List<City>[numberofants];
        double[] Distances = new double[numberofants];
        yield return StartCoroutine(CalculateRoutes(Distances, Routes, 16, numberofants, cities));
        for(int i = 0; i < ants.Length; i++)
        {
            ants[i].WalkPath(new Queue<City>(Routes[i]), AntSpeed);
        }

        int bestindex = 0;
        double bestdistance = double.MaxValue;
        for(int i =0; i < ants.Length; i++)
        {
            if (Distances[i] < bestdistance)
            {
                bestdistance = Distances[i];
                bestindex = i;
            }
        }
        //If a new city has been added since previous record the previous record is no longer valid
        if (bestdistance < bestDistanceSoFar || cities.Length > BestSoFar.Count)
        {
            bestDistanceSoFar = bestdistance;
            BestSoFar = Routes[bestindex];
            lineRenderer.positionCount = BestSoFar.Count;
            for (int i = 0; i < BestSoFar.Count; i++)
            {
                lineRenderer.SetPosition(i, BestSoFar[i].transform.position);
            }
         
           Besttext.text = "Best Distance: " + (int)bestDistanceSoFar;
        }

        if (depositMode == DepositMode.Standard)
        {
            for (int i = 0; i < ants.Length; i++)
            {


                double PheremoneDeposit = PhermoneDepositRate * System.Math.Pow((bestdistance / Distances[i]), PheromeDepositExponent);

                for (int j = 0; j < Routes[i].Count - 1; j++)
                {
                    SetPhermone(Routes[i][j], Routes[i][j + 1], GetPhermone(Routes[i][j], Routes[i][j + 1]) + PheremoneDeposit);
                }
                SetPhermone(Routes[i][Routes[i].Count - 1], Routes[i][0], GetPhermone(Routes[i][Routes[i].Count - 1], Routes[i][0]) + PheremoneDeposit);

            }
        }
        else if(depositMode == DepositMode.BestIniterationOnly)
        {
            double PheremoneDeposit = PhermoneDepositRate;
            int i = bestindex;
            for (int j = 0; j < Routes[i].Count - 1; j++)
            {
                SetPhermone(Routes[i][j], Routes[i][j + 1], GetPhermone(Routes[i][j], Routes[i][j + 1]) + PheremoneDeposit);
            }
            SetPhermone(Routes[i][Routes[i].Count - 1], Routes[i][0], GetPhermone(Routes[i][Routes[i].Count - 1], Routes[i][0]) + PheremoneDeposit);
        }
        else if(depositMode == DepositMode.GlobalBestOnly)
        {
            double PheremoneDeposit = PhermoneDepositRate;
            for (int j = 0; j < BestSoFar.Count - 1; j++)
            {
                SetPhermone(BestSoFar[j], BestSoFar[j + 1], GetPhermone(BestSoFar[j], BestSoFar[j + 1]) + PheremoneDeposit);
            }
            SetPhermone(BestSoFar[BestSoFar.Count - 1], BestSoFar[0], GetPhermone(BestSoFar[BestSoFar.Count - 1], BestSoFar[0]) + PheremoneDeposit);
        }

        else
        {
            throw new System.Exception("Unknown Deposit Mode");
        }



        DecayPhermones();
    }

    private IEnumerator InitialiseAnts(Ant[] ants, int numberofframes)
    {
        int antsperframe = (ants.Length / numberofframes)+1;
        int antsdone = 0;
        while(antsdone < ants.Length)
        {
            for(int i = 0; i < antsperframe && antsdone < ants.Length; i++)
            {
                ants[antsdone] = Instantiate(AntPrefab, new Vector3(0, 0, 0), Quaternion.identity).GetComponent<Ant>();
                antsdone++;
            }
            yield return null;
        }
    }

    private IEnumerator CalculateRoutes(double[] Distances, List<City>[] Routes, int NumerOfFrames, int numberofants, City[] cities)
    {
        Debug.Log("Calculating Routes");
        int antsperframe = (numberofants / NumerOfFrames)+1;
        int antsdone = 0;
        while (antsdone < numberofants)
        {
            for (int i = 0; i < antsperframe && antsdone < numberofants; i++)
            {
                (double, List<City>) result = CalculateRoute(cities);
                Routes[antsdone] = result.Item2;
                Distances[antsdone] = result.Item1;
                antsdone++;
            }
            yield return null;
        }
        Debug.Log("Finished Calculating Routes");
    }
    
    private (double, List<City>) CalculateRoute(City[] cities)
    {
        double distance = 0;
        HashSet<City> VisitedCities = new HashSet<City>(cities.Length);
        City CurrentCity = cities[System.Math.Min((int)(Random.value*cities.Length), cities.Length-1)];
        List<City> finalRoute = new List<City>(cities.Length);
        finalRoute.Add(CurrentCity);
        while (finalRoute.Count < cities.Length)
        {
            VisitedCities.Add(CurrentCity);
            double[] CityWeights = new double[cities.Length];
            double totalweight = 0;
            for (int i = 0; i < cities.Length; i++)
            {
              
                if (VisitedCities.Contains(cities[i]))
                    CityWeights[i] = 0;
                else
                    CityWeights[i] = System.Math.Pow(GetPhermone(CurrentCity, cities[i]), PhermoneWeight) * System.Math.Pow(1 / GetDistance(CurrentCity, cities[i]), DistanceWeight);
                totalweight += CityWeights[i];
            }
            double random = Random.Range(0, (float)totalweight);
            int currentcityindex = 0;
            while (random > 0)
            {
                random -= CityWeights[currentcityindex];
                currentcityindex++;
            }
            currentcityindex--;
            finalRoute.Add(cities[currentcityindex]);
            CurrentCity = cities[currentcityindex];
            distance += GetDistance(finalRoute[finalRoute.Count - 2], finalRoute[finalRoute.Count - 1]);
        }
        distance += GetDistance(finalRoute[finalRoute.Count - 1], finalRoute[0]);
        return (distance, finalRoute);
    }
 
}
