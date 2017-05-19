using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;

public class HexCell : MonoBehaviour {

	public HexCoordinates coordinates;

	public RectTransform uiRect;

	public HexGridChunk chunk;

    public Season Season {
        set
        {
            if(Elevation <= 2) {
                switch (value)
                {
                    case Season.Spring:
                        if (WaterCount > 0) TerrainTypeIndex = 1; else TerrainTypeIndex = 0;
                        break;
                    case Season.Summer:
                        if (WaterCount > 0) TerrainTypeIndex = 1; else TerrainTypeIndex = 0;
                        break;
                    case Season.Fall:
                        TerrainTypeIndex = 3;
                        break;
                    case Season.Winter:
                        TerrainTypeIndex = 4;
                        break;
                }
            }
        }
    }

	public int Elevation {
		get {
			return elevation;
		}
		set {
			if (elevation == value) {
				return;
			}
			elevation = value;
			RefreshPosition();
			ValidateRivers();

			for (int i = 0; i < roads.Length; i++) {
				if (roads[i] && GetElevationDifference((HexDirection)i) > 1) {
					SetRoad(i, false);
				}
			}

            if (Elevation > 4)
                TerrainTypeIndex = 4;
            else if (Elevation > 2)
                TerrainTypeIndex = 2;
            else
                Irrigate();

            Refresh();
		}
	}


	public int WaterLevel {
		get {
			return waterLevel;
		}
		set {
			if (waterLevel == value) {
				return;
			}
			waterLevel = value;
			ValidateRivers();
			Refresh();
            Irrigate();
        }
	}

	public bool IsUnderwater {
		get {
			return waterLevel > elevation;
		}
	}



	public bool HasIncomingRiver {
		get {
			return hasIncomingRiver;
		}
	}

	public bool HasOutgoingRiver {
		get {
			return hasOutgoingRiver;
		}
	}

	public bool HasRiver {
		get {
			return hasIncomingRiver || hasOutgoingRiver;
		}
	}

	public bool HasRiverBeginOrEnd {
		get {
			return hasIncomingRiver != hasOutgoingRiver;
		}
	}

	public HexDirection RiverBeginOrEndDirection {
		get {
			return hasIncomingRiver ? incomingRiver : outgoingRiver;
		}
	}

	public bool HasRoads {
		get {
			for (int i = 0; i < roads.Length; i++) {
				if (roads[i]) {
					return true;
				}
			}
			return false;
		}
	}

	public HexDirection IncomingRiver {
		get {
			return incomingRiver;
		}
	}

	public HexDirection OutgoingRiver {
		get {
			return outgoingRiver;
		}
	}

	public Vector3 Position {
		get {
			return transform.localPosition;
		}
	}

    public Population CellPopulation {
    get {
            return GetComponent<Population>();
        }
    }

	public float StreamBedY {
		get {
			return
				(elevation + HexMetrics.streamBedElevationOffset) *
				HexMetrics.elevationStep;
		}
	}

	public float RiverSurfaceY {
		get {
			return
				(elevation + HexMetrics.waterElevationOffset) *
				HexMetrics.elevationStep;
		}
	}

	public float WaterSurfaceY {
		get {
			return
				(waterLevel + HexMetrics.waterElevationOffset) *
				HexMetrics.elevationStep;
		}
	}

	public int UrbanLevel {
		get {
			return urbanLevel;
		}
		set {
			if (urbanLevel != value) {
				urbanLevel = value;
				RefreshSelfOnly();
			}
		}
	}

	public int FarmLevel {
		get {
			return farmLevel;
		}
		set {
			if (farmLevel != value) {
				farmLevel = value;
				RefreshSelfOnly();
			}
		}
	}

    public float WaterCount
    {
        get { return waterCount; }
        set { waterCount = value;
            if ((waterCount > 0 || HasRiver) && Elevation <= 2)
            {
                terrainTypeIndex = 1;
            }
            else if (Elevation <= 2)
            {
                terrainTypeIndex = 0;
            }
            if (terrainTypeIndex == 1)
                PlantLevel = Mathf.Min((int)waterCount, PlantThreshold);
        }
    }

