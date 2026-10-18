using System.Linq;

public static class GameEndRules
{
    public const int TenantLossLimit = 10;
    public static int DestroyedFloors(GameSaveData save) => save.Houses.Sum(h => h.Floors.Count(f => f.WasDestroyed));
    public static int DestroyedBuildings(GameSaveData save) => save.Houses.Count(h => h.WasDestroyed);

    public static void Evaluate(GameSaveData save)
    {
        // Kaldırılan bitiş koşullarıyla kapanmış kayıtları yeni kurala göre değerlendir.
        if (save.IsFinished && (save.EndReason?.Contains("200. güne ulaşıldı.") == true ||
            save.EndReason?.Contains("6 kat yıkıldı.") == true ||
            save.EndReason?.Contains("3 bina yıkıldı.") == true))
        {
            save.IsFinished = false;
            save.EndReason = "";
        }
        if (save.IsFinished) return;
        if (save.LostTenants < TenantLossLimit) return;
        save.IsFinished = true;
        save.EndReason = $"{TenantLossLimit} kiracı kaybına ulaşıldı.";
    }
}
