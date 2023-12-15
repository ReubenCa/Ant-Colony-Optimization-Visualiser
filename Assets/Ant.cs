using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Ant : MonoBehaviour
{
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
        City nextCity = route.Dequeue();
        transform.position = nextCity.transform.position;
        while(true)
        {
            double distanceleftthisframe = speed * Time.deltaTime;
          //  Debug.Log("Distance left this frame: " + distanceleftthisframe);
            while(distanceleftthisframe >0) { 
                double distancefromtarget = Vector3.Distance(transform.position, nextCity.transform.position);
                if(distancefromtarget < distanceleftthisframe)
                {
                    transform.position = nextCity.transform.position;
                    distanceleftthisframe -= distancefromtarget;
                    if (route.Count == 0)
                        yield break;
                    nextCity = route.Dequeue();
                  //  Debug.Log("Next city: " + nextCity.ID);
                }
                else
                {
                    transform.position += (nextCity.transform.position - transform.position).normalized * (float)distanceleftthisframe;
                    distanceleftthisframe = -1;
                   // Debug.Log("Not Reached city this frame");
                }
            }
            yield return null;
        }

    }
}
