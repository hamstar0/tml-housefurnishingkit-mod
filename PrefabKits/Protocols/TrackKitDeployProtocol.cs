using System;
using Terraria;
using HamstarHelpers.Classes.Errors;
using HamstarHelpers.Classes.Protocols.Packet.Interfaces;
using HamstarHelpers.Helpers.Debug;
using PrefabKits.Tiles;


namespace PrefabKits.Protocols {
	class TrackKitDeployProtocol : PacketProtocolSendToServer {
		public static void SendToServer( bool isAimedRight, int tileX, int tileY, bool isRedeploy ) {
			if( Main.netMode != 1 ) { throw new ModHelpersException( "Not client" ); }

			var protocol = new TrackKitDeployProtocol( isAimedRight, tileX, tileY, isRedeploy );

			protocol.SendToServer( true );
		}



		////////////////

		public bool IsAimedRight;
		public int TileX;
		public int TileY;
		public bool IsRedeploy;



		////////////////

		private TrackKitDeployProtocol() { }

		private TrackKitDeployProtocol( bool isAimedRight, int tileX, int tileY, bool isRedeploy ) {
			this.IsAimedRight = isAimedRight;
			this.TileX = tileX;
			this.TileY = tileY;
			this.IsRedeploy = isRedeploy;
		}

		protected override void InitializeClientSendData() { }


		////////////////

		protected override void Receive( int fromWho ) {
			TrackDeploymentTile.DeployAt( this.TileX, this.TileY, this.IsAimedRight, fromWho );
		}
	}
}
