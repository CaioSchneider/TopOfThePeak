using UnityEngine;

public class Life : MonoBehaviour
{
    PlayerController player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = PlayerController.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    protected void OnTriggerStay2D(Collider2D _other)
    {
        if (_other.CompareTag("Player"))
        {
            //player.IncreaseHealth();
            player.Heal();
            Destroy(gameObject);
        }
    }

  
}
