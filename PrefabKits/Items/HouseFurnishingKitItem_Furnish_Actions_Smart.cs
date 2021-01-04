using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using HamstarHelpers.Classes.Errors;
using HamstarHelpers.Helpers.Debug;
using HamstarHelpers.Helpers.DotNET.Extensions;


namespace PrefabKits.Items {
	public partial class HouseFurnishingKitItem : ModItem {
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


		////////////////

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
	}
}
