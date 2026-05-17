
using JANOARG.Chartmaker.Utils.NativeAPI.Internal.NativeWindow;

namespace JANOARG.Chartmaker.Utils.NativeAPI.Internal
{
    internal interface INativeAPIProvider<T>
    {
        static NativeWindowController Controller = new NativeWindowController();

        
    }    
}