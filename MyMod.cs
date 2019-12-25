using HamstarHelpers.Helpers.TModLoader.Mods;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;


namespace PrefabKits {
	public class PrefabKitsMod : Mod {
		public static string GithubUserName => "hamstar0";
		public static string GithubProjectName => "tml-housefurnishingkit-mod";


		////////////////

		public static PrefabKitsMod Instance { get; private set; }




		////////////////

		internal IList<Func<int, int, Item, bool>> OnPreHouseCreate = new List<Func<int, int, Item, bool>>();
		internal IList<Action<int, int, Item>> OnPostHouseCreate = new List<Action<int, int, Item>>();



		////////////////

		public PrefabKitsMod() {
			PrefabKitsMod.Instance = this;
		}

		public override void Unload() {
			PrefabKitsMod.Instance = null;
		}


		////////////////

		public override object Call( params object[] args ) {
			return ModBoilerplateHelpers.HandleModCall( typeof(PrefabKitsAPI), args );
		}
	}
}