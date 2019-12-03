using System;
using Terraria;


namespace HouseFurnishingKit {
	public class HouseFurnishingKitAPI {
		public static void OnPreHouseCreate( Func<int, int, Item, bool> func ) {
			HouseFurnishingKitMod.Instance.OnPreHouseCreate.Add( func );
		}


		public static void OnPostHouseCreate( Action<int, int, Item> action ) {
			HouseFurnishingKitMod.Instance.OnPostHouseCreate.Add( action );
		}

		
		public static void SetCustomFurniture( ushort tileType ) {
			HouseFurnishingKitMod.Instance.CustomFurniture = tileType;
		}

		public static void SetCustomWallMount1( ushort tileType ) {
			HouseFurnishingKitMod.Instance.CustomWallMount1 = tileType;
		}

		public static void SetCustomWallMount2( ushort tileType ) {
			HouseFurnishingKitMod.Instance.CustomWallMount2 = tileType;
		}
	}
}