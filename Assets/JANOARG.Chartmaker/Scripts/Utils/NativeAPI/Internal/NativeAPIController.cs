
namespace JANOARG.Chartmaker.Utils.NativeAPI.Internal
{
    internal abstract class NativeAPIController<TSelf>
    {
        protected bool IsInitAttempted;

        public abstract bool IsSupported { get; }

        protected void EnsureInitialized()
        {
            if (!IsInitAttempted)
            {
                IsInitAttempted = true;
                Initialize();
            }
        }

        protected abstract bool Initialize();
    }    
}