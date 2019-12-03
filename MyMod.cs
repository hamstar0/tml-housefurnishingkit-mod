using HamstarHelpers.Helpers.TModLoader.Mods;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;


namespace HouseFurnishingKit {
	public class HouseFurnishingKitMod : Mod {
		public static string GithubUserName => "hamstar0";
		public static string GithubProjectName => "tml-housefurnishingkit-mod";


		////////////////

		public static HouseFurnishingKitMod Instance { get; private set; }




		////////////////

		internal ushort CustomFurniture = 0;
		internal ushort CustomWallMount1 = 0;
		internal ushort CustomWallMount2 = 0;

		internal IList<Func<int, int, Item, bool>> OnPreHouseCreate = new List<Func<int, int, Item, bool>>();
		internal IList<Action<int, int, Item>> OnPostHouseCreate = new List<Action<int, int, Item>>();



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