using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDrop : MonoBehaviour
{
    public WeaponData data;
    
    private void Update()
    {
        transform.Rotate(new Vector3(0f, 180f * Time.deltaTime, 0f));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        other.GetComponent<Player>().Equip(data);
        Destroy(gameObject);
    }
}
