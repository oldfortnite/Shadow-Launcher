using System;
using System.Diagnostics;
using System.Net;
using System.Windows;

namespace ZYTROZLauncher.Utilities;

internal class Utils
{
	public static void KillProcess(string name)
	{
		try
		{
			Process[] processes = Process.GetProcessesByName(name);
			Process[] array = processes;
			foreach (Process process in array)
			{
				process.Kill();
			}
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message);
		}
	}

	public static void DownloadFile(string url, string path)
	{
		try
		{
			using WebClient client = new WebClient();
			client.DownloadFile(url, path);
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message);
		}
	}
}
