using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using HamstarHelpers.Helpers.Debug;


namespace PrefabKits.Items {
	public partial class TrackDeploymentKitItem : ModItem {
		private static void SmoothPath( IList<(int tileX, int tileY)> path ) {
			int prevHighNodeIdx = -1;

			for( int i=0; i<path.Count; i++ ) {
				if( prevHighNodeIdx == -1 ) {
					prevHighNodeIdx = i;
					continue;
				}

				(int tileX, int tileY) prevNode = path[prevHighNodeIdx];
				(int tileX, int tileY) currNode = path[i];
				if( prevNode.tileY >= currNode.tileY ) {
					continue;
				}

				int? nextHighIdx = TrackDeploymentKitItem.SmoothPathTestAhead( path, prevHighNodeIdx );

				prevHighNodeIdx = i;

				if( !nextHighIdx.HasValue ) {
					continue;
				}

				TrackDeploymentKitItem.SmoothPathBetween( path, prevHighNodeIdx, nextHighIdx.Value );

				i = nextHighIdx.Value + 1;
			}
		}


		private static int? SmoothPathTestAhead( IList<(int tileX, int tileY)> path, int fromNodeIdx ) {
			(int tileX, int tileY) prevNode = path[fromNodeIdx];

			for( int i=fromNodeIdx+1; i<path.Count; i++ ) {
				(int tileX, int tileY) currNode = path[i];
				(int tileX, int tileY) testNode = ( currNode.tileX, prevNode.tileY );

				if( !TrackDeploymentKitItem.TrackValid.Check(testNode.tileX, testNode.tileY) ) {
					return null;
				}

				if( currNode.tileY == prevNode.tileY ) {
					return i;
				}
			}

			return null;
		}


		private static void SmoothPathBetween( IList<(int tileX, int tileY)> path, int fromNodeIdx, int toNodeIdx ) {
			(int tileX, int tileY) fromNode = path[ fromNodeIdx ];

			for( int i=fromNodeIdx+1; i<path.Count; i++ ) {
				(int tileX, int tileY) currNode = path[i];

				currNode.tileY = fromNode.tileY;
				path[i] = currNode;
			}
		}
	}
}
