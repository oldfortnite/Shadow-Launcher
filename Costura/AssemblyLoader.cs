using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Costura;

[CompilerGenerated]
internal static class AssemblyLoader
{
	private static object nullCacheLock = new object();

	private static Dictionary<string, bool> nullCache = new Dictionary<string, bool>();

	private static Dictionary<string, string> assemblyNames = new Dictionary<string, string>();

	private static Dictionary<string, string> symbolNames = new Dictionary<string, string>();

	private static int isAttached;

	private static string CultureToString(CultureInfo culture)
	{
		if (culture == null)
		{
			return "";
		}
		return culture.Name;
	}

	private static Assembly ReadExistingAssembly(AssemblyName name)
	{
		AppDomain currentDomain = AppDomain.CurrentDomain;
		Assembly[] assemblies = currentDomain.GetAssemblies();
		Assembly[] array = assemblies;
		foreach (Assembly assembly in array)
		{
			AssemblyName name2 = assembly.GetName();
			if (string.Equals(name2.Name, name.Name, StringComparison.InvariantCultureIgnoreCase) && string.Equals(CultureToString(name2.CultureInfo), CultureToString(name.CultureInfo), StringComparison.InvariantCultureIgnoreCase))
			{
				return assembly;
			}
		}
		return null;
	}

	private static void CopyTo(Stream source, Stream destination)
	{
		byte[] array = new byte[81920];
		int count;
		while ((count = source.Read(array, 0, array.Length)) != 0)
		{
			destination.Write(array, 0, count);
		}
	}

	private static Stream LoadStream(string fullName)
	{
		Assembly executingAssembly = Assembly.GetExecutingAssembly();
		if (fullName.EndsWith(".compressed"))
		{
			using (Stream stream = executingAssembly.GetManifestResourceStream(fullName))
			{
				using DeflateStream source = new DeflateStream(stream, CompressionMode.Decompress);
				MemoryStream memoryStream = new MemoryStream();
				CopyTo(source, memoryStream);
				memoryStream.Position = 0L;
				return memoryStream;
			}
		}
		return executingAssembly.GetManifestResourceStream(fullName);
	}

	private static Stream LoadStream(Dictionary<string, string> resourceNames, string name)
	{
		if (resourceNames.TryGetValue(name, out var value))
		{
			return LoadStream(value);
		}
		return null;
	}

	private static byte[] ReadStream(Stream stream)
	{
		byte[] data = new byte[stream.Length];
		stream.Read(data, 0, data.Length);
		return data;
	}

	private static Assembly ReadFromEmbeddedResources(Dictionary<string, string> assemblyNames, Dictionary<string, string> symbolNames, AssemblyName requestedAssemblyName)
	{
		string name = requestedAssemblyName.Name.ToLowerInvariant();
		if (requestedAssemblyName.CultureInfo != null && !string.IsNullOrEmpty(requestedAssemblyName.CultureInfo.Name))
		{
			name = requestedAssemblyName.CultureInfo.Name + "." + name;
		}
		byte[] assemblyData;
		using (Stream stream = LoadStream(assemblyNames, name))
		{
			if (stream == null)
			{
				return null;
			}
			assemblyData = ReadStream(stream);
		}
		using (Stream stream2 = LoadStream(symbolNames, name))
		{
			if (stream2 != null)
			{
				byte[] rawSymbolStore = ReadStream(stream2);
				return Assembly.Load(assemblyData, rawSymbolStore);
			}
		}
		return Assembly.Load(assemblyData);
	}

