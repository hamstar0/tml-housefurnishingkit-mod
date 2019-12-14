using System;
using Terraria;


namespace HouseKits {
	public class HouseKitsAPI {
		public static void OnPreHouseCreate( Func<int, int, Item, bool> func ) {
			HouseKitsMod.Instance.OnPreHouseCreate.Add( func );
		}


		public static void OnPostHouseCreate( Action<int, int, Item> action ) {
			HouseKitsMod.Instance.OnPostHouseCreate.Add( action );
		}
	}
}