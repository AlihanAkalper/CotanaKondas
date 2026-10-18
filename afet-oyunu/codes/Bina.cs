using Godot;

public partial class Bina : Area2D
{
    [Export] public Ev_numaraları type;
    [Export] private PackedScene Kat_Template;
    [Export] public int MaxKat = ConstructionRules.MaxFloors;
    [Export] public int KatHeight = 120;
    private Kat[] _floors;
    public override void _Ready()
    {
        // Eski kayıtlardaki mevcut katları gizleme; yeni inşaatlar kurallarla sınırlandırılır.
        var house = GameManager.Instance.SaveData.Houses.Find(h => h.Type == GameManager.Instance.SelectedRegion);
        _floors = new Kat[System.Math.Max(ConstructionRules.MaxFloors, house?.Floors.Count ?? 0)];
        for (int i = 0; i < _floors.Length; i++)
        {
            var floor = Kat_Template.Instantiate<Kat>();
            floor.Position += new Vector2(0, -KatHeight * i);
            AddChild(floor);
            _floors[i] = floor;
        }
        Refresh();
    }
    public void Refresh()
    {
        if (_floors == null || GameManager.Instance == null) return;
        var house = GameManager.Instance.SaveData.Houses.Find(h => h.Type == GameManager.Instance.SelectedRegion);
        int standingFloors = 0;
        if (house != null)
        {
            while (standingFloors < house.Floors.Count && house.Floors[standingFloors].Health > 0)
                standingFloors++;
        }
        for (int i = 0; i < _floors.Length; i++)
        {
            // Yıkılan kat kayıtları onarım için kalır; yalnızca ilk boş yerde iskele görünür.
            bool built = i < standingFloors;
            int material = built ? house.material switch
            {
                Materyal.Odun => 2, Materyal.Çelik => 3, Materyal.Tuğla => 4, _ => 0
            } : (house != null && i == standingFloors && i < ConstructionRules.MaxFloors ? 1 : 0);
            _floors[i].sakin?.BindTenant(house, built ? house.Floors[i] : null, i);
            _floors[i].SetState(material, built ? house.Floors[i].Health : 0,
                built && standingFloors == ConstructionRules.MaxFloors && i == ConstructionRules.MaxFloors - 1,
                house?.material ?? Materyal.Alınmadı, house?.PurchasedPerks);
        }
    }
}

