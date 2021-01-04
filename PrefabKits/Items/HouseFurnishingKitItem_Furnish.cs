using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using HamstarHelpers.Helpers.Debug;
using HamstarHelpers.Services.Timers;


namespace PrefabKits.Items {
	public partial class HouseFurnishingKitItem : ModItem {
		public static void FurnishHouse(
					Player player,
					IList<(ushort TileX, ushort TileY)> innerHouseSpace,
					IList<(ushort TileX, ushort TileY)> fullHouseSpace,
					int floorX,
					int floorY,
					Action onFinish ) {
			(int x, int y) innerTopLeft, innerTopRight;
			(int x, int y) outerTopLeft, outerTopRight;
			int floorLeft, floorRight;
			(int x, int y) farTopLeft, farTopRight;
			IDictionary<int, ISet<int>> occupiedTiles = new Dictionary<int, ISet<int>>();

			HouseFurnishingKitItem.FindHousePoints(
				innerHouseSpace: innerHouseSpace,
				outerHouseSpace: fullHouseSpace,
				floorX: floorX,
				floorY: floorY,
				outerTopLeft: out outerTopLeft,
				outerTopRight: out outerTopRight,
				innerTopLeft: out innerTopLeft,
				innerTopRight: out innerTopRight,
				floorLeft: out floorLeft,
				floorRight: out floorRight,
				farTopLeft: out farTopLeft,
				farTopRight: out farTopRight
			);

			HouseFurnishingKitItem.CleanHouse( fullHouseSpace );

			//

			(bool success, int _) checkPlacement( int x, int y, int width ) {
				for( int x2 = x; x2 < x+width; x2++ ) {
					for( int y2 = y; y2 >= y-3; y2-- ) {
						if( Main.tile[x2, y2].active() && Main.tileSolid[Main.tile[x2, y2].type] ) {
							return (false, -1);
						}
					}
				}
				return (true, -1);
			}
			
			(bool success, int tileType) placeBed( int x, int y ) {
				if( !checkPlacement( x, y, 4 ).success ) {
					return (false, TileID.Beds);
				}
				bool success = HouseFurnishingKitItem.MakeHouseTile( x, y, TileID.Beds, 0, 1, fullHouseSpace, occupiedTiles );
				return (success, TileID.Beds);
			}
			(bool success, int tileType) placeWorkbench( int x, int y ) {
				if( !checkPlacement(x, y, 2).success ) {
					return (false, TileID.WorkBenches);
				}
				return (
					HouseFurnishingKitItem.MakeHouseTile( x, y, TileID.WorkBenches, 0, 1, fullHouseSpace, occupiedTiles ),
					TileID.WorkBenches
				);
			}
			(bool success, int tileType) placeChair( int x, int y ) {
				if( !checkPlacement(x, y, 1).success ) {
					return (false, TileID.Chairs);
				}
				return (
					HouseFurnishingKitItem.MakeHouseTile( x, y, TileID.Chairs, 0, 1, fullHouseSpace, occupiedTiles ),
					TileID.Chairs
				);
			}
			(bool success, int tileType) placeTorch( int x, int y ) {
				return (WorldGen.PlaceTile( x, y, TileID.Torches ), TileID.Torches);
			}

			//

			var config = PrefabKitsConfig.Instance;

			Timers.SetTimer( "HouseKitsFurnishingDelay", 1, false, () => {
				HouseFurnishingKitItem.MakeHouseWalls( fullHouseSpace );
				HouseFurnishingKitItem.MakeHouseTileNear( placeBed,			floorLeft,		floorY, fullHouseSpace, occupiedTiles );
				HouseFurnishingKitItem.MakeHouseTileNear( placeWorkbench,	floorRight - 2,	floorY, fullHouseSpace, occupiedTiles );
				HouseFurnishingKitItem.MakeHouseTileNear( placeChair,		floorRight - 3,	floorY, fullHouseSpace, occupiedTiles );
				HouseFurnishingKitItem.MakeHouseCustomFurnishings(			floorLeft,		floorRight, floorY, fullHouseSpace, occupiedTiles );
				HouseFurnishingKitItem.MakeHouseTileNear( placeTorch,		innerTopLeft.x,		innerTopLeft.y, fullHouseSpace, occupiedTiles );
				HouseFurnishingKitItem.MakeHouseTileNear( placeTorch,		innerTopRight.x,		innerTopRight.y, fullHouseSpace, occupiedTiles );

				if( config.Get<ushort>( nameof(config.CustomFloorTile) ) > 0 ) {
					HouseFurnishingKitItem.ChangeFlooring(
						config.Get<ushort>( nameof(config.CustomFloorTile) ),
						floorLeft,
						floorRight,
						floorY
					);
				}
				onFinish();

				if( Main.netMode == 2 ) {
					int width = outerTopRight.x - outerTopLeft.x;
					int height = (floorY - outerTopLeft.y) + 2;

					Timers.SetTimer( "PrefabKitsFurnishingKitLeft", 30, false, () => {
						NetMessage.SendTileRange(
							whoAmi: -1,
							tileX: outerTopLeft.x,
							tileY: outerTopLeft.y,
							xSize: width / 2,
							ySize: height
						);
						return false;
					} );

					Timers.SetTimer( "PrefabKitsFurnishingKitRight", 45, false, () => {
						NetMessage.SendTileRange(
							whoAmi: -1,
							tileX: outerTopLeft.x + (width/2),
							tileY: outerTopLeft.y,
							xSize: (width - (width/2)) + 1,
							ySize: height
						);
						return false;
					} );
				}

				return false;
			} );
		}


