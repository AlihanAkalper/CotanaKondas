using System;
using System.Linq;

public record ScoreBreakdown(int CompletedDays, int OccupiedUsableFloors, int Preparations,
    int PositiveAnswers, int NeutralAnswers, int NegativeAnswers, int DestroyedFloors, int DestroyedBuildings)
{
    public int DayPoints => CompletedDays * 10;
    public int FloorPoints => OccupiedUsableFloors * 100;
    public int PreparationPoints => Preparations * 50;
    public int PositivePoints => PositiveAnswers * 20;
    public int NegativePoints => -NegativeAnswers * 20;
    public int FloorPenalty => -DestroyedFloors * 100;
    public int BuildingPenalty => -DestroyedBuildings * 200;
    public int RawTotal => DayPoints + FloorPoints + PreparationPoints + PositivePoints +
        NegativePoints + FloorPenalty + BuildingPenalty;
    public int Total => Math.Max(0, RawTotal);
}

public static class ScoreRules
{
    public static ScoreBreakdown Calculate(GameSaveData save) => new(
        // Oyun 1. gün başlar; gün ilerletildiğinde önceki gün tamamlanmış olur.
        Math.Max(0, save.CurrentDay - 1),
        save.Houses.Where(h => h.IsBuilt)
            .Sum(h => h.Floors.Count(f => f.Health > 0 && f.HasTenant)),
        save.PurchasedPerks.Distinct().Count(),
        save.DialogueAnswers.Count(a => a.Tone == DialogueTone.Positive),
        save.DialogueAnswers.Count(a => a.Tone == DialogueTone.Neutral),
        save.DialogueAnswers.Count(a => a.Tone == DialogueTone.Negative),
        GameEndRules.DestroyedFloors(save),
        GameEndRules.DestroyedBuildings(save));
}
