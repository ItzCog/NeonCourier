using UnityEngine;

public class Gate : MonoBehaviour
{
    public Transform nextRoom;
    
    [SerializeField] private Vector3 spawnOffset;
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        
        print($"Moving player to {nextRoom.name}");
        var controller = other.GetComponent<CharacterController>();
        controller.enabled = false;
        other.transform.position = nextRoom.position + spawnOffset;
        controller.enabled = true;
    }
}
