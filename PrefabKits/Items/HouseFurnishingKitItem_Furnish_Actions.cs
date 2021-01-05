using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ObjectData;
using HamstarHelpers.Classes.Errors;
using HamstarHelpers.Helpers.Debug;
using HamstarHelpers.Helpers.DotNET.Extensions;
using HamstarHelpers.Helpers.Tiles;


namespace PrefabKits.Items {
	public partial class HouseFurnishingKitItem : ModItem {
		private static void MarkFurnishedTiles(
					int leftX,
					int topY,
					int width,
					int height,
					IDictionary<int, ISet<int>> furnishedTiles ) {
			for( int x = leftX; x < leftX + width; x++ ) {
				for( int y = topY; y < topY + height; y++ ) {
					furnishedTiles.Set2D( x, y );
				}
			}
		}


		////////////////

		private static void MakeHouseWalls( IList<(ushort TileX, ushort TileY)> fullHouseSpace ) {
			foreach( (ushort tileX, ushort tileY) in fullHouseSpace ) {
				Tile tile = Main.tile[tileX, tileY];

				tile.wall = 0;
				tile.wallFrameX( 0 );
				tile.wallFrameY( 0 );
				//if( !Main.wallHouse[tile.wall] ) {
				WorldGen.PlaceWall( tileX, tileY, WallID.Wood, true );
				//}
			}
		}


		////////////////

		private static bool MakeHouseTile(
					int leftTileX,
					int floorTileY,
					ushort tileType,
					int style,
					sbyte direction,
					IList<(ushort TileX, ushort TileY)> houseTiles,
					IDictionary<int, ISet<int>> furnishedTiles ) {
			bool isContainer = tileType == TileID.Containers
				|| tileType == TileID.Containers2
				|| tileType == TileID.FakeContainers
				|| tileType == TileID.FakeContainers2;

			if( isContainer ) {
				int chestIdx = WorldGen.PlaceChest( leftTileX + 1, floorTileY, tileType );
				if( chestIdx == -1 ) {
					return false;
				}

				if( Main.netMode == 2 ) {
					int? chestTypeRaw = HamstarHelpers.Helpers.Tiles.Attributes.TileAttributeHelpers.GetChestTypeCode( tileType );
					int? chestType = chestTypeRaw.HasValue ? chestTypeRaw.Value : 0;

					NetMessage.SendData(
						msgType: MessageID.ChestUpdates,
						remoteClient: -1,
						ignoreClient: -1,
						text: null,
						number: chestType.Value,
						number2: (float)leftTileX,
						number3: (float)floorTileY,
						number4: 0f,
						number5: chestIdx,
						number6: tileType,
						number7: 0
					);
				}
			} else {
				if( !TilePlacementHelpers.PlaceObject( leftTileX, floorTileY, tileType, 0, direction ) ) {
					//if( !TilePlacementHelpers.TryPrecisePlace(leftTileX, floorTileY, tileType, style, direction) ) {
					if( !WorldGen.PlaceTile( leftTileX, floorTileY, tileType ) ) {
						HouseFurnishingKitItem.OutputPlacementError( leftTileX, floorTileY, tileType, "house tile" );
						return false;
					}
					WorldGen.SquareTileFrame( leftTileX, floorTileY );
				}
			}

			var tileObjData = TileObjectData.GetTileData( tileType, style );
			if( tileObjData == null ) {
				tileObjData = TileObjectData.Style1x1;
			}

			int aboveFloorTileY = floorTileY - (tileObjData.Height - 1);
			HouseFurnishingKitItem.MarkFurnishedTiles(
				leftTileX,
				aboveFloorTileY,
				tileObjData.Width,
				tileObjData.Height,
				furnishedTiles
			);

/*int BLAH = 0;
Timers.SetTimer( "BLHA_"+tileType, 3, false, () => {
	for( int i=leftTileX; i<leftTileX+tileObjData.Width; i++ ) {
		for( int j=floorTileY; j>floorTileY-tileObjData.Height; j-- ) {
			Dust.QuickDust( new Point(i, j), Color.Red );
		}
	}
	return BLAH++ < 100;
} );*/
			return true;
		}

		////

		private static bool MakeHouseWallTile3x3(
					int leftTileX,
					int topTileY,
					ushort tileType,
					IDictionary<int, ISet<int>> furnishedTiles ) {
			TilePlacementHelpers.Place3x3Wall( leftTileX, topTileY, tileType, 0 );

			Tile tile = Main.tile[ leftTileX, topTileY ];
			if( !tile.active() || tile.type != tileType ) {
				HouseFurnishingKitItem.OutputPlacementError( leftTileX, topTileY, tileType, "3x3 wall tile" );
				return false;
			}

			HouseFurnishingKitItem.MarkFurnishedTiles( leftTileX, topTileY, 3, 3, furnishedTiles );
			return true;
		}
	}
}
