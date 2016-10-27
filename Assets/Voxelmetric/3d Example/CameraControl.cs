using UnityEngine;
using UnityEngine.UI;

public class CameraControl : MonoBehaviour
{
    Vector2 rot;

    void Update()
    {
        //Movement
        if (Input.GetMouseButton(1))
        {
            rot = new Vector2(
                rot.x + Input.GetAxis("Mouse X") * 3,
                rot.y + Input.GetAxis("Mouse Y") * 3);

            transform.localRotation = Quaternion.AngleAxis(rot.x, Vector3.up);
            transform.localRotation *= Quaternion.AngleAxis(rot.y, Vector3.left);
        }

        transform.position += transform.forward * 50 * Input.GetAxis("Vertical") * Time.deltaTime;
        transform.position += transform.right * 50 * Input.GetAxis("Horizontal") * Time.deltaTime;
    }
}
