using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using HamstarHelpers.Helpers.Debug;


namespace PrefabKits.Items {
	public partial class TrackDeploymentKitItem : ModItem {
		private static void SmoothPath( IList<(int tileX, int tileY)> path ) {
			int prevHighNodeIdx = 0;

			for( int i=1; i<path.Count; i++ ) {
				if( path[prevHighNodeIdx].tileY < path[i].tileY ) {
					int? nextHighIdx = TrackDeploymentKitItem.SmoothPathTestAhead( path, prevHighNodeIdx );

					if( nextHighIdx.HasValue ) {
						TrackDeploymentKitItem.SmoothPathBetween( path, prevHighNodeIdx, nextHighIdx.Value );
						i = nextHighIdx.Value;
					}
				}

				prevHighNodeIdx = i;
			}
		}


		private static int? SmoothPathTestAhead( IList<(int tileX, int tileY)> path, int fromNodeIdx ) {
			(int tileX, int tileY) prevNode = path[fromNodeIdx];

			for( int i=fromNodeIdx+1; i<path.Count; i++ ) {
				(int tileX, int tileY) currNode = path[i];
				(int tileX, int tileY) testNode = ( currNode.tileX, prevNode.tileY );

				// Found a matching height node
				if( currNode.tileY == prevNode.tileY ) {
//int t = 60*10;
//Timers.RunUntil( () => {
//	Dust.QuickDust(new Point(testNode.tileX, testNode.tileY), Color.Lime);
//	return t-- > 0;
//}, true );
					return i;
				}

				// Not a clear path
				if( !TrackDeploymentKitItem.TrackValid.Check(testNode.tileX, testNode.tileY) ) {
//int t = 60 * 10;
//Timers.RunUntil( () => {
//	Dust.QuickDust(new Point(testNode.tileX, testNode.tileY), Color.Red);
//	return t-- > 0;
//}, true );
					return null;
				}
			}

			return null;
		}


		private static void SmoothPathBetween( IList<(int tileX, int tileY)> path, int fromNodeIdx, int toNodeIdx ) {
			(int tileX, int tileY) fromNode = path[ fromNodeIdx ];

			for( int i=fromNodeIdx+1; i<toNodeIdx; i++ ) {
				(int tileX, int tileY) currNode = path[i];

				currNode.tileY = fromNode.tileY;

				path[i] = currNode;
			}
		}
	}
}
