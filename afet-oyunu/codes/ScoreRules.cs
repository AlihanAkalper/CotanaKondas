using System;
using System.Linq;

public record ScoreBreakdown(int CompletedDays, int OccupiedUsableFloors, int Preparations,
    int PositiveAnswers, int NeutralAnswers, int NegativeAnswers, int DestroyedFloors, int DestroyedBuildings,
    int LostTenants)
{
    public int DayPoints => Math.Min(CompletedDays, ScoreRules.DayPointLimit) * ScoreRules.PointsPerDay;
    public int FloorPoints => OccupiedUsableFloors * ScoreRules.PointsPerFloor;
    public int PreparationPoints => Preparations * ScoreRules.PointsPerPreparation;
    public int PositivePoints => PositiveAnswers * ScoreRules.PointsPerAnswer;
    public int NegativePoints => -NegativeAnswers * ScoreRules.PointsPerAnswer;
    public int FloorPenalty => -DestroyedFloors * ScoreRules.PenaltyPerFloor;
    public int BuildingPenalty => -DestroyedBuildings * ScoreRules.PenaltyPerBuilding;
    public int TenantPenalty => -LostTenants * ScoreRules.PenaltyPerTenant;
    public int RawTotal => DayPoints + FloorPoints + PreparationPoints + PositivePoints +
        NegativePoints + FloorPenalty + BuildingPenalty + TenantPenalty;
    public int Total => Math.Max(0, RawTotal);
}

public static class ScoreRules
{
    public const int PointsPerDay = 2, DayPointLimit = 100;
    public const int PointsPerFloor = 150, PointsPerPreparation = 25, PointsPerAnswer = 40;
    public const int PenaltyPerFloor = 75, PenaltyPerBuilding = 100, PenaltyPerTenant = 75;
    public static ScoreBreakdown Calculate(GameSaveData save) => new(
        // Oyun 1. gün başlar; gün ilerletildiğinde önceki gün tamamlanmış olur.
        Math.Max(0, save.CurrentDay - 1),
        save.Houses.Where(h => h.IsBuilt)
            .Sum(h => h.Floors.Count(f => f.Health > 0 && f.HasTenant)),
        save.Houses.Where(h => h.IsBuilt).Sum(h => h.PurchasedPerks.Distinct().Count()),
        // Aynı kiracının aynı sorusundan tekrar tekrar puan kazanılmasın.
        save.DialogueAnswers.Where(a => a.Tone == DialogueTone.Positive)
            .Select(a => (a.TenantId, a.DialogueId)).Distinct().Count(),
        save.DialogueAnswers.Count(a => a.Tone == DialogueTone.Neutral),
        save.DialogueAnswers.Count(a => a.Tone == DialogueTone.Negative),
        GameEndRules.DestroyedFloors(save),
        GameEndRules.DestroyedBuildings(save), save.LostTenants);
}
