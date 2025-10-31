using UnityEngine;
using TMPro;
using System.Collections;

public class Picaxe : MonoBehaviour
{
    public GameObject messageUI;
    public float displayTimePick = 4f;
    SpriteRenderer sr;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        if (messageUI != null)
        {
            messageUI.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    protected void OnTriggerEnter2D(Collider2D _other)
    {
        if (_other.CompareTag("Player"))
        {
            
            PlayerController.hasPick = true;
            sr.sprite = null;
            StartCoroutine(ShowTextPick());

        }
    }
    IEnumerator ShowTextPick()
    {
        messageUI.SetActive(true);   
        yield return new WaitForSeconds(displayTimePick);    
        messageUI.SetActive(false);
        Destroy(gameObject);

    }

}
