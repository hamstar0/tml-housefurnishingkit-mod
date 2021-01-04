using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using HamstarHelpers.Classes.Errors;
using HamstarHelpers.Helpers.Debug;


namespace PrefabKits.Items {
	public partial class HouseFurnishingKitItem : ModItem {
		private static void CleanHouse( IList<(ushort TileX, ushort TileY)> fullHouseSpace ) {	// Careful!
			foreach( (ushort tileX, ushort tileY) in fullHouseSpace ) {
				Tile tile = Main.tile[ tileX, tileY ];
				if( !HouseFurnishingKitItem.IsCleanableTile(tile) ) {
					continue;
				}

				if( tile.type == TileID.Containers || tile.type == TileID.Containers2 ) {
					if( Main.netMode == NetmodeID.Server ) {
						int? chestTypeRaw = HamstarHelpers.Helpers.Tiles.Attributes.TileAttributeHelpers.GetChestTypeCode(tile.type);
						int? chestType = chestTypeRaw.HasValue ? chestTypeRaw.Value : 0;

						NetMessage.SendData(
							msgType: MessageID.ChestUpdates,
							remoteClient: -1,
							ignoreClient: -1,
							text: null,
							number: chestType.Value,
							number2: (float)tileX,
							number3: (float)tileY,
							number4: 0f,
							number5: Chest.FindChest( tileX, tileY ),
							number6: tile.type,
							number7: 0
						);
					}
				}

				tile.ClearEverything();
				tile.active( false );
				tile.type = 0;
				//WorldGen.KillTile( tileX, tileY, false, false, true );
			}
		}


		////////////////

		public static void ChangeFlooring( ushort tileType, int leftX, int rightX, int floorY ) {
			for( int i=leftX-1; i<rightX+2; i++ ) {
				Tile tile = Main.tile[i, floorY + 1];
				tile.ClearEverything();

				tile.active( true );
				tile.type = tileType;

				WorldGen.SquareTileFrame( i, floorY + 1 );
			}
		}
	}
}
