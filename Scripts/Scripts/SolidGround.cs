using UnityEngine;

public class SolidGround : MonoBehaviour
{
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
            RecordPlace();
        }
    }

    private void RecordPlace()
    {
        PlayerController.lastGround = transform.position + new Vector3(0, 1, 0);
        
    }
}
