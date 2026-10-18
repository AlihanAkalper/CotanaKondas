using Godot;
using System;
using System.ComponentModel;

public partial class Kat : Sprite2D
{
	[Export] public Boolean SonKat;

	[Export]public Kiraci sakin;
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
		UpdateKiraciVisibility();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private bool _buildingUsable = true;

    public void SetState(int material, float health, bool topFloor, bool buildingUsable)
    {
        Saglamlık = (int)Math.Ceiling(health);
        SonKat = topFloor;
        _buildingUsable = buildingUsable;
        Malzeme = material;
    }

    // Atlas dilimlerinin şeffaf kenarlarını kaldırıp tüm parçaları aynı kat ölçüsüne oturt.
    private Texture2D Layer(CompressedTextureLayered source, int index)
    {
        var image = source.GetLayerData(index);
        return new AtlasTexture
        {
            Atlas = ImageTexture.CreateFromImage(image),
            Region = image.GetUsedRect()
        };
    }

    private void Fit(Sprite2D sprite, Texture2D texture, Vector2 position, Vector2 size)
    {
        sprite.Texture = texture;
        sprite.Centered = false;
        sprite.Offset = Vector2.Zero;
        sprite.RegionEnabled = false;
        sprite.Position = position;
        sprite.Scale = size / texture.GetSize();
    }

    private void UpdateLook()
    {
        UpdateKiraciVisibility();
        Texture = null;
        ArkaDuv.Texture = SolDuv.Texture = SagDuv.Texture = Çatı.Texture = null;
        if (_malzeme == 0) return;
        if (_malzeme == 1)
        {
            var scaffoldImage = iskele.GetImage();
            Texture = new AtlasTexture { Atlas = iskele, Region = scaffoldImage.GetUsedRect() };
            Centered = true;
            RegionEnabled = false;
            // İskele ayaklarını boş arsada zemine, sonraki katlarda çatıya oturt.
            float baseOffset = Position.Y < 0 ? 13.5f : 4.5f;
            Offset = new Vector2(0, -Texture.GetHeight() / 2f - baseOffset);
            return;
        }

        var source = _malzeme == 2 ? Odun : _malzeme == 3 ? Metal : Tugla;
        bool wood = _malzeme == 2;
        int wall = wood
            ? (Saglamlık > 75 ? 2 : Saglamlık > 50 ? 0 : Saglamlık > 25 ? 1 : 3)
            : (Saglamlık > 66 ? 0 : Saglamlık > 33 ? 1 : 2);
        Texture = Layer(source, wood ? 4 : 3);
        Centered = true;
        Offset = Vector2.Zero;
        RegionEnabled = false;

        float width = Texture.GetWidth();
        float height = (GetParent() is Bina bina ? bina.KatHeight : 120) / Scale.Y;
        float bottom = -Texture.GetHeight() / 2f;
        float top = bottom - height;
        const float wallWidth = 9f;
        float left = -width / 2f;
        Fit(ArkaDuv, Layer(source, wood ? 5 : 4),
            new Vector2(left + wallWidth, top), new Vector2(width - 2 * wallWidth, height));
        var wallTexture = Layer(source, wall);
        Fit(SolDuv, wallTexture, new Vector2(left, top), new Vector2(wallWidth, height));
        Fit(SagDuv, wallTexture, new Vector2(width / 2f - wallWidth, top), new Vector2(wallWidth, height));
        if (SonKat)
            Fit(Çatı, Layer(source, wood ? 6 : 5),
                new Vector2(left - 2, top - 9), new Vector2(width + 4, 9));

        if (sakin != null)
            sakin.Position = new Vector2(sakin.Position.X, bottom - 16);
    }
private void UpdateKiraciVisibility()
	{
		if (sakin == null) return;
		bool katInsaaEdildi = _malzeme >= 2 && _malzeme <= 4 && Saglamlık > 0 && _buildingUsable;
		sakin.Visible = katInsaaEdildi;
		sakin.ProcessMode = katInsaaEdildi ? ProcessModeEnum.Inherit : ProcessModeEnum.Disabled;
	}

	[Serializable]
	public class KatSaveData
	{
		public bool kiraciVar = false;
		public int saglam = 0;
		public int farki = 0;

		public KatSaveData(Kat hedef)
		{
			kiraciVar = hedef.sakin != null;
			saglam = hedef.Saglamlık;
			if (kiraciVar)
			{
				farki = hedef.sakin.Farkindalik;
			}
		}
	}
}




