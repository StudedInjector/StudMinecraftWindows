using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace StudInjector
{
    public static class Injector
    {
        private const uint PROCESS_ALL_ACCESS = 0x1F0FFF;
        private const uint MEM_COMMIT = 0x00001000;
        private const uint MEM_RESERVE = 0x00002000;
        private const uint PAGE_READWRITE = 0x04;
        private const uint WAIT_TIMEOUT = 0x00000102;
        private const uint MEM_RELEASE = 0x8000;

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out IntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern uint WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint dwFreeType);

        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        public static async Task<bool> Inject(Action<string> statusCallback = null)
        {
            if (!IsAdministrator())
            {
                MessageBox.Show("Please run as administrator!");
                return false;
            }

            string dllPath = await DownloadDLLAsync(statusCallback);
            if (string.IsNullOrEmpty(dllPath))
            {
                return false;
            }

            if (!File.Exists(dllPath))
            {
                MessageBox.Show("DLL not found, your Antivirus might have deleted it.");
                return false;
            }

            if (new FileInfo(dllPath).Length < 10)
            {
                MessageBox.Show("DLL broken (Less than 10 bytes)");
                return false;
            }

            try
            {
                statusCallback?.Invoke("Setting file permissions");
                var fileInfo = new FileInfo(dllPath);
                var fileSecurity = fileInfo.GetAccessControl();
                fileSecurity.AddAccessRule(new FileSystemAccessRule(
                    new SecurityIdentifier("S-1-15-2-1"),
                    FileSystemRights.FullControl,
                    AccessControlType.Allow));
                fileInfo.SetAccessControl(fileSecurity);
            }
            catch
            {
                MessageBox.Show("Could not set permissions, try running the injector as admin.");
                return false;
            }

            statusCallback?.Invoke("Finding process");
            var processes = Process.GetProcessesByName("RobloxPLay");

            if (processes.Length == 0)
            {
                statusCallback?.Invoke("Launching Minecraft");
                try
                {
                    Process.Start("minecraft://");

                    int attempts = 0;
                    while (processes.Length == 0 && attempts++ < 200)
                    {
                        await Task.Delay(10);
                        processes = Process.GetProcessesByName("Minecraft.Windows");
                    }

                    if (processes.Length == 0)
                    {
                        MessageBox.Show("Minecraft launch took too long.");
                        return false;
                    }

                    await Task.Delay(3000);
                }
                catch
                {
                    MessageBox.Show("Failed to launch Minecraft (Is it installed?)");
                    return false;
                }
            }

            var process = processes.FirstOrDefault(p => p.Responding);
            if (process == null)
            {
                MessageBox.Show("Minecraft is not responding");
                return false;
            }

            if (process.Modules.Cast<ProcessModule>().Any(m => m.FileName == dllPath))
            {
                MessageBox.Show("Already injected!");
                return true;
            }

            statusCallback?.Invoke($"Injecting into {process.Id}");
            IntPtr hProcess = OpenProcess(PROCESS_ALL_ACCESS, false, process.Id);

            if (hProcess == IntPtr.Zero || !process.Responding)
            {
                MessageBox.Show("Failed to get process handle");
                return false;
            }

            IntPtr allocatedMem = IntPtr.Zero;
            IntPtr hThread = IntPtr.Zero;

            try
            {
                allocatedMem = VirtualAllocEx(hProcess, IntPtr.Zero, (uint)(dllPath.Length + 1), MEM_COMMIT | MEM_RESERVE, PAGE_READWRITE);
                if (allocatedMem == IntPtr.Zero)
                {
                    MessageBox.Show("Failed to allocate memory");
                    return false;
                }

                byte[] pathBytes = Encoding.ASCII.GetBytes(dllPath);
                if (!WriteProcessMemory(hProcess, allocatedMem, pathBytes, (uint)pathBytes.Length, out _))
                {
                    MessageBox.Show("Failed to write memory");
                    return false;
                }

                IntPtr loadLibAddr = GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryA");
                if (loadLibAddr == IntPtr.Zero)
                {
                    MessageBox.Show("Failed to get LoadLibrary address");
                    return false;
                }

                hThread = CreateRemoteThread(hProcess, IntPtr.Zero, 0, loadLibAddr, allocatedMem, 0, IntPtr.Zero);
                if (hThread == IntPtr.Zero)
                {
                    MessageBox.Show("Failed to create remote thread");
                    return false;
                }

                uint waitResult = WaitForSingleObject(hThread, 5000);
                if (waitResult == WAIT_TIMEOUT)
                {
                    MessageBox.Show("Injection timed out");
                    return false;
                }

                IntPtr mcWindow = FindWindow(null, "Minecraft");
                if (mcWindow != IntPtr.Zero)
                {
                    SetForegroundWindow(mcWindow);
                }

                return true;
            }
            finally
            {
                if (allocatedMem != IntPtr.Zero)
                    VirtualFreeEx(hProcess, allocatedMem, 0, MEM_RELEASE);

                if (hThread != IntPtr.Zero)
                    CloseHandle(hThread);

                if (hProcess != IntPtr.Zero)
                    CloseHandle(hProcess);
            }
        }

        private static async Task<string> DownloadDLLAsync(Action<string> statusCallback = null)
        {
            string dllDir = Path.Combine(Environment.CurrentDirectory, "dll");
            Directory.CreateDirectory(dllDir);
            string dllPath = Path.Combine(dllDir, "Horion.dll");
            string dllUrl = "https://horion.download/bin/Horion.dll";

            try
            {
                if (File.Exists(dllPath) && new FileInfo(dllPath).Length > 1024)
                {
                    statusCallback?.Invoke("Using cached DLL");
                    return dllPath;
                }

                statusCallback?.Invoke("Downloading DLL...");
                using (WebClient client = new WebClient())
                {
                    await client.DownloadFileTaskAsync(new Uri(dllUrl), dllPath);
                }

                if (new FileInfo(dllPath).Length < 1024)
                {
                    File.Delete(dllPath);
                    MessageBox.Show("Downloaded DLL is too small (possibly corrupted)");
                    return null;
                }

                return dllPath;
            }
            catch (Exception ex)
            {
                if (File.Exists(dllPath))
                    File.Delete(dllPath);

                MessageBox.Show($"Download failed: {ex.Message}");
                return null;
            }
        }

        private static bool IsAdministrator()
        {
            using (var identity = WindowsIdentity.GetCurrent())
            {
                var principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }
    }
}