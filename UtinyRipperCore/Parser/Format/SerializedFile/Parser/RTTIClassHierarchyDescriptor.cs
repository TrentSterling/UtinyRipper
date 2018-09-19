﻿using System;
using System.Collections.Generic;

namespace UtinyRipper.SerializedFiles
{
	public sealed class RTTIClassHierarchyDescriptor
	{
		public RTTIClassHierarchyDescriptor(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException(nameof(name));
			}
			Name = name;
		}

		/// <summary>
		/// 3.0.0b and greater
		/// </summary>
		public static bool IsReadSignature(FileGeneration generation)
		{
			return generation > FileGeneration.FG_300b;
		}
		/// <summary>
		/// 3.0.0 and greater
		/// </summary>
		public static bool IsReadAttributes(FileGeneration generation)
		{
			return generation >= FileGeneration.FG_300_342;
		}
		/// <summary>
		/// 5.0.0Unk2 and greater
		/// </summary>
		public static bool IsReadSerializeTypeTrees(FileGeneration generation)
		{
			return generation >= FileGeneration.FG_500aunk2;
		}
		/// <summary>
		/// 3.0.0b to 4.x.x
		/// </summary>
		public static bool IsReadUnknown(FileGeneration generation)
		{
			return generation >= FileGeneration.FG_300b && generation < FileGeneration.FG_500;
		}

		public void Read(SerializedFileReader reader)
		{
			if (IsReadSignature(reader.Generation))
			{
				string signature = reader.ReadStringZeroTerm();
				Version.Parse(signature);
			}
			if (IsReadAttributes(reader.Generation))
			{
				Platform = (Platform)reader.ReadUInt32();
				if (!Enum.IsDefined(typeof(Platform), Platform))
				{
					throw new Exception($"Unsuported platform {Platform} for asset file '{Name}'");
				}
			}
			if (IsReadSerializeTypeTrees(reader.Generation))
			{
				SerializeTypeTrees = reader.ReadBoolean();
			}
			else
			{
				SerializeTypeTrees = true;
			}
			m_types = reader.ReadArray(() => new RTTIBaseClassDescriptor(SerializeTypeTrees));

			if (IsReadUnknown(reader.Generation))
			{
				Unknown = reader.ReadInt32();
			}
		}

		/// <summary>
		/// Attributes
		/// </summary>
		public Platform Platform { get; private set; }
		internal bool SerializeTypeTrees { get; private set; }
		internal IReadOnlyList<RTTIBaseClassDescriptor> Types => m_types;
		internal int Unknown { get; private set; }
		
		private string Name { get; }

		/// <summary>
		/// Signature
		/// </summary>
		public Version Version;

		private RTTIBaseClassDescriptor[] m_types;
	}
}
