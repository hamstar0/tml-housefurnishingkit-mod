using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using HamstarHelpers.Classes.Tiles.TilePattern;
using HamstarHelpers.Helpers.Debug;


namespace PrefabKits.Items {
	public partial class TrackDeploymentKitItem : ModItem {
		private static TilePattern TrackValid = new TilePattern( new TilePatternBuilder {
			AreaFromCenter = new Rectangle( 0, -3, 1, 5 ),
			HasLava = false,
			CustomCheck = (x, y) => {
				return Main.tile[x, y]?.active() != true || Main.tile[x, y].type == TileID.MinecartTrack;
			}
		} );



		////////////////

		private static IList<(int, int)> TracePath(
					int tileX,
					int tileY,
					int dir,
					int tracks ) {
			if( Main.tile[tileX, tileY]?.active() == true ) {
				return new List<(int, int)>();
			}

			IDictionary<(int, int), PathTree> pathMap = new Dictionary<(int, int), PathTree>();
			PathTree pathTree = TrackDeploymentKitItem.TracePathTree( tileX, tileY, dir, 0, tracks, pathMap );

			IList<(int, int)> path = new List<(int, int)> { (tileX, tileY) };
			TrackDeploymentKitItem.TraceTreeForLongestPath( pathTree, path );

			TrackDeploymentKitItem.SmoothPath( path );

			return path;
		}


		private static PathTree TracePathTree(
					int tileX,
					int tileY,
					int horizontalDir,
					int verticalDir,
					int tracks,
					IDictionary<(int, int), PathTree> pathMap ) {
			var tree = new PathTree {
				TileX = tileX,
				TileY = tileY,
				HighestDepthCount = 1
			};

			if( tracks <= 0 ) {
				tree.HighestDepthCount = 0;
				return tree;
			}

			//

			PathTree createTree( int x, int y, int oldVerticalDir, int newVerticalDir ) {
				var mytree = new PathTree {
					TileX = x,
					TileY = y,
					HighestDepthCount = 0
				};

				if( (oldVerticalDir == 1 && newVerticalDir == -1) || (oldVerticalDir == -1 && newVerticalDir == 1) ) {
					return mytree;
				}

				if( pathMap.ContainsKey( (x, y) ) ) {
					return pathMap[ (x, y) ];
				}

				if( TrackDeploymentKitItem.TrackValid.Check(x, y) ) {
					mytree = TrackDeploymentKitItem.TracePathTree(
						x,
						y,
						horizontalDir,
						newVerticalDir,
						tracks - 1,
						pathMap
					);
				}

				pathMap[ (x, y) ] = mytree;

				return mytree;
			}

			//

			tree.Bot = createTree( tileX + horizontalDir,	tileY + 1,	verticalDir,	1 );
			tree.Mid = createTree( tileX + horizontalDir,	tileY,		verticalDir,	0 );
			tree.Top = createTree( tileX + horizontalDir,	tileY - 1,	verticalDir,	-1 );
			
			if( tree.Bot.HighestDepthCount >= tree.Top.HighestDepthCount ) {
				if( tree.Bot.HighestDepthCount >= tree.Mid.HighestDepthCount ) {
					tree.HighestDepthCount += tree.Bot.HighestDepthCount;
				} else {
					tree.HighestDepthCount += tree.Mid.HighestDepthCount;
				}
			} else if( tree.Top.HighestDepthCount >= tree.Mid.HighestDepthCount ) {
				tree.HighestDepthCount += tree.Top.HighestDepthCount;
			} else {
				tree.HighestDepthCount += tree.Mid.HighestDepthCount;
			}

			return tree;
		}


		private static void TraceTreeForLongestPath( PathTree pathTree, IList<(int, int)> path ) {
			if( pathTree.Bot == null ) {	//|| pathTree.Mid == null || pathTree.Top == null ) {
				return;
			}
			
			if( pathTree.Bot.HighestDepthCount >= pathTree.Mid.HighestDepthCount ) {
				if( pathTree.Bot.HighestDepthCount >= pathTree.Top.HighestDepthCount ) {
					if( pathTree.Bot.HighestDepthCount > 0 ) {
						path.Add( (pathTree.Bot.TileX, pathTree.Bot.TileY) );
						TrackDeploymentKitItem.TraceTreeForLongestPath( pathTree.Bot, path );
					}
				} else {
					path.Add( (pathTree.Top.TileX, pathTree.Top.TileY) );
					TrackDeploymentKitItem.TraceTreeForLongestPath( pathTree.Top, path );
				}
			} else if( pathTree.Mid.HighestDepthCount >= pathTree.Top.HighestDepthCount ) {
				path.Add( (pathTree.Mid.TileX, pathTree.Mid.TileY) );
				TrackDeploymentKitItem.TraceTreeForLongestPath( pathTree.Mid, path );
			} else {
				path.Add( (pathTree.Top.TileX, pathTree.Top.TileY) );
				TrackDeploymentKitItem.TraceTreeForLongestPath( pathTree.Top, path );
			}
		}
	}
}
