using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        
        if (Input.GetKeyDown("e"))
        {

            // Remove Cursor when in build mode
            if (buildModeOn)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
            }
        }

        if (Input.GetKeyDown("r")) {
            blockSelectCounter++;
            if (blockSelectCounter >= bSys.allBlocks.Count) {
                blockSelectCounter = 0;
            }
        }

        if (buildModeOn)
        {   
    
            RaycastHit buildPosHit;
            if (Physics.Raycast(playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0)), out buildPosHit, 50, buildableSurfacesLayer))
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

            if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began) // Left mouse click
            {
                PlaceBlock();
            }
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
}


