using UnityEngine;
using TMPro;

public class TutorialZone : MonoBehaviour
{
    public GameObject tutorialText;
    void OnTriggerEnter2D(Collider2D collision) {
        if(collision.CompareTag("Player")) {
            tutorialText.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D collision) {
        if(collision.CompareTag("Player")) {
            tutorialText.SetActive(false);
        }
    }
}
