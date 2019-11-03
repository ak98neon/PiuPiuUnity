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

        if (Physics.Raycast(r, out hitInfo, 5))
        {
            if (hitInfo.transform.CompareTag("Pistol") || hitInfo.transform.CompareTag("Rifle"))
            {
                isCanTakeItem = true;
                if (Input.GetKeyDown(KeyCode.F))
                {
                    isCanTakeItem = false;

                    for (int i = 0; i < playerHand.transform.childCount; i++)
                    {
                        Destroy(playerHand.transform.GetChild(i).gameObject);
                    }
                    GameObject weapon = hitInfo.transform.gameObject;
                    weapon.transform.parent = playerHand.transform;
                    weapon.transform.localPosition = new Vector3(0, -0.135f, 0);
                    weapon.transform.localRotation = Quaternion.identity;

                    Shooting shotComponent = GetComponent<Shooting>();
                    if (hitInfo.transform.CompareTag("Pistol"))
                    {
                        shotComponent.CurrentGun = Shooting.CurrentWeapon.pistol;
                    } else
                    {
                        shotComponent.CurrentGun = Shooting.CurrentWeapon.rifle;
                    }
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
            guiStyle.normal.textColor = Color.white;
            guiStyle.fontSize = 20;
            GUI.Label(new Rect(playerCamera.pixelWidth / 2, playerCamera.pixelHeight / 2 + 200, 150, 30), "For take push F", guiStyle);
        }
    }
}
