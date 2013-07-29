#if NET20

namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Assembly)]
    sealed class ExtensionAttribute : Attribute { }
}

namespace System
{
    public delegate void Action();
    public delegate void Action<in T1, in T2>(T1 arg1, T2 arg2);
    public delegate void Action<in T1, in T2, in T3>(T1 arg1, T2 arg2, T3 arg3);
    public delegate void Action<in T1, in T2, in T3, in T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4);

    public delegate TRet Func<out TRet>();
    public delegate TRet Func<in T, out TRet>(T a);
    public delegate TRet Func<in T1, in T2, out TRet>(T1 arg1, T2 arg2);
    public delegate TRet Func<in T1, in T2, in T3, out TRet>(T1 arg1, T2 arg2, T3 arg3);
    public delegate TRet Func<in T1, in T2, in T3, in T4, out TRet>(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
}

#endif