using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace WpfApp6.Services.Launch;

public static class PSBasics
{
	public static Process _FortniteProcess;

	public static void Start(string PATH, string args, string Email, string Password)
	{
		if (Email == null || Password == null)
		{
			MessageBox.Show("Please add your Account Details in Login");
		}
		else if (File.Exists(Path.Combine(PATH, "FortniteGame\\Binaries\\Win64\\", "FortniteClient-Win64-Shipping.exe")))
		{
			Process process = new Process();
			process.StartInfo = new ProcessStartInfo
			{
				Arguments = "-AUTH_LOGIN=" + Email + " -AUTH_PASSWORD=" + Password + " -AUTH_TYPE=epic " + args,
				FileName = Path.Combine(PATH, "FortniteGame\\Binaries\\Win64\\", "FortniteClient-Win64-Shipping.exe")
			};
			process.EnableRaisingEvents = true;
			_FortniteProcess = process;
			_FortniteProcess.Exited += OnFortniteExit;
			_FortniteProcess.Start();
		}
	}

	public static void OnFortniteExit(object sender, EventArgs e)
	{
		Process fortniteProcess = _FortniteProcess;
		if (fortniteProcess != null && fortniteProcess.HasExited)
		{
			_FortniteProcess = null;
		}
		FakeAC._FNLauncherProcess?.Kill();
		FakeAC._FNAntiCheatProcess?.Kill();
	}
}
