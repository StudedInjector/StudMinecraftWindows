using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.VisualBasic;

namespace DotNet
{
    internal class Injector
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr OpenProcess(int a, bool b, int c);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr VirtualAllocEx(IntPtr p, IntPtr q, uint r, uint s, uint t);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool WriteProcessMemory(IntPtr p, IntPtr q, byte[] r, uint s, out IntPtr t);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr CreateRemoteThread(IntPtr p, IntPtr q, uint r, IntPtr s, IntPtr t, uint u, IntPtr v);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string n);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetProcAddress(IntPtr m, string p);

        static void Notify(string t, string m) =>
            Console.WriteLine($"{t}: {m}");

        static string MCPath() => ShellExec("powershell.exe", "-Command \"(Get-AppxPackage -Name Microsoft.MinecraftUWP).InstallLocation\"")?.Trim();

        static void mc()
        {
            if (!Process.GetProcessesByName("Minecraft.Windows").Any())
                if (Interaction.Shell("explorer.exe shell:appsFolder\\Microsoft.MinecraftUWP_8wekyb3d8bbwe!App", AppWinStyle.MinimizedFocus, false, -1) == 0)
                    Notify("DLL", "MC fail?");
        }

        static string dll()
        {
            string u = "https://horion.download/bin/Horion.dll";
            string p = Path.Combine(Path.GetTempPath(), "Horion.dll");

            try
            {
                new WebClient().DownloadFile(u, p);
                return File.Exists(p) && new FileInfo(p).Length >= 10 ? p : throw new Exception("Bad DLL.");
            }
            catch (Exception e)
            {
                Notify("DL Error", $"Fail: {e.Message}");
                return null;
            }
        }

        public static void Inject()
        {
            string d = dll();
            if (d == null) return;

            mc();
            Task.Delay(3000).Wait();

            var mcp = Process.GetProcessesByName("Minecraft.Windows").FirstOrDefault();
            if (mcp == null) return;

            IntPtr h = OpenProcess(0x1F0FFF, false, mcp.Id);
            IntPtr mem = VirtualAllocEx(h, IntPtr.Zero, (uint)d.Length + 1, 12288U, 64U);
            WriteProcessMemory(h, mem, System.Text.Encoding.ASCII.GetBytes(d), (uint)d.Length, out _);

            CreateRemoteThread(h, IntPtr.Zero, 0U, GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryA"), mem, 0U, IntPtr.Zero);

        }

        static string ShellExec(string f, string a)
        {
            var p = new Process
            {
                StartInfo = new ProcessStartInfo(f, a) { RedirectStandardOutput = true, UseShellExecute = false, CreateNoWindow = true }
            };
            p.Start();
            return p.StandardOutput.ReadLine();
        }
    }
}