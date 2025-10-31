using System.Collections;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public GameObject messageUI;
    public float displayTime = 4f;
    private bool alreadyTriggered = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(messageUI != null)
        {
            messageUI.SetActive(false);
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")&& !alreadyTriggered)
        {
           
           alreadyTriggered = true;
           StartCoroutine(ShowText()); 
        }
    }
    IEnumerator ShowText()
    {
        messageUI.SetActive(true);
        yield return new WaitForSeconds(displayTime);
        Debug.Log("Desativando texto");
        messageUI.SetActive(false);
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
