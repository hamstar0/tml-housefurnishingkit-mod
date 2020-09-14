using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using HamstarHelpers.Helpers.Debug;
using HamstarHelpers.Services.Timers;
using HamstarHelpers.Helpers.Fx;
using PrefabKits.Protocols;


namespace PrefabKits.Items {
	public partial class TrackDeploymentKitItem : ModItem {
		public static void PlaceTrack( int x, int y ) {
			WorldGen.PlaceTile( x, y, TileID.MinecartTrack );
			WorldGen.SquareTileFrame( x, y );

			var pos = new Vector2( x << 4, y << 4 );

			Main.PlaySound( SoundID.Item52, pos );
			ParticleFxHelpers.MakeDustCloud( pos, 1 );
		}

		public static bool PlaceResumePoint( int x, int y, bool dir ) {
			if( Main.tile[x, y]?.active() == true ) {
				return false;
			}

			var kitSingleton = ModContent.GetInstance<TrackDeploymentKitItem>();
			kitSingleton.ResumeDeploymentAt = (x, y, dir ? 1 : -1);
			return true;
		}


		////////////////

		public static int Deploy( int fromPlayerWho, int tileX, int tileY, bool isAimedRight ) {
			int tracks = PrefabKitsConfig.Instance.Get<int>( nameof(PrefabKitsConfig.TrackDeploymentKitTracks) );
			int tracksScanRange = tracks + ( tracks / 2 );

			int dir = isAimedRight ? 1 : -1;

			IList<(int, int)> path = TrackDeploymentKitItem.TracePath( tileX, tileY, dir, tracksScanRange );

			if( path.Count > 0 ) {
				TrackDeploymentKitItem.DeployRunner( fromPlayerWho, path, isAimedRight, tracks, 0 );
			}

			return Math.Max( tracks - path.Count, 0 );
		}

		public static int Redeploy( int fromPlayerWho ) {
			var kitSingleton = ModContent.GetInstance<TrackDeploymentKitItem>();
			(int x, int y, int dir) resume = kitSingleton.ResumeDeploymentAt;

			kitSingleton.ResumeDeploymentAt = (0, 0, 0);

			if( Main.tile[resume.x, resume.y]?.active() == true ) {
				Main.NewText( "Track kit auto-deploy obstructed.", Color.Yellow );
				return 0;
			}
/*int blah=120;
Timers.SetTimer( "blah_"+resume.x+"_"+resume.y, 3, false, () => {
	Dust.QuickDust( new Point(resume.x,resume.y), Color.Red );
	return blah-- > 0;
} );*/

			if( Main.netMode == 0 ) {
				return TrackDeploymentKitItem.Deploy( fromPlayerWho, resume.x, resume.y, resume.dir > 0 );
			} else {
				TrackKitDeployProtocol.SendToServer( resume.dir > 0, resume.x, resume.y, true );
				return 0;
			}
		}


		////

		public static void DeployRunner(
					int fromPlayerWho,
					IList<(int x, int y)> path,
					bool isAimedRight,
					int trackMax,
					int trackNum ) {
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

			int lastTrackIdx = trackMax < path.Count
				? trackMax - 1
				: path.Count - 1;
			bool isLastTrack = trackNum >= ( lastTrackIdx - 1 );
			bool isNotAborted = trackMax < path.Count;	// if path cuts this short, do not run event

			if( Main.tile[x, y]?.type != TileID.MinecartTrack ) {
				TrackDeploymentKitItem.PlaceTrack( x, y );

				if( Main.netMode == 2 ) {
					//NetMessage.SendTileSquare( -1, x, y, 1 );
					TrackKitTileProtocol.SendToClients( x, y );
				}

				if( isLastTrack && isNotAborted ) {
					(int x, int y) lastPathNode = path[trackNum + 1];

					if( Main.netMode == 0 ) {
						TrackDeploymentKitItem.PlaceResumePoint( lastPathNode.x, lastPathNode.y, isAimedRight );
					} else if( Main.netMode == 2 ) {
						TrackKitResumeProtocol.SendToClient( fromPlayerWho, lastPathNode.x, lastPathNode.y, isAimedRight );
					}
				}

				if( trackNum % 8 == 0 ) {
					TrackDeploymentKitItem.CreateSupportPillar( x, y );
				}
			}

			if( !isLastTrack ) {
				Timers.SetTimer( "PrefabKitsTrackDeploy_" + path.GetHashCode(), 7, false, () => {
					TrackDeploymentKitItem.DeployRunner( fromPlayerWho, path, isAimedRight, trackMax, trackNum + 1 );
					return false;
				} );
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

				if( Main.netMode != 0 ) {
					NetMessage.SendTileSquare( -1, tileX, y, 1 );
				}
			}
		}


		////////////////

		public static void DropLeftovers( int leftovers, int tileX, int tileY ) {
			int itemWho = -1;
			if( leftovers > 0 ) {
				itemWho = Item.NewItem( new Vector2((tileX<<4) + 8, (tileY<<4) + 8), ItemID.MinecartTrack, leftovers );
			}

			if( Main.netMode != 0 ) {
				if( itemWho != -1 ) {
					NetMessage.SendData( MessageID.SyncItem, -1, -1, null, itemWho, 1f );
				}
			}
		}
	}
}
