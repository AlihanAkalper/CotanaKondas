using Godot;
using System;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;

public partial class Bina : Area2D
{
	[Export] public Ev_numaraları type;

    [Export] private PackedScene Kat_Template;
    
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
        katlar = new Sprite2D[5];
        GD.Print("uwu");
        var Zemin = (Sprite2D)Kat_Template.Instantiate();
        AddChild(Zemin);
        katlar[0] = (Zemin);
        Zemin.Texture = _forSaleTexture;
        GD.Print(katlar);
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
