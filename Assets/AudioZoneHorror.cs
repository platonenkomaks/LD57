using UnityEngine;

public class AudioZoneHorror : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;
        
        G.AudioManager.Play("HorrorSound");
        other.GetComponent<BatEyesSwarm>().StartSpawning();
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;
        G.AudioManager.Stop("HorrorSound");
        other.GetComponent<BatEyesSwarm>().StopSpawning();
    }
    
    
}
