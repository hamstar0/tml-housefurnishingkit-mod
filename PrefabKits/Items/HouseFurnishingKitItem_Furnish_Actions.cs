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

		////

		private static void MakeHouseCustomFurnishings(
					int leftTileX,
					int rightTileX,
					int floorTileY,
					IList<(ushort TileX, ushort TileY)> houseTiles,
					IDictionary<int, ISet<int>> furnishedTiles ) {
			var config = PrefabKitsConfig.Instance;

			ushort custFurnType = config.Get<ushort>( nameof(config.CustomFurnitureTile) );
			if( custFurnType > 0 ) {
				HouseFurnishingKitItem.MakeHouseCustomMainFurniture(
					custFurnType,
					leftTileX,
					floorTileY,
					houseTiles,
					furnishedTiles
				);
			}

			ushort custWallMount1 = config.Get<ushort>( nameof(config.CustomWallMount1Tile) );
			if( custWallMount1 != 0 ) {
				HouseFurnishingKitItem.MakeHouseTileNear(
					( x, y ) => {
						if( y >= floorTileY - 4 ) {
							return (false, custWallMount1);
						}
						return (
							HouseFurnishingKitItem.MakeHouseWallTile3x3( x, y, custWallMount1, furnishedTiles ),
							custWallMount1
						);
					},
					leftTileX,
					floorTileY - 3,
					houseTiles,
					furnishedTiles
				);
			}

			ushort custWallMount2 = config.Get<ushort>( nameof(config.CustomWallMount2Tile) );
			if( custWallMount2 != 0 ) {
				HouseFurnishingKitItem.MakeHouseTileNear(
					( x, y ) => {
						if( y >= floorTileY - 4 ) {
							return (false, custWallMount2);
						}
						return (
							HouseFurnishingKitItem.MakeHouseWallTile3x3( x, y, custWallMount2, furnishedTiles ),
							custWallMount2
						);
					},
					rightTileX - 3,
					floorTileY - 4,
					houseTiles,
					furnishedTiles
				);
			}
		}


		private static void MakeHouseCustomMainFurniture(
					ushort tileType,
					int leftTileX,
					int floorTileY,
					IList<(ushort TileX, ushort TileY)> houseTiles,
					IDictionary<int, ISet<int>> occupiedTiles ) {
			(bool, int) placeTile( int x, int y ) {
				switch( tileType ) {
				case TileID.Bottles:
					TilePlacementHelpers.Place2x1( x, y, TileID.WorkBenches );
					if( WorldGen.PlaceTile( x, y - 1, TileID.Bottles ) ) {
						HouseFurnishingKitItem.MarkFurnishedTiles( x, y - 1, 2, 2, occupiedTiles );
						return (true, tileType);
					}
					break;
				case TileID.PiggyBank:
					TilePlacementHelpers.Place2x1( x, y, TileID.WorkBenches );
					TilePlacementHelpers.Place2x1( x, y - 1, TileID.PiggyBank );
					if( Main.tile[x, y].type == TileID.WorkBenches ) {
						HouseFurnishingKitItem.MarkFurnishedTiles( x, y - 1, 2, 2, occupiedTiles );
						return (true, tileType);
					}
					break;
				default:
					return (
						HouseFurnishingKitItem.MakeHouseTile( x, y, tileType, 0, -1, houseTiles, occupiedTiles ),
						tileType
					);
				}

				HouseFurnishingKitItem.OutputPlacementError( x, y, tileType, "custom main furniture" );

				return (false, tileType);
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
					Func<int, int, (bool, int)> placer,
					int tileX,
					int tileY,
					IList<(ushort TileX, ushort TileY)> houseTiles,
					IDictionary<int, ISet<int>> furnishedTiles ) {
			var scan = new HashSet<(int, int)>();

			return HouseFurnishingKitItem.MakeHouseTileNearScan( placer, tileX, tileY, houseTiles, furnishedTiles, scan );
		}

		private static bool MakeHouseTileNearScan(
					Func<int, int, (bool, int)> placer,
					int tileX,
					int tileY,
					IList<(ushort TileX, ushort TileY)> houseTiles,
					IDictionary<int, ISet<int>> furnishedTiles,
					ISet<(int, int)> scan ) {
			if( HouseFurnishingKitItem.RunPlacerAt(placer, tileX, tileY, houseTiles, furnishedTiles) ) {
				return true;
			}
			if( HouseFurnishingKitItem.RunPlacerAt(placer, tileX, tileY - 1, houseTiles, furnishedTiles) ) {
				return true;
			}
			if( HouseFurnishingKitItem.RunPlacerAt(placer, tileX, tileY + 1, houseTiles, furnishedTiles) ) {
				return true;
			}
			if( HouseFurnishingKitItem.RunPlacerAt(placer, tileX - 1, tileY, houseTiles, furnishedTiles) ) {
				return true;
			}
			if( HouseFurnishingKitItem.RunPlacerAt(placer, tileX + 1, tileY, houseTiles, furnishedTiles) ) {
				return true;
			}

			//

			if( !scan.Contains((tileX, tileY-1)) ) {
				scan.Add( (tileX, tileY - 1) );
				if( !furnishedTiles.Contains2D(tileX, tileY-1) && houseTiles.Contains( ((ushort)tileX, (ushort)(tileY - 1)) ) ) {
					if( HouseFurnishingKitItem.MakeHouseTileNearScan( placer, tileX, tileY - 1, houseTiles, furnishedTiles, scan) ) {
						return true;
					}
				}
			}
			if( !scan.Contains((tileX, tileY+1)) ) {
				scan.Add( (tileX, tileY + 1) );
				if( !furnishedTiles.Contains2D(tileX, tileY+1) && houseTiles.Contains( ((ushort)tileX, (ushort)(tileY + 1)) ) ) {
					if( HouseFurnishingKitItem.MakeHouseTileNearScan( placer, tileX, tileY + 1, houseTiles, furnishedTiles, scan) ) {
						return true;
					}
				}
			}
			if( !scan.Contains((tileX-1, tileY)) ) {
				scan.Add( (tileX - 1, tileY) );
				if( !furnishedTiles.Contains2D(tileX-1, tileY) && houseTiles.Contains( ((ushort)(tileX - 1), (ushort)tileY) ) ) {
					if( HouseFurnishingKitItem.MakeHouseTileNearScan( placer, tileX - 1, tileY, houseTiles, furnishedTiles, scan) ) {
						return true;
					}
				}
			}
			if( !scan.Contains((tileX+1, tileY)) ) {
				scan.Add( (tileX + 1, tileY) );
				if( !furnishedTiles.Contains2D(tileX+1, tileY) && houseTiles.Contains( ((ushort)(tileX + 1), (ushort)tileY) ) ) {
					if( HouseFurnishingKitItem.MakeHouseTileNearScan( placer, tileX + 1, tileY, houseTiles, furnishedTiles, scan) ) {
						return true;
					}
				}
			}

			//

			return false;
		}

		public static bool RunPlacerAt(
					Func<int, int, (bool, int)> placer,
					int tileX,
					int tileY,
					IList<(ushort TileX, ushort TileY)> houseTiles,
					IDictionary<int, ISet<int>> furnishedTiles ) {
			if( furnishedTiles.Contains2D(tileX, tileY) ) {
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

			(bool isPlaced, int tileType) result = placer( tileX, tileY );
			if( result.isPlaced ) {
				furnishedTiles.Set2D( tileX, tileY );
			} else {
				HouseFurnishingKitItem.OutputPlacementError( tileX, tileY, result.tileType, "placer" );
			}
//else {
//int BLAH = 0;
//Timers.SetTimer( "BLHA3_"+tileX+"_"+tileY, 3, false, () => {
//	Dust.QuickDust( new Point(tileX, tileY), Color.Purple );
//	return BLAH++ < 100;
//} );
//}
			return result.isPlaced;
		}


		public static void ChangeFlooring( ushort tileType, int leftX, int rightX, int floorY ) {
			for( int i=leftX-1; i<rightX+2; i++ ) {
				Tile tile = Main.tile[i, floorY + 1];
				tile.ClearEverything();

				tile.active( true );
				tile.type = tileType;

				WorldGen.SquareTileFrame( i, floorY + 1 );
			}
		}


		////////////////

		private static void OutputPlacementError( int tileX, int tileY, int tileType, string context ) {
			if( PrefabKitsConfig.Instance.DebugModeSuppressPlacementErrors ) {
				return;
			}

			LogHelpers.Log( "Could not place "+context+" "
				+ ( tileType >= Main.tileTexture.Length
					? tileType.ToString()
					: TileID.Search.GetName(tileType) )
				+ " at "+tileX+", "+tileY
			);

			LogHelpers.Log( "  "+(tileX-1)+", "+(tileY-1)+" - "+Main.tile[tileX-1, tileY-1].ToString() );
			LogHelpers.Log( "  "+(tileX)+", "+(tileY-1)+" - "+Main.tile[tileX, tileY-1].ToString() );
			LogHelpers.Log( "  "+(tileX+1)+", "+(tileY-1)+" - "+Main.tile[tileX+1, tileY-1].ToString() );
			LogHelpers.Log( "  "+(tileX-1)+", "+(tileY)+" - "+Main.tile[tileX-1, tileY].ToString() );
			LogHelpers.Log( "  "+(tileX)+", "+(tileY)+" - "+Main.tile[tileX, tileY].ToString() );
			LogHelpers.Log( "  "+(tileX+1)+", "+(tileY)+" - "+Main.tile[tileX+1, tileY].ToString() );
			LogHelpers.Log( "  "+(tileX-1)+", "+(tileY+1)+" - "+Main.tile[tileX-1, tileY+1].ToString() );
			LogHelpers.Log( "  "+(tileX)+", "+(tileY+1)+" - "+Main.tile[tileX, tileY+1].ToString() );
			LogHelpers.Log( "  "+(tileX+1)+", "+(tileY+1)+" - "+Main.tile[tileX+1, tileY+1].ToString() );
		}
	}
}
