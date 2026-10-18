using System.Collections.Generic;
using System.Linq;

public static class GameEndRules
{
    public const int FloorLimit = 6;
    public const int BuildingLimit = 3;
    public static int DestroyedFloors(GameSaveData save) => save.Houses.Sum(h => h.Floors.Count(f => f.WasDestroyed));
    public static int DestroyedBuildings(GameSaveData save) => save.Houses.Count(h => h.WasDestroyed);

    public static void Evaluate(GameSaveData save)
    {
        // Eski kayıtlarda yalnızca gün sınırı nedeniyle biten oyunlar devam edebilir.
        if (save.IsFinished && save.EndReason?.Contains("200. güne ulaşıldı.") == true)
        {
            save.IsFinished = false;
            save.EndReason = "";
        }
        if (save.IsFinished) return;
        var reasons = new List<string>();
        if (DestroyedFloors(save) >= FloorLimit) reasons.Add("6 kat yıkıldı.");
        if (DestroyedBuildings(save) >= BuildingLimit) reasons.Add("3 bina yıkıldı.");
        if (reasons.Count == 0) return;
        save.IsFinished = true;
        save.EndReason = string.Join("\n", reasons);
    }
}
