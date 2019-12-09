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

		
		public static void SetCustomFurniture( ushort tileType ) {
			HouseKitsMod.Instance.CustomFurniture = tileType;
		}

		public static void SetCustomWallMount1( ushort tileType ) {
			HouseKitsMod.Instance.CustomWallMount1 = tileType;
		}

		public static void SetCustomWallMount2( ushort tileType ) {
			HouseKitsMod.Instance.CustomWallMount2 = tileType;
		}
	}
}