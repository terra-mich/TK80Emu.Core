using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace TK80Emu.Core
{
    public abstract class RegisterBase<T> where T : IConvertible
    {
        private T data;
        public T Data
        {
            get { return data; }
            set { data = value; }
        }

        protected string name;

        public RegisterBase(string name)
        {
            data = default(T);
            this.name = name;
        }
        public RegisterBase(T d, string name)
        {
            data = d;
            this.name = name;
        }

        public byte this[int i]
        {
            get
            {
                if(data is string)
                {
                    return (byte)(data.ToString()[i]);
                }
                long l = GetValue(this);
                return (byte)((l >> i) & 0x1);
            }
            set
            {
                if (data is string)
                {
                    StringBuilder str = new StringBuilder(data.ToString());
                    str[i] = (char)value;
                    data = Parse(str.ToString()); 
                    return;
                }
                if (value > 1)
                {
                    value = 1;
                }
                long l = GetValue(this);
                data = Parse(l | ((long)value << i));
            }
        }
        public static long GetValue(RegisterBase<T> reg)
        {
            return GetValue(reg.data);
        }
        public static long GetValue(T data)
        {
            if(data is string)
            {
                long ret = 0;
                string str = data.ToString();
                for(int i = 0;i < str.Length;i++)
                {
                    ret += (1 << i) * str[i];
                }
                return ret;
            }
            return data.ToInt64(null);
        }
        public static T Parse(object dec)
        {
            return (T)Convert.ChangeType(dec, typeof(T));
        }
        #region Operations

        public void Add(T val,byte flag, bool carry = false)
        {
            if(data is string)
            {
                string str0 = data as string;
                string str1 = val as string;
                if(str0 != null && str1 != null)
                {
                    data = Parse(str0 + str1);
                }
                return;
            }
            data = Parse(GetValue(data) + GetValue(val) + (carry && flag == 1 ? 1 : 0));
        }
        public void Sub(T val,byte flag, bool borrow = false)
        {
            data = Parse(GetValue(data) - GetValue(val) - (borrow && flag == 1 ? 1 : 0));
        }
        public void Reset()
        {
            data = default(T);
        }
        public void Increment()
        {
            data = Parse(GetValue(data) + 1);
        }
        public void Decrement()
        {
            data = Parse(GetValue(data) - 1);
        }

        #endregion

        public string ToBinaryString()
        {
            int size = Marshal.SizeOf(data) << 3;
            Console.WriteLine(size + "bit");
            StringBuilder sb = new StringBuilder();
            for (int i = size - 1; i >= 0; i--)
            {
                sb.Append(this[(byte)i]);
            }
            return sb.ToString();
        }
        #region Overrides

        public override string ToString()
        {
            return GetValue(this).ToString();
        }
        #endregion

        #region Operators
        public static T operator +(RegisterBase<T> a, RegisterBase<T> b)
        {
            if(a.data is string || b.data is string)
            {
                return Parse(a.data.ToString() + b.data.ToString());
            }
            return Parse(GetValue(a.Data) + GetValue(b.Data));
        }
        public static T operator -(RegisterBase<T> a, RegisterBase<T> b)
        {
            if (a.data is string || b.data is string)
            {
                return Parse(a.data.ToString().Replace(b.data.ToString(),""));
            }
            return Parse(GetValue(a.Data) - GetValue(b.Data));
        }
        public static T operator ^(RegisterBase<T> a, RegisterBase<T> b)
        {
            if (a.data is string || b.data is string)
            {
                return default(T);
            }
            return Parse(GetValue(a.data) ^ GetValue(b));
        }
        public static T operator ^(RegisterBase<T> a, long val)
        {
            if (a.data is string)
            {
                return default(T);
            }
            return Parse(GetValue(a.data) ^ val);
        }
        public static T operator ~(RegisterBase<T> a)
        {
            if (a.data is string)
            {
                return default(T);
            }
            return Parse(GetValue(a.Data) ^ -1L);
        }
        #endregion
    }
}
