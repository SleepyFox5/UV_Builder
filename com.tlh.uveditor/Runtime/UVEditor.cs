#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor.Formats.Fbx.Exporter;

[ExecuteInEditMode]
public class UVEditor : MonoBehaviour
{
    [SerializeField, HideInInspector]
    public List<Vector2> originalUvs;
    [SerializeField, HideInInspector]
    public List<Vector2> scaledUVs;
    [SerializeField, HideInInspector]
    public List<Vector2> originalUv2s;

    [SerializeField]
    public Mesh alteredMesh;

    public UVEditorConfig config;
    public string savePath;
    public int exportIndex;
    public bool showOriginal = true;
    public int originalUvId;
    public Vector2 UVOffsetAmount;
    public Vector2 OriginalUvPosition;
    public Vector2 ScaledOriginalPos;
    public Vector2 UVScale = new Vector2(1,1);

    public Vector2 textureTiling;
    public Vector2 targetTile;

    public Vector2 top, left, bottom, right;

    private void Start()
    {

    }

    private void OnEnable()
    {
        if (originalUvs == null)
            GetUVs();
    }

    public void ChangeUV()
    {
        List<Vector2> newUvs = new List<Vector2>();
        scaledUVs = new List<Vector2>();
        Vector2 UVCenter = new Vector2((left.x + right.x) / 2f, (top.y + bottom.y) / 2f);

        for(int i = 0; i < originalUvs.Count; i++)
        {
            Vector2 centerDist = originalUvs[i] - UVCenter;
            Vector2 scaledPos = UVCenter + (centerDist * UVScale);
            scaledUVs.Add(scaledPos);
            if (i == originalUvId)
                ScaledOriginalPos = scaledPos;
        }

        for (int i = 0; i < scaledUVs.Count; i++)
        {
            newUvs.Add(scaledUVs[i] + UVOffsetAmount);
        }
        if (GetComponent<MeshFilter>() != null)
            alteredMesh = Instantiate(GetComponent<MeshFilter>().sharedMesh);
        else if (GetComponent<SkinnedMeshRenderer>() != null)
        {
            alteredMesh = Instantiate(GetComponent<SkinnedMeshRenderer>().sharedMesh);
        }
        alteredMesh.SetUVs(0, newUvs);
        alteredMesh.SetUVs(1, originalUv2s);
        if (GetComponent<MeshFilter>() != null)
            GetComponent<MeshFilter>().sharedMesh = alteredMesh;
        else if (GetComponent<SkinnedMeshRenderer>() != null)
            GetComponent<SkinnedMeshRenderer>().sharedMesh = alteredMesh;
    }

    public void SetUVTile()
    {
        UVOffsetAmount = (Vector2.one / textureTiling) * targetTile;
        ChangeUV();
    }

    public void GetUVs()
    {
        // Reset the UVs before getting references if they are set
        UVOffsetAmount = new Vector2(0, 0);
        UVScale = new Vector2(1, 1);
        if (originalUvs != null)
            ChangeUV();

        originalUvs = new List<Vector2>();
        originalUv2s = new List<Vector2>();
        if (GetComponent<MeshFilter>() != null)
        {
            GetComponent<MeshFilter>().sharedMesh.GetUVs(0, originalUvs);
            GetComponent<MeshFilter>().sharedMesh.GetUVs(1, originalUv2s);
        }
        else if (GetComponent<SkinnedMeshRenderer>() != null)
        {
            GetComponent<SkinnedMeshRenderer>().sharedMesh.GetUVs(0, originalUvs);
            GetComponent<SkinnedMeshRenderer>().sharedMesh.GetUVs(1, originalUv2s);
        }
        if (originalUvs.Count > 0)
        {
            top = new Vector2(0, 0);
            left = new Vector2(1, 0);
            right = new Vector2(0, 1);
            bottom = new Vector2(0, 1);
            for (int i = 0; i < originalUvs.Count; i++)
            {
                if (originalUvs[i].y > top.y)
                {
                    top = originalUvs[i];
                    originalUvId = i;
                }
                if (originalUvs[i].y < bottom.y)
                    bottom = originalUvs[i];
                if (originalUvs[i].x > right.x)
                    right = originalUvs[i];
                if (originalUvs[i].x < left.x)
                    left = originalUvs[i];
            }
            OriginalUvPosition = top;
        }

    }

    public void ExportObject()
    {
        string exportPath = Path.Combine(savePath, gameObject.name + exportIndex + ".fbx");
        exportIndex++;

        ExportModelOptions exportSettings = new ExportModelOptions();
        exportSettings.ExportFormat = ExportFormat.Binary;
        exportSettings.KeepInstances = false;

        string realPath = ModelExporter.ExportObject(exportPath, gameObject, exportSettings);
    }

    public Texture GetTexture()
    {
        if (GetComponent<MeshRenderer>() != null)
            return GetComponent<MeshRenderer>().sharedMaterial.mainTexture;
        else
            return GetComponent<SkinnedMeshRenderer>().sharedMaterial.mainTexture;
    }


}
#endif

