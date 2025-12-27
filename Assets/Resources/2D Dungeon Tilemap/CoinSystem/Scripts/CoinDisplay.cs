using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro; // Using TextMeshPro for UI text rendering
namespace CoinSystem
{
    public class CoinDisplay : MonoBehaviour
    {
        public TextMeshProUGUI coinText; // Reference to the UI Text component that displays the coin count
        public Coin coinManager; // Reference to the Coin manager that tracks the player's coins

        private void Start()
        {
            // Get the singleton instance of the Coin manager
            coinManager = Coin.instance;

            // Check if the Coin manager exists in the scene
            if (coinManager == null)
            {
                Debug.LogError("Coin object not found. Make sure it exists in the scene.");
            }
        }

        private void Update()
        {
            // Continuously update the coin display every frame
            UpdateCoinDisplay();
        }

        public void UpdateCoinDisplay()
        {
            // Set the text of the UI to show the current number of coins the player has
            coinText.text = coinManager.playerCoins.ToString();
        }
    }
}