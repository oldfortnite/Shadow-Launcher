using System;
using DiscordRPC;
using DiscordRPC.Events;
using DiscordRPC.Logging;
using DiscordRPC.Message;

namespace Shadow_Launcher.Resources.Extra;

internal static class RPC
{
	private static DiscordRpcClient client;

	private static RichPresence presence;

	public static void StartRPC()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Expected O, but got Unknown
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Expected O, but got Unknown
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Expected O, but got Unknown
		try
		{
			client = new DiscordRpcClient("1251655533727973478");
			client.Logger = (ILogger)new ConsoleLogger
			{
				Level = (LogLevel)3
			};
			client.OnReady += new OnReadyEvent(Client_OnReady);
			client.OnPresenceUpdate += new OnPresenceUpdateEvent(Client_OnPresenceUpdate);
			client.Initialize();
			UpdateRPC("Exploring Shadow Launcher");
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}

	private static void Client_OnReady(object sender, ReadyMessage e)
	{
		Console.WriteLine("Discord RPC is ready.");
	}

	private static void Client_OnPresenceUpdate(object sender, PresenceMessage e)
	{
		Console.WriteLine("Presence updated.");
	}

	public static void UpdateRPC(string details, bool bTimeStamp = false)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Expected O, but got Unknown
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Expected O, but got Unknown
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Expected O, but got Unknown
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Expected O, but got Unknown
		try
		{
			if (client == null || !client.IsInitialized)
			{
				StartRPC();
			}
			RichPresence val = new RichPresence();
			((BaseRichPresence)val).Details = details;
			((BaseRichPresence)val).Timestamps = new Timestamps
			{
				Start = (bTimeStamp ? new DateTime?(DateTime.UtcNow) : null)
			};
			((BaseRichPresence)val).Assets = new Assets
			{
				LargeImageKey = "Shadow",
				LargeImageText = "Large Image Text"
			};
			val.Buttons = (Button[])(object)new Button[1]
			{
				new Button
				{
					Label = "Join Our Discord",
					Url = "https://discord.gg/syBY9t7hDF"
				}
			};
			RichPresence presence = val;
			client.SetPresence(presence);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}

	public static void SetState(string state, bool watching = false)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Expected O, but got Unknown
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Expected O, but got Unknown
		try
		{
			if (client == null || !client.IsInitialized)
			{
				StartRPC();
			}
			if (presence == null)
			{
				RichPresence val = new RichPresence();
				((BaseRichPresence)val).Assets = new Assets
				{
					LargeImageKey = "Shadow",
					LargeImageText = "Large Image Text"
				};
				val.Buttons = (Button[])(object)new Button[1]
				{
					new Button
					{
						Label = "Join Our Discord",
						Url = "https://discord.gg/syBY9t7hDF"
					}
				};
				presence = val;
			}
			if (watching)
			{
				state = state ?? "";
			}
			((BaseRichPresence)presence).State = state;
			client.SetPresence(presence);
		}
		catch (Exception ex)
		{
			Console.WriteLine("Failed to set RPC state: " + ex.Message);
		}
	}

	public static void StopRPC()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Expected O, but got Unknown
		try
		{
			if (client != null)
			{
				client.OnReady -= new OnReadyEvent(Client_OnReady);
				client.OnPresenceUpdate -= new OnPresenceUpdateEvent(Client_OnPresenceUpdate);
				client.Dispose();
				client = null;
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}
}
