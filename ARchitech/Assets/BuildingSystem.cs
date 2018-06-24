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
    private bool destroyModeOn = false;
    private bool canBuild = false;
    private bool canDestroy = false;

    private BlockSystem bSys;

    [SerializeField]
    private LayerMask buildableSurfacesLayer;

    [SerializeField]
    private LayerMask destroyableSurfacesLayer;

    private Vector3 buildPos;
    private GameObject blockToDestroy;

    private GameObject currentTemplateBlock; // variable to store current template block

    [SerializeField]
    private GameObject modeToggleButton;
    [SerializeField]
    private GameObject placeButton;

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
        destroyModeOn = false;
    }

    private void Update()
    {
        // Toggle Build and Destroy Mode
        if (CrossPlatformInputManager.GetButtonDown("ModeToggle"))
        {
            buildModeOn = !buildModeOn;
            destroyModeOn = !destroyModeOn;
        }

        // Print Current Mode
        if (buildModeOn && !destroyModeOn)
        {
            modeToggleButton.GetComponentInChildren<Text>().text = "Build Mode";
            placeButton.GetComponentInChildren<Text>().text = "Place";
            buildMode();
        }
        else if (!buildModeOn && destroyModeOn)
        {
            modeToggleButton.GetComponentInChildren<Text>().text = "Destroy Mode";
            placeButton.GetComponentInChildren<Text>().text = "Destroy";
            destroyMode();
        }


    }


    private void buildMode()
    {
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
            if (CrossPlatformInputManager.GetButtonDown("Place"))
            {
                placeBlock();
            }
        }

        // Change Block when "Change Button is Clicked"
        if (CrossPlatformInputManager.GetButtonDown("Change"))
        {
            changeBlock();
        }
    }

    private void changeBlock()
    {
        blockSelectCounter++;
        if (blockSelectCounter >= bSys.allBlocks.Count)
        {
            blockSelectCounter = 0;
        }
    }

    // Create Block
    private void placeBlock()
    {
        GameObject newBlock = Instantiate(blockPrefab, buildPos, Quaternion.identity); // Create new block object
        Block tempBlock = bSys.allBlocks[blockSelectCounter]; // Spawn new block from dictionary
        newBlock.name = tempBlock.blockName + "-Block";
        newBlock.GetComponent<MeshRenderer>().material = tempBlock.blockMaterial;
    }

    private void destroyMode()
    {
        // ray cast if destroy mode is true
        if (destroyModeOn)
        {
            RaycastHit buildPosHit;
            if (Physics.Raycast(playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0)), out buildPosHit, 100, destroyableSurfacesLayer)) // raycast on the buildable surfaces
            {
                Vector3 point = buildPosHit.point;
                buildPos = new Vector3(Mathf.Round(point.x) , Mathf.Round(point.y) + 0.8f, Mathf.Round(point.z));
                blockToDestroy = buildPosHit.transform.gameObject;
                print("BLOCK " + blockToDestroy);
                canDestroy = true;
            }
            else
            {
                Destroy(currentTemplateBlock.gameObject);
                canDestroy = false;
            }

            Debug.Log(canDestroy);
        }

        //// destroy block when destroy mode is off and template is still present. make sure that the block cannot be build without build mode 
        //if (!destroyModeOn && currentTemplateBlock != null)
        //{
        //    Destroy(currentTemplateBlock.gameObject);
        //    canDestroy = false;
        //}

        //// create currenttemplateblock when canDestroy is on 
        //if (canDestroy && currentTemplateBlock == null)
        //{
        //    currentTemplateBlock = Instantiate(blockTemplatePrefab, buildPos, Quaternion.identity); // creates template block
        //    currentTemplateBlock.GetComponent<MeshRenderer>().material = templateMaterial; // create template block and update the material
        //}

        // building part of system
        if (canDestroy)
        {
            //currentTemplateBlock.transform.position = buildPos;
            if (CrossPlatformInputManager.GetButtonDown("Place"))
            {   
                destroyblock();
            }
        }
    }

    // destroy block
    private void destroyblock()
    {
        if (blockToDestroy.name == "Ground")
        {
            Debug.Log(message: "ITS A GROUND NO DELETE");
        }
        else
        {
            Debug.Log(message: "ITS A HIT");
            Destroy(blockToDestroy);
        }
    }
}


