using UnityEngine;

public class move : MonoBehaviour
{
    float speed = 5f;
    Vector3 Pos = Vector3.one;
    Vector3 Rotate = Vector3.zero;
    GameObject sight;

    void Start() { sight = gameObject; }

    void Update()
    {
        //move 
        Vector3 forward = sight.transform.forward;
        if (Input.GetKey(KeyCode.LeftShift)) { speed = 20f; } else { speed = 5f; }
        if (Input.GetKey(KeyCode.W) | Input.GetKey(KeyCode.UpArrow)) { Pos.z += speed * forward.z * Time.deltaTime; Pos.x += speed * forward.x * Time.deltaTime; }
        if (Input.GetKey(KeyCode.S) | Input.GetKey(KeyCode.DownArrow)) { Pos.z -= speed * forward.z * Time.deltaTime; Pos.x -= speed * forward.x * Time.deltaTime; }
        if (Input.GetKey(KeyCode.D) | Input.GetKey(KeyCode.RightArrow)) { Pos.x += speed * forward.z * Time.deltaTime; Pos.z -= speed * forward.x * Time.deltaTime; }
        if (Input.GetKey(KeyCode.A) | Input.GetKey(KeyCode.LeftArrow)) { Pos.x -= speed * forward.z * Time.deltaTime; Pos.z += speed * forward.x * Time.deltaTime; }
        if (Input.GetKey(KeyCode.Q)) { Pos.y += speed * Time.deltaTime; }
        if (Input.GetKey(KeyCode.E)) { Pos.y -= speed * Time.deltaTime; }
        sight.transform.position = Pos;

        //rotate
        bool rotate = Input.GetKey(KeyCode.LeftControl);
        if (rotate)
        {
            float My = Input.GetAxis("Mouse X");
            float Mx = Input.GetAxis("Mouse Y");
            Rotate.y += My * speed * Time.deltaTime * 3;
            Rotate.x += -Mx * speed * Time.deltaTime * 3;
        }
        sight.transform.rotation = Quaternion.Euler(Rotate);
    }
}