using System;
using System.IO;
using System.Text;
using IniParser;
using IniParser.Model;

namespace ZYTROZLauncher.Resources;

public static class UpdateINI
{
	public static void WriteToConfig(string SectionName, string PathKey, string NewValue)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Expected O, but got Unknown
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Expected O, but got Unknown
		string BaseFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
		string DataFolder = Path.Combine(BaseFolder, "Shadow");
		Directory.CreateDirectory(DataFolder);
		string FilePath = Path.Combine(DataFolder, "Settings.ini");
		FileIniDataParser parser = new FileIniDataParser();
		IniData iniData = (IniData)((!File.Exists(FilePath)) ? ((object)new IniData()) : ((object)parser.ReadFile(FilePath)));
		iniData[SectionName][PathKey] = NewValue;
		parser.WriteFile(FilePath, iniData, (Encoding)null);
	}

	public static string ReadValue(string SectionName, string PathKey)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		string BaseFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
		string DataFolder = Path.Combine(BaseFolder, "Shadow");
		string FilePath = Path.Combine(DataFolder, "Settings.ini");
		FileIniDataParser parser = new FileIniDataParser();
		if (File.Exists(FilePath))
		{
			IniData iniData = parser.ReadFile(FilePath);
			return iniData[SectionName][PathKey];
		}
		return "NONE";
	}
}
