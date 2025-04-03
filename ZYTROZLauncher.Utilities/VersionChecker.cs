using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ZYTROZLauncher.Utilities;

internal class VersionChecker
{
	private static List<int> Search(byte[] src, byte[] pattern)
	{
		List<int> indices = new List<int>();
		int srcLength = src.Length;
		int patternLength = pattern.Length;
		int maxSearchIndex = srcLength - patternLength;
		for (int i = 0; i <= maxSearchIndex; i++)
		{
			if (src[i] != pattern[0])
			{
				continue;
			}
			bool found = true;
			for (int j = 1; j < patternLength; j++)
			{
				if (src[i + j] != pattern[j])
				{
					found = false;
					break;
				}
			}
			if (found)
			{
				indices.Add(i);
			}
		}
		return indices;
	}

	public static async Task<string> GetBuildVersion(string exePath)
	{
		string targetFileName = "FortniteClient-Win64-Shipping.exe";
		string targetFilePath = Path.Combine(exePath, "FortniteGame", "Binaries", "Win64", targetFileName);
		if (!File.Exists(targetFilePath))
		{
			return "ERROR: File not found";
		}
		try
		{
			string result = "";
			byte[] pattern = Encoding.Unicode.GetBytes("++Fortnite+Release-");
			using (BinaryReader binaryReader = new BinaryReader(new FileStream(targetFilePath, FileMode.Open, FileAccess.Read, FileShare.Read)))
			{
				long fileSize = binaryReader.BaseStream.Length;
				long position = 0L;
				int bufferSize = 4096;
				byte[] buffer = new byte[bufferSize];
				while (true)
				{
					if (position < fileSize)
					{
						int bytesRead = await binaryReader.BaseStream.ReadAsync(buffer, 0, bufferSize);
						if (bytesRead != 0)
						{
							List<int> indices = Search(buffer, pattern);
							foreach (int num in indices)
							{
								string chunkText = Encoding.Unicode.GetString(buffer, num, bytesRead - num);
								Match match = Regex.Match(chunkText, "\\+\\+Fortnite\\+Release-((\\d{1,2})\\.(\\d{1,2})|Live|Next|Cert)[-CL]*(\\d*)", RegexOptions.IgnoreCase);
								if (match.Success)
								{
									result = match.Value;
									goto end_IL_02e2;
								}
							}
							position += bytesRead;
							if (position < fileSize)
							{
								binaryReader.BaseStream.Position -= pattern.Length - 1;
								continue;
							}
						}
					}
					goto end_IL_00c0;
					continue;
					end_IL_02e2:
					break;
				}
				end_IL_00c0:;
			}
			if (result.Contains("-CL"))
			{
				result = result.Substring(0, result.LastIndexOf("-CL"));
			}
			return result.Remove(0, 19);
		}
		catch (Exception ex)
		{
			Console.WriteLine("Error while getting build version: " + ex.Message);
			Console.WriteLine("Stack Trace: " + ex.StackTrace);
			return "ERROR";
		}
	}
}
