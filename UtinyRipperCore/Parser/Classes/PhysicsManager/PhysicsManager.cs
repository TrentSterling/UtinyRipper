﻿using System.Collections.Generic;
using UtinyRipper.AssetExporters;
using UtinyRipper.Classes.PhysicsManagers;
using UtinyRipper.Exporter.YAML;
using UtinyRipper.SerializedFiles;

namespace UtinyRipper.Classes
{
	public sealed class PhysicsManager : GlobalGameManager
	{
		public PhysicsManager(AssetInfo assetInfo):
			base(assetInfo)
		{
		}

		/// <summary>
		/// 5.0.0 and greater
		/// </summary>
		public static bool IsReadSleepThreshold(Version version)
		{
			return version.IsGreaterEqual(5);
		}
		/// <summary>
		/// 5.0.0b1 and less
		/// </summary>
		public static bool IsReadSleepAngularVelocity(Version version)
		{
			return version.IsLessEqual(5, 0, 0, VersionType.Beta, 1);
		}
		/// <summary>
		/// Greater than 5.0.0b1
		/// </summary>
		public static bool IsReadDefaultContactOffset(Version version)
		{
			return version.IsGreater(5, 0, 0, VersionType.Beta, 1);
		}
		/// <summary>
		/// 5.4.0 and greater
		/// </summary>
		public static bool IsReadDefaultSolverVelocityIterations(Version version)
		{
			return version.IsGreaterEqual(5, 4);
		}
		/// <summary>
		/// 5.5.0 and greater
		/// </summary>
		public static bool IsReadQueriesHitBackfaces(Version version)
		{
			return version.IsGreaterEqual(5, 5);
		}
		/// <summary>
		/// 2.6.0 and greater
		/// </summary>
		public static bool IsReadQueriesHitTriggers(Version version)
		{
			return version.IsGreaterEqual(2, 6);
		}
		/// <summary>
		/// 5.0.0 and greater
		/// </summary>
		public static bool IsReadEnableAdaptiveForce(Version version)
		{
			return version.IsGreaterEqual(5);
		}
		/// <summary>
		/// 5.0.0 to 2017.2
		/// </summary>
		public static bool IsReadEnablePCM(Version version)
		{
			return version.IsGreaterEqual(5) && version.IsLessEqual(2017, 2);
		}
		/// <summary>
		/// 2017.3 and greater
		/// </summary>
		public static bool IsReadClothInterCollisionDistance(Version version)
		{
			return version.IsGreaterEqual(2017, 3);
		}
		/// <summary>
		/// 3.0.0 and greater
		/// </summary>
		public static bool IsReadLayerCollisionMatrix(Version version)
		{
			return version.IsGreaterEqual(3);
		}
		/// <summary>
		/// 2017.1.0b2 and greater
		/// </summary>
		public static bool IsReadAutoSimulation(Version version)
		{
			return version.IsGreaterEqual(2017, 1, 0, VersionType.Beta, 2);
		}
		/// <summary>
		/// 2017.2 and greater
		/// </summary>
		public static bool IsReadAutoSyncTransforms(Version version)
		{
			return version.IsGreaterEqual(2017, 2);
		}
		/// <summary>
		/// 2017.3 and greater
		/// </summary>
		public static bool IsReadClothInterCollisionSettingsToggle(Version version)
		{
			return version.IsGreaterEqual(2017, 3);
		}

		/// <summary>
		/// 3.0.0 and greater
		/// </summary>
		private static bool IsAlign(Version version)
		{
			return version.IsGreaterEqual(3);
		}

		private static int GetSerializedVersion(Version version)
		{
			if (Config.IsExportTopmostSerializedVersion)
			{
				return 7;
			}

			if (version.IsGreaterEqual(2017, 3))
			{
				return 7;
			}

			// somewhere in 2017.3 alpha
			//return 6;
			//return 5;
			//return 4;

			// SolverIterationCount renamed to DefaultSolverIterations
			// SolverVelocityIterations renamed to DefaultSolverVelocityIterations
			if (version.IsGreaterEqual(5, 5))
			{
				return 3;
			}
			// RaycastsHitTriggers renamed to QueriesHitTriggers
			if (version.IsGreaterEqual(5, 2, 1))
			{
				return 2;
			}
			return 1;
		}

