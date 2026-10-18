using Godot;

public partial class Bina : Area2D
{
    [Export] public Ev_numaraları type;
    [Export] private PackedScene Kat_Template;
    [Export] public int MaxKat = 5;
    [Export] public int KatHeight = 120;
    private Kat[] _floors;
    public override void _Ready()
    {
        _floors = new Kat[ConstructionRules.MaxFloors];
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
        for (int i = 0; i < _floors.Length; i++)
        {
            bool built = house != null && i < house.Floors.Count;
            int material = built ? house.material switch
            {
                Materyal.Odun => 2, Materyal.Çelik => 3, Materyal.Tuğla => 4, _ => 0
            } : (house != null && i == house.Floors.Count ? 1 : 0);
            _floors[i].SetState(material, built ? house.Floors[i].Health : 0,
                built && i == house.Floors.Count - 1, house != null && house.Health > 0);
        }
    }
}

