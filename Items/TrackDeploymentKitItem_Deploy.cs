using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using HamstarHelpers.Helpers.Debug;


namespace PrefabKits.Items {
	public partial class TrackDeploymentKitItem : ModItem {
		public static int Deploy( bool isAimedRight, int tileX, int tileY ) {
			int tracks = PrefabKitsConfig.Instance.TrackDeploymentKitTracks;
			int tracksScount = tracks + (tracks / 2);

			int dir = isAimedRight ? 1 : -1;

			IList<(int, int)> path = TrackDeploymentKitItem.TracePath( tileX, tileY, dir, tracksScount );

			int i = 0;
			foreach( (int x, int y) in path ) {
/*int blah=120;
Timers.SetTimer( "blah_"+x+"_"+y, 3, false, () => {
	Dust.QuickDust( new Point(x,y), Color.Red );
	return blah-- > 0;
} );*/
				if( Main.tile[x, y]?.type != TileID.MinecartTrack ) {
					WorldGen.PlaceTile( x, y, TileID.MinecartTrack );
					WorldGen.SquareTileFrame( x, y );
				}

				if( i++ >= tracks ) {
					break;
				}
			}

			return tracks - path.Count;
		}


		private static IList<(int, int)> TracePath( int tileX, int tileY, int dir, int tracks ) {
			if( Main.tile[tileX, tileY]?.active() == true ) {
				return new List<(int, int)>();
			}

			IList<(int, int)> path = new List<(int, int)> { (tileX, tileY) };
			IDictionary<(int, int), PathTree> pathMap = new Dictionary<(int, int), PathTree>();

			PathTree pathTree = TrackDeploymentKitItem.TracePathTree( tileX, tileY, dir, 0, tracks, pathMap );

			TrackDeploymentKitItem.TraceTreeForLongestPath( pathTree, path );
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

				if( Main.tile[x, y]?.active() != true || Main.tile[x, y].type == TileID.MinecartTrack ) {
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




		public class PathTree {
			public int TileX;
			public int TileY;
			public int HighestDepthCount;

			public PathTree Top;
			public PathTree Mid;
			public PathTree Bot;


			public int Count() {
				int count = this.HighestDepthCount > 0 ? 1 : 0;
				count += this.Top?.Count() ?? 0;
				count += this.Mid?.Count() ?? 0;
				count += this.Bot?.Count() ?? 0;

				return count;
			}
		}
	}
}
