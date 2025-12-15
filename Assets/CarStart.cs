using UnityEngine;
using System.Collections.Generic;

public class CarStart : MonoBehaviour
{
    public List<GameObject> frontTires;
    public List<GameObject> rearTires;

    public AudioSource audioSource;

    public GameObject cargameobject;

    public float rearTireSpeed = 300f;  // Speed for rear wheels spinning on X axis (like moving car)
    public float rearTireYRotationSpeed = 30f;  // Speed for rear wheels rotating on Y axis (steering continuously)
    public float carRotationSpeed = 50f;  // Speed for car body rotation on Y axis (drifting in circle)

    private bool isCarRunning = false;  // toggle state

    void Start()
    {
        // No automatic rotation on start
    }

    void Update()
    {
        if (isCarRunning)
        {
            // Rotate rear tires continuously 360° on X axis (spinning like a moving car)
            // Front wheels stay at 30 degrees on Y axis (no continuous rotation)
            foreach (GameObject tire in rearTires)
            {
                if (tire != null)
                {
                    tire.transform.Rotate(rearTireSpeed * Time.deltaTime, 0f, 0f);
                }
            }

            // Rotate the car body continuously 360° on Y axis (drifting in a circle)
            if (cargameobject != null)
            {
                cargameobject.transform.Rotate(0f, carRotationSpeed * Time.deltaTime, 0f);
            }
        }
    }

    // Call this from UI Button (Toggle)
    public void ToggleCar()
    {
        isCarRunning = !isCarRunning;
        
        if (isCarRunning)
        {
            // Rotate front wheels by 30 degrees on Y axis (steering angle) - happens once per toggle ON
            // Front wheels stay at this angle and don't rotate continuously
            foreach (GameObject tire in frontTires)
            {
                if (tire != null)
                {
                    tire.transform.Rotate(0f, 30f, 0f);
                }
            }

            // Rear wheels don't need initial rotation - they will rotate continuously on X axis in Update()
            // No Y axis rotation for rear wheels

            // Play audio when car starts
            if (audioSource != null)
            {
                audioSource.Play();
            }
        }
        else
        {
            // Pause audio when car stops
            if (audioSource != null)
            {
                audioSource.Pause();
            }
        }
    }

    // Call this to start rotation
    public void StartCar()
    {
        if (!isCarRunning)
        {
            ToggleCar();
        }
    }

    // Call this to stop rotation
    public void StopCar()
    {
        isCarRunning = false;
        
        // Pause audio when car stops
        if (audioSource != null)
        {
            audioSource.Pause();
        }
    }
}