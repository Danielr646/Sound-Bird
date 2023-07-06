using UnityEngine;

public class Player : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public Sprite[] sprites;
    private int spriteIndex;

    public float strength = 5f;
    public float gravity = -9.81f;
    public float tilt = 5f;

    private Vector3 direction;

    private AudioClip audioClip;
    private bool isRecording;

    public int audioSourceIndex = 0;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        audioClip = Microphone.Start(Microphone.devices[audioSourceIndex], true, 1, AudioSettings.outputSampleRate);
        isRecording = true;

        InvokeRepeating(nameof(AnimateSprite), 0.15f, 0.15f);
    }

    private void OnEnable()
    {
        Vector3 position = transform.position;
        position.y = 0f;
        transform.position = position;
        direction = Vector3.zero;
    }

    private void Update()
    {
        float loudness = GetLoudness();

        if (loudness > 0.1f) {
            direction = Vector3.up * strength;
        }

        // Apply gravity and update the position
        direction.y += gravity * Time.deltaTime;
        transform.position += direction * Time.deltaTime;

        // Tilt the bird based on the direction
        Vector3 rotation = transform.eulerAngles;
        rotation.z = direction.y * tilt;
        transform.eulerAngles = rotation;
    }

    private void AnimateSprite()
    {
        spriteIndex++;

        if (spriteIndex >= sprites.Length) {
            spriteIndex = 0;
        }

        if (spriteIndex < sprites.Length && spriteIndex >= 0) {
            spriteRenderer.sprite = sprites[spriteIndex];
        }
    }

    private float GetLoudness()
    {
        if (isRecording) {
            float[] samples = new float[1024];
            int sampleCount = Microphone.GetPosition(null);
            Microphone.GetPosition(null);
            audioClip.GetData(samples, sampleCount - samples.Length);
            float sum = 0f;

            for (int i = 0; i < samples.Length; i++) {
                sum += Mathf.Abs(samples[i]);
            }

            return sum / samples.Length;
        }

        return 0f;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Obstacle")) {
            FindObjectOfType<GameManager>().GameOver();
        } else if (other.gameObject.CompareTag("Scoring")) {
            FindObjectOfType<GameManager>().IncreaseScore();
        }
    }
}
