using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Shadow_Launcher;
using WindowsAPICodePack.Dialogs;
using ZYTROZLauncher;
using ZYTROZLauncher.Resources;
using ZYTROZLauncher.Utilities;

namespace Shadow_Launcher_V2;

public partial class Settings : Window, IComponentConnector
{
	private DispatcherTimer colorTimer;

	private SolidColorBrush[] partyColors;

	private int currentColorIndex = 0;

	private bool isPartyModeEnabled = false;

	private WebClient webClient;

	private bool isDownloading = false;

	public Settings()
	{
		InitializeComponent();
		LoadAlwaysOnTop();
		colorTimer = new DispatcherTimer();
		colorTimer.Interval = TimeSpan.FromSeconds(0.1);
		colorTimer.Tick += ColorTimer_Tick;
		partyColors = new SolidColorBrush[4]
		{
			new SolidColorBrush(Colors.Red),
			new SolidColorBrush(Colors.Blue),
			new SolidColorBrush(Colors.Black),
			new SolidColorBrush(Colors.Purple)
		};
		LoadSettings();
	}

	private void Window_MouseDown(object sender, MouseButtonEventArgs e)
	{
		if (e.LeftButton == MouseButtonState.Pressed && e.GetPosition(this).Y <= 30.0)
		{
			DragMove();
		}
	}

	private void Settings_Loaded(object sender, RoutedEventArgs e)
	{
		colorTimer.Start();
	}

	private void AlwaysOnTopCheckBox_Checked(object sender, RoutedEventArgs e)
	{
		base.Topmost = true;
	}

	private void AlwaysOnTopCheckBox_Unchecked(object sender, RoutedEventArgs e)
	{
		base.Topmost = false;
	}

	private void LoadAlwaysOnTop()
	{
		bool result;
		bool alwaysOnTop = bool.TryParse(UpdateINI.ReadValue("Settings", "AlwaysOnTop"), out result) && result;
		base.Topmost = alwaysOnTop;
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

	private async void TwelveFortyOne_Path(object sender, RoutedEventArgs e)
	{
		try
		{
			CommonOpenFileDialog commonOpenFileDialog = new CommonOpenFileDialog
			{
				IsFolderPicker = true,
				Title = "Select A Fortnite Build",
				Multiselect = false
			};
			CommonFileDialogResult result = ((CommonFileDialog)commonOpenFileDialog).ShowDialog();
			if ((int)result != 1)
			{
				return;
			}
			if (File.Exists(Path.Combine(((CommonFileDialog)commonOpenFileDialog).FileName, "FortniteGame\\Binaries\\Win64\\FortniteClient-Win64-Shipping.exe")))
			{
				if (await VersionChecker.GetBuildVersion(((CommonFileDialog)commonOpenFileDialog).FileName) == "12.41")
				{
					PathBox.Text = ((CommonFileDialog)commonOpenFileDialog).FileName;
					UpdateINI.WriteToConfig("Auth", "Path", PathBox.Text);
					ShowSuccess("12.41 Build Added");
				}
				else
				{
					ShowError("You can only select version 12.41!");
				}
			}
			else
			{
				ShowError("Please make sure that your folder contains FortniteGame and Engine.");
			}
		}
		catch (Exception ex)
		{
			Exception ex2 = ex;
			MessageBox.Show("An error occurred: " + ex2.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
	}

	private void TogglePartyMode_Click(object sender, RoutedEventArgs e)
	{
		isPartyModeEnabled = !isPartyModeEnabled;
		if (isPartyModeEnabled)
		{
			colorTimer.Start();
			return;
		}
		colorTimer.Stop();
		MainContentBorder.Background = Brushes.Transparent;
	}

	private void ColorTimer_Tick(object sender, EventArgs e)
	{
		MainContentBorder.Background = partyColors[currentColorIndex];
		currentColorIndex = (currentColorIndex + 1) % partyColors.Length;
	}

	private void GoBack_Click(object sender, RoutedEventArgs e)
	{
		MainWindow main = new MainWindow();
		main.Show();
		Close();
	}

	private void Mods_Click(object sender, RoutedEventArgs e)
	{
		Mods main = new Mods();
		main.Show();
		Close();
	}

	private void LoadSettings()
	{
		string savedPath = UpdateINI.ReadValue("Auth", "Path");
		if (!string.IsNullOrEmpty(savedPath) && Directory.Exists(savedPath))
		{
			PathBox.Text = savedPath;
			return;
		}
		PathBox.Text = string.Empty;
		UpdateINI.WriteToConfig("Auth", "Path", string.Empty);
	}

	private void PathBox_TextChanged(object sender, TextChangedEventArgs e)
	{
	}
}