	public static Assembly ResolveAssembly(object sender, ResolveEventArgs e)
	{
		lock (nullCacheLock)
		{
			if (nullCache.ContainsKey(e.Name))
			{
				return null;
			}
		}
		AssemblyName requestedAssemblyName = new AssemblyName(e.Name);
		Assembly assembly = ReadExistingAssembly(requestedAssemblyName);
		if ((object)assembly != null)
		{
			return assembly;
		}
		assembly = ReadFromEmbeddedResources(assemblyNames, symbolNames, requestedAssemblyName);
		if ((object)assembly == null)
		{
			lock (nullCacheLock)
			{
				nullCache[e.Name] = true;
			}
			if ((requestedAssemblyName.Flags & AssemblyNameFlags.Retargetable) != 0)
			{
				assembly = Assembly.Load(requestedAssemblyName);
			}
		}
		return assembly;
	}

	static AssemblyLoader()
	{
		assemblyNames.Add("af-za.modernwpf.controls.resources", "costura.af-za.modernwpf.controls.resources.dll.compressed");
		assemblyNames.Add("af-za.modernwpf.resources", "costura.af-za.modernwpf.resources.dll.compressed");
		assemblyNames.Add("am-et.modernwpf.controls.resources", "costura.am-et.modernwpf.controls.resources.dll.compressed");
		assemblyNames.Add("am-et.modernwpf.resources", "costura.am-et.modernwpf.resources.dll.compressed");
		assemblyNames.Add("ar-sa.modernwpf.controls.resources", "costura.ar-sa.modernwpf.controls.resources.dll.compressed");
		assemblyNames.Add("ar-sa.modernwpf.resources", "costura.ar-sa.modernwpf.resources.dll.compressed");
		assemblyNames.Add("awssdk.core", "costura.awssdk.core.dll.compressed");
		symbolNames.Add("awssdk.core", "costura.awssdk.core.pdb.compressed");
		assemblyNames.Add("awssdk.securitytoken", "costura.awssdk.securitytoken.dll.compressed");
		symbolNames.Add("awssdk.securitytoken", "costura.awssdk.securitytoken.pdb.compressed");
		assemblyNames.Add("az-latn-az.modernwpf.controls.resources", "costura.az-latn-az.modernwpf.controls.resources.dll.compressed");
		assemblyNames.Add("az-latn-az.modernwpf.resources", "costura.az-latn-az.modernwpf.resources.dll.compressed");
		assemblyNames.Add("be-by.modernwpf.controls.resources", "costura.be-by.modernwpf.controls.resources.dll.compressed");
		assemblyNames.Add("be-by.modernwpf.resources", "costura.be-by.modernwpf.resources.dll.compressed");
		assemblyNames.Add("bg-bg.modernwpf.controls.resources", "costura.bg-bg.modernwpf.controls.resources.dll.compressed");
		assemblyNames.Add("bg-bg.modernwpf.resources", "costura.bg-bg.modernwpf.resources.dll.compressed");
		assemblyNames.Add("bn-bd.modernwpf.controls.resources", "costura.bn-bd.modernwpf.controls.resources.dll.compressed");
		assemblyNames.Add("bn-bd.modernwpf.resources", "costura.bn-bd.modernwpf.resources.dll.compressed");
		assemblyNames.Add("bs-latn-ba.modernwpf.controls.resources", "costura.bs-latn-ba.modernwpf.controls.resources.dll.compressed");
		assemblyNames.Add("bs-latn-ba.modernwpf.resources", "costura.bs-latn-ba.modernwpf.resources.dll.compressed");
		assemblyNames.Add("ca-es.modernwpf.controls.resources", "costura.ca-es.modernwpf.controls.resources.dll.compressed");
		assemblyNames.Add("ca-es.modernwpf.resources", "costura.ca-es.modernwpf.resources.dll.compressed");
		assemblyNames.Add("costura", "costura.costura.dll.compressed");
		symbolNames.Add("costura", "costura.costura.pdb.compressed");
		assemblyNames.Add("cs-cz.modernwpf.controls.resources", "costura.cs-cz.modernwpf.controls.resources.dll.compressed");
		assemblyNames.Add("cs-cz.modernwpf.resources", "costura.cs-cz.modernwpf.resources.dll.compressed");
		assemblyNames.Add("da-dk.modernwpf.controls.resources", "costura.da-dk.modernwpf.controls.resources.dll.compressed");
		assemblyNames.Add("da-dk.modernwpf.resources", "costura.da-dk.modernwpf.resources.dll.compressed");
		assemblyNames.Add("de-de.modernwpf.controls.resources", "costura.de-de.modernwpf.controls.resources.dll.compressed");
		assemblyNames.Add("de-de.modernwpf.resources", "costura.de-de.modernwpf.resources.dll.compressed");
		assemblyNames.Add("discordrpc", "costura.discordrpc.dll.compressed");
		symbolNames.Add("discordrpc", "costura.discordrpc.pdb.compressed");
		assemblyNames.Add("dnsclient", "costura.dnsclient.dll.compressed");
		assemblyNames.Add("el-gr.modernwpf.controls.resources", "costura.el-gr.modernwpf.controls.resources.dll.compressed");
		assemblyNames.Add("el-gr.modernwpf.resources", "costura.el-gr.modernwpf.resources.dll.compressed");
		assemblyNames.Add("en-gb.modernwpf.controls.resources", "costura.en-gb.modernwpf.controls.resources.dll.compressed");
		assemblyNames.Add("en-gb.modernwpf.resources", "costura.en-gb.modernwpf.resources.dll.compressed");
		assemblyNames.Add("es-es.modernwpf.controls.resources", "costura.es-es.modernwpf.controls.resources.dll.compressed");
		assemblyNames.Add("es-es.modernwpf.resources", "costura.es-es.modernwpf.resources.dll.compressed");
		assemblyNames.Add("es-mx.modernwpf.controls.resources", "costura.es-mx.modernwpf.controls.resources.dll.compressed");
		assemblyNames.Add("es-mx.modernwpf.resources", "costura.es-mx.modernwpf.resources.dll.compressed");
		assemblyNames.Add("et-ee.modernwpf.controls.resources", "costura.et-ee.modernwpf.controls.resources.dll.compressed");
		assemblyNames.Add("et-ee.modernwpf.resources", "costura.et-ee.modernwpf.resources.dll.compressed");
		assemblyNames.Add("eu-es.modernwpf.controls.resources", "costura.eu-es.modernwpf.controls.resources.dll.compressed");
		assemblyNames.Add("eu-es.modernwpf.resources", "costura.eu-es.modernwpf.resources.dll.compressed");
		assemblyNames.Add("fa-ir.modernwpf.controls.resources", "costura.fa-ir.modernwpf.controls.resources.dll.compressed");
		assemblyNames.Add("fa-ir.modernwpf.resources", "costura.fa-ir.modernwpf.resources.dll.compressed");
		assemblyNames.Add("fi-fi.modernwpf.controls.resources", "costura.fi-fi.modernwpf.controls.resources.dll.compressed");
		assemblyNames.Add("fi-fi.modernwpf.resources", "costura.fi-fi.modernwpf.resources.dll.compressed");
		assemblyNames.Add("fil-ph.modernwpf.controls.resources", "costura.fil-ph.modernwpf.controls.resources.dll.compressed");
		assemblyNames.Add("fil-ph.modernwpf.resources", "costura.fil-ph.modernwpf.resources.dll.compressed");
		assemblyNames.Add("fr-ca.modernwpf.controls.resources", "costura.fr-ca.modernwpf.controls.resources.dll.compressed");
		assemblyNames.Add("fr-ca.modernwpf.resources", "costura.fr-ca.modernwpf.resources.dll.compressed");
		assemblyNames.Add("fr-fr.modernwpf.controls.resources", "costura.fr-fr.modernwpf.controls.resources.dll.compressed");
		assemblyNames.Add("fr-fr.modernwpf.resources", "costura.fr-fr.modernwpf.resources.dll.compressed");
		assemblyNames.Add("gl-es.modernwpf.controls.resources", "costura.gl-es.modernwpf.controls.resources.dll.compressed");
		assemblyNames.Add("gl-es.modernwpf.resources", "costura.gl-es.modernwpf.resources.dll.compressed");
		assemblyNames.Add("he-il.modernwpf.controls.resources", "costura.he-il.modernwpf.controls.resources.dll.compressed");
		assemblyNames.Add("he-il.modernwpf.resources", "costura.he-il.modernwpf.resources.dll.compressed");
		assemblyNames.Add("hi-in.modernwpf.controls.resources", "costura.hi-in.modernwpf.controls.resources.dll.compressed");
		assemblyNames.Add("hi-in.modernwpf.resources", "costura.hi-in.modernwpf.resources.dll.compressed");
		assemblyNames.Add("hr-hr.modernwpf.controls.resources", "costura.hr-hr.modernwpf.controls.resources.dll.compressed");
		assemblyNames.Add("hr-hr.modernwpf.resources", "costura.hr-hr.modernwpf.resources.dll.compressed");
		assemblyNames.Add("hu-hu.modernwpf.controls.resources", "costura.hu-hu.modernwpf.controls.resources.dll.compressed");
		assemblyNames.Add("hu-hu.modernwpf.resources", "costura.hu-hu.modernwpf.resources.dll.compressed");
		assemblyNames.Add("id-id.modernwpf.controls.resources", "costura.id-id.modernwpf.controls.resources.dll.compressed");
		assemblyNames.Add("id-id.modernwpf.resources", "costura.id-id.modernwpf.resources.dll.compressed");
		assemblyNames.Add("inifileparser", "costura.inifileparser.dll.compressed");
		assemblyNames.Add("is-is.modernwpf.controls.resources", "costura.is-is.modernwpf.controls.resources.dll.compressed");
		assemblyNames.Add("is-is.modernwpf.resources", "costura.is-is.modernwpf.resources.dll.compressed");
		assemblyNames.Add("it-it.modernwpf.controls.resources", "costura.it-it.modernwpf.controls.resources.dll.compressed");
		assemblyNames.Add("it-it.modernwpf.resources", "costura.it-it.modernwpf.resources.dll.compressed");
		assemblyNames.Add("ja-jp.modernwpf.controls.resources", "costura.ja-jp.modernwpf.controls.resources.dll.compressed");
		assemblyNames.Add("ja-jp.modernwpf.resources", "costura.ja-jp.modernwpf.resources.dll.compressed");
		assemblyNames.Add("ka-ge.modernwpf.controls.resources", "costura.ka-ge.modernwpf.controls.resources.dll.compressed");
		assemblyNames.Add("ka-ge.modernwpf.resources", "costura.ka-ge.modernwpf.resources.dll.compressed");
		assemblyNames.Add("kk-kz.modernwpf.controls.resources", "costura.kk-kz.modernwpf.controls.resources.dll.compressed");
		assemblyNames.Add("kk-kz.modernwpf.resources", "costura.kk-kz.modernwpf.resources.dll.compressed");
		assemblyNames.Add("km-kh.modernwpf.controls.resources", "costura.km-kh.modernwpf.controls.resources.dll.compressed");
		assemblyNames.Add("km-kh.modernwpf.resources", "costura.km-kh.modernwpf.resources.dll.compressed");
		assemblyNames.Add("kn-in.modernwpf.controls.resources", "costura.kn-in.modernwpf.controls.resources.dll.compressed");
		assemblyNames.Add("kn-in.modernwpf.resources", "costura.kn-in.modernwpf.resources.dll.compressed");
		assemblyNames.Add("ko-kr.modernwpf.controls.resources", "costura.ko-kr.modernwpf.controls.resources.dll.compressed");
		assemblyNames.Add("ko-kr.modernwpf.resources", "costura.ko-kr.modernwpf.resources.dll.compressed");
		assemblyNames.Add("lo-la.modernwpf.controls.resources", "costura.lo-la.modernwpf.controls.resources.dll.compressed");
		assemblyNames.Add("lo-la.modernwpf.resources", "costura.lo-la.modernwpf.resources.dll.compressed");
		assemblyNames.Add("lt-lt.modernwpf.controls.resources", "costura.lt-lt.modernwpf.controls.resources.dll.compressed");
		assemblyNames.Add("lt-lt.modernwpf.resources", "costura.lt-lt.modernwpf.resources.dll.compressed");
		assemblyNames.Add("lv-lv.modernwpf.controls.resources", "costura.lv-lv.modernwpf.controls.resources.dll.compressed");
		assemblyNames.Add("lv-lv.modernwpf.resources", "costura.lv-lv.modernwpf.resources.dll.compressed");
		assemblyNames.Add("microsoft.bcl.asyncinterfaces", "costura.microsoft.bcl.asyncinterfaces.dll.compressed");
		assemblyNames.Add("microsoft.extensions.logging.abstractions", "costura.microsoft.extensions.logging.abstractions.dll.compressed");
		assemblyNames.Add("microsoft.toolkit.uwp.notifications", "costura.microsoft.toolkit.uwp.notifications.dll.compressed");
		symbolNames.Add("microsoft.toolkit.uwp.notifications", "costura.microsoft.toolkit.uwp.notifications.pdb.compressed");
		assemblyNames.Add("microsoft.win32.registry", "costura.microsoft.win32.registry.dll.compressed");
		assemblyNames.Add("mk-mk.modernwpf.controls.resources", "costura.mk-mk.modernwpf.controls.resources.dll.compressed");
		assemblyNames.Add("mk-mk.modernwpf.resources", "costura.mk-mk.modernwpf.resources.dll.compressed");
		assemblyNames.Add("ml-in.modernwpf.controls.resources", "costura.ml-in.modernwpf.controls.resources.dll.compressed");
		assemblyNames.Add("ml-in.modernwpf.resources", "costura.ml-in.modernwpf.resources.dll.compressed");
		assemblyNames.Add("modernwpf.controls", "costura.modernwpf.controls.dll.compressed");
		assemblyNames.Add("modernwpf", "costura.modernwpf.dll.compressed");
		assemblyNames.Add("mongocrypt", "costura.mongocrypt.dll.compressed");
		assemblyNames.Add("mongodb.bson", "costura.mongodb.bson.dll.compressed");
		assemblyNames.Add("mongodb.driver.core", "costura.mongodb.driver.core.dll.compressed");
		assemblyNames.Add("mongodb.driver", "costura.mongodb.driver.dll.compressed");
		assemblyNames.Add("mongodb.libmongocrypt", "costura.mongodb.libmongocrypt.dll.compressed");
		assemblyNames.Add("ms-my.modernwpf.controls.resources", "costura.ms-my.modernwpf.controls.resources.dll.compressed");
		assemblyNames.Add("ms-my.modernwpf.resources", "costura.ms-my.modernwpf.resources.dll.compressed");
		assemblyNames.Add("nb-no.modernwpf.controls.resources", "costura.nb-no.modernwpf.controls.resources.dll.compressed");
		assemblyNames.Add("nb-no.modernwpf.resources", "costura.nb-no.modernwpf.resources.dll.compressed");
		assemblyNames.Add("newtonsoft.json", "costura.newtonsoft.json.dll.compressed");
		assemblyNames.Add("nl-nl.modernwpf.controls.resources", "costura.nl-nl.modernwpf.controls.resources.dll.compressed");
		assemblyNames.Add("nl-nl.modernwpf.resources", "costura.nl-nl.modernwpf.resources.dll.compressed");
		assemblyNames.Add("nn-no.modernwpf.controls.resources", "costura.nn-no.modernwpf.controls.resources.dll.compressed");
		assemblyNames.Add("nn-no.modernwpf.resources", "costura.nn-no.modernwpf.resources.dll.compressed");
		assemblyNames.Add("pl-pl.modernwpf.controls.resources", "costura.pl-pl.modernwpf.controls.resources.dll.compressed");
		assemblyNames.Add("pl-pl.modernwpf.resources", "costura.pl-pl.modernwpf.resources.dll.compressed");
		assemblyNames.Add("pt-br.modernwpf.controls.resources", "costura.pt-br.modernwpf.controls.resources.dll.compressed");
		assemblyNames.Add("pt-br.modernwpf.resources", "costura.pt-br.modernwpf.resources.dll.compressed");
		assemblyNames.Add("pt-pt.modernwpf.controls.resources", "costura.pt-pt.modernwpf.controls.resources.dll.compressed");
		assemblyNames.Add("pt-pt.modernwpf.resources", "costura.pt-pt.modernwpf.resources.dll.compressed");
		assemblyNames.Add("ro-ro.modernwpf.controls.resources", "costura.ro-ro.modernwpf.controls.resources.dll.compressed");
		assemblyNames.Add("ro-ro.modernwpf.resources", "costura.ro-ro.modernwpf.resources.dll.compressed");
		assemblyNames.Add("ru-ru.modernwpf.controls.resources", "costura.ru-ru.modernwpf.controls.resources.dll.compressed");
		assemblyNames.Add("ru-ru.modernwpf.resources", "costura.ru-ru.modernwpf.resources.dll.compressed");
		assemblyNames.Add("sharpcompress", "costura.sharpcompress.dll.compressed");
		assemblyNames.Add("sk-sk.modernwpf.controls.resources", "costura.sk-sk.modernwpf.controls.resources.dll.compressed");
		assemblyNames.Add("sk-sk.modernwpf.resources", "costura.sk-sk.modernwpf.resources.dll.compressed");
		assemblyNames.Add("sl-si.modernwpf.controls.resources", "costura.sl-si.modernwpf.controls.resources.dll.compressed");
		assemblyNames.Add("sl-si.modernwpf.resources", "costura.sl-si.modernwpf.resources.dll.compressed");
		assemblyNames.Add("snappier", "costura.snappier.dll.compressed");
		assemblyNames.Add("sq-al.modernwpf.controls.resources", "costura.sq-al.modernwpf.controls.resources.dll.compressed");
		assemblyNames.Add("sq-al.modernwpf.resources", "costura.sq-al.modernwpf.resources.dll.compressed");
		assemblyNames.Add("sr-latn-rs.modernwpf.controls.resources", "costura.sr-latn-rs.modernwpf.controls.resources.dll.compressed");
		assemblyNames.Add("sr-latn-rs.modernwpf.resources", "costura.sr-latn-rs.modernwpf.resources.dll.compressed");
		assemblyNames.Add("sv-se.modernwpf.controls.resources", "costura.sv-se.modernwpf.controls.resources.dll.compressed");
		assemblyNames.Add("sv-se.modernwpf.resources", "costura.sv-se.modernwpf.resources.dll.compressed");
		assemblyNames.Add("sw-ke.modernwpf.controls.resources", "costura.sw-ke.modernwpf.controls.resources.dll.compressed");
		assemblyNames.Add("sw-ke.modernwpf.resources", "costura.sw-ke.modernwpf.resources.dll.compressed");
		assemblyNames.Add("system.buffers", "costura.system.buffers.dll.compressed");
		assemblyNames.Add("system.memory", "costura.system.memory.dll.compressed");
		assemblyNames.Add("system.numerics.vectors", "costura.system.numerics.vectors.dll.compressed");
		assemblyNames.Add("system.runtime.compilerservices.unsafe", "costura.system.runtime.compilerservices.unsafe.dll.compressed");
		assemblyNames.Add("system.security.accesscontrol", "costura.system.security.accesscontrol.dll.compressed");
		assemblyNames.Add("system.security.principal.windows", "costura.system.security.principal.windows.dll.compressed");
		assemblyNames.Add("system.text.encoding.codepages", "costura.system.text.encoding.codepages.dll.compressed");
		assemblyNames.Add("system.threading.tasks.extensions", "costura.system.threading.tasks.extensions.dll.compressed");
		assemblyNames.Add("system.valuetuple", "costura.system.valuetuple.dll.compressed");
		assemblyNames.Add("ta-in.modernwpf.controls.resources", "costura.ta-in.modernwpf.controls.resources.dll.compressed");
		assemblyNames.Add("ta-in.modernwpf.resources", "costura.ta-in.modernwpf.resources.dll.compressed");
		assemblyNames.Add("te-in.modernwpf.controls.resources", "costura.te-in.modernwpf.controls.resources.dll.compressed");
		assemblyNames.Add("te-in.modernwpf.resources", "costura.te-in.modernwpf.resources.dll.compressed");
		assemblyNames.Add("th-th.modernwpf.controls.resources", "costura.th-th.modernwpf.controls.resources.dll.compressed");
		assemblyNames.Add("th-th.modernwpf.resources", "costura.th-th.modernwpf.resources.dll.compressed");
		assemblyNames.Add("tr-tr.modernwpf.controls.resources", "costura.tr-tr.modernwpf.controls.resources.dll.compressed");
		assemblyNames.Add("tr-tr.modernwpf.resources", "costura.tr-tr.modernwpf.resources.dll.compressed");
		assemblyNames.Add("uk-ua.modernwpf.controls.resources", "costura.uk-ua.modernwpf.controls.resources.dll.compressed");
		assemblyNames.Add("uk-ua.modernwpf.resources", "costura.uk-ua.modernwpf.resources.dll.compressed");
		assemblyNames.Add("uz-latn-uz.modernwpf.controls.resources", "costura.uz-latn-uz.modernwpf.controls.resources.dll.compressed");
		assemblyNames.Add("uz-latn-uz.modernwpf.resources", "costura.uz-latn-uz.modernwpf.resources.dll.compressed");
		assemblyNames.Add("vi-vn.modernwpf.controls.resources", "costura.vi-vn.modernwpf.controls.resources.dll.compressed");
		assemblyNames.Add("vi-vn.modernwpf.resources", "costura.vi-vn.modernwpf.resources.dll.compressed");
		assemblyNames.Add("windowsapicodepack.shell.commonfiledialogs", "costura.windowsapicodepack.shell.commonfiledialogs.dll.compressed");
		assemblyNames.Add("wpf.ui", "costura.wpf.ui.dll.compressed");
		symbolNames.Add("wpf.ui", "costura.wpf.ui.pdb.compressed");
		assemblyNames.Add("zstdsharp", "costura.zstdsharp.dll.compressed");
	}

