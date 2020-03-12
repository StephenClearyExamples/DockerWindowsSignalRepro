using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace DockerWindowsSignalRepro
{
    class Program
    {
        static Task Main(string[] args) => new Program().RunAsync();

        public Program()
        {
            _cts = new CancellationTokenSource();
            if (!SetConsoleCtrlHandler(ConsoleCtrlHandler, add: true))
                throw Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error());
        }

        public async Task RunAsync()
        {
            while (true)
            {
                Console.WriteLine($"Running at: {DateTimeOffset.Now}");
                await Task.Delay(1000, _cts.Token);
            }
        }

        private readonly CancellationTokenSource _cts;

        private bool ConsoleCtrlHandler(ConsoleControlEvent controlType)
        {
            if (controlType == ConsoleControlEvent.CTRL_C_EVENT ||
                controlType == ConsoleControlEvent.CTRL_CLOSE_EVENT ||
                controlType == ConsoleControlEvent.CTRL_SHUTDOWN_EVENT)
            {
                Console.WriteLine($"Received event: {controlType}");
                Task.Run(() => _cts.Cancel());
                return true;
            }

            return false;
        }

        private enum ConsoleControlEvent : uint
        {
            CTRL_C_EVENT = 0,
            CTRL_CLOSE_EVENT = 2,
            CTRL_SHUTDOWN_EVENT = 6,
        }

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private delegate bool SetConsoleCtrlHandlerHandlerRoutine(ConsoleControlEvent controlType);

        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetConsoleCtrlHandler(SetConsoleCtrlHandlerHandlerRoutine handler,
            [MarshalAs(UnmanagedType.Bool)] bool add);
    }
}
