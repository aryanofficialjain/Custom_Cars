using UnityEngine;
using System.Collections.Generic;


public class Wheel : MonoBehaviour
{
    public List<GameObject> WheelsGameobject;
    public List<Transform> WheelPosition;
    public List<Vector3> WheelScales = new List<Vector3>();  // Scale for each wheel type (adjustable from Inspector)
    public CarStart carStartScript;  // Reference to CarStart script to update tire lists

    private List<GameObject> currentActiveWheels = new List<GameObject>();  // Track currently active wheel instances
    private int currentWheelIndex = -1;  // Track which wheel type is currently active (-1 means none)

    // Method to change all 4 wheels to a specific wheel type
    public void ChangeWheels(int wheelIndex)
    {
        Debug.Log($"ChangeWheels called with index: {wheelIndex}");
        
        // Validate wheel index
        if (wheelIndex < 0 || wheelIndex >= WheelsGameobject.Count)
        {
            Debug.LogWarning($"Invalid wheel index: {wheelIndex}. WheelsGameobject count: {WheelsGameobject?.Count ?? 0}");
            return;
        }

        // Validate wheel positions
        if (WheelPosition == null || WheelPosition.Count != 4)
        {
            Debug.LogWarning($"WheelPosition list is invalid. Count: {WheelPosition?.Count ?? 0}");
            return;
        }

        // Validate wheel GameObject exists
        if (WheelsGameobject[wheelIndex] == null)
        {
            Debug.LogWarning($"Wheel GameObject at index {wheelIndex} is null");
            return;
        }
        
        Debug.Log($"All validations passed. Proceeding to instantiate wheels...");

        // Remove/destroy previous wheels
        // IMPORTANT: Only destroy actual wheel GameObjects, NOT the WheelPosition transforms
        foreach (GameObject oldWheel in currentActiveWheels)
        {
            if (oldWheel != null)
            {
                // Make sure we're not destroying a WheelPosition transform
                bool isWheelPosition = false;
                if (WheelPosition != null)
                {
                    foreach (Transform wheelPos in WheelPosition)
                    {
                        if (wheelPos != null && wheelPos.gameObject == oldWheel)
                        {
                            isWheelPosition = true;
                            Debug.LogWarning($"Trying to destroy WheelPosition transform! Skipping destruction of {oldWheel.name}");
                            break;
                        }
                    }
                }
                
                if (!isWheelPosition)
                {
                    Destroy(oldWheel);
                }
            }
        }

        // Clear the list of active wheels
        currentActiveWheels.Clear();
        
        // Validate WheelPosition transforms are still valid before proceeding
        Debug.Log($"Validating WheelPosition transforms...");
        for (int i = 0; i < WheelPosition.Count; i++)
        {
            if (WheelPosition[i] == null)
            {
                Debug.LogError($"WheelPosition[{i}] is NULL! This will prevent wheel instantiation. Please check your Wheel script configuration in the Inspector.");
            }
            else
            {
                Debug.Log($"WheelPosition[{i}] is valid: {WheelPosition[i].name}");
            }
        }

        // Get the scale for this wheel type (default to Vector3.one if not set)
        Vector3 wheelScale = Vector3.one;
        if (WheelScales != null && wheelIndex < WheelScales.Count)
        {
            wheelScale = WheelScales[wheelIndex];
        }

        // Instantiate new wheels at each position
        Debug.Log($"Starting wheel instantiation loop. WheelPosition.Count: {WheelPosition.Count}, wheelIndex: {wheelIndex}");
        for (int i = 0; i < WheelPosition.Count; i++)
        {
            Debug.Log($"Loop iteration {i}: Checking WheelPosition[{i}] - IsNull: {WheelPosition[i] == null}");
            if (WheelPosition[i] != null)
            {
                Debug.Log($"WheelPosition[{i}] is NOT null. Name: {WheelPosition[i].name}, Active: {WheelPosition[i].gameObject.activeSelf}");
            {
                // CRITICAL: Ensure the parent transform (WheelPosition) is active
                // If parent is inactive, child wheels will be invisible even if active
                if (!WheelPosition[i].gameObject.activeSelf)
                {
                    Debug.LogWarning($"WheelPosition[{i}] ({WheelPosition[i].name}) is INACTIVE! Activating it now...");
                    WheelPosition[i].gameObject.SetActive(true);
                }
                
                // Instantiate the wheel
                GameObject newWheel = Instantiate(WheelsGameobject[wheelIndex]);
                
                // Ensure the wheel is active BEFORE parenting
                newWheel.SetActive(true);
                
                // Make it a child of the car rim (WheelPosition transform) so it moves with the car
                newWheel.transform.SetParent(WheelPosition[i], worldPositionStays: false);
                
                // Set local position and rotation to align perfectly with the rim
                newWheel.transform.localPosition = Vector3.zero;
                newWheel.transform.localRotation = Quaternion.identity;
                
                // Apply the scale from Inspector
                newWheel.transform.localScale = wheelScale;
                
                // Double-check the wheel is still active after parenting
                if (!newWheel.activeSelf)
                {
                    Debug.LogWarning($"Wheel {i} became inactive after parenting! Reactivating...");
                    newWheel.SetActive(true);
                }
                
                // Verify the wheel is visible in hierarchy
                bool isVisibleInHierarchy = newWheel.activeInHierarchy;
                Debug.Log($"Wheel {i} instantiated - Name: {newWheel.name}, Active: {newWheel.activeSelf}, ActiveInHierarchy: {isVisibleInHierarchy}, Parent: {WheelPosition[i].name}, ParentActive: {WheelPosition[i].gameObject.activeSelf}, Scale: {wheelScale}, Position: {newWheel.transform.position}");
                
                // Add to active wheels list
                currentActiveWheels.Add(newWheel);
            }
            }
            else
            {
                Debug.LogWarning($"WheelPosition[{i}] is NULL! This is why no wheels are being created.");
            }
        }
        
        Debug.Log($"Loop completed. Total wheels instantiated: {currentActiveWheels.Count}");

        // Auto-find CarStart script if not assigned
        if (carStartScript == null)
        {
            carStartScript = FindObjectOfType<CarStart>();
        }

        // Update CarStart's tire lists with new wheels
        // Assuming positions 0,1 are front wheels and 2,3 are rear wheels
        if (carStartScript != null && currentActiveWheels.Count >= 4)
        {
            Debug.Log("Updating CarStart tire lists...");
            
            // Destroy any old wheels in frontTires list before clearing
            // Make sure we don't destroy the wheels we just created OR WheelPosition transforms
            if (carStartScript.frontTires != null)
            {
                foreach (GameObject oldTire in carStartScript.frontTires)
                {
                    // Only destroy if it's not one of our newly created wheels AND not a WheelPosition transform
                    if (oldTire != null && !currentActiveWheels.Contains(oldTire))
                    {
                        // Check if this is a WheelPosition transform
                        bool isWheelPosition = false;
                        if (WheelPosition != null)
                        {
                            foreach (Transform wheelPos in WheelPosition)
                            {
                                if (wheelPos != null && wheelPos.gameObject == oldTire)
                                {
                                    isWheelPosition = true;
                                    Debug.LogWarning($"Skipping destruction of WheelPosition transform: {oldTire.name}");
                                    break;
                                }
                            }
                        }
                        
                        if (!isWheelPosition)
                        {
                            Debug.Log($"Destroying old front tire: {oldTire.name}");
                            Destroy(oldTire);
                        }
                    }
                }
                carStartScript.frontTires.Clear();
            }
            else
            {
                carStartScript.frontTires = new List<GameObject>();
            }
            // Based on your setup: positions 2,3 are FRONT wheels (turn 30 deg)
            // positions 0,1 are REAR wheels (rotate continuously)
            carStartScript.frontTires.Add(currentActiveWheels[2]);
            carStartScript.frontTires.Add(currentActiveWheels[3]);
            Debug.Log($"Added {carStartScript.frontTires.Count} front tires to CarStart (positions 2,3)");

            // Destroy any old wheels in rearTires list before clearing
            // Make sure we don't destroy the wheels we just created OR WheelPosition transforms
            if (carStartScript.rearTires != null)
            {
                foreach (GameObject oldTire in carStartScript.rearTires)
                {
                    // Only destroy if it's not one of our newly created wheels AND not a WheelPosition transform
                    if (oldTire != null && !currentActiveWheels.Contains(oldTire))
                    {
                        // Check if this is a WheelPosition transform
                        bool isWheelPosition = false;
                        if (WheelPosition != null)
                        {
                            foreach (Transform wheelPos in WheelPosition)
                            {
                                if (wheelPos != null && wheelPos.gameObject == oldTire)
                                {
                                    isWheelPosition = true;
                                    Debug.LogWarning($"Skipping destruction of WheelPosition transform: {oldTire.name}");
                                    break;
                                }
                            }
                        }
                        
                        if (!isWheelPosition)
                        {
                            Debug.Log($"Destroying old rear tire: {oldTire.name}");
                            Destroy(oldTire);
                        }
                    }
                }
                carStartScript.rearTires.Clear();
            }
            else
            {
                carStartScript.rearTires = new List<GameObject>();
            }
            // Positions 0,1 are REAR wheels (rotate continuously)
            carStartScript.rearTires.Add(currentActiveWheels[0]);
            carStartScript.rearTires.Add(currentActiveWheels[1]);
            Debug.Log($"Added {carStartScript.rearTires.Count} rear tires to CarStart");
        }
        else
        {
            Debug.LogWarning($"CarStart update skipped. carStartScript: {carStartScript != null}, currentActiveWheels.Count: {currentActiveWheels.Count}");
        }

        // Update current wheel index
        currentWheelIndex = wheelIndex;
        
        Debug.Log($"ChangeWheels completed. Current wheel index: {currentWheelIndex}, Active wheels: {currentActiveWheels.Count}");
    }

    // UI Button Methods - Call these from your 3 UI buttons
    public void ChangeToWheel0()
    {
        ChangeWheels(0);
    }

    public void ChangeToWheel1()
    {
        ChangeWheels(1);
    }

    public void ChangeToWheel2()
    {
        ChangeWheels(2);
    }

    // Get the currently active wheel index
    public int GetCurrentWheelIndex()
    {
        return currentWheelIndex;
    }
}
