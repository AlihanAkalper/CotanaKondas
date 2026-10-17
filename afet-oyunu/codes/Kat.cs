using Godot;
using System;

public partial class Kat : Sprite2D
{
	[ExportGroup("Duvarlar")]
	[Export]private Sprite2D ArkaDuv;
	[Export]private Sprite2D SolDuv;
	[Export]private Sprite2D SagDuv;
	[Export]private Sprite2D Çatı;

	[ExportGroup("Texturelar")]
	[Export]private AtlasTexture Odun;
	[Export]private AtlasTexture Metal;
	[Export]private AtlasTexture Tugla;


    public int Malzeme
    {
		get{return Malzeme;}

		set
		{
			Malzeme = value;
			UpdateLook(value);
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

	private void UpdateLook(int YeniMalz)
	{
		
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