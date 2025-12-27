using UnityEngine;
using UnityEngine.Events;

namespace CoinSystem
{
    public class Coin : MonoBehaviour
    {
        public static Coin instance; // Singleton instance so other scripts can access the coin manager easily

        public int playerCoins; // Current number of coins the player has
        private UnityEvent<int> OnCoinsChanged; // Event triggered when the number of coins changes

        private void Awake()
        {
            // Singleton pattern: if no instance exists, assign this one
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject); // Make sure this object isn't destroyed when changing scenes

                // Load saved coins from PlayerPrefs if available
                if (PlayerPrefs.HasKey("PlayerCoins"))
                {
                    playerCoins = PlayerPrefs.GetInt("PlayerCoins");
                }
                else
                {
                    playerCoins = 0; // Default to 0 if no saved data exists
                    SavePlayerCoins(); // Save default value
                }
            }
            else
            {
                // If an instance already exists, destroy the duplicate
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            // Ensure coins are loaded from PlayerPrefs on start
            playerCoins = PlayerPrefs.GetInt("PlayerCoins", playerCoins);

            // NOTE: Make sure to call AddCoins or RemoveCoins elsewhere
            // to trigger the OnCoinsChanged event if you're using it in your UI.
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                // Save coin data when the app is paused (e.g. user switches app)
                SavePlayerCoins();
            }
        }

        // Adds coins to the player's total
        public void AddCoins(int amount)
        {
            playerCoins += amount;

            // Trigger event for coin change (e.g. to update UI)
            OnCoinsChanged?.Invoke(playerCoins);

            // Save the updated value to PlayerPrefs
            PlayerPrefs.Save();
        }

        // Tries to subtract coins from the player
        public bool RemoveCoins(int amount)
        {
            if (playerCoins >= amount)
            {
                playerCoins -= amount;

                // Trigger event and save only if the transaction is valid
                OnCoinsChanged?.Invoke(playerCoins);
                PlayerPrefs.Save();

                return true; // Successful transaction
            }
            else
            {
                return false; // Not enough coins
            }
        }

        private void OnDestroy()
        {
            // Save coin count when this object is destroyed (e.g. on exit or scene change)
           
            SavePlayerCoins();
         
        }

        // Saves the player's coin count to PlayerPrefs
        private void SavePlayerCoins()
        {
            PlayerPrefs.SetInt("PlayerCoins", playerCoins);
            PlayerPrefs.Save();
        }
    }
}