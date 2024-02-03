using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Ant : MonoBehaviour
{
     public static List<Ant> AllAnts = new List<Ant>();
    public void Start()
    {
        AllAnts.Add(this);
    }
    public void OnDestroy()
    {
        AllAnts.Remove(this);
    }
    public void WalkPath(Queue<City> path)
    {
        //stop all previous walk coroutines
        StopAllCoroutines();
        //start walk coroutine
        StartCoroutine(Walk(path));

        
    }

    private IEnumerator Walk(Queue<City> route)
    {
      //  Debug.Log("Walking");
        
        route.Enqueue(route.Peek()); //Makes ant finish back at start
        Queue<Vector3> positions = new Queue<Vector3>(route.Count);
        
        while (route.Count > 0) 
        {
            positions.Enqueue(route.Dequeue().currentPosition);
        }
        Vector3 nextDestination = positions.Dequeue();
        transform.position = nextDestination;
        while (true)
        {
            if(SimulationManager.instance.state == SimulationState.Paused)
            {
                yield return (new WaitUntil(() => SimulationManager.instance.state == SimulationState.Running));
            }
            double distanceleftthisframe = SimulationManager.instance.AntSpeed * Time.deltaTime;
          //  Debug.Log("Distance left this frame: " + distanceleftthisframe);
            while(distanceleftthisframe >0) { 
                double distancefromtarget = Vector3.Distance(transform.position, nextDestination);
                if(distancefromtarget < distanceleftthisframe)
                {
                    transform.position = nextDestination;
                    distanceleftthisframe -= distancefromtarget;
                    if (positions.Count == 0)
                    {
                        Destroy(gameObject);
                        yield break;                       
                    }
                    nextDestination = positions.Dequeue();
                  //  Debug.Log("Next city: " + nextCity.ID);
                }
                else
                {
                    transform.position += (nextDestination - transform.position).normalized * (float)distanceleftthisframe;
                    distanceleftthisframe = -1;
                   // Debug.Log("Not Reached city this frame");
                }
            }
            yield return null;
        }

    }
}
