using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRig : MonoBehaviour {

    Camera[] cameras;
    public float zoomSpeed = 18;
    public float panMult = 180;
    public float maxZoomedOut = 500;
    public float maxZoomedIn = 50;
    public float topRotation = 90;
    public float bottomRotation = 60;

    public float camTranslationSpeed = 20;
    public float mouseScrollArea = 20;

    float fov = 60;

    float rotationStep;

    public Player player;
    public HexGrid grid;

    // Use this for initialization
    void Start ()
    {
        cameras = GetComponentsInChildren<Camera>();

        cameras[0].fieldOfView = cameras[1].fieldOfView = cameras[2].fieldOfView = fov;

        rotationStep = zoomSpeed * (topRotation - bottomRotation) / (maxZoomedOut - maxZoomedIn);
        transform.position = new Vector3(player.unit.transform.position.x, maxZoomedIn, player.unit.transform.position.z - (maxZoomedIn / Mathf.Tan(Mathf.Deg2Rad * bottomRotation)));
        transform.rotation = Quaternion.Euler(bottomRotation, transform.eulerAngles.y, transform.eulerAngles.z);

        cameras[0].transform.localPosition = Vector3.zero;
        cameras[0].transform.localRotation = Quaternion.identity;

        cameras[1].transform.name = "Left Camera";
        cameras[1].transform.localPosition = Vector3.left * grid.width * HexMetrics.innerRad * 2;
        cameras[1].transform.localRotation = Quaternion.identity;

        cameras[2].transform.name = "Right Camera";
        cameras[2].transform.localPosition = Vector3.right * grid.width * HexMetrics.innerRad * 2;
        cameras[2].transform.localRotation = Quaternion.identity;
    }
	
	// Update is called once per frame
	void Update ()
    {
        float horizAxis = Input.GetAxis("Horizontal") * Time.deltaTime * camTranslationSpeed * (transform.position.y - maxZoomedIn / 2);
        float vertAxis = Input.GetAxis("Vertical") * Time.deltaTime * camTranslationSpeed * (transform.position.y - maxZoomedIn / 2);

        float mPosX = Input.mousePosition.x;
        float mPosY = Input.mousePosition.y;
        if (mPosX >= 0 && mPosX < mouseScrollArea)
        {
            horizAxis = -Time.deltaTime * camTranslationSpeed * (transform.position.y - maxZoomedIn / 2);
        }
        else if (mPosX < Screen.width && mPosX >= Screen.width - mouseScrollArea)
        {
            horizAxis = Time.deltaTime * camTranslationSpeed * (transform.position.y - maxZoomedIn / 2);
        }

        if (mPosY >= 0 && mPosY < mouseScrollArea)
        {
            vertAxis = -Time.deltaTime * camTranslationSpeed * (transform.position.y - maxZoomedIn / 2);
        }
        else if (mPosY < Screen.height && mPosY >= Screen.height - mouseScrollArea)
        {
            vertAxis = Time.deltaTime * camTranslationSpeed * (transform.position.y - maxZoomedIn / 2);
        }

        transform.Translate(horizAxis, 0f, vertAxis, Space.World);

        if(transform.position.x > grid.width * HexMetrics.innerRad * 2f)
        {
            transform.position += Vector3.left * grid.width * HexMetrics.innerRad * 2f;
        }
        else if(transform.position.x < 0)
        {
            transform.position += Vector3.right * grid.width * HexMetrics.innerRad * 2f;
        }
    }

    public void ZoomIn()
    {
        if (transform.position.y - zoomSpeed >= maxZoomedIn)
        {
            Vector2 camMovement = Vector2.zero;

            RaycastHit hit;
            if (Raycast(Input.mousePosition, out hit))
            {
                camMovement = new Vector2((hit.point.x - transform.position.x) * zoomSpeed / panMult, (hit.point.z - transform.position.z) * zoomSpeed / panMult);
            }

            transform.Translate(new Vector3(camMovement.x, -zoomSpeed, camMovement.y), Space.World);
            transform.rotation = Quaternion.Euler(transform.eulerAngles.x - rotationStep, transform.eulerAngles.y, transform.eulerAngles.z);
        }
    }

    public void ZoomOut()
    {
        Vector2 camMovement = Vector2.zero;

        RaycastHit hit;
        if (Raycast(Input.mousePosition, out hit))
        {
            camMovement = new Vector2((transform.position.x - hit.point.x) * zoomSpeed / panMult, (transform.position.z - hit.point.z) * zoomSpeed / panMult);
        }

        if (transform.position.y + zoomSpeed <= maxZoomedOut)
        {
            transform.Translate(camMovement.x, +zoomSpeed, camMovement.y, Space.World);
            transform.rotation = Quaternion.Euler(transform.eulerAngles.x + rotationStep, transform.eulerAngles.y, transform.eulerAngles.z);
        }
    }

    public bool Raycast(Vector3 mousePosition, out RaycastHit hit)
    {
        Ray inputRay = cameras[0].ScreenPointToRay(mousePosition);
        if (Physics.Raycast(inputRay, out hit))
        {
            return true;
        }
        else
        {
            inputRay = cameras[1].ScreenPointToRay(mousePosition);
            if (Physics.Raycast(inputRay, out hit))
            {
                return true;
            }
            else
            {
                inputRay = cameras[2].ScreenPointToRay(mousePosition);
                if (Physics.Raycast(inputRay, out hit))
                {
                    return true;
                }
            }
        }
        return false;
    }
}
