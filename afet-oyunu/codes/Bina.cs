using Godot;
using System;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;

public partial class Bina : Area2D
{
    private int DEV_Malzeme = 1;

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
        katlar = new Kat[MaxKat];
        for (int i = 0; i < MaxKat; i++)
        {
            var EmptyKat = (Kat)Kat_Template.Instantiate();
            EmptyKat.Position += new Vector2(0,-1*KatHeight*i);
            EmptyKat.SonKat = false;
            EmptyKat.Malzeme = 0;
            EmptyKat.Saglamlık = 0;
            EmptyKat.Kiraci = null;
            AddChild(EmptyKat);
            katlar[i] = (EmptyKat);
        }
        katlar[0].Malzeme = 1;
        katlar[MaxKat-1].SonKat = true;
        GD.Print(katlar);
    }

    public void KatEkle()
    {
        for (int i = 0; i < MaxKat; i++)
            {
                if(DEV_Malzeme > 4)
                {
                    DEV_Malzeme = 1;
                }
                if(katlar[i].Malzeme == 1)
                {
                    GD.Print("YENİ KAT ÇIKILIYOR!");
                    katlar[i].Malzeme = DEV_Malzeme; //GEÇİCİ OLARAK KOYDUM, MALZEME NO'su olacak
                    if (i+1 == MaxKat)
                    {
                        katlar[i].SonKat = true;
                        katlar[i].Malzeme = DEV_Malzeme;
                        GD.Print("ÇATI KATI ÇIKILDI");
                        // katlar[i].       EN ÜST KATI ÇATI YAPACAK    
                    }
                    else
                    {
                        katlar[i+1].Malzeme = 1;
                    }
                    GD.Print(katlar);
                    break;
                }
            }
            DEV_Malzeme++;  
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	private double NewKatTime = 0.0;
    public override void _Process(double delta)
    {
        if (katlar[MaxKat-1].Malzeme  < 2)
        {
        NewKatTime += delta;
        if(NewKatTime > 1.5)
        {
            NewKatTime = 0.0;
            KatEkle();
        }   
        }
    }
}
