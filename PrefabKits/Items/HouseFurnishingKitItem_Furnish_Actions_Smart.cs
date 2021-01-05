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
			(bool success, int tileType, int x, int y) result = HouseFurnishingKitItem.MakeHouseTileNearScan(
				attempts: new HashSet<(int, int)>(),
				depth: 5,
				placer,
				tileX,
				tileY,
				houseTiles,
				furnishedTiles
			);
//if( result.success ) {
//LogHelpers.Log( "Placed "+result.tileType+" at "+result.x+", "+result.y+" (was "+tileX+", "+tileY+")" );
//}

			return result.success;
		}

		private static (bool success, int tileType, int x, int y) MakeHouseTileNearScan(
					ISet<(int x, int y)> attempts,
					int depth,
					Func<int, int, (bool, int)> placer,
					int tileX,
					int tileY,
					IList<(ushort TileX, ushort TileY)> houseTiles,
					IDictionary<int, ISet<int>> furnishedTiles ) {
			if( depth < 0 ) {
				return (false, -1, tileX, tileY);
			}
			if( attempts.Contains((tileX, tileY)) ) {
				return (false, -1, tileX, tileY);
			}
			if( !houseTiles.Contains(((ushort)tileX, (ushort)tileY)) ) {
				return (false, -1, tileX, tileY);
			}

			//

			(bool success, int tileType, int x, int y) result = default;

			//

			(bool success, int tileType, int x, int y) attemptPlacerAt( int x, int y ) {
				if( attempts.Contains( (x, y) ) ) {
					return (false, -1, x, y);
				}

				(bool s, int t) myresult = HouseFurnishingKitItem.RunPlacerAt( placer, x, y, houseTiles, furnishedTiles );
				attempts.Add( (x, y) );
/*if( myresult.t == 480 ) {
Timers.RunUntil( () => {
	Dust.QuickDust( new Point(x, y), myresult.s ? Color.Lime : Color.Red );
	return true;
}, 60, true );
}*/
				return (myresult.s, myresult.t, x, y);
			}

			(bool success, int tileType, int x, int y) attemptRescanAndPlaceAt( int x, int y ) {
				return HouseFurnishingKitItem.MakeHouseTileNearScan(
					attempts,
					depth-1,
					placer,
					x,
					y,
					houseTiles,
					furnishedTiles
				);
			}

			//

			result = attemptPlacerAt( tileX, tileY );
			if( result.success ) {
				return result;
			}
			result = attemptPlacerAt( tileX, tileY - 1 );
			if( result.success ) {
				return result;
			}
			result = attemptPlacerAt( tileX, tileY + 1 );
			if( result.success ) {
				return result;
			}
			result = attemptPlacerAt( tileX - 1, tileY );
			if( result.success ) {
				return result;
			}
			result = attemptPlacerAt( tileX + 1, tileY );
			if( result.success ) {
				return result;
			}

			HouseFurnishingKitItem.OutputPlacementError( tileX, tileY, result.tileType, "placer for" );

			result = attemptRescanAndPlaceAt( tileX - 1, tileY - 1 );
			if( result.success ) {
				return result;
			}
			result = attemptRescanAndPlaceAt( tileX + 1, tileY - 1 );
			if( result.success ) {
				return result;
			}
			result = attemptRescanAndPlaceAt( tileX - 1, tileY + 1 );
			if( result.success ) {
				return result;
			}
			result = attemptRescanAndPlaceAt( tileX + 1, tileY + 1 );

			return result;
		}


		////////////////

		public static (bool, int) RunPlacerAt(
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
				return (false, -1);
			}

			if( !houseTiles.Contains( ((ushort)tileX, (ushort)tileY)) ) {
//int BLAH = 0;
//Timers.SetTimer( "BLHA2_"+tileX+"_"+tileY, 3, false, () => {
//	Dust.QuickDust( new Point(tileX, tileY), Color.Green );
//	return BLAH++ < 100;
//} );
				return (false, -1);
			}

			(bool isPlaced, int tileType) result = placer( tileX, tileY );
			if( result.isPlaced ) {
				furnishedTiles.Set2D( tileX, tileY );
			}
//else {
//int BLAH = 0;
//Timers.SetTimer( "BLHA3_"+tileX+"_"+tileY, 3, false, () => {
//	Dust.QuickDust( new Point(tileX, tileY), Color.Purple );
//	return BLAH++ < 100;
//} );
//}
			return result;
		}
	}
}
