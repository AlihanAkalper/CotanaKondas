using System;
using System.Linq;

public static class TenantRules
{
    public const int QuestionThreshold = 50;
    public const int DailyAwarenessLoss = 2;
    public const int PerkAwarenessBonus = 10;
    public const double DailyMoveInChance = 0.20;
    public static int AnswerEffect(DialogueTone tone) => tone switch
    {
        DialogueTone.Positive => 15, DialogueTone.Neutral => 5, _ => -10
    };

    public static bool CanTalk(HouseSaveData house, FloorSaveData floor) =>
        house.IsBuilt && floor.Health > 0 && floor.HasTenant &&
        floor.Awareness < QuestionThreshold && floor.PendingQuestionId >= 0 &&
        floor.PendingQuestionId < TenantDialogues.All.Length;

    public static void ChangeAwareness(FloorSaveData floor, int delta)
    {
        if (!floor.HasTenant) return;
        if (floor.Awareness <= 0) floor.LeavesOnNextDisaster = true;
        floor.Awareness = Math.Clamp(floor.Awareness + delta, 0, 100);
        if (floor.Awareness == 0) floor.LeavesOnNextDisaster = true;
        if (floor.Awareness >= QuestionThreshold) ClearQuestion(floor);
    }

    public static void ClearQuestion(FloorSaveData floor)
    {
        floor.PendingQuestionId = -1;
        floor.QuestionAnswers = Array.Empty<int>();
    }

    public static void PrepareQuestion(FloorSaveData floor, Random random)
    {
        if (floor.PendingQuestionId < 0) floor.PendingQuestionId = random.Next(TenantDialogues.All.Length);
        if (floor.QuestionAnswers is { Length: 3 } &&
            floor.QuestionAnswers.All(i => i >= 0 && i < 6) &&
            floor.QuestionAnswers.Select(i => i / 2).Distinct().Count() == 3) return;
        floor.QuestionAnswers = new[] { random.Next(2), 2 + random.Next(2), 4 + random.Next(2) };
        random.Shuffle(floor.QuestionAnswers);
    }

    public static void AdvanceDay(GameSaveData save, Random random)
    {
        if (save.LastDialogueRollDay >= save.CurrentDay) return;
        save.LastDialogueRollDay = save.CurrentDay;
        foreach (var house in save.Houses)
            foreach (var floor in house.Floors)
            {
                if (!floor.HasTenant)
                {
                    // Ayrılma afet çözümünde, bu kontrol ise sonraki gün yapılır.
                    if (house.IsBuilt && floor.Health > 0 && random.NextDouble() < DailyMoveInChance)
                        floor.Tenant = new TenantSaveData { Awareness = random.Next(50, 101) };
                    continue;
                }
                ChangeAwareness(floor, -DailyAwarenessLoss);
            }
        if (random.NextDouble() >= TenantDialogues.DailyChance) return;
        var candidates = save.Houses.Where(h => h.IsBuilt)
            .SelectMany(h => h.Floors).Where(f => f.HasTenant && f.Health > 0 &&
                f.Awareness < QuestionThreshold && f.PendingQuestionId < 0).ToArray();
        if (candidates.Length > 0) PrepareQuestion(candidates[random.Next(candidates.Length)], random);
    }

    public static void ApplyPerk(HouseSaveData house)
    {
        foreach (var floor in house.Floors) ChangeAwareness(floor, PerkAwarenessBonus);
    }

    public static int DepartAfterDisaster(HouseSaveData house)
    {
        int departed = 0;
        foreach (var floor in house.Floors)
        {
            if (!floor.HasTenant || (floor.Health > 0 && !floor.LeavesOnNextDisaster && floor.Awareness > 0)) continue;
            floor.Tenant = null;
            departed++;
        }
        return departed;
    }

    public static bool Answer(GameSaveData save, Ev_numaraları region, int floorIndex, int question, int answer)
    {
        if (save.IsFinished || save.PendingDisasters.Count > 0) return false;
        var house = save.Houses.Find(h => h.Type == region);
        if (house == null || floorIndex < 0 || floorIndex >= house.Floors.Count) return false;
        var floor = house.Floors[floorIndex];
        if (!CanTalk(house, floor) || floor.PendingQuestionId != question ||
            floor.QuestionAnswers == null || !floor.QuestionAnswers.Contains(answer)) return false;
        var tone = (DialogueTone)(answer / 2);
        save.DialogueAnswers ??= new();
        save.DialogueAnswers.Add(new TenantDialogueAnswer {
            Day = save.CurrentDay, DialogueId = question, AnswerIndex = answer, Tone = tone,
            Region = region, FloorNumber = floorIndex + 1, TenantId = floor.Tenant.Id
        });
        ChangeAwareness(floor, AnswerEffect(tone));
        ClearQuestion(floor);
        return true;
    }
}
