using Godot;
using System;
using System.Collections.Generic;

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

    private readonly Dictionary<(CompressedTextureLayered Source, int Index), Texture2D> _layerTextures = new();

    public void SetState(int material, float health, bool topFloor)
    {
        Saglamlık = (int)Math.Ceiling(health);
        SonKat = topFloor;

        Malzeme = material;
    }

    // Dilimi bağımsız dokuya çevir; tekrar ederken atlasın diğer parçaları görünmesin.
    private Texture2D Layer(CompressedTextureLayered source, int index)
    {
        var key = (source, index);
        if (_layerTextures.TryGetValue(key, out var texture)) return texture;
        using var image = source.GetLayerData(index);
        using var cropped = image.GetRegion(image.GetUsedRect());
        texture = ImageTexture.CreateFromImage(cropped);
        _layerTextures.Add(key, texture);
        return texture;
    }

    private void Tile(Sprite2D sprite, Texture2D texture, Vector2 position, Vector2 size)
    {
        sprite.Texture = texture;
        sprite.Centered = false;
        sprite.Offset = Vector2.Zero;
        sprite.Position = position;
        sprite.Scale = Vector2.One;
        sprite.TextureRepeat = CanvasItem.TextureRepeatEnum.Enabled;
        sprite.RegionEnabled = true;
        sprite.RegionRect = new Rect2(Vector2.Zero, size);
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
        // Bina yalnızca sıradaki inşa edilebilir kat için iskele ister.
        if (_malzeme == 1)
        {
            var scaffoldImage = iskele.GetImage();
            Texture = new AtlasTexture { Atlas = iskele, Region = scaffoldImage.GetUsedRect() };
            Centered = true;
            RegionEnabled = false;
            // Ara katlarda çatı yok; iskele ayaklarını katın taban hizasına oturt.
            const float baseOffset = 4.5f;
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
        // Son dilimdeki iç duvar karosunu esnetmeden yan yana ve üst üste döşe.
        Tile(ArkaDuv, Layer(source, wood ? 7 : 6),
            new Vector2(left + wallWidth, top), new Vector2(width - 2 * wallWidth, height));
        var wallTexture = Layer(source, wall);
        Fit(SolDuv, wallTexture, new Vector2(left, top), new Vector2(wallWidth, height));
        Fit(SagDuv, wallTexture, new Vector2(width / 2f - wallWidth, top), new Vector2(wallWidth, height));
        if (SonKat)
        {
            var roofTexture = Layer(source, wood ? 6 : 5);
            // Çatı parçasını kendi yüksekliğinde tut, yalnızca yatayda tekrar et.
            float roofHeight = roofTexture.GetHeight();
            Tile(Çatı, roofTexture,
                new Vector2(left - 2, top - roofHeight), new Vector2(width + 4, roofHeight));
        }

        sakin?.SetFloorSurface(bottom);
    }
private void UpdateKiraciVisibility()
	{
		if (sakin == null) return;
		bool katInsaaEdildi = _malzeme >= 2 && _malzeme <= 4 && Saglamlık > 0 && sakin.HasTenant;
		sakin.Visible = katInsaaEdildi;
		sakin.ProcessMode = katInsaaEdildi ? ProcessModeEnum.Inherit : ProcessModeEnum.Disabled;
	}

}