	public int PlantLevel {
		get {
			return plantLevel;
		}
		set {
			if (plantLevel != value) {
				plantLevel = value;
				RefreshSelfOnly();
			}
		}
	}

	public int SpecialIndex {
		get {
			return specialIndex;
		}
		set {
			if (specialIndex != value && !HasRiver) {
				specialIndex = value;
				RemoveRoads();
				RefreshSelfOnly();
			}
		}
	}

	public bool IsSpecial {
		get {
			return specialIndex > 0;
		}
	}

	public bool Walled {
		get {
			return walled;
		}
		set {
			if (walled != value) {
				walled = value;
				Refresh();
			}
		}
	}

	public int TerrainTypeIndex {
		get {
			return terrainTypeIndex;
		}
		set {
			if (terrainTypeIndex != value) {
				terrainTypeIndex = value;
			}
		}
	}

    public int PlantThreshold
    {
        get
        {
            if (terrainTypeIndex == 4) { return 1; }
            else if (terrainTypeIndex == 2) return 2;
            else return 3;
        }
    }

	public int Distance {
		get {
			return distance;
		}
		set {
			distance = value;
			UpdateDistanceLabel();
		}
	}

    public int Sustainability
    {
        get
        {
            int sus = 0;
            if (IsUnderwater)
                return 0;
            for(int i=0; i<neighbors.Length; i++)
            {
                if (neighbors[i])
                {
                    if(GetEdgeType(neighbors[i]) != HexEdgeType.Slope && !neighbors[i].IsUnderwater)
                    {
                        sus++;
                    }
                }
            }
            sus += (int) (2 * plantLevel + farmLevel - 2 * urbanLevel);
            if(CellPopulation.Count > 0)
            sus -= CellPopulation.Count / 50;
            return sus;
        }
    }

    public int Security
    {
        get
        {

            if (IsUnderwater)
                return 0;
            int s = 0;
            for(int i = 0; i<neighbors.Length; i++)
            {
                if (neighbors[i])
                {
                    HexCell n = neighbors[i];
                    if (walled && n.walled)
                        s++;
                    if (n.elevation > elevation)
                        s++;
                    if (n.HasRiver && !HasRiver)
                        s++;
                }
            }
            return s;
        }
    }

    public int RawDesirability
    {
        get
        {
            return Sustainability + Security;
        }
    }

    public int Desirability(float[] desirabilityFactors)
    {
        return (int) (desirabilityFactors[0] * Sustainability + desirabilityFactors[1] * Security);
    }

	int terrainTypeIndex;

	int elevation = int.MinValue;
    
    int waterLevel;
    int plantThreshold;
    float waterCount;

    int settlingUrban = 0;
    int settlingRural = 0;

	int urbanLevel, farmLevel, plantLevel;

	int specialIndex;

	int distance;

	bool walled;

	bool hasIncomingRiver, hasOutgoingRiver;
	HexDirection incomingRiver, outgoingRiver;

	[SerializeField]
	HexCell[] neighbors;

	[SerializeField]
	bool[] roads;

	public HexCell GetNeighbor (HexDirection direction) {
		return neighbors[(int)direction];
	}

	public void SetNeighbor (HexDirection direction, HexCell cell) {
		neighbors[(int)direction] = cell;
		cell.neighbors[(int)direction.Opposite()] = this;
	}

	public HexEdgeType GetEdgeType (HexDirection direction) {
		return HexMetrics.GetEdgeType(
			elevation, neighbors[(int)direction].elevation
		);
	}

	public HexEdgeType GetEdgeType (HexCell otherCell) {
		return HexMetrics.GetEdgeType(
			elevation, otherCell.elevation
		);
	}

    public void Urbanize()
    {
            UrbanLevel++;
        if(CellPopulation.dFactor(1) > 0.5 && !Walled)
        {
            Walled = true;
        }
            if (plantLevel == 0)
                farmLevel--;
            else
                plantLevel--;
        
    }

