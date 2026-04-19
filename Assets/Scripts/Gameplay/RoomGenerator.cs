using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    [SerializeField] private int seed;
    
    [SerializeField] private Transform startRoomPool;
    [SerializeField] private Transform normalRoomPool;
    [SerializeField] private Transform eliteRoomPool;
    [SerializeField] private Transform bossRoomPool;

    private enum RoomType
    {
        Start, Normal, Elite, Boss
    }

    [SerializeField] private List<RoomType> rooms;

    private List<Transform> startQueue = new();
    private List<Transform> normalQueue = new();
    private List<Transform> eliteQueue = new();
    private List<Transform> bossQueue = new();

    private Gate lastGate;
    private System.Random rng;
    
    private void Awake()
    {
        rng = new System.Random(seed);
        for (int i = 0; i < rooms.Count; i++)
        {
            RoomType room = rooms[i];
            Transform roomPick = room switch
            {
                RoomType.Start => PickFromQueue(startQueue, startRoomPool),
                RoomType.Normal => PickFromQueue(normalQueue, normalRoomPool),
                RoomType.Elite => PickFromQueue(eliteQueue, eliteRoomPool),
                RoomType.Boss => PickFromQueue(bossQueue, bossRoomPool),
                _ => throw new InvalidEnumArgumentException("Unknown Room Type")
            };
            
            Transform newRoom = Instantiate(roomPick, new Vector3(0f, 0f, 25f * i), Quaternion.identity);
            print($"Spawned {roomPick.name}");

            if (lastGate != null)
            {
                lastGate.nextRoom = newRoom;
            }
            lastGate = newRoom.GetComponentInChildren<Gate>();
        }
    }

    private Transform PickFromQueue(List<Transform> queue, Transform pool)
    {
        if (queue.Count == 0)
        {
            for (int i = 0; i < pool.childCount; ++i)
            {
                queue.Add(pool.GetChild(i));
            }
            Shuffle(queue);
        }
        
        var pick = queue[^1];
        queue.RemoveAt(queue.Count - 1);
        return pick;
    }
    
    private void Shuffle<T>(IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }
    }
}
