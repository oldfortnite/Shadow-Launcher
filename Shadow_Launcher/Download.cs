using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Shadow_Launcher_V2;
using Shadow_Launcher.Resources.AntiCheat;
using Shadow_Launcher.Resources.Extra;
using ZYTROZLauncher;
using ZYTROZLauncher.Resources;
using ZYTROZLauncher.Utilities;

namespace Shadow_Launcher;

public partial class Download : Window, IComponentConnector
{
	public class ClientInfo
	{
		public ClientsInfo Clients { get; set; }
	}

	public class ClientsInfo
	{
		public int amount { get; set; }

		public string[] clients { get; set; }
	}

	public class User
	{
		[BsonId]
		public ObjectId Id { get; set; }

		[BsonElement("username")]
		public string Username { get; set; }

		[BsonElement("email")]
		public string Email { get; set; }

		[BsonElement("banned")]
		public bool Banned { get; set; }
	}

	private bool _isLaunchingTheGame = false;

	private readonly Timer _timer;

	private static string hwid = string.Empty;

	private static readonly string AppDataFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ShadowLauncher");

	private static readonly string WebhookStatusFilePath = Path.Combine(AppDataFolderPath, "webhook_status.txt");

	private string _username = string.Empty;

	public Download()
	{
		InitializeComponent();
		ApplySettings();
		RPC.StartRPC();
		string storedEmail = UpdateINI.ReadValue("Auth", "Email");
		string storedPassword = UpdateINI.ReadValue("Auth", "Password");
		_timer = new Timer(UpdatePlayerCount, null, TimeSpan.Zero, TimeSpan.FromSeconds(5.0));
	}