		////////////////

		private static void FindHousePoints(
					IList<(ushort TileX, ushort TileY)> innerHouseSpace,
					IList<(ushort TileX, ushort TileY)> outerHouseSpace,
					int floorX,
					int floorY,
					out (int x, int y) innerTopLeft,
					out (int x, int y) innerTopRight,
					out (int x, int y) outerTopLeft,
					out (int x, int y) outerTopRight,
					out int floorLeft,
					out int floorRight,
					out (int x, int y) farTopLeft,
					out (int x, int y) farTopRight ) {
			innerTopLeft = (0, 0);
			innerTopRight = (0, 0);
			outerTopLeft = (0, 0);
			outerTopRight = (0, 0);
			floorLeft = 0;
			floorRight = 0;
			farTopLeft = ( Math.Max(0, floorX - 512), Math.Max(0, floorY - 512) );
			farTopRight = ( Math.Min(Main.maxTilesX-1, floorX + 512), Math.Max(0, floorY - 512) );

			foreach( (ushort tileX, ushort tileY) in outerHouseSpace ) {
				if( outerTopLeft.x == 0 ) {
					outerTopLeft.x = outerTopRight.x = tileX;
					outerTopLeft.y = outerTopRight.y = tileY;
				}

				(int x, int y) oldTopLeft = (outerTopLeft.x - farTopLeft.x, outerTopLeft.y - farTopLeft.y);
				(int x, int y) newTopLeft = (tileX - farTopLeft.x, tileY - farTopLeft.y);

				int oldTopLeftDistSqr = ( oldTopLeft.x * oldTopLeft.x ) + ( oldTopLeft.y * oldTopLeft.y );
				int newTopLeftDistSqr = ( newTopLeft.x * newTopLeft.x ) + ( newTopLeft.y * newTopLeft.y );
				if( newTopLeftDistSqr < oldTopLeftDistSqr ) {
					outerTopLeft.x = tileX;
					outerTopLeft.y = tileY;
				}

				(int x, int y) oldTopRight = (outerTopRight.x - farTopRight.x, outerTopRight.y - farTopRight.y);
				(int x, int y) newTopRight = (tileX - farTopRight.x, tileY - farTopRight.y);

				int oldTopRightDistSqr = ( oldTopRight.x * oldTopRight.x ) + ( oldTopRight.y * oldTopRight.y );
				int newTopRightDistSqr = ( newTopRight.x * newTopRight.x ) + ( newTopRight.y * newTopRight.y );
				if( newTopRightDistSqr < oldTopRightDistSqr ) {
					outerTopRight.x = tileX;
					outerTopRight.y = tileY;
				}
			}

			foreach( (ushort tileX, ushort tileY) in innerHouseSpace ) {
				Tile tile = Framing.GetTileSafely( tileX, tileY );

				if( innerTopLeft.x == 0 ) {
					innerTopLeft.x = innerTopRight.x = tileX;
					innerTopLeft.y = innerTopRight.y = tileY;
				}

				//if( tileY == (floorY-2) ) {
				if( tileY == (floorY-1) ) {
					if( floorLeft == 0 ) {
						floorLeft = floorRight = tileX;
					} else {
						if( floorLeft > tileX ) {
							floorLeft = tileX;
						} else if( floorRight < tileX ) {
							floorRight = tileX;
						}
					}
				}

				(int x, int y) oldTopLeft = (innerTopLeft.x - farTopLeft.x, innerTopLeft.y - farTopLeft.y);
				(int x, int y) newTopLeft = (tileX - farTopLeft.x, tileY - farTopLeft.y);

				int oldTopLeftDistSqr = ( oldTopLeft.x * oldTopLeft.x ) + ( oldTopLeft.y * oldTopLeft.y );
				int newTopLeftDistSqr = ( newTopLeft.x * newTopLeft.x ) + ( newTopLeft.y * newTopLeft.y );
				if( newTopLeftDistSqr < oldTopLeftDistSqr ) {
					innerTopLeft.x = tileX;
					innerTopLeft.y = tileY;
				}

				(int x, int y) oldTopRight = (innerTopRight.x - farTopRight.x, innerTopRight.y - farTopRight.y);
				(int x, int y) newTopRight = (tileX - farTopRight.x, tileY - farTopRight.y);

				int oldTopRightDistSqr = ( oldTopRight.x * oldTopRight.x ) + ( oldTopRight.y * oldTopRight.y );
				int newTopRightDistSqr = ( newTopRight.x * newTopRight.x ) + ( newTopRight.y * newTopRight.y );
				if( newTopRightDistSqr < oldTopRightDistSqr ) {
					innerTopRight.x = tileX;
					innerTopRight.y = tileY;
				}
			}
		}
	}
}
