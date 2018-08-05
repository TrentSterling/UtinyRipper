﻿using System.Collections.Generic;
using System.IO;

namespace UtinyRipper.Exporters.Scripts
{
	public abstract class ScriptExportParameter
	{
		public abstract void Init(IScriptExportManager manager);

		public void Export(TextWriter writer, int intent)
		{
			writer.Write("{0} {1}", Type.Name, Name);
		}

		public void GetUsedNamespaces(ICollection<string> namespaces)
		{
			Type.GetTypeNamespaces(namespaces);
		}

		protected abstract ScriptExportType Type { get; }

		protected abstract string Name { get; }
	}
}
