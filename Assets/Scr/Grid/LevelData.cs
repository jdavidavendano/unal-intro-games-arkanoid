using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Data/Level Data")]
public class LevelData : ScriptableObject
{
    [Range(1, 10)]
    public int RowCount = 7;
    public float rowSpacing = 0.1f;
    public List<GridRowData> Rows = new List<GridRowData>();
}

[System.Serializable]
public class GridRowData
{
    [Range(1, 24)]
    public int BlockAmount = 7;
    public float blockSpacing = 0.1f;
    public BlockType BlockType = BlockType.Big;
    public BlockColor BlockColor = BlockColor.Green;
}