    public void BuildFarms()
    {
        FarmLevel++;
        PlantLevel--;
    }

    public void Reclaim()
    {
        UrbanLevel--;
        FarmLevel--;
        PlantLevel++;
    }

	public bool HasRiverThroughEdge (HexDirection direction) {
		return
			hasIncomingRiver && incomingRiver == direction ||
			hasOutgoingRiver && outgoingRiver == direction;
	}

	public void RemoveIncomingRiver () {
		if (!hasIncomingRiver) {
			return;
		}
		hasIncomingRiver = false;
		RefreshSelfOnly();

		HexCell neighbor = GetNeighbor(incomingRiver);
		neighbor.hasOutgoingRiver = false;
		neighbor.RefreshSelfOnly();
	}

	public void RemoveOutgoingRiver () {
		if (!hasOutgoingRiver) {
			return;
		}
		hasOutgoingRiver = false;
		RefreshSelfOnly();

		HexCell neighbor = GetNeighbor(outgoingRiver);
		neighbor.hasIncomingRiver = false;
		neighbor.RefreshSelfOnly();
	}

	public void RemoveRiver () {
		RemoveOutgoingRiver();
		RemoveIncomingRiver();
	}

    public void Tick()
    {
        //If populated...
        if (CellPopulation.Count > 0)
        {
            CellPopulation.Tick();
            if (CellPopulation.Count < Sustainability * WorldMetrics.popRuralMultiplier)
            {
                settlingRural++;
                if(settlingRural >= WorldMetrics.popStepsRequired)
                {
                    if(PlantLevel > 0)
                    {
                        BuildFarms();
                    }
                    settlingRural = 0;
                }
            }
            if (CellPopulation.Count < (Sustainability) * WorldMetrics.popUrbanMultiplier)
            { settlingUrban++;
            if(settlingUrban >= WorldMetrics.popStepsRequired)
                {
                    if(PlantLevel > 0 || FarmLevel > 0)
                    {
                        Urbanize();
                    }
                    settlingUrban = 0;
                }
            }
        }
        if(CellPopulation.Count > (Sustainability) * (WorldMetrics.popUrbanMultiplier + WorldMetrics.popRuralMultiplier))
        {
            //Do stuff?
        }

    }

	public void SetOutgoingRiver (HexDirection direction) {
		if (hasOutgoingRiver && outgoingRiver == direction) {
			return;
		}

		HexCell neighbor = GetNeighbor(direction);
		if (!IsValidRiverDestination(neighbor)) {
			return;
		}

		RemoveOutgoingRiver();
		if (hasIncomingRiver && incomingRiver == direction) {
			RemoveIncomingRiver();
		}
		hasOutgoingRiver = true;
		outgoingRiver = direction;
		specialIndex = 0;

		neighbor.RemoveIncomingRiver();
		neighbor.hasIncomingRiver = true;
		neighbor.incomingRiver = direction.Opposite();
		neighbor.specialIndex = 0;

        Irrigate();

        foreach(HexCell c in neighbors)
        {
            if(c)
            c.Irrigate();
        }

		SetRoad((int)direction, false);
	}

	public bool HasRoadThroughEdge (HexDirection direction) {
		return roads[(int)direction];
	}

    public void Irrigate()
    {
        WaterCount = 0;
        foreach (HexCell c in neighbors)
        {
            if (c) {
                if (c.IsUnderwater)
                {
                        WaterCount++;
                }
                else if (Mathf.Abs(c.Elevation - Elevation) < 2)
                {
                    if (c.WaterLevel > c.Elevation)
                        WaterCount++;
                    if (c.HasRiver)
                        WaterCount += 0.5f;
                }

            }
            
        }
    }

	public void AddRoad (HexDirection direction) {
		if (
			!roads[(int)direction] && !HasRiverThroughEdge(direction) &&
			!IsSpecial && !GetNeighbor(direction).IsSpecial &&
			GetElevationDifference(direction) <= 1
		) {
			SetRoad((int)direction, true);
		}
	}

	public void RemoveRoads () {
		for (int i = 0; i < neighbors.Length; i++) {
			if (roads[i]) {
				SetRoad(i, false);
			}
		}
	}

