using System;
using Terraria;


namespace HouseFurnishingKit {
	public class HouseFurnishingKitAPI {
		public static void OnHouseCreate( Action<int, int, Item> action ) {
			HouseFurnishingKitMod.Instance.OnHouseCreate.Add( action );
		}
	}
}