using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using Shadow_Launcher.Resources.Extra;
using ZYTROZLauncher;
using ZYTROZLauncher.Utilities;

namespace Shadow_Launcher.Resources.AntiCheat;

public static class Anticheat
{
	private static string detectedItem = "";

	private static bool isTriggered = false;

	private static MainWindow mainWindow;

	private static readonly string[] suspiciousProcesses = new string[13]
	{
		"ProcessHacker.exe", "UUUClient.exe", "CheatEngine.exe", "x64dbg.exe", "OllyDbg.exe", "Winject.exe", "ExtremeInjector.exe", "Injector.exe", "GameGuardian.exe", "FridaServer.exe",
		"PEInjector.exe", "HackersDelight.exe", "Loader.exe"
	};

	private static readonly string[] suspiciousDlls = new string[11]
	{
		"cheat.dll", "cheeto.dll", "real.dll", "ril.dll", "1hack.dll", "2hack.dll", "3hack.dll", "raax.dll", "RaaxLLA-27213a0.dll", "RaaxMM-27213a0.dll",
		"Raax-OG-FN-Internal.dll"
	};

	private static readonly string[] suspiciousKeywords = new string[5] { "1hack", "raax", "kernel injector", "kernel injector usermode", "kdmapper" };

	private static readonly string[] suspiciousProductNames = new string[11]
	{
		"IGCSClient", "Process Hacker", "Cheat Engine", "x64dbg", "OllyDbg", "Winject", "Extreme Injector", "GameGuardian", "Frida", "PEInjector",
		"Hackers Delight"
	};

	private static readonly string[] disallowedModuleKeywords = new string[10] { "cheat", "cheet", "cheeto", "1hack", "2hack", "3hack", "4hack", "hack", "raax", "Raax-OG-FN-Internal" };

	public static bool IsTriggered => isTriggered;

	public static void Check()
	{
		isTriggered = false;
		string[] array = suspiciousProcesses;
		foreach (string suspiciousProcess in array)
		{
			if (IsProcessRunning(suspiciousProcess))
			{
				detectedItem = "Suspicious process detected: " + suspiciousProcess;
				isTriggered = true;
				break;
			}
		}
		if (IsProcessWithSuspiciousKeyword())
		{
			detectedItem = "Suspicious process detected with keyword.";
			isTriggered = true;
		}
		if (IsDllLoaded())
		{
			detectedItem = "Suspicious DLL detected.";
			isTriggered = true;
		}
		if (IsProcessWithSuspiciousProductName())
		{
			detectedItem = "Suspicious product name detected.";
			isTriggered = true;
		}
		if (isTriggered)
		{
			ShowDetectionMessage();
			TerminateFortniteClient();
		}
	}

	private static void ShowDetectionMessage()
	{
		DispatcherTimer timer = new DispatcherTimer
		{
			Interval = TimeSpan.FromSeconds(15.0)
		};
		timer.Tick += delegate
		{
			Application.Current.Shutdown();
		};
		timer.Start();
		MessageBoxResult result = MessageBox.Show(detectedItem ?? "", "Anti-Cheat Alert", MessageBoxButton.OK, MessageBoxImage.Exclamation);
		timer.Stop();
		if (result == MessageBoxResult.OK)
		{
			RPC.StopRPC();
			Utils.KillProcess("FortniteClient-Win64-Shipping");
			Utils.KillProcess("FortniteClient-Win64-Shipping_BE");
			Utils.KillProcess("FortniteClient-Win64-Shipping_EAC");
			Utils.KillProcess("EasyAntiCheat_EOS_Setup");
			Utils.KillProcess("ShadowEAC");
			Application.Current.Shutdown();
		}
	}

	public static void SetMainWindow(MainWindow window)
	{
		mainWindow = window;
	}

	private static bool IsProcessRunning(string processName)
	{
		try
		{
			return Process.GetProcessesByName(processName.Replace(".exe", "")).Length != 0;
		}
		catch
		{
			return false;
		}
	}

	private static bool IsProcessWithSuspiciousKeyword()
	{
		try
		{
			Process[] processes = Process.GetProcesses();
			foreach (Process process in processes)
			{
				string[] array = suspiciousKeywords;
				foreach (string suspiciousKeyword in array)
				{
					if (process.ProcessName.IndexOf(suspiciousKeyword, StringComparison.OrdinalIgnoreCase) >= 0)
					{
						return true;
					}
				}
			}
		}
		catch
		{
		}
		return false;
	}

	private static bool IsDllLoaded()
	{
		try
		{
			Process[] processes = Process.GetProcesses();
			foreach (Process process in processes)
			{
				try
				{
					if (process.Modules.Cast<ProcessModule>().Any((ProcessModule module) => IsSuspiciousDll(module.ModuleName)))
					{
						return true;
					}
				}
				catch
				{
				}
			}
		}
		catch
		{
		}
		return false;
	}

	private static bool IsSuspiciousDll(string dllName)
	{
		return new string[3] { "fortnitecheat", "cheet", "cheeto" }.Any((string substring) => dllName.IndexOf(substring, StringComparison.OrdinalIgnoreCase) >= 0);
	}

	private static bool IsProcessWithSuspiciousProductName()
	{
		try
		{
			Process[] processes = Process.GetProcesses();
			foreach (Process process in processes)
			{
				try
				{
					string productName = GetProductName(process);
					if (suspiciousProductNames.Any((string suspiciousName) => productName.IndexOf(suspiciousName, StringComparison.OrdinalIgnoreCase) >= 0))
					{
						return true;
					}
				}
				catch
				{
				}
			}
		}
		catch
		{
		}
		return false;
	}

	private static string GetProductName(Process process)
	{
		try
		{
			return FileVersionInfo.GetVersionInfo(process.MainModule.FileName).ProductName;
		}
		catch
		{
			return string.Empty;
		}
	}

	private static void TerminateFortniteClient()
	{
		try
		{
			Process[] processesByName = Process.GetProcessesByName("FortniteClient-Win64-Shipping");
			foreach (Process process in processesByName)
			{
				process.Kill();
			}
		}
		catch
		{
		}
	}
}
