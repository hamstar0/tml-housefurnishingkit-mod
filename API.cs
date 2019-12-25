using System;
using Terraria;


namespace PrefabKits {
	public class PrefabKitsAPI {
		public static void OnPreHouseCreate( Func<int, int, Item, bool> func ) {
			PrefabKitsMod.Instance.OnPreHouseCreate.Add( func );
		}


		public static void OnPostHouseCreate( Action<int, int, Item> action ) {
			PrefabKitsMod.Instance.OnPostHouseCreate.Add( action );
		}
	}
}