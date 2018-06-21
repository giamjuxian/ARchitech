using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public class BuildingSystem : MonoBehaviour
{

    [SerializeField]
    private Camera playerCamera;

    private bool buildModeOn = false;
    private bool canBuild = false;

    private BlockSystem bSys;

    [SerializeField]
    private LayerMask buildableSurfacesLayer;

    private Vector3 buildPos;

    private GameObject currentTemplateBlock; // variable to store current template block

    [SerializeField]
    private GameObject modeToggleButton;

    [SerializeField]
    private GameObject blockTemplatePrefab;
    [SerializeField]
    private GameObject blockPrefab;

    [SerializeField]
    private Material templateMaterial;

    private int blockSelectCounter;

    private void Start()
    {
        bSys = GetComponent<BlockSystem>();
        blockSelectCounter = 0;
        buildModeOn = true;
    }

    private void Update()
    {
        // Toggle Build and Destroy Mode
        if (CrossPlatformInputManager.GetButtonDown("ModeToggle"))
        {
            buildModeOn = !buildModeOn;
            // Debug.Log(message: buildModeOn);
        }

        // Print Current Mode
        if (buildModeOn)
        {
            modeToggleButton.GetComponentInChildren<Text>().text = "Build Mode";
        }
        else
        {
            modeToggleButton.GetComponentInChildren<Text>().text = "Destroy Mode";
        }

        // Ray Cast if build mode is true
        if (buildModeOn)
        {
            RaycastHit buildPosHit;
            if (Physics.Raycast(playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0)), out buildPosHit, 100, buildableSurfacesLayer)) // Raycast on the buildable surfaces
            {
                Vector3 point = buildPosHit.point;
                buildPos = new Vector3(Mathf.Round(point.x), Mathf.Round(point.y) + 0.8f, Mathf.Round(point.z));
                canBuild = true;
            }
            else
            {
                Destroy(currentTemplateBlock.gameObject);
                canBuild = false;
            }
        }

        // Destroy block when build mode is off and template is still present. Make sure that the block cannot be build without build mode 
        if (!buildModeOn && currentTemplateBlock != null)
        {
            Destroy(currentTemplateBlock.gameObject);
            canBuild = false;
        }

        // Create currentTemplateBlock when canBuild is on 
        if (canBuild && currentTemplateBlock == null)
        {
            currentTemplateBlock = Instantiate(blockTemplatePrefab, buildPos, Quaternion.identity); // Creates template block
            currentTemplateBlock.GetComponent<MeshRenderer>().material = templateMaterial; // Create template block and update the material
        }

        // Building part of system
        if (canBuild && currentTemplateBlock != null)
        {
            currentTemplateBlock.transform.position = buildPos;

            // if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began) // Left mouse click
            if (CrossPlatformInputManager.GetButtonDown("Place"))
            {
                Debug.Log("CLICKED PLACED");
                PlaceBlock();
            }
        }

        // Change Block when "Change Button is Clicked"
        if (CrossPlatformInputManager.GetButtonDown("Change"))
        {
            changeBlock();
        }
    }

    // Create
    private void PlaceBlock()
    {
        GameObject newBlock = Instantiate(blockPrefab, buildPos, Quaternion.identity); // Create new block object
        Block tempBlock = bSys.allBlocks[blockSelectCounter]; // Spawn new block from dictionary
        newBlock.name = tempBlock.blockName + "-Block";
        newBlock.GetComponent<MeshRenderer>().material = tempBlock.blockMaterial;
    }

    private void changeBlock()
    {
        blockSelectCounter++;
        if (blockSelectCounter >= bSys.allBlocks.Count)
        {
            blockSelectCounter = 0;
        }
    }
}


