using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GameControl
{
  [System.Serializable]
  public class TilemapSnapshot
  {
    public Vector3Int[] positions;
    public TileBase[] tiles;

    public TilemapSnapshot(Tilemap tilemap)
    {
      List<Vector3Int> allPositions = new();

      foreach (Vector3Int pos in tilemap.cellBounds.allPositionsWithin)
      {
        if (tilemap.HasTile(pos))
          allPositions.Add(pos);
      }
      
      positions = allPositions.ToArray();
      tiles = positions.Select(tilemap.GetTile).ToArray();
    }

    public void ApplyTo(Tilemap tilemap)
    {
      tilemap.ClearAllTiles();
      for (int i = 0; i < positions.Length; i++)
      {
        tilemap.SetTile(positions[i], tiles[i]);
      }
    }
  }
}