		public override void Read(AssetReader reader)
		{
			base.Read(reader);

			Gravity.Read(reader);
			DefaultMaterial.Read(reader);
			BounceThreshold = reader.ReadSingle();
			if (IsReadSleepThreshold(reader.Version))
			{
				SleepThreshold = reader.ReadSingle();
			}
			else
			{
				SleepVelocity = reader.ReadSingle();
				SleepAngularVelocity = reader.ReadSingle();
			}
			if (IsReadSleepAngularVelocity(reader.Version))
			{
				MaxAngularVelocity = reader.ReadSingle();
			}
			if(IsReadDefaultContactOffset(reader.Version))
			{
				DefaultContactOffset = reader.ReadSingle();
			}
			else
			{
				MinPenetrationForPenalty = reader.ReadSingle();
			}
			DefaultSolverIterations = reader.ReadInt32();
			if (IsReadDefaultSolverVelocityIterations(reader.Version))
			{
				DefaultSolverVelocityIterations = reader.ReadInt32();
			}
			if (IsReadQueriesHitBackfaces(reader.Version))
			{
				QueriesHitBackfaces = reader.ReadBoolean();
			}
			if (IsReadQueriesHitTriggers(reader.Version))
			{
				QueriesHitTriggers = reader.ReadBoolean();
			}
			if (IsReadEnableAdaptiveForce(reader.Version))
			{
				EnableAdaptiveForce = reader.ReadBoolean();
			}
			if (IsReadEnablePCM(reader.Version))
			{
				EnablePCM = reader.ReadBoolean();
			}
			if (IsAlign(reader.Version))
			{
				reader.AlignStream(AlignType.Align4);
			}

			if (IsReadClothInterCollisionDistance(reader.Version))
			{
				ClothInterCollisionDistance = reader.ReadSingle();
				ClothInterCollisionStiffness = reader.ReadSingle();
				ContactsGeneration = (ContactsGeneration)reader.ReadInt32();
				reader.AlignStream(AlignType.Align4);
			}

			if (IsReadLayerCollisionMatrix(reader.Version))
			{
				m_layerCollisionMatrix = reader.ReadUInt32Array();
			}

			if (IsReadAutoSimulation(reader.Version))
			{
				AutoSimulation = reader.ReadBoolean();
			}
			if (IsReadAutoSyncTransforms(reader.Version))
			{
				AutoSyncTransforms = reader.ReadBoolean();
			}
			if (IsReadClothInterCollisionSettingsToggle(reader.Version))
			{
				ClothInterCollisionSettingsToggle = reader.ReadBoolean();
				reader.AlignStream(AlignType.Align4);

				ContactPairsMode = (ContactPairsMode)reader.ReadInt32();
				BroadphaseType = (BroadphaseType)reader.ReadInt32();
				WorldBounds.Read(reader);
				WorldSubdivisions = reader.ReadInt32();
			}
		}

		public override IEnumerable<Object> FetchDependencies(ISerializedFile file, bool isLog = false)
		{
			foreach(Object asset in base.FetchDependencies(file, isLog))
			{
				yield return asset;
			}

			yield return DefaultMaterial.FetchDependency(file, isLog, ToLogString, "m_DefaultMaterial");
		}

		protected override YAMLMappingNode ExportYAMLRoot(IExportContainer container)
		{
#warning TODO: values acording to read version (current 2017.3.0f3)
			YAMLMappingNode node = base.ExportYAMLRoot(container);
			node.AddSerializedVersion(GetSerializedVersion(container.Version));
			node.Add("m_Gravity", Gravity.ExportYAML(container));
			node.Add("m_DefaultMaterial", DefaultMaterial.ExportYAML(container));
			node.Add("m_BounceThreshold", BounceThreshold);
			node.Add("m_SleepThreshold", GetSleepThreshold(container.Version));
			node.Add("m_DefaultContactOffset", GetDefaultContactOffset(container.Version));
			node.Add("m_DefaultSolverIterations", DefaultSolverIterations);
			node.Add("m_DefaultSolverVelocityIterations", GetDefaultSolverVelocityIterations(container.Version));
			node.Add("m_QueriesHitBackfaces", QueriesHitBackfaces);
			node.Add("m_QueriesHitTriggers", GetQueriesHitTriggers(container.Version));
			node.Add("m_EnableAdaptiveForce", EnableAdaptiveForce);
			node.Add("m_ClothInterCollisionDistance", ClothInterCollisionDistance);
			node.Add("m_ClothInterCollisionStiffness", ClothInterCollisionStiffness);
			node.Add("m_ContactsGeneration", (int)GetContactsGeneration(container.Version));
			node.Add("m_LayerCollisionMatrix", GetLayerCollisionMatrix(container.Version).ExportYAML(true));
			node.Add("m_AutoSimulation", GetAutoSimulation(container.Version));
			node.Add("m_AutoSyncTransforms", GetAutoSyncTransforms(container.Version));
			node.Add("m_ClothInterCollisionSettingsToggle", ClothInterCollisionSettingsToggle);
			node.Add("m_ContactPairsMode", (int)ContactPairsMode);
			node.Add("m_BroadphaseType", (int)BroadphaseType);
			node.Add("m_WorldBounds", GetWorldBounds(container.Version).ExportYAML(container));
			node.Add("m_WorldSubdivisions", GetWorldSubdivisions(container.Version));
			return node;
		}

