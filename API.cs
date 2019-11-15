using System;
using Terraria;


namespace HouseFurnishingKit {
	public class HouseFurnishingKitAPI {
		public static void OnHouseCreate( Action<int, int, Item> action ) {
			HouseFurnishingKitMod.Instance.OnHouseCreate.Add( action );
		}


		public static void SetCustomFurniture( ushort tileType, int width, int height ) {
			HouseFurnishingKitMod.Instance.CustomFurniture = (tileType, width, height);
		}

		public static void SetCustom3x3WallMount1( ushort tileType, int width, int height ) {
			HouseFurnishingKitMod.Instance.Custom3x3WallMount1 = (tileType, width, height);
		}

		public static void SetCustom3x3WallMount2( ushort tileType, int width, int height ) {
			HouseFurnishingKitMod.Instance.Custom3x3WallMount2 = (tileType, width, height);
		}
	}
}