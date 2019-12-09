using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ObjectData;
using HamstarHelpers.Helpers.DotNET.Extensions;
using HamstarHelpers.Helpers.Tiles;
using HamstarHelpers.Classes.Errors;
using HamstarHelpers.Helpers.Debug;


namespace HouseKits.Items {
	public partial class HouseFurnishingKitItem : ModItem {
		private static void CleanHouse( IList<(ushort TileX, ushort TileY)> fullHouseSpace ) {	// Careful!
			foreach( (ushort tileX, ushort tileY) in fullHouseSpace ) {
				Tile tile = Main.tile[ tileX, tileY ];

				if( HouseFurnishingKitItem.IsCleanableTile(tile) ) {
					tile.active( false );
					tile.type = 0;
					//WorldGen.KillTile( tileX, tileY, false, false, true );
				}
			}
		}


		////////////////

		private static void MakeHouseWalls( IList<(ushort TileX, ushort TileY)> fullHouseSpace ) {
			foreach( (ushort tileX, ushort tileY) in fullHouseSpace ) {
				Tile tile = Main.tile[tileX, tileY];

				if( !Main.wallHouse[tile.wall] ) {
					WorldGen.PlaceWall( tileX, tileY, WallID.Wood, true );
				}
			}
		}


		////////////////

		private static void MarkOccupiedTiles( int leftX, int topY, int width, int height, IDictionary<int, ISet<int>> occupiedTiles ) {
			for( int x = leftX; x < leftX + width; x++ ) {
				for( int y = topY; y < topY + height; y++ ) {
					occupiedTiles.Set2D( x, y );
				}
			}
		}

		////

		private static void MakeHouseTile(
					int leftTileX,
					int floorTileY,
					ushort tileType,
					int style,
					sbyte direction,
					IDictionary<int, ISet<int>> occupiedTiles ) {
//int BLAH = 0;
//Timers.SetTimer( "BLHA_"+tileType, 5, false, () => {
//	Dust.NewDustPerfect( new Vector2((leftTileX<<4) + 8, (floorTileY<<4) + 8), 1, default(Vector2) );
//	return BLAH++ < 100;
//} );
			if( !TilePlacementHelpers.PlaceObject(leftTileX, floorTileY, tileType, 0, direction) ) {
				if( !TilePlacementHelpers.TryPrecisePlace(leftTileX, floorTileY, tileType, style, direction) ) {
					if( !WorldGen.PlaceTile(leftTileX, floorTileY, tileType ) ) {
						throw new ModHelpersException( "Could not place tile "
							+ (tileType >= Main.tileTexture.Length ? ""+tileType : TileID.Search.GetName(tileType))
						);
					}
				}
			}

			var tileObjData = TileObjectData.GetTileData( tileType, style );
			HouseFurnishingKitItem.MarkOccupiedTiles( leftTileX, floorTileY, tileObjData.Width, tileObjData.Height, occupiedTiles );
		}

		////

		private static void MakeHouseWallTile3x3(
					int leftTileX,
					int topTileY,
					ushort tileType,
					IDictionary<int, ISet<int>> occupiedTiles ) {
			TilePlacementHelpers.Place3x3Wall( leftTileX, topTileY, tileType, 0 );
			HouseFurnishingKitItem.MarkOccupiedTiles( leftTileX, topTileY, 3, 3, occupiedTiles );
		}

		////

