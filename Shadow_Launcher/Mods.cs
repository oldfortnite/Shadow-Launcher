using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using ZYTROZLauncher;

namespace Shadow_Launcher;

public partial class Mods : Window, IComponentConnector
{
	public Mods()
	{
		InitializeComponent();
	}

	private void GoBack_Click(object sender, RoutedEventArgs e)
	{
		MainWindow main = new MainWindow();
		main.Show();
		Close();
	}

	private void Window_MouseDown(object sender, MouseButtonEventArgs e)
	{
		if (e.LeftButton == MouseButtonState.Pressed && e.GetPosition(this).Y <= 30.0)
		{
			DragMove();
		}
	}

	private void AboutPaksButton_Click(object sender, RoutedEventArgs e)
	{
		if (AboutPaksInfo.Visibility == Visibility.Visible)
		{
			AboutPaksInfo.Visibility = Visibility.Collapsed;
		}
		else
		{
			AboutPaksInfo.Visibility = Visibility.Visible;
		}
	}

	private void ComingSoon_Click(object sender, RoutedEventArgs e)
	{
		MessageBox.Show("Coming Soon");
	}
}
