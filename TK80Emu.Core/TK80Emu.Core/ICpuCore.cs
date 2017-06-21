
namespace TK80Emu.Core
{
    public interface ICpuCore
    {
        void SetRam(RamBase ram);
        void SetDebugMode(bool flag);
        void OnClockCpu();
        void OnResetCpu();
        string GetCpuName();
        ushort GetProgramCounter();
    }
}
