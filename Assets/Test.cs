using UnityEngine;
using System.Runtime.InteropServices;
using IntPtr = System.IntPtr;
using StringBuilder = System.Text.StringBuilder;

public sealed class Test : MonoBehaviour
{
    #if !UNITY_EDITOR && PLATFORM_IOS
    const string _dll = "__Internal";
    #else
    const string _dll = "testbed";
    #endif

    [DllImport(_dll)]
    static extern uint count_substring(string s, string sub);

    [DllImport(_dll, EntryPoint="count_substring")]
    static extern uint count_substring2(StringBuilder sb, string sub);

    [DllImport(_dll)]
    static extern void set_string(StringBuilder sb, int size);

    [DllImport(_dll)]
    static extern IntPtr format_float(float value);

    void Start()
    {
        var str = "hi hi high ho hige";
        var sub = "hi";

        Debug.Log(count_substring(str, sub));
        Debug.Log(count_substring2(new StringBuilder(str), sub));

        var sb = new StringBuilder(256);
        set_string(sb, sb.Capacity + 1);
        Debug.Log(sb);

        Debug.Log(Marshal.PtrToStringAnsi(format_float(Mathf.PI)));
    }
}
