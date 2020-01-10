using System;
using Terraria;
using HamstarHelpers.Classes.Errors;
using HamstarHelpers.Classes.Protocols.Packet.Interfaces;
using HamstarHelpers.Helpers.Debug;
using PrefabKits.Items;


namespace PrefabKits.Protocols {
	class TrackKitResumeProtocol : PacketProtocolSendToClient {
		public static void SendToClient( int fromPlayerWho, int tileX, int tileY, bool isAimedRight ) {
			if( Main.netMode != 2 ) { throw new ModHelpersException( "Not server" ); }

			var protocol = new TrackKitResumeProtocol( fromPlayerWho, tileX, tileY, isAimedRight );

			protocol.SendToClient( -1, -1 );
		}



		////////////////

		public int FromPlayerWho;
		public int TileX;
		public int TileY;
		public bool IsAimedRight;



		////////////////

		private TrackKitResumeProtocol() { }

		private TrackKitResumeProtocol( int fromPlayerWho, int tileX, int tileY, bool isAimedRight ) {
			this.FromPlayerWho = fromPlayerWho;
			this.TileX = tileX;
			this.TileY = tileY;
			this.IsAimedRight = isAimedRight;
		}

		protected override void InitializeServerSendData( int toWho ) { }


		////////////////

		protected override void Receive() {
			TrackDeploymentKitItem.PlaceResumePoint( this.TileX, this.TileY, this.IsAimedRight );
		}
	}
}
