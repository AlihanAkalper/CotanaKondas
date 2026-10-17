using Godot;
using System;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;

public partial class Bina : Area2D
{
	[Export] public Ev_numaraları type;

    [Export] private PackedScene Kat_Template;
    
    [Export] public int MaxKat;
    [Export] public int KatHeight;
    [Export] private Kat[] katlar;

    [ExportGroup("Durum Görselleri")]
    [Export] private Texture2D _forSaleTexture;   // Satılık tabela/boş arazi görseli
    [Export] private Texture2D _builtHouseTexture_odun; // odun
    [Export] private Texture2D _builtHouseTexture_tuğla; // tuğla
    [Export] private Texture2D _builtHouseTexture_celik; // çelik
    [Export] private Sprite2D _sprite;

    public override void _Ready()
    {
        var Zemin = (Kat)Kat_Template.Instantiate();
        //MaxKat = 5;
        //KatHeight = 15;     //Şimdilik koydum, ayarlanacak
        AddChild(Zemin);
        katlar = new Kat[MaxKat];
        katlar[0] = Zemin;
        for (int i = 1; i < MaxKat; i++)
        {
            var EmptyKat = (Kat)Kat_Template.Instantiate();
            EmptyKat.Malzeme = 0;
            //EmptyKat.Malzeme = 0;
            EmptyKat.Position += new Vector2(0,KatHeight*i);
            AddChild(EmptyKat);
            katlar[i] = (EmptyKat);
        }
        GD.Print(katlar);
    }

    public void KatEkle()
    {
        for (int i = 0; i < MaxKat; i++)
            {
                if(katlar[i].Malzeme == 0)
                {
                    katlar[i].Malzeme = 1; //GEÇİCİ OLARAK KOYDUM, MALZEME ATLASI OLACAK
                    if (i+1 == MaxKat)
                    {
                        // katlar[i].       EN ÜST KATI ÇATI YAPACAK    
                    }
                    else
                    {
                        katlar[i+1].Malzeme = 0;
                    }
                    GD.Print(katlar);
                }
            }
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	private double NewKatTime = 0.0;
    public override void _Process(double delta)
    {
        if (katlar[MaxKat-1].Malzeme != 0)
        {
        NewKatTime += delta;
        if(NewKatTime > 4000.0)
        {
            NewKatTime = 0.0;
            GD.Print("YENİ KAT!");
            KatEkle();
        }   
        }
    }
}
