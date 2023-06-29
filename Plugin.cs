using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dotx64Dbg;
using Microsoft.Diagnostics.Runtime;
namespace DotInfo
{
	public class Plugin : IPlugin
	{
		public DataTarget Target;
		public ClrRuntime Runtime;
		
		[Command("dinit")]
		public void Init(string[] args)
		{
			Target = DataTarget.AttachToProcess((int)Process.PID, false);
			Runtime = Target.ClrVersions.First().CreateRuntime();
			Console.WriteLine("Success!");
		}

		[Command("nameallmodules")]
		public void NameAllModules(string[] args)
		{
			if (Runtime == null)
			{
				Console.WriteLine("Please, use command dinit to init runtime!");
				return;
			}

			foreach(var m in Runtime.EnumerateModules())
			{
				Console.WriteLine(m.Name.Split(Path.DirectorySeparatorChar).Last());
			}
		}

		[Command("analyzemodule")]
		public void AnalyzeModuleCmd(string[] args)
		{
			if (Runtime == null)
			{
				Console.WriteLine("Please, use command dinit to init runtime!");
				return;
			}

			var module = Runtime.EnumerateModules().FirstOrDefault(m => m.Name.Split(Path.DirectorySeparatorChar).Last() == args[1]);

			if (module == null)
			{
				Console.WriteLine("Bad name!");
				return; 
			}

			AnalyzeModule(module);
		}

		[Command("analyzeallemodules")]
		public void AnalyzeAllModulesCmd(string[] args)
		{
			if (Runtime == null)
			{
				Console.WriteLine("Please, use command dinit to init runtime!");
				return;
			}

			foreach(var module in Runtime.EnumerateModules())
			{
				AnalyzeModule(module);
			}
		}

		public void AnalyzeModule(ClrModule module)
		{
			foreach (var typetoken in module.EnumerateTypeDefToMethodTableMap())
			{
				var table = Runtime.GetTypeByMethodTable(typetoken.MethodTable);
				if (table == null) continue;
				foreach (var method in table.Methods)
				{
					if (method.NativeCode == 0xFFFFFFFFFFFFFFFF)
						continue;

					if (Symbols.Label.Get((UIntPtr)method.NativeCode) != null)
						continue;

					Console.WriteLine($"Setted {method.Name} at 0x{method.NativeCode.ToString("X2")}. Signature: {method.Signature}");
					//Symbols.Comment.Set((UIntPtr)method.NativeCode, method.Signature, false);
					Symbols.Function.Set((UIntPtr)method.HotColdInfo.HotStart, (UIntPtr)(method.HotColdInfo.HotStart + method.HotColdInfo.HotSize), false);
					Symbols.Function.Set((UIntPtr)method.HotColdInfo.ColdStart, (UIntPtr)(method.HotColdInfo.ColdStart + method.HotColdInfo.ColdSize), false);
					Symbols.Label.Set((UIntPtr)method.NativeCode, method.Signature, Symbols.Label.Attribs.None);
				}
				foreach (var staticField in table.StaticFields)
				{
					var addr = (UIntPtr)staticField.GetAddress(module.AppDomain);
					Symbols.Label.Set(addr, $"{table.Name}.{staticField.Name}", Symbols.Label.Attribs.None);
				}
			}
		}
	}
}