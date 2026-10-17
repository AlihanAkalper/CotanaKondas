using Godot;
using System;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;

public partial class Bina : Area2D
{
	[Export] public Ev_numaraları type;

    [Export] private PackedScene Kat_Template;
    
    public int MaxKat;
    private Sprite2D[] katlar;

    [ExportGroup("Durum Görselleri")]
    [Export] private Texture2D _forSaleTexture;   // Satılık tabela/boş arazi görseli
    [Export] private Texture2D _builtHouseTexture_odun; // odun
    [Export] private Texture2D _builtHouseTexture_tuğla; // tuğla
    [Export] private Texture2D _builtHouseTexture_celik; // çelik
    [Export] private Sprite2D _sprite;

	public void UpdateView(HouseSaveData data)
	{
        if(_sprite == null) return;
        if(!data.IsBuilt)
        {
            _sprite.Texture = _forSaleTexture;
            return;
        }
        _sprite.Texture = data.material switch
        {
            Materyal.Odun => _builtHouseTexture_odun,
            Materyal.Tuğla => _builtHouseTexture_tuğla,
            Materyal.Çelik => _builtHouseTexture_celik,
            _ => _forSaleTexture
        };
    }
    public override void _Ready()
    {
        katlar = new Sprite2D[MaxKat];
        var Zemin = (Sprite2D)Kat_Template.Instantiate();
        AddChild(Zemin);
        katlar[0] = (Zemin);
        Zemin.Texture = _forSaleTexture;
        GD.Print(katlar);
    }

    public void KatEkle()
    {
        if(katlar[0].Texture == _forSaleTexture)
        {
            katlar[0].Texture = _forSaleTexture; //GEÇİCİ OLARAK KOYDUM, MALZEME ATLASI OLACAK
        }
        else
        {
            for (int i = 0; i < MaxKat; i++)
            {
                if(katlar[i] == null)
                {
                    var YeniKat = (Sprite2D)Kat_Template.Instantiate();
                    AddChild(YeniKat);
                    YeniKat.Position += new Vector2(0,6);
                    YeniKat.Texture = _forSaleTexture; //GEÇİCİ OLARAK KOYDUM, MALZEME ATLASI OLACAK
                    katlar[i] = YeniKat;
                    GD.Print(katlar);
                }
            }
        }
        
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
