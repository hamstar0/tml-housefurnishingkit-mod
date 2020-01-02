using System;
using System.ComponentModel;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader.Config;
using HamstarHelpers.Services.Configs;


namespace PrefabKits {
	public class PrefabKitsConfig : StackableModConfig {
		public static PrefabKitsConfig Instance => ModConfigStack.GetMergedConfigs<PrefabKitsConfig>();



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
		public int HouseFurnishingKitPrice { get; set; } = 100000;  //Item.buyPrice( 0, 10, 0, 0 );


		[DefaultValue( true )]
		public bool TrackDeploymentKitEnabled { get; set; } = true;
		
		[Range( 0, 9999 )]
		[DefaultValue( 100 )]
		[ReloadRequired]
		public int TrackDeploymentKitTracks { get; set; } = 100;

		[Range( -1, 9999 )]
		[DefaultValue( TileID.WorkBenches )]
		[ReloadRequired]
		public int TrackDeploymentKitRecipeTile { get; set; } = TileID.WorkBenches;

		[ReloadRequired]
		public List<ItemDefinition> TrackDeploymentKitRecipeExtraIngredient { get; set; } = new List<ItemDefinition> {
			new ItemDefinition( ItemID.GrapplingHook )
		};


		[Range( 0, 9999 )]
		public ushort CustomFurnitureTile { get; set; } = 0;

		[Range( 0, 9999 )]
		public ushort CustomWallMount1Tile { get; set; } = 0;

		[Range( 0, 9999 )]
		public ushort CustomWallMount2Tile { get; set; } = 0;

		[Range( 0, 9999 )]
		public ushort CustomFloorTile { get; set; } = 0;


		[Range( 16, 1024 )]
		[DefaultValue( 80 )]
		public int MinimumFurnishableHouseArea { get; set; } = 80;

		[Range( 4, 128 )]
		[DefaultValue( 12 )]
		public int MinimumFurnishableHouseFloorWidth { get; set; } = 12;
	}
}
