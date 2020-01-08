using System;
using Terraria;
using HamstarHelpers.Classes.Errors;
using HamstarHelpers.Classes.Protocols.Packet.Interfaces;
using HamstarHelpers.Helpers.Debug;
using PrefabKits.Items;


namespace PrefabKits.Protocols {
	class TrackDeploymentProtocol : PacketProtocolBroadcast {
		public static void BroadcastFromClient( bool isAimedRight, int tileX, int tileY, bool isRedeploy ) {
			if( Main.netMode != 1 ) { throw new ModHelpersException( "Not client" ); }

			var protocol = new TrackDeploymentProtocol( isAimedRight, tileX, tileY, isRedeploy );

			protocol.SendToServer( true );
		}



		////////////////

		public bool IsAimedRight;
		public int TileX;
		public int TileY;
		public bool IsRedeploy;



		////////////////

		private TrackDeploymentProtocol() { }

		private TrackDeploymentProtocol( bool isAimedRight, int tileX, int tileY, bool isRedeploy ) {
			this.IsAimedRight = isAimedRight;
			this.TileX = tileX;
			this.TileY = tileY;
			this.IsRedeploy = isRedeploy;
		}


		////////////////

		protected override void ReceiveOnServer( int fromWho ) {
			Main.tile[this.TileX, this.TileY].ClearTile();

			if( this.IsRedeploy ) {
				TrackDeploymentKitItem.ForceRedeploy( this.IsAimedRight, this.TileX, this.TileY );
			} else {
				TrackDeploymentKitItem.Deploy( this.IsAimedRight, this.TileX, this.TileY );
			}
		}

		protected override void ReceiveOnClient() {
			Main.tile[this.TileX, this.TileY].ClearTile();

			if( this.IsRedeploy ) {
				TrackDeploymentKitItem.ForceRedeploy( this.IsAimedRight, this.TileX, this.TileY );
			} else {
				TrackDeploymentKitItem.Deploy( this.IsAimedRight, this.TileX, this.TileY );
			}
		}
	}
}
