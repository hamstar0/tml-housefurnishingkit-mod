using HamstarHelpers.Helpers.TModLoader.Mods;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;


namespace HouseKits {
	public class HouseKitsMod : Mod {
		public static string GithubUserName => "hamstar0";
		public static string GithubProjectName => "tml-housefurnishingkit-mod";


		////////////////

		public static HouseKitsMod Instance { get; private set; }




		////////////////

		internal ushort CustomFurniture = 0;
		internal ushort CustomWallMount1 = 0;
		internal ushort CustomWallMount2 = 0;

		internal IList<Func<int, int, Item, bool>> OnPreHouseCreate = new List<Func<int, int, Item, bool>>();
		internal IList<Action<int, int, Item>> OnPostHouseCreate = new List<Action<int, int, Item>>();



		////////////////

		public HouseKitsMod() {
			HouseKitsMod.Instance = this;
		}

		public override void Unload() {
			HouseKitsMod.Instance = null;
		}


		////////////////

		public override object Call( params object[] args ) {
			return ModBoilerplateHelpers.HandleModCall( typeof(HouseKitsAPI), args );
		}
	}
}