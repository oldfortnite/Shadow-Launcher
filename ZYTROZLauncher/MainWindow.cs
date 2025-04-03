using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Shadow_Launcher;
using Shadow_Launcher_V2;
using Shadow_Launcher.Resources.Extra;
using WpfApp6.Services.Launch;
using ZYTROZLauncher.Resources;
using ZYTROZLauncher.Utilities;

namespace ZYTROZLauncher;

public partial class MainWindow : Window, IComponentConnector
{
	public class Vars
	{
		public static string Email = "NONE";

		public static string Password = "NONE";

		public static string Path1 = "NONE";
	}

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

	private static bool hwidFetched = false;

	private DispatcherTimer antiCheatTimer;

	private static string hwid = string.Empty;

	private string _username = string.Empty;

	private Thread launcherThread;

	private bool running = false;

	[DllImport("user32.dll")]
	private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

	public MainWindow()
	{
		InitializeComponent();
		ApplySettings();
		StartImageAnimation();
		RPC.StartRPC();
	}

	private void StartImageAnimation()
	{
		Storyboard storyboard = new Storyboard();
		TimeSpan fadeDuration = TimeSpan.FromSeconds(1.0);
		TimeSpan visibleDuration = TimeSpan.FromSeconds(2.0);
		DoubleAnimation fadeInImage1 = new DoubleAnimation
		{
			From = 0.0,
			To = 1.0,
			Duration = new Duration(fadeDuration),
			BeginTime = TimeSpan.FromSeconds(0.0)
		};
		Storyboard.SetTarget(fadeInImage1, Image1);
		Storyboard.SetTargetProperty(fadeInImage1, new PropertyPath("Opacity"));
		storyboard.Children.Add(fadeInImage1);
		DoubleAnimation fadeOutImage1 = new DoubleAnimation
		{
			From = 1.0,
			To = 0.0,
			Duration = new Duration(fadeDuration),
			BeginTime = fadeDuration + visibleDuration
		};
		Storyboard.SetTarget(fadeOutImage1, Image1);
		Storyboard.SetTargetProperty(fadeOutImage1, new PropertyPath("Opacity"));
		storyboard.Children.Add(fadeOutImage1);
		ObjectAnimationUsingKeyFrames visibilityChangeImage1Visible = new ObjectAnimationUsingKeyFrames
		{
			BeginTime = TimeSpan.FromSeconds(0.0),
			Duration = new Duration(TimeSpan.FromSeconds(0.0))
		};
		visibilityChangeImage1Visible.KeyFrames.Add(new DiscreteObjectKeyFrame(Visibility.Visible, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.0))));
		Storyboard.SetTarget(visibilityChangeImage1Visible, Image1);
		Storyboard.SetTargetProperty(visibilityChangeImage1Visible, new PropertyPath("Visibility"));
		storyboard.Children.Add(visibilityChangeImage1Visible);
		ObjectAnimationUsingKeyFrames visibilityChangeImage1Collapsed = new ObjectAnimationUsingKeyFrames
		{
			BeginTime = fadeDuration + visibleDuration + fadeDuration,
			Duration = new Duration(TimeSpan.FromSeconds(0.0))
		};
		visibilityChangeImage1Collapsed.KeyFrames.Add(new DiscreteObjectKeyFrame(Visibility.Collapsed, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.0))));
		Storyboard.SetTarget(visibilityChangeImage1Collapsed, Image1);
		Storyboard.SetTargetProperty(visibilityChangeImage1Collapsed, new PropertyPath("Visibility"));
		storyboard.Children.Add(visibilityChangeImage1Collapsed);
		ObjectAnimationUsingKeyFrames visibilityChangeImage2Visible = new ObjectAnimationUsingKeyFrames
		{
			BeginTime = fadeDuration + visibleDuration + fadeDuration,
			Duration = new Duration(TimeSpan.FromSeconds(0.0))
		};
		visibilityChangeImage2Visible.KeyFrames.Add(new DiscreteObjectKeyFrame(Visibility.Visible, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.0))));
		Storyboard.SetTarget(visibilityChangeImage2Visible, Image2);
		Storyboard.SetTargetProperty(visibilityChangeImage2Visible, new PropertyPath("Visibility"));
		storyboard.Children.Add(visibilityChangeImage2Visible);
		DoubleAnimation fadeInImage2 = new DoubleAnimation
		{
			From = 0.0,
			To = 1.0,
			Duration = new Duration(fadeDuration),
			BeginTime = fadeDuration + visibleDuration + fadeDuration
		};
		Storyboard.SetTarget(fadeInImage2, Image2);
		Storyboard.SetTargetProperty(fadeInImage2, new PropertyPath("Opacity"));
		storyboard.Children.Add(fadeInImage2);
		DoubleAnimation fadeOutImage2 = new DoubleAnimation
		{
			From = 1.0,
			To = 0.0,
			Duration = new Duration(fadeDuration),
			BeginTime = fadeDuration + visibleDuration + fadeDuration + fadeDuration + visibleDuration
		};
		Storyboard.SetTarget(fadeOutImage2, Image2);
		Storyboard.SetTargetProperty(fadeOutImage2, new PropertyPath("Opacity"));
		storyboard.Children.Add(fadeOutImage2);
		ObjectAnimationUsingKeyFrames visibilityChangeImage2Collapsed = new ObjectAnimationUsingKeyFrames
		{
			BeginTime = fadeDuration + visibleDuration + fadeDuration + fadeDuration + visibleDuration + fadeDuration,
			Duration = new Duration(TimeSpan.FromSeconds(0.0))
		};
		visibilityChangeImage2Collapsed.KeyFrames.Add(new DiscreteObjectKeyFrame(Visibility.Collapsed, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.0))));
		Storyboard.SetTarget(visibilityChangeImage2Collapsed, Image2);
		Storyboard.SetTargetProperty(visibilityChangeImage2Collapsed, new PropertyPath("Visibility"));
		storyboard.Children.Add(visibilityChangeImage2Collapsed);
		ColorAnimation dot1FillWhite = new ColorAnimation
		{
			From = Colors.Gray,
			To = Colors.White,
			Duration = new Duration(fadeDuration),
			BeginTime = TimeSpan.FromSeconds(0.0)
		};
		Storyboard.SetTarget(dot1FillWhite, Dot1);
		Storyboard.SetTargetProperty(dot1FillWhite, new PropertyPath("(Ellipse.Fill).(SolidColorBrush.Color)"));
		storyboard.Children.Add(dot1FillWhite);
		ColorAnimation dot1FillGray = new ColorAnimation
		{
			From = Colors.White,
			To = Colors.Gray,
			Duration = new Duration(fadeDuration),
			BeginTime = fadeDuration + visibleDuration
		};
		Storyboard.SetTarget(dot1FillGray, Dot1);
		Storyboard.SetTargetProperty(dot1FillGray, new PropertyPath("(Ellipse.Fill).(SolidColorBrush.Color)"));
		storyboard.Children.Add(dot1FillGray);
		ColorAnimation dot2FillWhite = new ColorAnimation
		{
			From = Colors.Gray,
			To = Colors.White,
			Duration = new Duration(fadeDuration),
			BeginTime = fadeDuration + visibleDuration + fadeDuration + fadeDuration
		};
		Storyboard.SetTarget(dot2FillWhite, Dot2);
		Storyboard.SetTargetProperty(dot2FillWhite, new PropertyPath("(Ellipse.Fill).(SolidColorBrush.Color)"));
		storyboard.Children.Add(dot2FillWhite);
		ColorAnimation dot2FillGray = new ColorAnimation
		{
			From = Colors.White,
			To = Colors.Gray,
			Duration = new Duration(fadeDuration),
			BeginTime = fadeDuration + visibleDuration + fadeDuration + fadeDuration + visibleDuration
		};
		Storyboard.SetTarget(dot2FillGray, Dot2);
		Storyboard.SetTargetProperty(dot2FillGray, new PropertyPath("(Ellipse.Fill).(SolidColorBrush.Color)"));
		storyboard.Children.Add(dot2FillGray);
		storyboard.RepeatBehavior = RepeatBehavior.Forever;
		storyboard.Begin();
	}

	private void Dot1_Click(object sender, MouseButtonEventArgs e)
	{
		Image1.Visibility = Visibility.Visible;
		Image1.Opacity = 1.0;
		Image2.Visibility = Visibility.Collapsed;
		Image2.Opacity = 0.0;
		Dot1.Fill = Brushes.White;
		Dot2.Fill = Brushes.Gray;
	}

	private void Dot2_Click(object sender, MouseButtonEventArgs e)
	{
		Image1.Visibility = Visibility.Collapsed;
		Image1.Opacity = 0.0;
		Image2.Visibility = Visibility.Visible;
		Image2.Opacity = 1.0;
		Dot1.Fill = Brushes.Gray;
		Dot2.Fill = Brushes.White;
	}

	private void OnImageLoaded(object sender, RoutedEventArgs e)
	{
		Image image = sender as Image;
		if (image == null)
		{
			return;
		}
		image.Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action)delegate
		{
			if (image.ActualWidth > 0.0 && image.ActualHeight > 0.0)
			{
				RectangleGeometry clip = new RectangleGeometry
				{
					Rect = new Rect(0.0, 0.0, image.ActualWidth, image.ActualHeight),
					RadiusX = 10.0,
					RadiusY = 10.0
				};
				image.Clip = clip;
			}
			else
			{
				image.SizeChanged += delegate
				{
					if (image.ActualWidth > 0.0 && image.ActualHeight > 0.0)
					{
						RectangleGeometry clip2 = new RectangleGeometry
						{
							Rect = new Rect(0.0, 0.0, image.ActualWidth, image.ActualHeight),
							RadiusX = 10.0,
							RadiusY = 10.0
						};
						image.Clip = clip2;
					}
				};
			}
		});
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

	private async void Launch_Click(object sender, RoutedEventArgs e)
	{
		try
		{
			Toast.ShowToast("Launching Shadow...", "This may take a bit.");
			Button button = sender as Button;
			await Task.Delay(300);
			base.Dispatcher.Invoke(() => button.Content = "Downloading Content...");
			button.IsEnabled = false;
			await Task.Delay(300);
			base.Dispatcher.Invoke(() => button.Content = "Launching Shadow...");
			button.IsEnabled = false;
			await Task.Delay(300);
			base.Dispatcher.Invoke(delegate
			{
				button.Content = "Shadow IS RUNNING";
				button.IsEnabled = false;
				button.FontSize = 20.0;
			});
			try
			{
				RPC.SetState("Launching Shadow...", watching: true);
			}
			catch (Exception ex)
			{
				Exception ex2 = ex;
				MessageBox.Show("Failed to set RPC state: " + ex2.Message);
				Console.WriteLine($"RPC SetState Error: {ex2}");
			}
			string Path69 = UpdateINI.ReadValue("Auth", "Path");
			if (!(Path69 != "NONE"))
			{
				return;
			}
			if (File.Exists(Path.Combine(Path69, "FortniteGame\\Binaries\\Win64\\FortniteClient-Win64-Shipping.exe")))
			{
				if (UpdateINI.ReadValue("Auth", "Email") == "NONE" || UpdateINI.ReadValue("Auth", "Password") == "NONE")
				{
					MessageBox.Show("Add Your Path");
					return;
				}
				await Task.Run(delegate
				{
					try
					{
						WebClient webClient = new WebClient();
						webClient.DownloadFile("https://cdn.discordapp.com/attachments/1348683615697109115/1357397085691052153/Starfall.dll?ex=67f00e0c&is=67eebc8c&hm=679c0963bdbbf56d35f69b33e950410df3b2253749424cfbf0454f48202a71c0&", Path.Combine(Path69, "Engine\\Binaries\\ThirdParty\\NVIDIA\\NVaftermath\\Win64", "GFSDK_Aftermath_Lib.x64.dll"));
						PSBasics.Start(Path69, "-epicapp=Fortnite -epicenv=Prod -epiclocale=en-us -epicportal -noeac -fromfl=be -fltoken=h1cdhchd10150221h130eB56 -skippatchcheck", UpdateINI.ReadValue("Auth", "Email"), UpdateINI.ReadValue("Auth", "Password"));
						FakeAC.Start(Path69, "FortniteClient-Win64-Shipping_BE.exe", "-epicapp=Fortnite -epicenv=Prod -epiclocale=en-us -epicportal -noeac -fromfl=be -fltoken=h1cdhchd10150221h130eB56 -skippatchcheck");
						FakeAC.Start(Path69, "FortniteLauncher.exe", "-epicapp=Fortnite -epicenv=Prod -epiclocale=en-us -epicportal -noeac -fromfl=be -fltoken=h1cdhchd10150221h130eB56 -skippatchcheck", "dsf");
						PSBasics._FortniteProcess.WaitForExit();
						base.Dispatcher.Invoke(delegate
						{
							try
							{
								FakeAC._FNLauncherProcess.Close();
								FakeAC._FNAntiCheatProcess.Close();
							}
							catch (Exception)
							{
								ShowError("There Been A Error Closing");
							}
						});
					}
					catch (Exception)
					{
						base.Dispatcher.Invoke(delegate
						{
							ShowError("ERROR");
						});
					}
				});
			}
			else
			{
				ShowError("Add Your Path");
			}
		}
		catch (Exception)
		{
			ShowError("ERROR");
		}
		finally
		{
			(sender as Button).Content = "Launch Shadow";
			(sender as Button).IsEnabled = true;
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
		Process.Start("");
	}

	private void Donate_Click(object sender, RoutedEventArgs e)
	{
		Process.Start("https://projectShadow.mysellix.io/");
	}

	private async void Build_Click(object sender, RoutedEventArgs e)
	{
		string downloadUrl = "https://fnbuilds.boostedv2.dev/12.41.rar";
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

	private void Settings_Click(object sender, RoutedEventArgs e)
	{
		Settings main = new Settings();
		main.Show();
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

	private void Home_Click(object sender, RoutedEventArgs e)
	{
		new MainWindow().Show();
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
