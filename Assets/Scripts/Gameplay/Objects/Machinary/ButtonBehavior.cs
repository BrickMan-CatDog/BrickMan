using System;
using System.Collections.Generic;
using UnityEngine;
using static Constants;


public class ButtonBehavior : MonoBehaviour
{
    public List<DoorBehavior> linkedDoors;
    [SerializeField] private GameObject ButtonOn;
    [SerializeField] private GameObject ButtonOff;
    private bool isOn = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("버튼 누름");

        if (collision.CompareTag("Player") && !isOn)
        {
            isOn = true;
            ButtonOn.SetActive(true);
            ButtonOff.SetActive(false);

            foreach (var door in linkedDoors)
            {
                door.Open();
            }
        }
        else if (collision.CompareTag("Player") && isOn)
        {
            isOn = false;
            ButtonOn.SetActive(false);
            ButtonOff.SetActive(true);

            foreach (var door in linkedDoors)
            {
                door.Close();
            }
        }
    }
}
