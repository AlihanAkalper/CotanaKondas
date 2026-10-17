using Godot;

public partial class FireEffect : Node
{
    [Export] private CpuParticles2D _fireParticles; // veya GpuParticles2D

    public void TriggerFire(float duration = 3.0f)
    {
        if (_fireParticles == null) return;

        // Ateşi başlat
        _fireParticles.Emitting = true;

        // Belirlenen süre sonunda ateşi söndür
        GetTree().CreateTimer(duration).Timeout += () =>
        {
            _fireParticles.Emitting = false;
        };
    }
}