using System;
using UnityEngine;

public class PlayerPunch : MonoBehaviour
{
    public event Action EndedPunch;
    public event Action EndedBeingHit;
    
    // Animation Event
    private void StartPunch()
    {

    }

    // Animation Event
    private void EndPunch()
    {
        EndedPunch();
    }
    
    // Animation Event
    private void EndBeingHit()
    {
        EndedBeingHit();
    }
}