	public static void Attach()
	{
		if (Interlocked.Exchange(ref isAttached, 1) == 1)
		{
			return;
		}
		AppDomain currentDomain = AppDomain.CurrentDomain;
		currentDomain.AssemblyResolve += delegate(object sender, ResolveEventArgs e)
		{
			lock (nullCacheLock)
			{
				if (nullCache.ContainsKey(e.Name))
				{
					return (Assembly)null;
				}
			}
			AssemblyName ResolveAssemblyrequestedAssemblyName = new AssemblyName(e.Name);
			Assembly ResolveAssemblyassembly = ReadExistingAssembly(ResolveAssemblyrequestedAssemblyName);
			if ((object)ResolveAssemblyassembly != null)
			{
				return ResolveAssemblyassembly;
			}
			ResolveAssemblyassembly = ReadFromEmbeddedResources(assemblyNames, symbolNames, ResolveAssemblyrequestedAssemblyName);
			if ((object)ResolveAssemblyassembly == null)
			{
				lock (nullCacheLock)
				{
					nullCache[e.Name] = true;
				}
				if ((ResolveAssemblyrequestedAssemblyName.Flags & AssemblyNameFlags.Retargetable) != 0)
				{
					ResolveAssemblyassembly = Assembly.Load(ResolveAssemblyrequestedAssemblyName);
				}
			}
			return ResolveAssemblyassembly;
		};
	}
}