	private async void Build_Click(object sender, RoutedEventArgs e)
	{
		string downloadUrl = "https://public.simplyblk.xyz/a12.41.rar";
		string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
		string zipFilePath = Path.Combine(baseDirectory, "ShadowInstaller.zip");
		string extractPath = Path.Combine(baseDirectory, "ShadowInstaller");
		string installerPath = Path.Combine(extractPath, "ShadowInstaller.exe");
		try
		{
			Downloadbtn.IsEnabled = false;
			Downloadbtn.Content = "Loading...";
			HttpClient client = new HttpClient();
			try
			{
				File.WriteAllBytes(zipFilePath, await client.GetByteArrayAsync(downloadUrl));
			}
			finally
			{
				((IDisposable)client)?.Dispose();
			}
			if (Directory.Exists(extractPath))
			{
				Directory.Delete(extractPath, recursive: true);
			}
			ZipFile.ExtractToDirectory(zipFilePath, extractPath);
			File.Delete(zipFilePath);
			if (File.Exists(installerPath))
			{
				Process.Start(installerPath);
			}
			else
			{
				MessageBox.Show("Shadow Installer file not found in the extracted directory. Contact Owner", "Error", MessageBoxButton.OK, MessageBoxImage.Hand);
			}
		}
		catch (Exception ex)
		{
			Exception ex2 = ex;
			MessageBox.Show("An error occurred: " + ex2.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
		finally
		{
			Downloadbtn.IsEnabled = true;
			Downloadbtn.Content = "Download";
		}
	}

	private void Verify_Click(object sender, RoutedEventArgs e)
	{
		string downloadUrl = "https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-6.0.16-windows-x64-installer";
		try
		{
			VerifyButton.IsEnabled = false;
			VerifyButton.Content = "Loading...";
			Process.Start(new ProcessStartInfo
			{
				FileName = downloadUrl,
				UseShellExecute = true
			});
		}
		catch (Exception ex)
		{
			MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
		finally
		{
			VerifyButton.IsEnabled = true;
			VerifyButton.Content = "Crash Fix";
		}
	}

	private void AntiCheatTimer_Tick(object sender, EventArgs e)
	{
		Anticheat.Check();
	}

	private void ApplySettings()
	{
		bool result;
		bool alwaysOnTop = bool.TryParse(UpdateINI.ReadValue("Settings", "AlwaysOnTop"), out result) && result;
		base.Topmost = alwaysOnTop;
	}

	public static void DeleteDirectory(string target_dir)
	{
		string[] files = Directory.GetFiles(target_dir);
		string[] dirs = Directory.GetDirectories(target_dir);
		string[] array = files;
		foreach (string file in array)
		{
			File.SetAttributes(file, FileAttributes.Normal);
			File.Delete(file);
		}
		string[] array2 = dirs;
		foreach (string dir in array2)
		{
			DeleteDirectory(dir);
		}
		Directory.Delete(target_dir, recursive: false);
	}

	private void LauncherClosed(object sender, EventArgs e)
	{
		RPC.StopRPC();
		Utils.KillProcess("FortniteClient-Win64-Shipping");
		Utils.KillProcess("FortniteClient-Win64-Shipping_BE");
		Utils.KillProcess("FortniteClient-Win64-Shipping_EAC");
		Utils.KillProcess("FortniteLauncher");
		Utils.KillProcess("EasyAntiCheat_EOS_Setup");
		Utils.KillProcess("ShadowEAC");
	}

	private string GetJsonBody(string url, string body)
	{
		WebRequest webRequest = WebRequest.Create(url);
		webRequest.ContentType = "application/json";
		webRequest.Method = "GET";
		object value = webRequest.GetType().GetProperty("CurrentMethod", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(webRequest);
		value.GetType().GetField("ContentBodyNotAllowed", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(value, false);
		using (StreamWriter streamWriter = new StreamWriter(webRequest.GetRequestStream()))
		{
			streamWriter.Write(body);
		}
		WebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();
		string result = "";
		using (Stream responseStream = webResponse.GetResponseStream())
		{
			result = new StreamReader(responseStream, Encoding.UTF8).ReadToEnd();
		}
		return result;
	}

	private bool ValidateCredentialsAsync(string email, string password)
	{
		try
		{
			return GetJsonBody("http://127.0.0.1:3551/login", "{\"email\":\"" + email + "\", \"password\":\"" + password + "\"}") == "true";
		}
		catch (Exception ex)
		{
			MessageBox.Show("Error: " + ex.Message);
			return false;
		}
	}

	private void UpdatePlayerCount(object state)
	{
		try
		{
			string url = "http://127.0.0.1:443/";
			WebClient client = new WebClient();
			string jsonString = client.DownloadString(url);
			ClientInfo clientInfo = JsonConvert.DeserializeObject<ClientInfo>(jsonString);
			int numberOfClients = clientInfo.Clients.amount;
			base.Dispatcher.Invoke(() => PlayerCountTextBlock.Text = $"{numberOfClients}");
		}
		catch (Exception)
		{
		}
	}

	private async void ShowError(string message)
	{
		ErrorGrid.Visibility = Visibility.Visible;
		ErrorButton.Content = message;
		DoubleAnimation fadeInAnimation = new DoubleAnimation
		{
			From = 0.0,
			To = 1.0,
			Duration = TimeSpan.FromSeconds(1.0)
		};
		ErrorGrid.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);
		await Task.Delay(3000);
		DoubleAnimation fadeOutAnimation = new DoubleAnimation
		{
			From = 1.0,
			To = 0.0,
			Duration = TimeSpan.FromSeconds(1.0)
		};
		ErrorGrid.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);
		await Task.Delay(3000);
		ErrorGrid.Visibility = Visibility.Collapsed;
	}

	private async void ShowSuccess(string message)
	{
		SuccessGrid.Visibility = Visibility.Visible;
		SuccessButton.Content = message;
		DoubleAnimation fadeInAnimation = new DoubleAnimation
		{
			From = 0.0,
			To = 1.0,
			Duration = TimeSpan.FromSeconds(1.0)
		};
		SuccessGrid.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);
		await Task.Delay(3000);
		DoubleAnimation fadeOutAnimation = new DoubleAnimation
		{
			From = 1.0,
			To = 0.0,
			Duration = TimeSpan.FromSeconds(1.0)
		};
		SuccessGrid.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);
		await Task.Delay(3000);
		SuccessGrid.Visibility = Visibility.Collapsed;
	}

	private void FadeInLoadingBuffer()
	{
		DoubleAnimation fadeInAnimation = new DoubleAnimation
		{
			From = 0.0,
			To = 1.0,
			Duration = TimeSpan.FromMilliseconds(200.0)
		};
		Storyboard.SetTargetProperty(fadeInAnimation, new PropertyPath(UIElement.OpacityProperty));
		Storyboard storyboard = new Storyboard();
		storyboard.Children.Add(fadeInAnimation);
		storyboard.Begin();
	}

	private void FadeOutLoadingBuffer()
	{
		DoubleAnimation fadeInAnimation = new DoubleAnimation
		{
			From = 1.0,
			To = 0.0,
			Duration = TimeSpan.FromMilliseconds(200.0)
		};
		Storyboard.SetTargetProperty(fadeInAnimation, new PropertyPath(UIElement.OpacityProperty));
		Storyboard storyboard = new Storyboard();
		storyboard.Children.Add(fadeInAnimation);
		storyboard.Begin();
	}

	private void Button_Click_1(object sender, RoutedEventArgs e)
	{
	}

	private void LearnMore_Click(object sender, RoutedEventArgs e)
	{
		Process.Start("https://discord.gg/Shadowmp");
	}

	private void Donate_Click(object sender, RoutedEventArgs e)
	{
		Process.Start("https://projectShadow.mysellix.io/");
	}

	private void Home_Click(object sender, RoutedEventArgs e)
	{
		new MainWindow().Show();
		Close();
	}

	private void Servers_Click(object sender, RoutedEventArgs e)
	{
		new Servers().Show();
		Close();
	}

	private void Download_Click(object sender, RoutedEventArgs e)
	{
		new Download().Show();
		Close();
	}

	private void Settings_Click(object sender, RoutedEventArgs e)
	{
		Settings main = new Settings();
		main.Show();
		Close();
	}

	private void Shop_Click(object sender, RoutedEventArgs e)
	{
		Window shopWindow = new Window();
		shopWindow.Show();
	}

	private void Close_Click(object sender, RoutedEventArgs e)
	{
		Close();
	}

	private void EmailBox_TextChanged(object sender, TextChangedEventArgs e)
	{
	}

	private void Minimize_Click(object sender, RoutedEventArgs e)
	{
		base.WindowState = WindowState.Minimized;
	}

	private void Window_MouseDown(object sender, MouseButtonEventArgs e)
	{
		if (e.LeftButton == MouseButtonState.Pressed && e.GetPosition(this).Y <= 30.0)
		{
			DragMove();
		}
	}
}
