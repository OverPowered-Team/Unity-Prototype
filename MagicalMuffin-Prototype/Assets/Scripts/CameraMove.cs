using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    enum CameraType
    {
        oblicua,

    }
    public Transform player1;
    public Transform player2;
    public float distance;
    public float angle1;
    public float angle2;

    public bool follow = false;
    public float high = 0;
    Vector3 initial_vec;

    Vector3 cam_pos;//gizmos

    Vector3 middle_point = Vector3.zero;
    Camera cam;


    // Start is called before the first frame update
    void Start()
    {
        middle_point = new Vector3((player1.position.x + player2.position.x) * 0.5f, high, (player1.position.z + player2.position.z) * 0.5f);
        cam = GetComponent<Camera>();
        initial_vec = Camera.main.transform.position - middle_point;
        transform.position = CameraPos(angle1, angle2, distance);
        transform.LookAt(middle_point);
    }

    // Update is called once per frame
    void Update()
    {
        
        middle_point = new Vector3((player1.position.x + player2.position.x) * 0.5f, high, (player1.position.z + player2.position.z) * 0.5f);
        transform.position = CameraPos(angle1, angle2, distance) + middle_point;
        transform.LookAt(middle_point);
        /*cam_pos = cam.transform.position;

        if (Input.GetKey(KeyCode.F))
            cam.transform.Translate(Vector3.back * 5 * Time.deltaTime, Space.Self);
        if (Input.GetKey(KeyCode.R))
            cam.transform.Translate(Vector3.forward * 5 * Time.deltaTime, Space.Self);
        if (Input.GetKey(KeyCode.G))
            cam.transform.Translate(Vector3.back * 5 * Time.deltaTime, Space.World);
        if (Input.GetKey(KeyCode.T))
            cam.transform.Translate(Vector3.forward * 5 * Time.deltaTime, Space.World);
        //Vector3 mid_to_cam = cam.transform.position - middle_point;
        //cam.transform.position = (middle_point + mid_to_cam) * Mathf.Abs(Vector3.Distance(player1.position, player2.position)) * 0.5f;

        if (follow)
            transform.position = middle_point + initial_vec;*/
    }

    Vector3 CameraPos(float angle1, float angle2, float dst)
    {
        angle1 = Mathf.Deg2Rad * angle1;
        angle2 = Mathf.Deg2Rad * angle2;
        float x = Mathf.Cos(angle1) * Mathf.Cos(angle2);
        float z = Mathf.Sin(angle1) * Mathf.Cos(angle2);
        float y = Mathf.Sin(angle2);

        return new Vector3(x, y, z) * dst;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f,0f,1f);
        Gizmos.DrawWireSphere(middle_point, 0.75f);
        Gizmos.DrawWireSphere(middle_point, 5f);
        Gizmos.DrawLine(middle_point, cam_pos);
    }
}
