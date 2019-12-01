using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using System;
using HamstarHelpers.Helpers.DotNET.Extensions;
using HamstarHelpers.Helpers.Tiles;


namespace HouseFurnishingKit.Items {
	public partial class HouseFurnishingKitItem : ModItem {
		private static void CleanHouse( IList<(ushort TileX, ushort TileY)> fullHouseSpace ) {	// Careful!
			foreach( (ushort tileX, ushort tileY) in fullHouseSpace ) {
				Tile tile = Main.tile[ tileX, tileY ];

				if( HouseFurnishingKitItem.IsCleanableTile(tile, false) ) {
					WorldGen.KillTile( tileX, tileY, false, false, true );
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

		private static void MakeHouseTile4x2( ushort tileType, int tileX, int tileY, IDictionary<int, ISet<int>> occupiedTiles ) {
			TileHelpers.PlaceTile4x2( tileX, tileY, tileType, 1, 0 );
			HouseFurnishingKitItem.MarkOccupiedTiles( tileX, tileY, 4, 2, occupiedTiles );
		}

		private static void MakeHouseTile3x2( ushort tileType, int tileX, int tileY, IDictionary<int, ISet<int>> occupiedTiles ) {
			TileHelpers.PlaceTile3x2( tileX, tileY, tileType, 0 );
			HouseFurnishingKitItem.MarkOccupiedTiles( tileX, tileY, 3, 2, occupiedTiles );
		}

		private static void MakeHouseTile3x3( ushort tileType, int tileX, int tileY, IDictionary<int, ISet<int>> occupiedTiles ) {
			TileHelpers.PlaceTile3x3( tileX, tileY, tileType, 0 );
			HouseFurnishingKitItem.MarkOccupiedTiles( tileX, tileY, 3, 3, occupiedTiles );
		}

		private static void MakeHouseTile2x1( ushort tileType, int tileX, int tileY, IDictionary<int, ISet<int>> occupiedTiles ) {
			TileHelpers.PlaceTile2x1( tileX, tileY, tileType );
			HouseFurnishingKitItem.MarkOccupiedTiles( tileX, tileY, 2, 1, occupiedTiles );
		}

		private static void MakeHouseTile1x2( ushort tileType, int tileX, int tileY, int xFrameOffset, IDictionary<int, ISet<int>> occupiedTiles ) {
			TileHelpers.PlaceTile1x2( tileX, tileY, tileType );
			HouseFurnishingKitItem.MarkOccupiedTiles( tileX, tileY, 1, 2, occupiedTiles );
			Main.tile[ tileX, tileY-1 ].frameX = (short)xFrameOffset;
			Main.tile[ tileX, tileY ].frameX = (short)xFrameOffset;
		}

		////

		private static void MakeHouseWallTile3x3( ushort tileType, int tileX, int tileY, IDictionary<int, ISet<int>> occupiedTiles ) {
			TileHelpers.PlaceTile3x3Wall( tileX, tileY, tileType, 0 );
			HouseFurnishingKitItem.MarkOccupiedTiles( tileX, tileY, 3, 3, occupiedTiles );
		}

		////

		private static void MakeHouseCustomFurnishings(
					int leftTileX,
					int rightTileX,
					int floorY,
					IDictionary<int, ISet<int>> occupiedTiles ) {
			(ushort TileType, int Width, int Height) custFurniture = HouseFurnishingKitMod.Instance.CustomFurniture;

			if( custFurniture.TileType != 0 ) {
//Main.NewText( "placing CustomFurniture " + custFurniture.TileType );
//Dust.NewDustPerfect( new Vector2(leftTileX<<4, tileY<<4), 1 );
				switch( custFurniture.TileType ) {
				case TileID.Bottles:
					WorldGen.PlaceTile(					leftTileX + 4,	floorY - 1, TileID.Platforms );
					WorldGen.PlaceTile(					leftTileX + 4,	floorY - 2, custFurniture.TileType );
					HouseFurnishingKitItem.MarkOccupiedTiles(	leftTileX + 5,	floorY - 2, 1, 2, occupiedTiles );
					break;
				case TileID.PiggyBank:
					WorldGen.PlaceTile(					leftTileX + 4,	floorY - 1, TileID.Platforms );
					WorldGen.PlaceTile(					leftTileX + 5,	floorY - 1, TileID.Platforms );
					TileHelpers.PlaceTile2x1(			leftTileX + 4,	floorY - 2, custFurniture.TileType );
					HouseFurnishingKitItem.MarkOccupiedTiles(	leftTileX + 5,	floorY - 2, 2, 2, occupiedTiles );
					break;
				default:
					if( custFurniture.Width == 3 && custFurniture.Height == 2 ) {
						HouseFurnishingKitItem.MakeHouseTile3x2( custFurniture.TileType, leftTileX + 4, floorY - 1, occupiedTiles );
					} else if( custFurniture.Width == 3 && custFurniture.Height == 3 ) {
						HouseFurnishingKitItem.MakeHouseTile3x3( custFurniture.TileType, leftTileX + 4, floorY - 2, occupiedTiles );
					} else {
						WorldGen.PlaceTile(		leftTileX + 4,	floorY, custFurniture.TileType );
						occupiedTiles.Set2D(	leftTileX + 4,	floorY );
					}
					break;
				}
			}

			(ushort TileType, int Width, int Height) custWallMount1 = HouseFurnishingKitMod.Instance.Custom3x3WallMount1;
			if( custWallMount1.TileType != 0 ) {
//Main.NewText( "placing Custom3x3WallMount1 " + custWallMount1.TileType );
//Dust.NewDustPerfect( new Vector2(leftTileX<<4, tileY<<4), 1 );
				if( custWallMount1.Width == 3 && custWallMount1.Height == 3 ) {
					HouseFurnishingKitItem.MakeHouseWallTile3x3( custWallMount1.TileType, leftTileX, floorY - 4, occupiedTiles );
				}
			}

			(ushort TileType, int Width, int Height) custWallMount2 = HouseFurnishingKitMod.Instance.Custom3x3WallMount2;
			if( custWallMount2.TileType != 0 ) {
//Main.NewText( "placing Custom3x3WallMount2 " + custWallMount2.TileType );
//Dust.NewDustPerfect( new Vector2(leftTileX<<4, tileY<<4), 1 );
				if( custWallMount2.Width == 3 && custWallMount2.Height == 3 ) {
					HouseFurnishingKitItem.MakeHouseWallTile3x3( custWallMount2.TileType, rightTileX - 3, floorY - 4, occupiedTiles );
				}
			}
		}


		////////////////

		public static void MakeHouseTileNear(
					ushort tileType,
					int tileX,
					int tileY,
					IList<(ushort TileX, ushort TileY)> houseTiles,
					IDictionary<int, ISet<int>> occupiedTiles ) {
			if( HouseFurnishingKitItem.MakeHouseTileNearOnce(tileType, tileX, tileY, houseTiles, occupiedTiles) ) {
				return;
			}
			if( HouseFurnishingKitItem.MakeHouseTileNearOnce(tileType, tileX, tileY - 1, houseTiles, occupiedTiles) ) {
				return;
			}
			if( HouseFurnishingKitItem.MakeHouseTileNearOnce(tileType, tileX, tileY + 1, houseTiles, occupiedTiles) ) {
				return;
			}
			if( HouseFurnishingKitItem.MakeHouseTileNearOnce(tileType, tileX - 1, tileY, houseTiles, occupiedTiles) ) {
				return;
			}
			if( HouseFurnishingKitItem.MakeHouseTileNearOnce(tileType, tileX + 1, tileY, houseTiles, occupiedTiles) ) {
				return;
			}

			if( !houseTiles.Contains( ((ushort)tileX, (ushort)(tileY - 1)) ) ) {
				HouseFurnishingKitItem.MakeHouseTileNear( tileType, tileX, tileY - 1, houseTiles, occupiedTiles );
			}
			if( houseTiles.Contains( ((ushort)tileX, (ushort)(tileY + 1)) ) ) {
				HouseFurnishingKitItem.MakeHouseTileNear( tileType, tileX, tileY + 1, houseTiles, occupiedTiles );
			}
			if( houseTiles.Contains( ((ushort)(tileX - 1), (ushort)tileY) ) ) {
				HouseFurnishingKitItem.MakeHouseTileNear( tileType, tileX - 1, tileY, houseTiles, occupiedTiles );
			}
			if( houseTiles.Contains( ((ushort)(tileX + 1), (ushort)tileY) ) ) {
				HouseFurnishingKitItem.MakeHouseTileNear( tileType, tileX + 1, tileY, houseTiles, occupiedTiles );
			}
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

			if( WorldGen.PlaceTile(tileX, tileY, tileType) ) {
				return true;
			}
			return false;
		}
	}
}
