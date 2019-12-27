using System;
using Terraria;
using HamstarHelpers.Classes.Errors;
using HamstarHelpers.Classes.Protocols.Packet.Interfaces;
using PrefabKits.Items;


namespace PrefabKits.Protocols {
	class TrackDeploymentProtocol : PacketProtocolBroadcast {
		public static void BroadcastFromClient( bool isAimedRight, int tileX, int tileY ) {
			if( Main.netMode != 1 ) { throw new ModHelpersException( "Not client" ); }

			var protocol = new TrackDeploymentProtocol( isAimedRight, tileX, tileY );

			protocol.SendToServer( true );
		}



		////////////////

		public bool IsAimedRight;
		public int TileX;
		public int TileY;



		////////////////

		private TrackDeploymentProtocol() { }

		private TrackDeploymentProtocol( bool isAimedRight, int tileX, int tileY ) {
			this.IsAimedRight = isAimedRight;
			this.TileX = tileX;
			this.TileY = tileY;
		}


		////////////////

		protected override void ReceiveOnServer( int fromWho ) {
			TrackDeploymentKitItem.Deploy( this.IsAimedRight, this.TileX, this.TileY );
		}

		protected override void ReceiveOnClient() {
			TrackDeploymentKitItem.Deploy( this.IsAimedRight, this.TileX, this.TileY );
		}
	}
}
