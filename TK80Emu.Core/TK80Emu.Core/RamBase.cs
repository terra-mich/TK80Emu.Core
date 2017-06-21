
namespace TK80Emu.Core
{
    public class RamBase
    {
        protected byte[] data;
        public byte this[int index]
        {
            get { return data[index]; }
            set { data[index] = value; }
        }
        public int Count
        {
            get { return data.Length; }
        }
        public RamBase()
        {
            data = new byte[0x10000];
        }
        public RamBase(int length)
        {
            data = new byte[length];
        }
    }
}
