#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(UVEditor))]

public class UVEditorInspect : Editor
{
    public UVEditorConfig config;
    private Vector2 buttonPos;
    private Vector2 previewPos;
    private static Color anchorColor;
    private static Color targetColor;
    private Texture2D subTex;
    private Rect buttonRect;
    private Rect previewRect;
    private bool wasOnTex;

    public override void OnInspectorGUI()
    {

        UVEditor editor = (UVEditor)target;

        if(GUILayout.Button("Export FBX"))
        {
            editor.ExportObject();
        }

        string filePath = EditorGUILayout.TextField("Save Path", editor.savePath);

        if(filePath != editor.savePath)
        {
            Undo.RecordObject(target, "Changed Save Path");
            editor.savePath = filePath;
        }


        int exportIndex = EditorGUILayout.IntField("Export Index", editor.exportIndex);

        if(exportIndex != editor.exportIndex)
        {
            Undo.RecordObject(target, "Changed Export Index");
            editor.exportIndex = exportIndex;
        }

        if (GUILayout.Button("Get Initial UVs"))
        {
            editor.GetUVs();
        }

        //DrawDefaultInspector();
        Vector2 offestInput = EditorGUILayout.Vector2Field("UV Offset", editor.UVOffsetAmount);

        if (offestInput != editor.UVOffsetAmount)
        {
            Undo.RecordObject(target, "Changed UV Offset");
            editor.UVOffsetAmount = offestInput;
            editor.ChangeUV();
        }

        bool showInput = EditorGUILayout.Toggle("Show Original", editor.showOriginal);

        if(showInput != editor.showOriginal)
        {
            Undo.RecordObject(target, "Changed Show Bool");
            editor.showOriginal = showInput;
        }
        /*
        Vector2 originalInput = EditorGUILayout.Vector2Field("Default UV Origin", editor.OriginalUvPosition);

        if(originalInput != editor.OriginalUvPosition)
        {
            Undo.RecordObject(target, "Changed UV Origin");
            editor.OriginalUvPosition = originalInput;
        }
        */

        Vector2 scaleInput = EditorGUILayout.Vector2Field("UV Scale", editor.UVScale);

        if (scaleInput != editor.UVScale)
        {
            Undo.RecordObject(target, "Changed UV Scale");
            editor.UVScale = scaleInput;
            editor.ChangeUV();
        }

        GUIStyle buttonStyle = new GUIStyle();

        buttonStyle.stretchHeight = false;
        buttonStyle.stretchWidth = false;
        buttonStyle.margin = new RectOffset(10, 10, 10, 10);
        buttonStyle.padding = new RectOffset(0, 0, 0, 0);

        Vector2 texSize = new Vector2(editor.GetTexture().width, editor.GetTexture().height);
        texSize.x = texSize.x > 500 ? 500 : texSize.x;
        texSize.y = texSize.y > 500 ? 500 : texSize.y;


        if (GUILayout.Button(new GUIContent(editor.GetTexture()), buttonStyle ,GUILayout.MinHeight(texSize.y), GUILayout.MinWidth(texSize.x), GUILayout.MaxHeight(texSize.y), GUILayout.MaxWidth(texSize.x)))
        {
            Undo.RecordObject(target, "Changed UV With Image");
            Vector2 mouseClick = Event.current.mousePosition - buttonPos;
            editor.UVOffsetAmount = (new Vector2(mouseClick.x, texSize.y - mouseClick.y) / new Vector2(texSize.x, texSize.y)) - editor.ScaledOriginalPos;
            editor.ChangeUV();
        }
        buttonRect = GUILayoutUtility.GetLastRect();

        subTex = new Texture2D(408, 408);
        Texture2D baseTex = (Texture2D)editor.GetTexture();

        Vector2 targetRatio = editor.ScaledOriginalPos + editor.UVOffsetAmount;

        for (int i = -25; i < 26; i++)
        {
            for (int j = -25; j < 26; j++)
            {
                Color pixel = baseTex.GetPixel((int)(targetRatio.x * baseTex.width) + i, (int)(targetRatio.y * baseTex.height) + j);
                for(int w = 0; w < 8; w++)
                {
                    for(int h = 0; h < 8; h++)
                    {
                        subTex.SetPixel((i + 25) * 8 + w, (j + 25) * 8 + h, pixel);
                    }
                }
            }
        }
        subTex.Apply();


        if (GUILayout.Button(subTex, buttonStyle, GUILayout.MinHeight(408), GUILayout.MinWidth(408), GUILayout.MaxHeight(408), GUILayout.MaxWidth(408)))
        {
            Undo.RecordObject(target, "Changed UV With Preview");
            Vector2 mouseClick = Event.current.mousePosition - previewPos;
            editor.UVOffsetAmount += (new Vector2((int)(mouseClick.x / 8), (int)((408 - mouseClick.y)/8)) - new Vector2(25,25)) * new Vector2(1f / (float)baseTex.width, 1f / (float)baseTex.height);
            editor.ChangeUV();
        }

        previewRect = GUILayoutUtility.GetLastRect();

        
        EditorGUI.DrawRect(new Rect(previewRect.position.x + 192, previewRect.position.y + 192, 8, 24), editor.config.anchorColor);
        EditorGUI.DrawRect(new Rect(previewRect.position.x + 192, previewRect.position.y + 192, 24, 8), editor.config.anchorColor);
        EditorGUI.DrawRect(new Rect(previewRect.position.x + 192, previewRect.position.y + 208, 24, 8), editor.config.anchorColor);
        EditorGUI.DrawRect(new Rect(previewRect.position.x + 208, previewRect.position.y + 192, 8, 24), editor.config.anchorColor);


        if (editor.scaledUVs != null)
        {
            foreach (Vector2 baseUv in editor.scaledUVs)
            {
                EditorGUI.DrawRect(new Rect(buttonPos.x + ((baseUv.x + editor.UVOffsetAmount.x) * 500),
                                        buttonPos.y + ((1 - baseUv.y - editor.UVOffsetAmount.y) * 500),
                                        editor.config.offsetSize, editor.config.offsetSize), editor.config.offsetColor);
            }
        }

        EditorGUI.DrawRect(new Rect(buttonPos.x + ((editor.ScaledOriginalPos.x + editor.UVOffsetAmount.x) * texSize.x),
                            buttonPos.y, editor.config.targetThickness, texSize.y), editor.config.targetColor);
        EditorGUI.DrawRect(new Rect(buttonPos.x, buttonPos.y + ((1 - editor.ScaledOriginalPos.y - editor.UVOffsetAmount.y) * texSize.y),
                                    texSize.x, editor.config.targetThickness), editor.config.targetColor);
        /*
        EditorGUI.DrawRect(new Rect(buttonPos.x + (editor.left.x * texSize.x), buttonPos.y + ((1 - editor.top.y) * texSize.y),
                                    Mathf.Abs(editor.left.x - editor.right.x) * texSize.x, Mathf.Abs(editor.top.y - editor.bottom.y) * texSize.y)
                                    , Color.black);
        */

        if (Event.current.type == EventType.Repaint)
        {
            buttonPos = buttonRect.position;
            previewPos = previewRect.position;
            if(editor.showOriginal && editor.scaledUVs != null)
            {
                foreach (Vector2 baseUv in editor.scaledUVs)
                {
                    EditorGUI.DrawRect(new Rect(buttonPos.x + ((baseUv.x) * texSize.x),
                                            buttonPos.y + ((1 - baseUv.y) * texSize.y),
                                            editor.config.baseSize, editor.config.baseSize), editor.config.baseColor);
                }
            }

            EditorGUI.DrawRect(new Rect(buttonPos.x + (editor.ScaledOriginalPos.x * texSize.x) - 1,
                                        buttonPos.y + ((1 - editor.ScaledOriginalPos.y) * texSize.y) - 1,
                                        editor.config.anchorSize, editor.config.anchorSize), editor.config.anchorColor);
        }

        

    }


}
#endif
