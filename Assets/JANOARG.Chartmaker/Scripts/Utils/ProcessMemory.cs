using System.Diagnostics;

namespace JANOARG.Chartmaker.Utils
{
    /// <summary>
    /// Reads the whole-process memory footprint, i.e. everything the OS attributes to the
    /// running app - the engine runtime, native plugins, graphics driver and managed heap.
    /// This is the ground truth the Unity Profiler's own counters cannot see.
    /// Note: in the Editor this is the entire Unity Editor process, not a player build.
    /// </summary>
    public static class ProcessMemory
    {
        static Process _process;

        static Process Current
        {
            get
            {
                if (_process == null)
                {
                    try { _process = Process.GetCurrentProcess(); }
                    catch { _process = null; }
                }
                return _process;
            }
        }

        /// <summary>
        /// Returns the process memory footprint in bytes. <paramref name="workingSet"/> is the
        /// resident set (what the OS has physically backed), <paramref name="privateBytes"/> is
        /// the committed private memory. Either may be 0 if the platform does not report it.
        /// </summary>
        public static bool TryGet(out long workingSet, out long privateBytes)
        {
            workingSet = privateBytes = 0;

            Process process = Current;
            if (process == null) return false;

            try
            {
                process.Refresh();
                workingSet = process.WorkingSet64;
                privateBytes = process.PrivateMemorySize64;
                return workingSet > 0 || privateBytes > 0;
            }
            catch
            {
                return false;
            }
        }
    }
}
