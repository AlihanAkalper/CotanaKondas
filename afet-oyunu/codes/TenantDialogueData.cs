public enum DialogueTone { Positive, Neutral, Negative }
public record TenantDialogue(string Topic, string Question, string[] Answers);
public class TenantDialogueAnswer
{
    public string TenantId { get; set; }
    public Ev_numaraları? Region { get; set; }
    public int FloorNumber { get; set; }
    public int Day { get; set; }
    public int DialogueId { get; set; }
    public int AnswerIndex { get; set; }
    public DialogueTone Tone { get; set; }
}

public static class TenantDialogues
{
    public const double DailyChance = 0.10;
    // Her soruda sırasıyla iki pozitif, iki nötr ve iki negatif yanıt.
    public static readonly TenantDialogue[] All =
    {
        new("Deprem farkındalığı", "Evde deprem kiti neden yok?", new[] {
            "Hemen sipariş veriyorum ve hızlıca sana iletilmesini sağlayacağım.",
            "Kilerdeki kilitli dolapta bütün yardım kitlerini bulabilirsin.",
            "Şu anlık ihtiyaç olduğunu düşünmüyorum ama halletmeye çalışacağım.",
            "Kit tutarının yarısını karşılayabilirim.",
            "Burada uzun zamandır deprem olmuyor bu yüzden gerek olduğunu düşünmüyorum.",
            "Almak istersen kendin sipariş verebilirsin." }),
        new("Deprem farkındalığı", "Binamızın yakınındaki afet toplanma alanları nerede acaba?", new[] {
            "Belediyemizin sitesinden bakıp hemen sana ileteceğim.",
            "100 metre ilerideki çocuk parkının yanındaki boş alanda tabelasını görebilirsin.",
            "Belediyenin sitesinden bakıp bulabilirsin.",
            "Toplanma yeri değişmişti bu yüzden tekrardan bir araştırmanı öneriyorum.",
            "Evin önündeki sokak arasında bekleyebilirsin.",
            "Böyle şeyleri bana sormadan araştırabilirsin. Hiç internet diye bir şey duydun mu?" }),
        new("Deprem farkındalığı", "Evimizdeki sığınak deprem anında kullanılabiliyor mu?", new[] {
            "Tabii ki anahtarlığındaki küçük anahtar ile kapıyı açıp afet durumunda kullanabilirsin.",
            "Bina yöneticisinden bilgi alıp sana ivedilikle ileteceğim.",
            "Genelde depo olarak kullanılıyor bu yüzden tam olarak emin değilim.",
            "Kullanmak istediğinde bir sorun olacağını düşünmüyorum.",
            "Zaten deprem anında oraya inecek kadar vakit bulamazsın.",
            "Sığınak anahtarını sana vermeyeceğim için kullanamazsın." }),
        new("Yangın farkındalığı", "Binamızda yangın söndürme sistemi bulunuyor mu?", new[] {
            "Evet her odada yağmurlama sistemleri kurulu durumda bulunuyor.",
            "Her koridorun sonunda yangın söndürme hortumu bulunuyor.",
            "Evlerde yangın söndürücü var ve bunun yeteceğini düşünüyorum.",
            "Var olduğunu biliyorum ama çalışır durumda olup olmadığını bilmiyorum.",
            "Bakıp öğrenebileceğin şeylerle beni meşgul etme.",
            "Bakımını yaptırmadığım için çalışmıyor." }),
        new("Yangın farkındalığı", "Yangın anında dışarıdaki yangın merdiveni kullanılıyor mu?", new[] {
            "Yangın durumunda ilk seçenek olarak orayı tercih etmelisin.",
            "Yangın anında panik yapmadan ve diğer insanları riske atmadan kullanabilirsin.",
            "Emin değilim ama açıp kullanmayı deneyebilirsin.",
            "Yöneticiye yangın dairesinin anahtarını sana verip veremeyeceğini sorup döneceğim.",
            "Yangın olursa camdan atlayıp kaçabilirsin.",
            "Asansör kullanıp inersin yani merdivene ihtiyaç yok." }),
        new("Yangın farkındalığı", "Yangın anında itfaiye ulaşımı durumu nedir?", new[] {
            "112’yi aradığınızda çevrede çok yakın bir tesis var ve oradan acil araç kaldırılıyor.",
            "Yangın bölgesi olduğu için alınan önlemler sayesinde ulaşım hızlıca gerçekleşiyor.",
            "Konu hakkında herhangi bir fikrim yok.",
            "Ulaşım durumunda bir aksaklık olacağını düşünmüyorum.",
            "İtfaiye şefi gibi bir halim olduğunu sanmıyorum.",
            "Hiç yangın çıkarmadığım için bilmiyorum. Sen de çıkarmazsan öğrenmek zorunda kalmazsın." }),
        new("Sel farkındalığı", "Altyapı sistemimiz ne durumda?", new[] {
            "Yeni boru hatlarımız döşendi ve su tahliyesi geliştirildi.",
            "Mazgal temizliği yapıldı ve tüm borular kontrol edildi.",
            "Bir ara kontrol yapılıyordu ama ne yapıldığını bilmiyorum.",
            "Bilgim olan bir konu değil.",
            "Belediye başkanı olmadığım için sorunun mantığını anlamadım.",
            "Böyle spesifik soruları özellikle mi düşünüyorsun?" }),
        new("Sel farkındalığı", "Binamız dere yatağına inşa edilmiş, bununla ilgili herhangi bir önlem alındı mı?", new[] {
            "Su tahliyesi için özel bir tahliye sistemi inşa edildi.",
            "Dere yatağı yatayda başka bir yere doğru genişletilip tehlike teşkil etmekten çıktı.",
            "Nasıl bir riske yol açacağı konusunda fikrim yok.",
            "Riskli bölgesinde olup olmadığımızı araştırmam lazım.",
            "Bence binanın malzeme kalitesi yerine böyle boş detaylara odaklanma.",
            "Önlem alınmadıysa yeniden mi inşa edeceksin?" })
    };

}
