using System;
using Terraria;


namespace PrefabKits {
	public class PrefabKitsAPI {
		public static void OnPreHouseCreate( Func<int, int, bool> func ) {
			PrefabKitsMod.Instance.OnPreHouseCreate.Add( func );
		}


		public static void OnPostHouseCreate( Action<int, int> action ) {
			PrefabKitsMod.Instance.OnPostHouseCreate.Add( action );
		}
	}
}