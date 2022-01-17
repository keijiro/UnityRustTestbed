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
    static extern void modify_string(StringBuilder sb, int size);

    [DllImport(_dll)]
    static extern IntPtr format_float(float value);

    void Start()
    {
        Debug.Log(count_substring("hi hi high ho hige", "hi"));

        var sb = new StringBuilder("hi hi high ho hige");
        Debug.Log(count_substring2(sb, "hi"));

        var sb2 = new StringBuilder(256);
        modify_string(sb2, sb2.Capacity + 1);
        Debug.Log(sb2);

        var txt = Marshal.PtrToStringAnsi(format_float(Mathf.PI));
        Debug.Log(txt);
    }
}
