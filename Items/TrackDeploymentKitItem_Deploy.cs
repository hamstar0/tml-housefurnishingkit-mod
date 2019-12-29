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
			int tracksScanRange = tracks + ( tracks / 2 );

			int dir = isAimedRight ? 1 : -1;

			IList<(int, int)> path = TrackDeploymentKitItem.TracePath( tileX, tileY, dir, tracksScanRange );

			if( path.Count > 0 ) {
				TrackDeploymentKitItem.DeployRunner( path, tracks, 0, ( x, y ) => {
					if( Main.tile[x, y]?.active() == true ) { return; }

					var kitSingleton = ModContent.GetInstance<TrackDeploymentKitItem>();
					kitSingleton.ResumeDeploymentAt = (x, y, dir);
				} );
			}

			return Math.Max( tracks - path.Count, 0 );
		}

		public static int ForceRedeploy( bool isAimedRight, int tileX, int tileY ) {
			var kitSingleton = ModContent.GetInstance<TrackDeploymentKitItem>();
			kitSingleton.ResumeDeploymentAt = (0, 0, 0);

			return TrackDeploymentKitItem.Deploy( isAimedRight, tileX, tileY );
		}

		public static int Redeploy() {
			var kitSingleton = ModContent.GetInstance<TrackDeploymentKitItem>();
			(int x, int y, int dir) resume = kitSingleton.ResumeDeploymentAt;
/*int blah=120;
Timers.SetTimer( "blah_"+resume.x+"_"+resume.y, 3, false, () => {
	Dust.QuickDust( new Point(resume.x,resume.y), Color.Red );
	return blah-- > 0;
} );*/

			kitSingleton.ResumeDeploymentAt = (0, 0, 0);

			return TrackDeploymentKitItem.Deploy( resume.dir > 0, resume.x, resume.y );
		}


		////

		public static void DeployRunner( IList<(int x, int y)> path, int trackMax, int trackNum, Action<int, int> onLastTrack ) {
			int x = path[trackNum].x;
			int y = path[trackNum].y;
			/*int blah=120;
			Timers.SetTimer( "blah_"+x+"_"+y, 3, false, () => {
				Dust.QuickDust( new Point(x,y), Color.Red );
				return blah-- > 0;
			} );*/
			if( Main.tile[x, y]?.active() == true ) {
				if( Main.tile[x, y]?.type != TileID.MinecartTrack ) {
					TrackDeploymentKitItem.DropLeftovers( trackMax - trackNum, x, y );
					return;
				}
			}

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

			int lastTrackIdx = trackMax < path.Count
				? trackMax - 1
				: path.Count - 1;

			if( trackNum < ( lastTrackIdx - 1 ) ) {
				Timers.SetTimer( "PrefabKitsTrackDeploy_" + path.GetHashCode(), 7, false, () => {
					TrackDeploymentKitItem.DeployRunner( path, trackMax, trackNum + 1, onLastTrack );
					return false;
				} );
			} else {
				if( trackMax < path.Count ) {   // if path cuts this short, do not run event
					(int x, int y) lastPathNode = path[trackNum + 1];
					onLastTrack( lastPathNode.x, lastPathNode.y );
				}
			}
		}


		public static void CreateSupportPillar( int tileX, int tileY ) {
			bool findBottomY( out int lowest ) {
				for( lowest = tileY; lowest < tileY + 128; lowest++ ) {
					Tile tile = Main.tile[tileX, lowest];
					if( tile.wall != 0 ) { break; }

					if( tile?.active() != true ) { continue; }
					if( !Main.tileSolid[tile.type] ) { continue; }
					if( Main.tileSolidTop[tile.type] ) { continue; }

					break;
				}

				return lowest < tileY + 128;
			}

			int bottom;
			if( !findBottomY( out bottom ) ) {
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


		////////////////

		public static void DropLeftovers( int leftovers, int tileX, int tileY ) {
			int itemWho = -1;
			if( leftovers > 0 ) {
				itemWho = Item.NewItem( new Vector2((tileX<<4) + 8, (tileY<<4) + 8), ItemID.MinecartTrack, leftovers );
			}

			if( Main.netMode == 1 ) {
				if( itemWho != -1 ) {
					NetMessage.SendData( MessageID.SyncItem, -1, -1, null, itemWho, 1f );
				}
			}
		}
	}
}
