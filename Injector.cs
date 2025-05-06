using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.VisualBasic;

namespace DotNet
{
    internal class Injector
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);
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

        public static void ShowNotification(string title, string message)
        {
            new ToastContentBuilder()
            .AddText(title)
            .AddText(message)
            .SetToastScenario(ToastScenario.Reminder);
        }

        public static string GetMinecraftInstallPath()
        {
            Process process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = "-Command \"(Get-AppxPackage -Name Microsoft.MinecraftUWP).InstallLocation\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            process.Start();
            string installPath = process.StandardOutput.ReadLine()?.Trim();
            process.WaitForExit();

            return installPath;
        }

        public static void LaunchMinecraftIfNeeded()
        {
            Process[] mcProcesses = Process.GetProcessesByName("Minecraft.Windows");
            if (mcProcesses.Length == 0)
            {
                if (Interaction.Shell("explorer.exe shell:appsFolder\\Microsoft.MinecraftUWP_8wekyb3d8bbwe!App", AppWinStyle.MinimizedFocus, false, -1) == 0)
                {
                    ShowNotification("Cr1tcal3Lib.dll", "Failed to launch Minecraft (Is it installed?)");
                }
            }
        }
        public static string DownloadDLL()
        {
            string dllUrl = "https://horion.download/bin/Horion.dll";
            string dllPath = Path.Combine(Path.GetTempPath(), "Horion.dll");

            try
            {
                using (WebClient wc = new WebClient())
                {
                    ShowNotification("Downloading DLL", "Fetching Horion.dll...");
                    wc.DownloadFile(dllUrl, dllPath);
                }

                if (!File.Exists(dllPath) || new FileInfo(dllPath).Length < 10)
                {
                    ShowNotification("Download Failed", "DLL is broken or missing.");
                    return null;
                }
                return dllPath;
            }
            catch (Exception ex)
            {
                ShowNotification("Download Error", $"Failed to download: {ex.Message}");
                return null;
            }
        }

        public static void Inject()
        {
            string dllPath = DownloadDLL();
            if (dllPath == null)
                return;

            var processes = Process.GetProcessesByName("Minecraft.Windows");
                LaunchMinecraftIfNeeded();
            Task.Delay(3000).Wait();
            IntPtr handle = OpenProcess(0x1F0FFF, false, processes.First().Id);
            IntPtr allocMemory = VirtualAllocEx(handle, IntPtr.Zero, (uint)(dllPath.Length + 1), 12288U, 64U);
            byte[] dllBytes = System.Text.Encoding.ASCII.GetBytes(dllPath);
            WriteProcessMemory(handle, allocMemory, dllBytes, (uint)dllBytes.Length, out _);

            IntPtr procAddress = GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryA");
            IntPtr remoteThread = CreateRemoteThread(handle, IntPtr.Zero, 0U, procAddress, allocMemory, 0U, IntPtr.Zero);

            ShowNotification("Injection Successful", "DLL successfully injected!");
        }
    }
}