	public int GetElevationDifference (HexDirection direction) {
		int difference = elevation - GetNeighbor(direction).elevation;
		return difference >= 0 ? difference : -difference;
	}

	bool IsValidRiverDestination (HexCell neighbor) {
		return neighbor && (
			elevation >= neighbor.elevation || waterLevel == neighbor.elevation
		);
	}

	void ValidateRivers () {
		if (
			hasOutgoingRiver &&
			!IsValidRiverDestination(GetNeighbor(outgoingRiver))
		) {
			RemoveOutgoingRiver();
		}
		if (
			hasIncomingRiver &&
			!GetNeighbor(incomingRiver).IsValidRiverDestination(this)
		) {
			RemoveIncomingRiver();
		}
	}

	void SetRoad (int index, bool state) {
		roads[index] = state;
		neighbors[index].roads[(int)((HexDirection)index).Opposite()] = state;
		neighbors[index].RefreshSelfOnly();
		RefreshSelfOnly();
	}

	void RefreshPosition () {
		Vector3 position = transform.localPosition;
		position.y = elevation * HexMetrics.elevationStep;
		position.y +=
			(HexMetrics.SampleNoise(position).y * 2f - 1f) *
			HexMetrics.elevationPerturbStrength;
		transform.localPosition = position;

		Vector3 uiPosition = uiRect.localPosition;
		uiPosition.z = -position.y;
		uiRect.localPosition = uiPosition;
	}

	void Refresh () {
		if (chunk) {
			chunk.Refresh();
			for (int i = 0; i < neighbors.Length; i++) {
				HexCell neighbor = neighbors[i];
				if (neighbor != null && neighbor.chunk != chunk) {
					neighbor.chunk.Refresh();
				}
			}
		}
	}

	void RefreshSelfOnly () {
		chunk.Refresh();
	}

	public void Save (BinaryWriter writer) {
		writer.Write((byte)terrainTypeIndex);
		writer.Write((byte)elevation);
		writer.Write((byte)waterLevel);
		writer.Write((byte)urbanLevel);
		writer.Write((byte)farmLevel);
		writer.Write((byte)plantLevel);
		writer.Write((byte)specialIndex);
		writer.Write(walled);

		if (hasIncomingRiver) {
			writer.Write((byte)(incomingRiver + 128));
		}
		else {
			writer.Write((byte)0);
		}

		if (hasOutgoingRiver) {
			writer.Write((byte)(outgoingRiver + 128));
		}
		else {
			writer.Write((byte)0);
		}

		int roadFlags = 0;
		for (int i = 0; i < roads.Length; i++) {
			if (roads[i]) {
				roadFlags |= 1 << i;
			}
		}
		writer.Write((byte)roadFlags);
	}

	public void Load (BinaryReader reader) {
		terrainTypeIndex = reader.ReadByte();
		elevation = reader.ReadByte();
		RefreshPosition();
		waterLevel = reader.ReadByte();
		urbanLevel = reader.ReadByte();
		farmLevel = reader.ReadByte();
		plantLevel = reader.ReadByte();
		specialIndex = reader.ReadByte();
		walled = reader.ReadBoolean();

		byte riverData = reader.ReadByte();
		if (riverData >= 128) {
			hasIncomingRiver = true;
			incomingRiver = (HexDirection)(riverData - 128);
		}
		else {
			hasIncomingRiver = false;
		}

		riverData = reader.ReadByte();
		if (riverData >= 128) {
			hasOutgoingRiver = true;
			outgoingRiver = (HexDirection)(riverData - 128);
		}
		else {
			hasOutgoingRiver = false;
		}

		int roadFlags = reader.ReadByte();
		for (int i = 0; i < roads.Length; i++) {
			roads[i] = (roadFlags & (1 << i)) != 0;
		}
	}

	void UpdateDistanceLabel () {
		UnityEngine.UI.Text label = uiRect.GetComponent<Text>();
		label.text = distance == int.MaxValue ? "" : distance.ToString();
	}
}