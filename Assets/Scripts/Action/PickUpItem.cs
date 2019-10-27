using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpItem : MonoBehaviour
{
    public GUIStyle guiStyle = new GUIStyle();
    public Camera playerCamera;
    public GameObject playerHand;

    public bool isCanTakeItem;

    // Update is called once per frame
    void Update()
    {
        RaycastHit hitInfo;
        Ray r = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(r, out hitInfo))
        {
            if (hitInfo.transform.CompareTag("Weapon"))
            {
                isCanTakeItem = true;
                if (Input.GetKeyDown(KeyCode.F))
                {
                    var weapon = hitInfo.transform.gameObject;
                    Destroy(hitInfo.transform.gameObject);
                    playerHand.transform.parent = weapon.transform;
                    playerHand.transform.position = Vector3.zero;
                    playerHand.transform.rotation = Quaternion.identity;

                    isCanTakeItem = false;
                }
            }
        } else
        {
            isCanTakeItem = false;
        }
    }

    void OnGUI()
    {
        if (isCanTakeItem)
        {
            guiStyle.normal.textColor = Color.black;
            guiStyle.fontSize = 20;
            GUI.Label(new Rect(playerCamera.pixelWidth / 2, playerCamera.pixelHeight / 2 + 200, 150, 30), "For take push F", guiStyle);
        }
    }
}
