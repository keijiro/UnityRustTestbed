using UnityEngine;
using System.Runtime.InteropServices;
using IntPtr = System.IntPtr;
using StringBuilder = System.Text.StringBuilder;

public sealed class Test : MonoBehaviour
{
    #if !UNITY_EDITOR && (UNITY_IOS || UNITY_WEBGL)
    const string _dll = "__Internal";
    #else
    const string _dll = "testbed";
    #endif

    //
    // Bool args/return
    //

    [DllImport(_dll)]
    [return: MarshalAs(UnmanagedType.U1)]
    static extern bool get_xor([MarshalAs(UnmanagedType.U1)] bool a,
                               [MarshalAs(UnmanagedType.U1)] bool b);

    void RunBoolTest()
    {
        Debug.Log($"true ^ true = {get_xor(true, true)}");
        Debug.Log($"true ^ false = {get_xor(true, false)}");
    }

    //
    // Struct layout
    //

    struct Data1
    {
        public byte   d8;
        public ushort d16;
        public uint   d32;
        public ulong  d64;
    }

    struct Data2
    {
        [MarshalAs(UnmanagedType.U1)] public bool d1;
        [MarshalAs(UnmanagedType.U1)] public bool d2;
        public float  d32;
        public double d64;
    }

    [DllImport(_dll)]
    static extern void modify_struct_data1(in Data1 src, out Data1 dst);

    [DllImport(_dll)]
    static extern void modify_inplace_struct_data1(ref Data1 data);

    [DllImport(_dll)]
    static extern void modify_struct_data2(in Data2 src, out Data2 dst);

    [DllImport(_dll)]
    static extern void modify_inplace_struct_data2(ref Data2 data);

    static void RunStructTest()
    {
        var data1 = new Data1
        {
            d8  = 0x12,
            d16 = 0x3456,
            d32 = 0x789abcdeu,
            d64 = 0x123456789abcdef0ul
        };

        var data2 = new Data2
        {
            d1  = false,
            d2  = true,
            d32 = Mathf.PI,
            d64 = System.Math.PI
        };

        var temp1 = new Data1();
        var temp2 = new Data2();

        modify_struct_data1(in data1, out temp1);
        modify_struct_data2(in data2, out temp2);

        modify_inplace_struct_data1(ref data1);
        modify_inplace_struct_data2(ref data2);

        Debug.Log("Data1 (must be 13, 3457, 789ABCDF, 123456789ABCDEF1)");
        Debug.Log($"{temp1.d8:X} {temp1.d16:X} {temp1.d32:X} {temp1.d64:X}");
        Debug.Log($"{data1.d8:X} {data1.d16:X} {data1.d32:X} {data1.d64:X}");

        Debug.Log("Data2 (must be true, false, pi+1, pi+1)");
        Debug.Log($"{temp2.d1} {temp2.d2} {temp2.d32} {temp2.d64}");
        Debug.Log($"{data2.d1} {data2.d2} {data2.d32} {data2.d64}");
    }

    //
    // String operations
    //

    [DllImport(_dll)]
    static extern uint count_substring(string s, string sub);

    [DllImport(_dll, EntryPoint="count_substring")]
    static extern uint count_substring2(StringBuilder sb, string sub);

    [DllImport(_dll)]
    static extern void set_string(StringBuilder sb, int size);

    [DllImport(_dll)]
    static extern IntPtr format_float(float value);

    static void RunStringTest()
    {
        var str = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";
        var sub = "do";

        var c1 = count_substring(str, sub);
        var c2 = count_substring2(new StringBuilder(str), sub);

        Debug.Log($"Substring search (should be 6): {c1}");
        Debug.Log($"With StringBuilder: {c2}");

        var sb = new StringBuilder(256);
        set_string(sb, sb.Capacity + 1);
        Debug.Log($"Modified string: {sb}");

        var fmt = Marshal.PtrToStringAnsi(format_float(Mathf.PI));
        Debug.Log($"Returned string: {fmt}");
    }

    //
    // MonoBehaviour entry point
    //

    void Start()
    {
        RunBoolTest();
        RunStructTest();
        RunStringTest();
    }
}
