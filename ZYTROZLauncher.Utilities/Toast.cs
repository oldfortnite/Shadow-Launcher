using Microsoft.Toolkit.Uwp.Notifications;

namespace ZYTROZLauncher.Utilities;

internal class Toast
{
	public static void ShowToast(string title, string message)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		new ToastContentBuilder().AddText(title, (AdaptiveTextStyle?)null, (bool?)null, (int?)null, (int?)null, (AdaptiveTextAlign?)null, (string)null).AddText(message, (AdaptiveTextStyle?)null, (bool?)null, (int?)null, (int?)null, (AdaptiveTextAlign?)null, (string)null).Show();
	}
}
