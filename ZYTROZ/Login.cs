using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using ZYTROZLauncher;
using ZYTROZLauncher.Resources;

namespace ZYTROZ;

public partial class Login : Window, IComponentConnector
{
	public class Vars
	{
		public static string Email = "NONE";

		public static string Password = "NONE";

		public static string Path1 = "NONE";
	}

	public Login()
	{
		InitializeComponent();
		string storedEmail = UpdateINI.ReadValue("Auth", "Email");
		string storedPassword = UpdateINI.ReadValue("Auth", "Password");
	}

	private void Button_Click_1(object sender, RoutedEventArgs e)
	{
		UpdateINI.WriteToConfig("Auth", "Email", EmailBox.Text);
		UpdateINI.WriteToConfig("Auth", "Password", PasswordBox.Password);
		MainWindow mainWindow = new MainWindow();
		mainWindow.Show();
		Close();
	}

	private void Close_Click(object sender, RoutedEventArgs e)
	{
		Close();
	}

	private void Window_MouseDown(object sender, MouseButtonEventArgs e)
	{
		if (e.LeftButton == MouseButtonState.Pressed && e.GetPosition(this).Y <= 30.0)
		{
			DragMove();
		}
	}

	private void Password_TextChanged(object sender, TextChangedEventArgs e)
	{
	}

	private void Email_TextChanged(object sender, TextChangedEventArgs e)
	{
	}

	private void PasswordBox_TextChanged(object sender, TextChangedEventArgs e)
	{
	}

	private void Minimize_Click(object sender, RoutedEventArgs e)
	{
		base.WindowState = WindowState.Minimized;
	}

	private void EmailBox_TextChanged(object sender, TextChangedEventArgs e)
	{
	}
}