		private float GetSleepThreshold(Version version)
		{
			return IsReadSleepThreshold(version) ? SleepThreshold : 0.005f;
		}
		private float GetDefaultContactOffset(Version version)
		{
			return IsReadDefaultContactOffset(version) ? DefaultContactOffset : 0.01f;
		}
		private int GetDefaultSolverVelocityIterations(Version version)
		{
			return IsReadDefaultSolverVelocityIterations(version) ? DefaultSolverVelocityIterations : 1;
		}
		private bool GetQueriesHitTriggers(Version version)
		{
			return IsReadQueriesHitTriggers(version) ? QueriesHitTriggers : true;
		}
		private ContactsGeneration GetContactsGeneration(Version version)
		{
			return IsReadClothInterCollisionDistance(version) ? ContactsGeneration : ContactsGeneration.PersistentContactManifold;
		}
		private IReadOnlyList<uint> GetLayerCollisionMatrix(Version version)
		{
			if(IsReadLayerCollisionMatrix(version))
			{
				return LayerCollisionMatrix;
			}
			uint[] matrix = new uint[32];
			for(int i = 0; i < matrix.Length; i++)
			{
				matrix[i] = uint.MaxValue;
			}
			return matrix;
		}
		private bool GetAutoSimulation(Version version)
		{
			return IsReadAutoSimulation(version) ? AutoSimulation : true;
		}
		private bool GetAutoSyncTransforms(Version version)
		{
			return IsReadAutoSyncTransforms(version) ? AutoSyncTransforms : true;
		}
		private AABB GetWorldBounds(Version version)
		{
			if(IsReadAutoSyncTransforms(version))
			{
				return WorldBounds;
			}
			return new AABB(default, new Vector3f(250.0f, 250.0f, 250.0f));
		}
		private int GetWorldSubdivisions(Version version)
		{
			return IsReadClothInterCollisionSettingsToggle(version) ? WorldSubdivisions : 8;
		}

		public override string ExportName => "DynamicsManager";

		public float BounceThreshold { get; private set; }
		public float SleepThreshold { get; private set; }
		public float SleepVelocity { get; private set; }
		public float SleepAngularVelocity { get; private set; }
		public float MaxAngularVelocity { get; private set; }
		public float MinPenetrationForPenalty { get; private set; }
		public float DefaultContactOffset { get; private set; }
		/// <summary>
		/// SolverIterationCount previosuly
		/// </summary>
		public int DefaultSolverIterations { get; private set; }
		/// <summary>
		/// SolverVelocityIterations previously
		/// </summary>
		public int DefaultSolverVelocityIterations { get; private set; }
		public bool QueriesHitBackfaces { get; private set; }
		/// <summary>
		/// RaycastsHitTriggers previosly
		/// </summary>
		public bool QueriesHitTriggers { get; private set; }
		public bool EnableAdaptiveForce { get; private set; }
		public bool EnablePCM { get; private set; }
		public float ClothInterCollisionDistance { get; private set; }
		public float ClothInterCollisionStiffness { get; private set; }
		public ContactsGeneration ContactsGeneration { get; private set; }
		public IReadOnlyList<uint> LayerCollisionMatrix => m_layerCollisionMatrix;
		public bool AutoSimulation { get; private set; }
		public bool AutoSyncTransforms { get; private set; }
		public bool ClothInterCollisionSettingsToggle { get; private set; }
		public ContactPairsMode ContactPairsMode { get; private set; }
		public BroadphaseType BroadphaseType { get; private set; }
		public int WorldSubdivisions { get; private set; }

		public Vector3f Gravity;
		public PPtr<PhysicMaterial> DefaultMaterial;
		public AABB WorldBounds;

		private uint[] m_layerCollisionMatrix;
	}
}
