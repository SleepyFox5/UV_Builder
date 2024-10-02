using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SpawnManagerScriptableObject", order = 1)]
public class UVEditorConfig : ScriptableObject
{
    [SerializeField]
    public Color targetColor;
    [SerializeField]
    public int targetThickness;
    [SerializeField]
    public Color anchorColor;
    [SerializeField]
    public int anchorSize;
    [SerializeField]
    public Color baseColor;
    [SerializeField]
    public int baseSize;
    [SerializeField]
    public Color offsetColor;
    [SerializeField]
    public int offsetSize;
}
