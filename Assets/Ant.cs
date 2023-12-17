using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Ant : MonoBehaviour
{

    public void Start()
    {
      
    }
    public void WalkPath(Queue<City> path, double speed)
    {
        //stop all previous walk coroutines
        StopAllCoroutines();
        //start walk coroutine
        StartCoroutine(Walk(path, speed));

        
    }

    private IEnumerator Walk(Queue<City> route, double speed)
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
            double distanceleftthisframe = speed * Time.deltaTime;
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
