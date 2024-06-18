using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct Stratagem
{
    public string name;
    public string code;
    public GameObject prefab;
}

public class PlayerStratagemMenu : MonoBehaviour
{
    [Header("Visuals")]
    public float equipSpeed = 12;
    public Transform menuParent;
    public GameObject screenVisual;
    public GameObject throwableVisual;
    public Text inputText;

    [Header("Stratagems")]
    public Stratagem[] stratagemList;
    public GameObject stratagemBallPrefab;
    public float throwForce = 5;

    string currentCode;
    bool waitForNextInput = false;
    public int selectedStratagem = -1;

    void Start()
    {
        menuParent.localEulerAngles = new Vector3(90, 0, 0);
    }

    void Update()
    {
        // Only operate menu if player is holding menu button.
        if (Inputs.ctrl && selectedStratagem == -1)
        {
            // Set active state of visuals.
            screenVisual.SetActive(true);
            throwableVisual.SetActive(false);

            // Properly rotate menu to face camera.
            menuParent.localRotation = Quaternion.Slerp(menuParent.localRotation, Quaternion.identity, Time.deltaTime * equipSpeed);

            // Get player input and act accordingly.
            Vector2 playerInput = Inputs.arrowKeys;
            if (playerInput.magnitude > 0)
            {
                // Only let player input a code after they let go of the previous input.
                // This prevents duplicate inputs from one button press.
                if (!waitForNextInput)
                {
                    if (playerInput.y > 0) // Up
                        currentCode += 1;
                    else if (playerInput.x > 0) // Right
                        currentCode += 2;
                    else if (playerInput.y < 0) // Down
                        currentCode += 3;
                    else if (playerInput.x < 0) // Left
                        currentCode += 4;

                    waitForNextInput = true;
                }
            }
            else
            {
                // Stop waiting for a new input once player stops pressing any of the arrow buttons.
                waitForNextInput = false;
            }

            for (int i = 0; i < stratagemList.Length; i++)
            {
                if (stratagemList[i].code == currentCode)
                {
                    selectedStratagem = i;
                }
            }
        }
        else if(selectedStratagem != -1)
        {
            // Set active state of visuals.
            screenVisual.SetActive(false);
            throwableVisual.SetActive(true);
            currentCode = "";

            if (Inputs.attack)
            {
                GameObject obj = Instantiate(stratagemBallPrefab, throwableVisual.transform.position, Quaternion.identity);
                obj.transform.forward = PlayerMovement.instance.cam.forward;

                StratagemBall ballScript = obj.GetComponent<StratagemBall>();
                ballScript.velocity = obj.transform.forward * throwForce;
                ballScript.spawnedPrefab = stratagemList[selectedStratagem].prefab;

                selectedStratagem = -1;
                menuParent.localRotation = Quaternion.Euler(90, 0, 0);
            }
        }
        else
        {
            // Reset the code and menu rotation if player is not holding down the menu button.
            menuParent.localRotation = Quaternion.Euler(90, 0, 0);
            currentCode = "";
            selectedStratagem = -1;
        }

        DisplayInputs();
    }

    void DisplayInputs()
    {
        inputText.text = "";

        for (int i = 0; i < currentCode.Length; i++)
        {
            switch (currentCode[i])
            {
                case '1':
                    inputText.text += @"/\ ";
                    break;
                case '2':
                    inputText.text += "> ";
                    break;
                case '3':
                    inputText.text += @"\/ ";
                    break;
                case '4':
                    inputText.text += "< ";
                    break;
            }
        }
    }
}
