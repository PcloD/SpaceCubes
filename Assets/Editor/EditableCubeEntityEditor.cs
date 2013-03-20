using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(EditableCubeEntity))]
public class EditableCubeEntityEditor : Editor 
{
	private bool editionEnabled = false;
	
	void OnSceneGUI()
	{
		EditableCubeEntity editable = (target as EditableCubeEntity);
		
		if (editionEnabled)
		{
			int controlId = GUIUtility.GetControlID(FocusType.Passive);
			
			switch(Event.current.type)
			{
				case EventType.MouseDown:
					editable.EditorMouseClick(Event.current);
					break;
					
				case EventType.Layout:
					HandleUtility.AddDefaultControl(controlId);
					break;
			}
		}
		
		Handles.BeginGUI();
		
		Vector2 guiSize = new Vector2(170, 85);
		Vector2 guiPosition = new Vector2(Screen.width - (guiSize.x + 15), Screen.height - (guiSize.y + 50));
		
		GUI.Box(new Rect(guiPosition.x - 5, guiPosition.y - 5, guiSize.x + 10, guiSize.y + 10), "");
		
		editionEnabled = GUI.Toggle(new Rect(guiPosition.x, guiPosition.y + 60, guiSize.x, 30), editionEnabled, "Enable Edition");
		
		if (GUI.Button(new Rect(guiPosition.x, guiPosition.y, 50, 50), "<<"))
		{
			editable.materialTypeToAdd--;
			if ((int) editable.materialTypeToAdd < 1)
				editable.materialTypeToAdd = (CubeMaterialType) 1;
		}
		
		float fromTX = (1.0f / 8.0f) * (((int) editable.materialTypeToAdd) % 8);
		float toTX = fromTX + (1.0f / 8.0f);
		
		float fromTY = 1.0f - (1.0f / 8.0f) * (((int) editable.materialTypeToAdd) / 8);
		float toTY = fromTY - (1.0f / 8.0f);
		
		GUI.DrawTextureWithTexCoords(
			new Rect(guiPosition.x + 60, guiPosition.y, 50, 50), 
			editable.renderer.sharedMaterial.mainTexture, 
			new Rect(fromTX, fromTY, toTX - fromTX, toTY - fromTY));
		
		if (GUI.Button(new Rect(guiPosition.x + 120, guiPosition.y, 50, 50), ">>"))
		{
			editable.materialTypeToAdd++;
			if ((int) editable.materialTypeToAdd > 255)
				editable.materialTypeToAdd = (CubeMaterialType) 255;
		}
		
		Handles.EndGUI();
	}
}
