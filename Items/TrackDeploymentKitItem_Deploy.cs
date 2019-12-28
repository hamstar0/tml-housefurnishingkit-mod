using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using HamstarHelpers.Helpers.Debug;
using HamstarHelpers.Services.Timers;
using HamstarHelpers.Helpers.Fx;


namespace PrefabKits.Items {
	public partial class TrackDeploymentKitItem : ModItem {
		public static int Deploy( bool isAimedRight, int tileX, int tileY ) {
			int tracks = PrefabKitsConfig.Instance.TrackDeploymentKitTracks;
			int tracksScount = tracks + (tracks / 2);

			int dir = isAimedRight ? 1 : -1;

			IList<(int, int)> path = TrackDeploymentKitItem.TracePath( tileX, tileY, dir, tracksScount );

			TrackDeploymentKitItem.DeployRunner( path, tracks );

			return tracks - path.Count;
		}


		public static void DeployRunner( IList<(int x, int y)> path, int trackMax, int trackNum=0 ) {
			int x = path[trackNum].x;
			int y = path[trackNum].y;
/*int blah=120;
Timers.SetTimer( "blah_"+x+"_"+y, 3, false, () => {
	Dust.QuickDust( new Point(x,y), Color.Red );
	return blah-- > 0;
} );*/
			if( Main.tile[x, y]?.type != TileID.MinecartTrack ) {
				WorldGen.PlaceTile( x, y, TileID.MinecartTrack );
				WorldGen.SquareTileFrame( x, y );

				var pos = new Vector2( x << 4, y << 4 );

				Main.PlaySound( SoundID.Item52, pos );
				ParticleFxHelpers.MakeDustCloud( pos, 1 );

				if( trackNum % 8 == 0 ) {
					TrackDeploymentKitItem.CreateSupportPillar( x, y );
				}
			}

			if( trackNum < trackMax - 1 && trackNum < path.Count - 1 ) {
				Timers.SetTimer( "PrefabKitsTrackDeploy_"+path.GetHashCode(), 7, false, () => {
					TrackDeploymentKitItem.DeployRunner( path, trackMax, trackNum + 1 );
					return false;
				} );
			}
		}


		public static void CreateSupportPillar( int tileX, int tileY ) {
			bool findBottomY( out int lowest ) {
				for( lowest = tileY; lowest < tileY + 128; lowest++ ) {
					Tile tile = Main.tile[tileX, lowest];
					if( tile?.active() != true ) { continue; }
					if( tile.type == TileID.MinecartTrack ) { continue; }
					if( !Main.tileSolid[tile.type] ) { continue; }
					if( tile.wall == 0 ) { continue; }

					break;
				}

				return lowest < tileY + 128;
			}

			int bottom;
			if( !findBottomY(out bottom) ) {
				return;
			}

			//if( Main.tile[tileX, bottom]?.active() == true ) {
			//	Main.tile[tileX, bottom].slope( 0 );
			//	Main.tile[tileX, bottom].halfBrick( false );
			//}

			for( int y = bottom; y >= tileY; y-- ) {
				if( Main.tile[tileX, y]?.wall == 0 ) {
					WorldGen.PlaceWall( tileX, y, WallID.RichMahoganyFence );
				}
			}
		}
	}
}
