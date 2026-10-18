using Godot;
using System.Linq;

public partial class GameEndScreen : Control
{
    public override void _Ready()
    {
        var game = GameManager.Instance;
        var save = game.SaveData;
        var score = ScoreRules.Calculate(save);
        var background = new ColorRect { Color = new Color(.06f, .09f, .13f) };
        AddChild(background);
        background.SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);
        var panel = new PanelContainer();
        AddChild(panel);
        panel.SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);
        panel.AnchorLeft = .15f; panel.AnchorRight = .85f;
        panel.AnchorTop = .08f; panel.AnchorBottom = .92f;
        var style = new StyleBoxFlat { BgColor = new Color(.10f, .15f, .20f) };
        style.ContentMarginLeft = style.ContentMarginRight = 24;
        style.ContentMarginTop = style.ContentMarginBottom = 20;
        panel.AddThemeStyleboxOverride("panel", style);
        var scroll = new ScrollContainer { HorizontalScrollMode = ScrollContainer.ScrollMode.Disabled };
        panel.AddChild(scroll);
        var rows = new VBoxContainer { SizeFlagsHorizontal = SizeFlags.ExpandFill };
        rows.AddThemeConstantOverride("separation", 14);
        scroll.AddChild(rows);
        void Line(string value, int size = 18)
        {
            var label = new Label { Text = value, AutowrapMode = TextServer.AutowrapMode.WordSmart };
            label.AddThemeFontSizeOverride("font_size", size);
            rows.AddChild(label);
        }
        Line("OYUN TAMAMLANDI", 30);
        Line(save.EndReason);
        Line($"TOPLAM PUAN: {score.Total:N0}", 28);
        Line($"Tamamlanan gün: {score.CompletedDays} × 10 = +{score.DayPoints:N0}\n" +
            $"Kullanılabilir ve kiracılı kat: {score.OccupiedUsableFloors} × 100 = +{score.FloorPoints:N0}\n" +
            $"Hazırlık: {score.Preparations} × 50 = +{score.PreparationPoints:N0}\n" +
            $"Pozitif cevap: {score.PositiveAnswers} × 20 = +{score.PositivePoints:N0}\n" +
            $"Nötr cevap: {score.NeutralAnswers} × 0 = 0\n" +
            $"Negatif cevap: {score.NegativeAnswers} × (−20) = {score.NegativePoints:N0}\n" +
            $"Yıkılan kat: {score.DestroyedFloors} × (−100) = {score.FloorPenalty:N0}\n" +
            $"Yıkılan bina: {score.DestroyedBuildings} × (−200) = {score.BuildingPenalty:N0}");
        if (score.RawTotal < 0) Line("Toplam puan sıfırın altına düşmez.");
        Line($"Ulaşılan gün: {save.CurrentDay}\nBakiye: {save.Money:N0} TL");
        Line($"Kiracı kaybı: {save.LostTenants} / {GameEndRules.TenantLossLimit}\nYıkılan kat: {GameEndRules.DestroyedFloors(save)}\nYıkılan bina: {GameEndRules.DestroyedBuildings(save)}");
        var tenants = save.Houses.SelectMany(h => h.Floors).Where(f => f.HasTenant).ToArray();
        double awareness = tenants.Length == 0 ? 0 : tenants.Average(f => f.Awareness);
        Line($"Kalan kiracı: {tenants.Length}\nOrtalama kiracı farkındalığı: {awareness:0.#}/100\nAlınan hazırlık: {save.Houses.Sum(h => h.PurchasedPerks.Distinct().Count())}");
        Line($"Diyaloglar — Pozitif: {save.DialogueAnswers.Count(a => a.Tone == DialogueTone.Positive)} | Nötr: {save.DialogueAnswers.Count(a => a.Tone == DialogueTone.Neutral)} | Negatif: {save.DialogueAnswers.Count(a => a.Tone == DialogueTone.Negative)}");
        var menu = new Button { Text = "Ana menüye dön", CustomMinimumSize = new Vector2(0, 44) };
        rows.AddChild(menu);
        menu.Pressed += () => { game.SaveGame(); GetTree().ChangeSceneToFile("res://scenes/GirişEkranı.tscn"); };
    }
}
