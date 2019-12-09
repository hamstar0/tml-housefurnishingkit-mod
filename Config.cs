using System;
using Terraria.ModLoader.Config;
using HamstarHelpers.Services.Configs;
using Terraria;
using System.ComponentModel;


namespace HouseKits {
	public class HouseKitsConfig : StackableModConfig {
		public static HouseKitsConfig Instance => ModConfigStack.GetMergedConfigs<HouseKitsConfig>();



		////////////////

		public override ConfigScope Mode => ConfigScope.ServerSide;


		////////////////

		public bool DebugModeInfo { get; set; } = false;

		////

		[Range(0, 99999999 )]
		[DefaultValue( 100000 )]
		public int HouseFramingKitPrice { get; set; } = 100000; //Item.buyPrice( 0, 10, 0, 0 );

		[Range( 0, 99999999 )]
		[DefaultValue( 100000 )]
		public int HouseFurnishingKitPrice { get; set; } = 100000;	//Item.buyPrice( 0, 10, 0, 0 );
	}
}
