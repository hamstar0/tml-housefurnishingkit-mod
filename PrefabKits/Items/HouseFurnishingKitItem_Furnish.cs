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
	}
}
