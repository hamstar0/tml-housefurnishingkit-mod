using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using HamstarHelpers.Helpers.Debug;
using HamstarHelpers.Services.Timers;


namespace PrefabKits.Items {
	public partial class HouseFurnishingKitItem : ModItem {
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
