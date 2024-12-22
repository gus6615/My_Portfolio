#if UNITASK_INSTALLED
using Cysharp.Threading.Tasks;
#endif

namespace DevelopKit
{
    public enum StartUpPriority
    {
        Default = 0,
    }
    
    public interface IStartUpTask
    {
        bool IsDone { get; set; }
        StartUpPriority Priority { get; }
#if UNITASK_INSTALLED && ADDRESSABLES_INSTALLED
        UniTask StartUp();
#endif
    }
}