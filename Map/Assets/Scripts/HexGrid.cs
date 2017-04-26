using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class HexGrid : MonoBehaviour {

	public int cellCountX = 20, cellCountZ = 15;

	public HexCell cellPrefab;
	public Text cellLabelPrefab;
	public HexGridChunk chunkPrefab;

	public Texture2D noiseSource;

	public int seed;

	HexGridChunk[] chunks;
	HexCell[] cells;

	int chunkCountX, chunkCountZ;
    int randomThreshold = 25;
    int[] waterHeights = { 0, 1, 1, 2, 2 };

	void Awake () {
		HexMetrics.noiseSource = noiseSource;
		HexMetrics.InitializeHashGrid(seed);
		CreateMap(cellCountX, cellCountZ);
	}

	public bool CreateMap (int x, int z) {
		if (
			x <= 0 || x % HexMetrics.chunkSizeX != 0 ||
			z <= 0 || z % HexMetrics.chunkSizeZ != 0
		) {
			Debug.LogError("Unsupported map size.");
			return false;
		}

		if (chunks != null) {
			for (int i = 0; i < chunks.Length; i++) {
				Destroy(chunks[i].gameObject);
			}
		}

		cellCountX = x;
		cellCountZ = z;
		chunkCountX = cellCountX / HexMetrics.chunkSizeX;
		chunkCountZ = cellCountZ / HexMetrics.chunkSizeZ;
		CreateChunks();
		CreateCells();
        CreateWater();
        CreateRivers();

        IrrigateGrid();
		return true;
	}

	void CreateChunks () {
		chunks = new HexGridChunk[chunkCountX * chunkCountZ];

		for (int z = 0, i = 0; z < chunkCountZ; z++) {
			for (int x = 0; x < chunkCountX; x++) {
				HexGridChunk chunk = chunks[i++] = Instantiate(chunkPrefab);
				chunk.transform.SetParent(transform);
			}
		}
	}

	void CreateCells () {
		cells = new HexCell[cellCountZ * cellCountX];

		for (int z = 0, i = 0; z < cellCountZ; z++) {
			for (int x = 0; x < cellCountX; x++) {
				CreateCell(x, z, i++);
			}
		}
	}

	void OnEnable () {
		if (!HexMetrics.noiseSource) {
			HexMetrics.noiseSource = noiseSource;
			HexMetrics.InitializeHashGrid(seed);
		}
	}

	public HexCell GetCell (Vector3 position) {
		position = transform.InverseTransformPoint(position);
		HexCoordinates coordinates = HexCoordinates.FromPosition(position);
		int index =
			coordinates.X + coordinates.Z * cellCountX + coordinates.Z / 2;
		return cells[index];
	}

	public HexCell GetCell (HexCoordinates coordinates) {
		int z = coordinates.Z;
		if (z < 0 || z >= cellCountZ) {
			return null;
		}
		int x = coordinates.X + z / 2;
		if (x < 0 || x >= cellCountX) {
			return null;
		}
		return cells[x + z * cellCountX];
	}

	public void ShowUI (bool visible) {
		for (int i = 0; i < chunks.Length; i++) {
			chunks[i].ShowUI(visible);
		}
	}

	void CreateCell (int x, int z, int i) {
		Vector3 position;
		position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
		position.y = 0f;
		position.z = z * (HexMetrics.outerRadius * 1.5f);

		HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);
		cell.transform.localPosition = position;
		cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);

		if (x > 0) {
			cell.SetNeighbor(HexDirection.W, cells[i - 1]);
		}
		if (z > 0) {
			if ((z & 1) == 0) {
				cell.SetNeighbor(HexDirection.SE, cells[i - cellCountX]);
				if (x > 0) {
					cell.SetNeighbor(HexDirection.SW, cells[i - cellCountX - 1]);
				}
			}
			else {
				cell.SetNeighbor(HexDirection.SW, cells[i - cellCountX]);
				if (x < cellCountX - 1) {
					cell.SetNeighbor(HexDirection.SE, cells[i - cellCountX + 1]);
				}
			}
		}

		Text label = Instantiate<Text>(cellLabelPrefab);
		label.rectTransform.anchoredPosition =
			new Vector2(position.x, position.z);
		cell.uiRect = label.rectTransform;

        cell.Elevation = waterHeights[Random.Range(0, waterHeights.Length)];
        if(Random.Range(0f, 100f) <= randomThreshold) {
            cell.Elevation += Random.Range(1, 5);
        }

		AddCellToChunk(x, z, cell);
	}

	void AddCellToChunk (int x, int z, HexCell cell) {
		int chunkX = x / HexMetrics.chunkSizeX;
		int chunkZ = z / HexMetrics.chunkSizeZ;
		HexGridChunk chunk = chunks[chunkX + chunkZ * chunkCountX];

		int localX = x - chunkX * HexMetrics.chunkSizeX;
		int localZ = z - chunkZ * HexMetrics.chunkSizeZ;
		chunk.AddCell(localX + localZ * HexMetrics.chunkSizeX, cell);
	}

    void CreateWater()
    {
        for(int i=0; i < cells.Length; i++)
        {
            HexCell cell = cells[i];
            int minElevation = 6;
            for(int j=0; j < 6; j++)
            {
                if(cell.GetNeighbor((HexDirection)j)) {
                    minElevation = Mathf.Min(minElevation, cell.GetNeighbor((HexDirection)j).Elevation);
                }
            }
                cell.WaterLevel = Mathf.Max(1, minElevation);
        }
    }

    void CreateRivers()
    {
        List<HexCell> riverOrigins = new List<HexCell>();
        for(int i=0; i < cells.Length; i++)
        {
            if (cells[i].IsUnderwater)
            {
                riverOrigins.Add(cells[i]);
            }
        }
        for(int i=0; i < riverOrigins.Count; i++)
        {
            CreateRiver(riverOrigins[i], riverOrigins[i].WaterLevel);
        }
    }

    void CreateRiver(HexCell cell, int height)
    {
        List<HexDirection> candidateDirections = new List<HexDirection>();
        for(HexDirection i = HexDirection.NE; i < HexDirection.NW; i++)
        {
            if (cell.GetNeighbor(i)) {
                HexCell candidate = cell.GetNeighbor(i);
                if (candidate.Elevation <= height && !candidate.HasRiver && !(cell.IsUnderwater && candidate.IsUnderwater))
                    candidateDirections.Add(i);
            }
            
        }
        if(candidateDirections.Count > 0)
        {
            HexDirection riverDirection = candidateDirections[Random.Range(0, candidateDirections.Count)];
            cell.SetOutgoingRiver(riverDirection);
            if(!cell.GetNeighbor(riverDirection).IsUnderwater)
            CreateRiver(cell.GetNeighbor(riverDirection), cell.GetNeighbor(riverDirection).Elevation) ;
        }
    }

    void IrrigateGrid()
    {
        for (int i = 0; i < cells.Length; i++)
        {
            cells[i].Irrigate();
        }
    }

	public void Save (BinaryWriter writer) {
		writer.Write(cellCountX);
		writer.Write(cellCountZ);

		for (int i = 0; i < cells.Length; i++) {
			cells[i].Save(writer);
		}
	}

	public void Load (BinaryReader reader, int header) {
		StopAllCoroutines();
		int x = 20, z = 15;
		if (header >= 1) {
			x = reader.ReadInt32();
			z = reader.ReadInt32();
		}
		if (x != cellCountX || z != cellCountZ) {
			if (!CreateMap(x, z)) {
				return;
			}
		}

		for (int i = 0; i < cells.Length; i++) {
			cells[i].Load(reader);
		}
		for (int i = 0; i < chunks.Length; i++) {
			chunks[i].Refresh();
		}
	}

	public void FindDistancesTo (HexCell cell) {
		StopAllCoroutines();
		StartCoroutine(Search(cell));
	}
	IEnumerator Search (HexCell cell) {
		for (int i = 0; i < cells.Length; i++) {
			cells[i].Distance = int.MaxValue;
		}

		WaitForSeconds delay = new WaitForSeconds(1 / 60f);
		List<HexCell> frontier = new List<HexCell>();
		cell.Distance = 0;
        HexCell previous;
		frontier.Add(cell);
		while (frontier.Count > 0) {
			yield return delay;
			HexCell current = frontier[0];
			frontier.RemoveAt(0);
			for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++) {
				HexCell neighbor = current.GetNeighbor(d);
				if (neighbor == null) {
					continue;
				}
				if (neighbor.IsUnderwater) {
					continue;
				}
				HexEdgeType edgeType = current.GetEdgeType(neighbor);
				if (edgeType == HexEdgeType.Cliff) {
					continue;
				}
                if (neighbor.HasRiver && !(neighbor.HasRoadThroughEdge(d.Opposite()) && current.HasRoadThroughEdge(d)))
                {
                    continue;
                }
				int distance = current.Distance;
				if (current.HasRoadThroughEdge(d)) {
					distance += 1;
				}
				else if (current.Walled != neighbor.Walled) {
					continue;
				}
				else {
					distance += edgeType == HexEdgeType.Flat ? 5 : 10;
					distance += neighbor.UrbanLevel + neighbor.FarmLevel +
						neighbor.PlantLevel;
				}
				if (neighbor.Distance == int.MaxValue) {
					neighbor.Distance = distance;
					frontier.Add(neighbor);
				}
				else if (distance < neighbor.Distance) {
					neighbor.Distance = distance;
				}
				frontier.Sort((x, y) => x.Distance.CompareTo(y.Distance));
			}
            previous = current;
		}
	}
}