using UnityEngine;

public class TestInteractable : MonoBehaviour, IInteractable
{
    private Player _player;

    private void Start()
    {
        _player = GameObject.FindWithTag("Player").GetComponent<Player>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        _player.InteractingObject = this;
    }

        private void OnTriggerExit(Collider other)
        {
            if (!other.gameObject.CompareTag("Player")) return;

    #pragma warning disable CS0253
            // Spurious warning, we intentionally want reference equality here
            if (this == _player.InteractingObject)
    #pragma warning restore CS0253
            {
                _player.InteractingObject = null;
            }
        }

    public void Interact()
    {
        print($"Player interacted with {name}");
    }
}
