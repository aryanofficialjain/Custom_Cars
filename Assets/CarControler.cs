// using UnityEngine;

// public class CarControler : MonoBehaviour
// {
//     [SerializeField] private float speed;
//     private FixedJoystick fixedJoystick;
//     private Rigidbody carRigidbody;

//     private void OnEnable()
//     {
//         fixedJoystick = FindAnyObjectByType<FixedJoystick>();
//         carRigidbody = gameObject.GetComponent<Rigidbody>();
//     }

//     private void FixedUpdate()
//     {
//         if (fixedJoystick == null || carRigidbody == null)
//             return;

//         float xVal = fixedJoystick.Horizontal;
//         float yVal = fixedJoystick.Vertical;

//         Vector3 movement = new Vector3(xVal, 0, yVal);
        
//         // Use linearVelocity for Unity 6+, fallback to velocity for older versions
//         #if UNITY_6000_0_OR_NEWER
//         carRigidbody.linearVelocity = movement * speed;
//         #else
//         carRigidbody.velocity = movement * speed;
//         #endif

//         if (xVal != 0 && yVal != 0)
//         {
//             transform.eulerAngles = new Vector3(transform.eulerAngles.x, Mathf.Atan2(xVal, yVal) * Mathf.Rad2Deg, transform.eulerAngles.z);
//         }
//     }
// }
