using Godot;
using System;
using System.ComponentModel;

public partial class Kat : Sprite2D
{
	[Export] public Boolean SonKat;

	[Export]public Kiraci Kiraci;
	public int Saglamlık;

	[ExportGroup("Duvarlar")]
	[Export]private Sprite2D ArkaDuv;
	[Export]private Sprite2D SolDuv;
	[Export]private Sprite2D SagDuv;
	[Export]private Sprite2D Çatı;

	[ExportGroup("Texturelar")]
	[Export]private Texture2D iskele;
	[Export]private CompressedTextureLayered Odun;
	[Export]private CompressedTextureLayered Metal;
	[Export]private CompressedTextureLayered Tugla;

	[Export] private int _malzeme;
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
		if(GetParent().ToString() == "<Window#26860324303>")
		{
			GD.PrintErr("In DEV mode, material set to 2");
			SonKat = true;
			Malzeme = 2;
		}
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
			case 0: //Boşluk
				this.Texture = null;
				ArkaDuv.Texture = null;
				SolDuv.Texture = null;
				SagDuv.Texture = null;
				Çatı.Texture = null;
				break;
			case 1: //İskele
				this.Texture = iskele;
				ArkaDuv.Texture = null;
				SolDuv.Texture = null;
				SagDuv.Texture = null;
				Çatı.Texture = null;
				break;
			case 2: //Odun
				if (Saglamlık > 100)
				{
					SolDuv.Texture = ImageTexture.CreateFromImage(Odun.GetLayerData(2));
					SagDuv.Texture = ImageTexture.CreateFromImage(Odun.GetLayerData(2));
				}else if (Saglamlık > 50)
				{
					SolDuv.Texture = ImageTexture.CreateFromImage(Odun.GetLayerData(0));
					SagDuv.Texture = ImageTexture.CreateFromImage(Odun.GetLayerData(0));
				}
				else if(Saglamlık > 25)
				{
					SolDuv.Texture = ImageTexture.CreateFromImage(Odun.GetLayerData(1));
					SagDuv.Texture = ImageTexture.CreateFromImage(Odun.GetLayerData(1));
				}
				else
				{
					SolDuv.Texture = ImageTexture.CreateFromImage(Odun.GetLayerData(3));
					SagDuv.Texture = ImageTexture.CreateFromImage(Odun.GetLayerData(3));
				}
				this.Texture = ImageTexture.CreateFromImage(Odun.GetLayerData(4));
				ArkaDuv.Texture = ImageTexture.CreateFromImage(Odun.GetLayerData(7));
				if (SonKat)
				{
					Çatı.Texture = ImageTexture.CreateFromImage(Odun.GetLayerData(8));
				}
				break;
			case 3: //Çelik
				this.Texture = ImageTexture.CreateFromImage(Metal.GetLayerData(3));
				ArkaDuv.Texture = ImageTexture.CreateFromImage(Metal.GetLayerData(6));
				if (Saglamlık > 200)
				{
					SolDuv.Texture = ImageTexture.CreateFromImage(Metal.GetLayerData(0));
					SagDuv.Texture = ImageTexture.CreateFromImage(Metal.GetLayerData(0));
				}
				else if(Saglamlık > 100)
				{
					SolDuv.Texture = ImageTexture.CreateFromImage(Metal.GetLayerData(1));
					SagDuv.Texture = ImageTexture.CreateFromImage(Metal.GetLayerData(1));
				}
				else
				{
					SolDuv.Texture = ImageTexture.CreateFromImage(Metal.GetLayerData(2));
					SagDuv.Texture = ImageTexture.CreateFromImage(Metal.GetLayerData(2));
				}
				if (SonKat)
				{
					Çatı.Texture = ImageTexture.CreateFromImage(Metal.GetLayerData(5));
				}
				break;
			case 4: //Tuğla
				this.Texture = ImageTexture.CreateFromImage(Tugla.GetLayerData(3));
				ArkaDuv.Texture = ImageTexture.CreateFromImage(Tugla.GetLayerData(6));
				if (Saglamlık > 200)
				{
					SolDuv.Texture = ImageTexture.CreateFromImage(Tugla.GetLayerData(0));
					SagDuv.Texture = ImageTexture.CreateFromImage(Tugla.GetLayerData(0));
				}
				else if(Saglamlık > 100)
				{
					SolDuv.Texture = ImageTexture.CreateFromImage(Tugla.GetLayerData(1));
					SagDuv.Texture = ImageTexture.CreateFromImage(Tugla.GetLayerData(1));
				}
				else
				{
					SolDuv.Texture = ImageTexture.CreateFromImage(Tugla.GetLayerData(2));
					SagDuv.Texture = ImageTexture.CreateFromImage(Tugla.GetLayerData(2));
				}
				if (SonKat)
				{
					Çatı.Texture = ImageTexture.CreateFromImage(Tugla.GetLayerData(5));
				}
				break;
			default:
				GD.PrintErr("Unknown malzeme");
				break;
		}
	}

}