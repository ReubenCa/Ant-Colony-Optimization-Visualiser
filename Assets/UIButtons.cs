using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIButtons : MonoBehaviour
{
    

    public void onStartClick()
    {
        SimulationManager.instance.StartorResumeSimulation();
    }
    public void onPauseStopClick()
    {
        if (SimulationManager.instance.state == SimulationState.Running)
        { 
            SimulationManager.instance.PauseSimulation();
        }
        else if(SimulationManager.instance.state == SimulationState.Paused)
        {
            SimulationManager.instance.StopSimulation();
        }
    }
}
