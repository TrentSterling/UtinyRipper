﻿namespace UtinyRipper.Classes
{
	public abstract class GlobalGameManager : GameManager
	{
		protected GlobalGameManager(AssetInfo assetInfo):
			base(assetInfo)
		{
		}

		public override string ExportName => ClassID.ToString();
	}
}
