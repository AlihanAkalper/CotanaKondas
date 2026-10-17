using Godot;

public partial class DisasterAnimationManager : Node
{
    [Export] private EarthquakeEffect _earthquake;
    [Export] private FireEffect _fire;
    [Export] private FloodEffect _flood;

    public void PlayDisasterAnimation(AfetTürü afet)
    {
        switch (afet)
        {
            case AfetTürü.Deprem:
                _earthquake?.TriggerEarthquake(2.5f, 18.0f); 
                break;

            case AfetTürü.Yangın:
                _fire?.TriggerFire(3.0f); 
                break;

            case AfetTürü.Sel:
                _flood?.TriggerFlood(300f, 4.0f); 
                break;

            default:
                GD.Print("Bu afet için özel görsel animasyon tanımlanmadı.");
                break;
        }
    }
}