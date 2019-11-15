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

		public static void SetCustomWallMount1( ushort tileType, int width, int height ) {
			if( width != 3 || height != 3 ) {
				throw new ArgumentException( "Non-3x3 sized wall mounts not implemented." );
			}
			HouseFurnishingKitMod.Instance.Custom3x3WallMount1 = (tileType, width, height);
		}

		public static void SetCustomWallMount2( ushort tileType, int width, int height ) {
			if( width != 3 || height != 3 ) {
				throw new ArgumentException( "Non-3x3 sized wall mounts not implemented." );
			}
			HouseFurnishingKitMod.Instance.Custom3x3WallMount2 = (tileType, width, height);
		}
	}
}