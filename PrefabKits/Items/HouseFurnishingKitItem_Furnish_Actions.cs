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
		private static void CleanHouse( IList<(ushort TileX, ushort TileY)> fullHouseSpace ) {	// Careful!
			foreach( (ushort tileX, ushort tileY) in fullHouseSpace ) {
				Tile tile = Main.tile[ tileX, tileY ];
				if( !HouseFurnishingKitItem.IsCleanableTile(tile) ) {
					continue;
				}

				tile.active( false );
				tile.type = 0;
				//WorldGen.KillTile( tileX, tileY, false, false, true );

				if( tile.type == TileID.Containers || tile.type == TileID.Containers2 ) {
					if( Main.netMode == 2 ) {
						int? chestTypeRaw = HamstarHelpers.Helpers.Tiles.Attributes.TileAttributeHelpers.GetChestTypeCode( tile.type );
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

		private static void MarkOccupiedTiles( int leftX, int topY, int width, int height, IDictionary<int, ISet<int>> occupiedTiles ) {
			for( int x = leftX; x < leftX + width; x++ ) {
				for( int y = topY; y < topY + height; y++ ) {
					occupiedTiles.Set2D( x, y );
				}
			}
		}

		////

		private static bool MakeHouseTile(
					int leftTileX,
					int floorTileY,
					ushort tileType,
					int style,
					sbyte direction,
					IList<(ushort TileX, ushort TileY)> houseTiles,
					IDictionary<int, ISet<int>> occupiedTiles ) {
			if( tileType == TileID.Containers || tileType == TileID.Containers2 || tileType == TileID.FakeContainers || tileType == TileID.FakeContainers2 ) {
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
						//throw new ModHelpersException( "Could not place tile "
						//	+ (tileType >= Main.tileTexture.Length ? ""+tileType : TileID.Search.GetName(tileType))
						//);
						return false;
					}
					WorldGen.SquareTileFrame( leftTileX, floorTileY );
				}
			}

			var tileObjData = TileObjectData.GetTileData( tileType, style );
			if( tileObjData == null ) {
				tileObjData = TileObjectData.Style1x1;
			}

			HouseFurnishingKitItem.MarkOccupiedTiles(
				leftTileX,
				floorTileY - (tileObjData.Height - 1),
				tileObjData.Width,
				tileObjData.Height,
				occupiedTiles
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
					IDictionary<int, ISet<int>> occupiedTiles ) {
			TilePlacementHelpers.Place3x3Wall( leftTileX, topTileY, tileType, 0 );

			Tile tile = Main.tile[leftTileX, topTileY];
			if( !tile.active() || tile.type != tileType ) {
				return false;
			}

			HouseFurnishingKitItem.MarkOccupiedTiles( leftTileX, topTileY, 3, 3, occupiedTiles );
			return true;
		}

		////

		private static void MakeHouseCustomFurnishings(
					int leftTileX,
					int rightTileX,
					int floorTileY,
					IList<(ushort TileX, ushort TileY)> houseTiles,
					IDictionary<int, ISet<int>> occupiedTiles ) {
			ushort custFurnType = PrefabKitsConfig.Instance.CustomFurnitureTile;
			if( custFurnType > 0 ) {
				HouseFurnishingKitItem.MakeHouseCustomMainFurniture(
					custFurnType,
					leftTileX,
					floorTileY,
					houseTiles,
					occupiedTiles
				);
			}

			ushort custWallMount1 = PrefabKitsConfig.Instance.CustomWallMount1Tile;
			if( custWallMount1 != 0 ) {
				HouseFurnishingKitItem.MakeHouseTileNear(
					( x, y ) => {
						if( y >= floorTileY - 4 ) { return false; }
						return HouseFurnishingKitItem.MakeHouseWallTile3x3( x, y, custWallMount1, occupiedTiles );
					},
					leftTileX,
					floorTileY - 3,
					houseTiles,
					occupiedTiles
				);
			}

			ushort custWallMount2 = PrefabKitsConfig.Instance.CustomWallMount2Tile;
			if( custWallMount2 != 0 ) {
				HouseFurnishingKitItem.MakeHouseTileNear(
					( x, y ) => {
						if( y >= floorTileY - 4 ) { return false; }
						return HouseFurnishingKitItem.MakeHouseWallTile3x3( x, y, custWallMount2, occupiedTiles );
					},
					rightTileX - 3,
					floorTileY - 4,
					houseTiles,
					occupiedTiles
				);
			}
		}


		private static void MakeHouseCustomMainFurniture(
					ushort tileType,
					int leftTileX,
					int floorTileY,
					IList<(ushort TileX, ushort TileY)> houseTiles,
					IDictionary<int, ISet<int>> occupiedTiles ) {
			bool placeTile( int x, int y ) {
				switch( tileType ) {
				case TileID.Bottles:
					TilePlacementHelpers.Place2x1( x, y, TileID.WorkBenches );
					if( WorldGen.PlaceTile( x, y - 1, TileID.Bottles ) ) {
						HouseFurnishingKitItem.MarkOccupiedTiles( x, y - 1, 2, 2, occupiedTiles );
						return true;
					}
					break;
				case TileID.PiggyBank:
					TilePlacementHelpers.Place2x1( x, y, TileID.WorkBenches );
					TilePlacementHelpers.Place2x1( x, y - 1, TileID.PiggyBank );
					if( Main.tile[x, y].type == TileID.WorkBenches ) {
						HouseFurnishingKitItem.MarkOccupiedTiles( x, y - 1, 2, 2, occupiedTiles );
						return true;
					}
					break;
				default:
					return HouseFurnishingKitItem.MakeHouseTile( x, y, tileType, 0, -1, houseTiles, occupiedTiles );
				}

				return false;
			}

			HouseFurnishingKitItem.MakeHouseTileNear(
				placeTile,
				leftTileX + 4,
				floorTileY,
				houseTiles,
				occupiedTiles
			);
		}


		////////////////

		public static bool MakeHouseTileNear(
					Func<int, int, bool> placer,
					int tileX,
					int tileY,
					IList<(ushort TileX, ushort TileY)> houseTiles,
					IDictionary<int, ISet<int>> occupiedTiles ) {
			var scan = new HashSet<(int, int)>();

			return HouseFurnishingKitItem.MakeHouseTileNearScan( placer, tileX, tileY, houseTiles, occupiedTiles, scan );
		}

		private static bool MakeHouseTileNearScan(
					Func<int, int, bool> placer,
					int tileX,
					int tileY,
					IList<(ushort TileX, ushort TileY)> houseTiles,
					IDictionary<int, ISet<int>> occupiedTiles,
					ISet<(int, int)> scan ) {
			if( HouseFurnishingKitItem.RunPlacerAt(placer, tileX, tileY, houseTiles, occupiedTiles) ) {
				return true;
			}
			if( HouseFurnishingKitItem.RunPlacerAt(placer, tileX, tileY - 1, houseTiles, occupiedTiles) ) {
				return true;
			}
			if( HouseFurnishingKitItem.RunPlacerAt(placer, tileX, tileY + 1, houseTiles, occupiedTiles) ) {
				return true;
			}
			if( HouseFurnishingKitItem.RunPlacerAt(placer, tileX - 1, tileY, houseTiles, occupiedTiles) ) {
				return true;
			}
			if( HouseFurnishingKitItem.RunPlacerAt(placer, tileX + 1, tileY, houseTiles, occupiedTiles) ) {
				return true;
			}

			//

			if( !scan.Contains((tileX, tileY-1)) ) {
				scan.Add( (tileX, tileY - 1) );
				if( !occupiedTiles.Contains2D(tileX, tileY-1) && houseTiles.Contains( ((ushort)tileX, (ushort)(tileY - 1)) ) ) {
					if( HouseFurnishingKitItem.MakeHouseTileNearScan( placer, tileX, tileY - 1, houseTiles, occupiedTiles, scan) ) {
						return true;
					}
				}
			}
			if( !scan.Contains((tileX, tileY+1)) ) {
				scan.Add( (tileX, tileY + 1) );
				if( !occupiedTiles.Contains2D(tileX, tileY+1) && houseTiles.Contains( ((ushort)tileX, (ushort)(tileY + 1)) ) ) {
					if( HouseFurnishingKitItem.MakeHouseTileNearScan( placer, tileX, tileY + 1, houseTiles, occupiedTiles, scan) ) {
						return true;
					}
				}
			}
			if( !scan.Contains((tileX-1, tileY)) ) {
				scan.Add( (tileX - 1, tileY) );
				if( !occupiedTiles.Contains2D(tileX-1, tileY) && houseTiles.Contains( ((ushort)(tileX - 1), (ushort)tileY) ) ) {
					if( HouseFurnishingKitItem.MakeHouseTileNearScan( placer, tileX - 1, tileY, houseTiles, occupiedTiles, scan) ) {
						return true;
					}
				}
			}
			if( !scan.Contains((tileX+1, tileY)) ) {
				scan.Add( (tileX + 1, tileY) );
				if( !occupiedTiles.Contains2D(tileX+1, tileY) && houseTiles.Contains( ((ushort)(tileX + 1), (ushort)tileY) ) ) {
					if( HouseFurnishingKitItem.MakeHouseTileNearScan( placer, tileX + 1, tileY, houseTiles, occupiedTiles, scan) ) {
						return true;
					}
				}
			}

			//

			return false;
		}

		public static bool RunPlacerAt(
					Func<int, int, bool> placer,
					int tileX,
					int tileY,
					IList<(ushort TileX, ushort TileY)> houseTiles,
					IDictionary<int, ISet<int>> occupiedTiles ) {
			if( occupiedTiles.Contains2D(tileX, tileY) ) {
//int BLAH = 0;
//Timers.SetTimer( "BLHA1_"+tileX+"_"+tileY, 3, false, () => {
//	Dust.QuickDust( new Point(tileX, tileY), Color.Red );
//	return BLAH++ < 100;
//} );
				return false;
			}

			if( !houseTiles.Contains( ((ushort)tileX, (ushort)tileY) ) ) {
//int BLAH = 0;
//Timers.SetTimer( "BLHA2_"+tileX+"_"+tileY, 3, false, () => {
//	Dust.QuickDust( new Point(tileX, tileY), Color.Green );
//	return BLAH++ < 100;
//} );
				return false;
			}

			bool isPlaced = placer( tileX, tileY );
			if( isPlaced ) {
				occupiedTiles.Set2D( tileX, tileY );
			}
//else {
//int BLAH = 0;
//Timers.SetTimer( "BLHA3_"+tileX+"_"+tileY, 3, false, () => {
//	Dust.QuickDust( new Point(tileX, tileY), Color.Purple );
//	return BLAH++ < 100;
//} );
//}
			return isPlaced;
		}


		public static void ChangeFlooring( ushort tileType, int leftX, int rightX, int floorY ) {
			for( int i=leftX-1; i<rightX+2; i++ ) {
				Main.tile[ i, floorY + 1 ].type = tileType;
				WorldGen.SquareTileFrame( i, floorY + 1 );
			}
		}
	}
}
