using Godot;
using System;
using System.ComponentModel;

public partial class Kat : Sprite2D
{
	[ExportGroup("Duvarlar")]
	[Export]private Sprite2D ArkaDuv;
	[Export]private Sprite2D SolDuv;
	[Export]private Sprite2D SagDuv;
	[Export]private Sprite2D Çatı;

	[ExportGroup("Texturelar")]
	[Export]private Texture2DArray Odun;
	[Export]private Texture2DArray Metal;
	[Export]private Texture2DArray Tugla;

	private int _malzeme;
    public int Malzeme
    {
		get{return _malzeme;}
		set
		{
			_malzeme = value;
			UpdateLook();
		}
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void UpdateLook()
	{
		GD.Print("CHANGE MY MALZEME TO " + _malzeme);
		switch (_malzeme)
		{
			case 0:
				
			case 1:
			
			case 2:
			
			case 3:
			
			case 4:

			default:
				GD.PrintErr("Unknown malzeme");
				break;
		}
	}

}

//ÇETO'nun Bina için kodu

// public void UpdateView(HouseSaveData data)
// 	{
//         if(_sprite == null) return;
//         if(!data.IsBuilt)
//         {
//             _sprite.Texture = _forSaleTexture;
//             return;
//         }
//         _sprite.Texture = data.material switch
//         {
//             Materyal.Odun => _builtHouseTexture_odun.,
//             Materyal.Tuğla => _builtHouseTexture_tuğla,
//             Materyal.Çelik => _builtHouseTexture_celik,
//             _ => _forSaleTexture
//         };
//     }