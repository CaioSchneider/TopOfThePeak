using UnityEngine;

public class Fire : MonoBehaviour
{
   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    protected void OnTriggerStay2D(Collider2D _other)
    {
        if (_other.CompareTag("Player"))
        {
            GiveFire();
        }
    }

    protected virtual void GiveFire()
    {
        PlayerController.hasFire = true;
       
    }
}