		private static void MakeHouseCustomFurnishings(
					int leftTileX,
					int rightTileX,
					int floorTileY,
					IDictionary<int, ISet<int>> occupiedTiles ) {
			ushort custFurnType = HouseKitsMod.Instance.CustomFurniture;
			
			if( custFurnType != 0 ) {
				switch( custFurnType ) {
				case TileID.Bottles:
					TilePlacementHelpers.Place2x1(	leftTileX + 4,	floorTileY,		TileID.WorkBenches );
					WorldGen.PlaceTile(				leftTileX + 4,	floorTileY - 1,	TileID.Bottles );
					HouseFurnishingKitItem.MarkOccupiedTiles( leftTileX + 4, floorTileY - 1, 2, 2, occupiedTiles );
					break;
				case TileID.PiggyBank:
					TilePlacementHelpers.Place2x1(	leftTileX + 4,	floorTileY,		TileID.WorkBenches );
					TilePlacementHelpers.Place2x1(	leftTileX + 4,	floorTileY - 1,	TileID.PiggyBank );
					HouseFurnishingKitItem.MarkOccupiedTiles( leftTileX + 4, floorTileY - 1, 2, 2, occupiedTiles );
					break;
				default:
					HouseFurnishingKitItem.MakeHouseTile( leftTileX + 4, floorTileY, custFurnType, 0, -1, occupiedTiles );
					break;
				}
			}

			ushort custWallMount1 = HouseKitsMod.Instance.CustomWallMount1;
			if( custWallMount1 != 0 ) {
				HouseFurnishingKitItem.MakeHouseWallTile3x3( leftTileX, floorTileY - 4, custWallMount1, occupiedTiles );
			}

			ushort custWallMount2 = HouseKitsMod.Instance.CustomWallMount2;
			if( custWallMount2 != 0 ) {
				HouseFurnishingKitItem.MakeHouseWallTile3x3( rightTileX - 3, floorTileY - 4, custWallMount2, occupiedTiles );
			}
		}


		////////////////

		public static bool MakeHouseTileNear(
					ushort tileType,
					int tileX,
					int tileY,
					IList<(ushort TileX, ushort TileY)> houseTiles,
					IDictionary<int, ISet<int>> occupiedTiles ) {
			if( HouseFurnishingKitItem.MakeHouseTileNearOnce(tileType, tileX, tileY, houseTiles, occupiedTiles) ) {
				return true;
			}
			if( HouseFurnishingKitItem.MakeHouseTileNearOnce(tileType, tileX, tileY - 1, houseTiles, occupiedTiles) ) {
				return true;
			}
			if( HouseFurnishingKitItem.MakeHouseTileNearOnce(tileType, tileX, tileY + 1, houseTiles, occupiedTiles) ) {
				return true;
			}
			if( HouseFurnishingKitItem.MakeHouseTileNearOnce(tileType, tileX - 1, tileY, houseTiles, occupiedTiles) ) {
				return true;
			}
			if( HouseFurnishingKitItem.MakeHouseTileNearOnce(tileType, tileX + 1, tileY, houseTiles, occupiedTiles) ) {
				return true;
			}
			
			if( !occupiedTiles.Contains2D(tileX, tileY) && houseTiles.Contains( ((ushort)tileX, (ushort)(tileY - 1)) ) ) {
				if( HouseFurnishingKitItem.MakeHouseTileNear(tileType, tileX, tileY - 1, houseTiles, occupiedTiles) ) {
					return true;
				}
			}
			if( !occupiedTiles.Contains2D(tileX, tileY) && houseTiles.Contains( ((ushort)tileX, (ushort)(tileY + 1)) ) ) {
				if( HouseFurnishingKitItem.MakeHouseTileNear(tileType, tileX, tileY + 1, houseTiles, occupiedTiles) ) {
					return true;
				}
			}
			if( !occupiedTiles.Contains2D(tileX, tileY) && houseTiles.Contains( ((ushort)(tileX - 1), (ushort)tileY) ) ) {
				if( HouseFurnishingKitItem.MakeHouseTileNear(tileType, tileX - 1, tileY, houseTiles, occupiedTiles) ) {
					return true;
				}
			}
			if( !occupiedTiles.Contains2D(tileX, tileY) && houseTiles.Contains( ((ushort)(tileX + 1), (ushort)tileY) ) ) {
				if( HouseFurnishingKitItem.MakeHouseTileNear(tileType, tileX + 1, tileY, houseTiles, occupiedTiles) ) {
					return true;
				}
			}

			return false;
		}

		public static bool MakeHouseTileNearOnce(
					ushort tileType,
					int tileX,
					int tileY,
					IList<(ushort TileX, ushort TileY)> houseTiles,
					IDictionary<int, ISet<int>> occupiedTiles ) {
			if( occupiedTiles.Contains2D(tileX, tileY) ) {
				return false;
			}

			// Regardless of outcome, this time must be marked
			occupiedTiles.Set2D( tileX, tileY );

			if( !houseTiles.Contains( ((ushort)tileX, (ushort)tileY) ) ) {
				return false;
			}

			return WorldGen.PlaceTile( tileX, tileY, tileType );
		}
	}
}
