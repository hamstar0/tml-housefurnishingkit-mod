using HamstarHelpers.Helpers.TModLoader.Mods;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;


namespace HouseFurnishingKit {
	public class HouseFurnishingKitMod : Mod {
		public static string GithubUserName => "hamstar0";
		public static string GithubProjectName => "tml-adventuremode-mod";


		////////////////

		public static HouseFurnishingKitMod Instance { get; private set; }




		////////////////

		internal (ushort TileType, int Width, int Height) CustomFurniture = (0, 0, 0);
		internal (ushort TileType, int Width, int Height) Custom3x3WallMount1 = (0, 0, 0);
		internal (ushort TileType, int Width, int Height) Custom3x3WallMount2 = (0, 0, 0);
		internal IList<Action<int, int, Item>> OnHouseCreate = new List<Action<int, int, Item>>();



		////////////////

		public HouseFurnishingKitMod() {
			HouseFurnishingKitMod.Instance = this;
		}

		public override void Unload() {
			HouseFurnishingKitMod.Instance = null;
		}


		////////////////

		public override object Call( params object[] args ) {
			return ModBoilerplateHelpers.HandleModCall( typeof(HouseFurnishingKitAPI), args );
		}
	}
}