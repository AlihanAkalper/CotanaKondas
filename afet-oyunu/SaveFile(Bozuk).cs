using Godot;
using System;
using System.Collections.Generic;

public partial class SaveFile : Node
{
	public bool HasPastSave = false;
	public Dictionary<string,Variant> SaveValues = new System.Collections.Generic.Dictionary<string, Variant>()
	{
		//Elemanların Type = System.Collections.Generic.KeyValuePair`2[System.String,Godot.Variant]
		{"Gün",0},
		{"Para",0},
		{"KıbrısEv",false},
		{"KaradenizEv",false},
		{"AkdenizEv",false}
	};
	const string SavePath = "user://save_json.json";

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (FileAccess.FileExists(SavePath))
   		{
			GD.Print("Save file detected!");
			// We have a save to load.
			using var file = FileAccess.Open(SavePath,FileAccess.ModeFlags.Write);
			while (file.GetPosition() < file.GetLength())
    		{
				GD.Print("uwu");
        		var jsonString = file.GetLine();
        		// Creates the helper class to interact with JSON.
        		var json = new Json();
        		var parseResult = json.Parse(jsonString);
        		if (parseResult != Error.Ok)
        		{
            		GD.Print($"JSON Parse Error: {json.GetErrorMessage()} in {jsonString} at line {json.GetErrorLine()}");
            		continue;
        		}
				GD.Print(json.Data.AsString());




			}
			HasPastSave = true; 
    	}
		SaveGame();
	}
	public void SaveGame()
	{
		using var file = FileAccess.Open(SavePath,FileAccess.ModeFlags.Write);
		foreach (var item in SaveValues)
		{
			var jsonString = item.Key + Json.Stringify(item.Value);
			if (!file.StoreLine(jsonString))
			{
				GD.PrintErr("ERROR when saving the file!!!!");
			}
		}
	}
}
