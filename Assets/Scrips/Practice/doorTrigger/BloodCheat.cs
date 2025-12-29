using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BloodCheat : MonoBehaviour
{
    public PlayerStats player;
    public Enemy RedMist;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
                if (Input.GetKeyDown(KeyCode.Keypad5))
                {
                    player.currentHealth += 50;
                }
                else if (Input.GetKeyDown(KeyCode.Keypad6))
                {
                    player.strength.AddModifier(10);
                }
                else if (Input.GetKey(KeyCode.Backspace) && Input.GetKeyDown(KeyCode.Keypad5))
                {
                    player.currentHealth -= 50;
                }
                else if (Input.GetKey(KeyCode.Backspace) && Input.GetKeyDown(KeyCode.Keypad6))
                {
                    player.strength.RemoveModifier(10);
                }
            }

       
    }
        

    }

