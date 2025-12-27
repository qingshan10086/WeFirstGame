using UnityEngine;

/// <summary>
/// Debug utility for MusicTriggerZone and BloodMusicManager
/// </summary>
public class MusicDebugger : MonoBehaviour
{
    [SerializeField] private bool checkAtStart = true;
    [SerializeField] private MusicTriggerZone[] triggerZones;
    
    private void Start()
    {
        if (checkAtStart)
        {
            CheckSetup();
        }
    }
    
    [ContextMenu("Check Music System Setup")]
    public void CheckSetup()
    {
        Debug.Log("=== Music System Setup Check ===");
        
        // Check BloodMusicManager
        if (BloodMusicManager.Instance == null)
        {
            Debug.LogError("❌ BloodMusicManager instance not found! Make sure it's in the scene.");
        }
        else
        {
            Debug.Log("✅ BloodMusicManager instance found.");
            
            if (BloodMusicManager.Instance.backgroundMusicSource == null)
            {
                Debug.LogError("❌ BloodMusicManager: Background music source not assigned.");
            }
            else
            {
                Debug.Log("✅ BloodMusicManager: Background music source assigned.");
                
                AudioClip currentMusic = BloodMusicManager.Instance.GetCurrentBackgroundMusic();
                if (currentMusic != null)
                {
                    Debug.Log($"🎵 Current music playing: {currentMusic.name}");
                }
                else
                {
                    Debug.Log("🎵 No music currently playing.");
                }
            }
        }
        
        // Check Player tag
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Length == 0)
        {
            Debug.LogError("❌ No objects found with tag 'Player'.");
        }
        else
        {
            Debug.Log($"✅ Found {players.Length} objects with tag 'Player'.");
            foreach (var player in players)
            {
                Debug.Log($"  - Player: {player.name} (Has Collider2D: {player.GetComponent<Collider2D>() != null})");
            }
        }
        
        // Check MusicTriggerZones
        if (triggerZones.Length == 0)
        {
            triggerZones = FindObjectsOfType<MusicTriggerZone>();
        }
        
        if (triggerZones.Length == 0)
        {
            Debug.LogError("❌ No MusicTriggerZone objects found in scene.");
        }
        else
        {
            Debug.Log($"✅ Found {triggerZones.Length} MusicTriggerZone objects.");
            
            foreach (var zone in triggerZones)
            {
                Debug.Log($"\n📐 Zone: {zone.name}");
                
                // Check collider
                BoxCollider2D collider = zone.GetComponent<BoxCollider2D>();
                if (collider == null)
                {
                    Debug.LogError($"  ❌ No BoxCollider2D found.");
                }
                else
                {
                    Debug.Log($"  ✅ BoxCollider2D found (IsTrigger: {collider.isTrigger}, Size: {collider.size})");
                    
                    if (!collider.isTrigger)
                    {
                        Debug.LogError("  ❌ Collider is not set to trigger!");
                    }
                }
                
                // Check zone music
                if (zone.zoneMusic == null)
                {
                    Debug.LogError("  ❌ zoneMusic is not assigned.");
                }
                else
                {
                    Debug.Log($"  ✅ zoneMusic assigned: {zone.zoneMusic.name}");
                }
                
                // Check other settings
                Debug.Log($"  🎮 Player Tag: {zone.playerTag}");
                Debug.Log($"  🔄 useFadeEffects: {zone.useFadeEffects}");
                Debug.Log($"  🔄 fadeDuration: {zone.fadeDuration}");
                Debug.Log($"  🔄 restorePreviousMusic: {zone.restorePreviousMusic}");
            }
        }
        
        Debug.Log("=== Setup Check Complete ===");
    }
    
    [ContextMenu("Test Music Switch")]
    public void TestMusicSwitch()
    {
        Debug.Log("=== Testing Music Switch ===");
        
        if (BloodMusicManager.Instance == null)
        {
            Debug.LogError("❌ BloodMusicManager not found.");
            return;
        }
        
        // Find all AudioClips in the scene
        AudioClip[] allClips = Resources.FindObjectsOfTypeAll<AudioClip>();
        if (allClips.Length == 0)
        {
            Debug.LogError("❌ No AudioClips found in project.");
            return;
        }
        
        // Test switching to the first available clip
        AudioClip testClip = allClips[0];
        Debug.Log($"📢 Testing music switch to: {testClip.name}");
        BloodMusicManager.Instance.PlayBackgroundMusic(testClip, true);
    }
}