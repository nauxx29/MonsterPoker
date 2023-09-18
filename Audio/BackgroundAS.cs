using UnityEngine;

// Background Music (PokerFace)
public class BackgroundAS : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    private void OnEnable()
    {
        Events.onOnChangeAudio.AddListener(OnChangeAudio);
    }

    private void OnDisable()
    {
        Events.onOnChangeAudio.RemoveListener(OnChangeAudio);
    }

    private void OnChangeAudio()
    {
        audioSource.enabled = !audioSource.enabled;

    }
}
