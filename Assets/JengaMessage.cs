using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEditor.Rendering;

public class Collision : MonoBehaviour
{
    public TextMeshProUGUI messageText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        messageText.text = ""; 
        //uiMessage.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void OnCollisionEnter(UnityEngine.Collision collision)
    {
        if (collision.gameObject.name == "Plane")
        {
            messageText.text = "Block touched the " + collision.gameObject.name;
        }
        if (collision.gameObject.CompareTag("Plane"))
        {
            messageText.text = "Block touched the " + collision.gameObject.name;
            Invoke("HideMessage", 2f);
        }
    }
    void HideMessage()
    {
        messageText.text = "";
    }
}
