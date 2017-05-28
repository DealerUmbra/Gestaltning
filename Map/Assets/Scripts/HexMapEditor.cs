using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;
public class HexMapEditor : MonoBehaviour {

	public HexGrid hexGrid;

	public Material terrainMaterial;

    public Texture2D createCursor;
    public Texture2D destroyCursor;

	int brushSize;


	bool editMode = false;

    bool isCreate = false;

	bool applyUrbanLevel, applyFarmLevel, applyPlantLevel, applySpecialIndex;

	public enum UseModes {
		Watch, Create, Destroy
	}
    UseModes useMode = UseModes.Watch;

    HexDirection dragDirection;

	public void SetBrushSize (float size) {
		brushSize = (int)size;
	}

    public void SetUseModeF(float mode)
    {
        SetUseMode((UseModes)mode);
    }

    public void SetUseMode(UseModes mode)
    {
        useMode = mode;
        switch (mode)
        {
            case UseModes.Create:
                SetCreateStatus(true);
                Cursor.SetCursor(createCursor, Vector2.zero, CursorMode.Auto);
                SetEditMode(true);
                break;
            case UseModes.Destroy:
                SetCreateStatus(false);
                Cursor.SetCursor(destroyCursor, Vector2.zero, CursorMode.Auto);
                SetEditMode(true);
                break;
            case UseModes.Watch:
                SetEditMode(false);
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                break;
        }
    }

    public void SetCreateStatus(bool status)
    {
        isCreate = status;
    }

	public void SetEditMode (bool toggle) {
		editMode = toggle;
	}


	void Awake () {
		terrainMaterial.DisableKeyword("GRID_ON");
	}

	void Update () {
		if (
			Input.GetMouseButton(0) &&
			!EventSystem.current.IsPointerOverGameObject()
		) {
			HandleInput();
		}
	}

	void HandleInput () {
		Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(inputRay, out hit)) {
			HexCell currentCell = hexGrid.GetCell(hit.point);
			if (editMode) {
				EditCells(currentCell);
			}
		}
	}

	void EditCells (HexCell center) {
		int centerX = center.coordinates.X;
		int centerZ = center.coordinates.Z;

		for (int r = 0, z = centerZ - brushSize; z <= centerZ; z++, r++) {
			for (int x = centerX - r; x <= centerX + brushSize; x++) {
				EditCell(hexGrid.GetCell(new HexCoordinates(x, z)));
			}
		}
		for (int r = 0, z = centerZ + brushSize; z > centerZ; z--, r++) {
			for (int x = centerX - brushSize; x <= centerX + r; x++) {
				EditCell(hexGrid.GetCell(new HexCoordinates(x, z)));
			}
		}
	}

	void EditCell (HexCell cell) {
		if (cell) {

            List<HexCell> neighbors = new List<HexCell>();
            for(int i=0; i < 6; i++)
            {
                if (cell.GetNeighbor((HexDirection)i))
                neighbors.Add(cell.GetNeighbor((HexDirection)i));
            }

            if (isCreate)
            {
                cell.Seed();
            }
            else
            {
                cell.Flame();
            }

		}
	}
}