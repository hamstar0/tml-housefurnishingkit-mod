using System;
using System.Collections.Generic;
using Terraria;
using HamstarHelpers.Helpers.Debug;
using HamstarHelpers.Classes.Errors;
using HamstarHelpers.Classes.Protocols.Packet.Interfaces;
using PrefabKits.Items;


namespace PrefabKits.Protocols {
	class FramingKitProtocol : PacketProtocolSendToServer {
		public static void SendToServer( int tileX, int tileY ) {
			if( Main.netMode != 1 ) { throw new ModHelpersException( "Not client" ); }

			var protocol = new FramingKitProtocol(  tileX, tileY );
			protocol.SendToServer( false );
		}



		////////////////

		public int TileX;
		public int TileY;



		////////////////

		private FramingKitProtocol() { }

		private FramingKitProtocol( int tileX, int tileY ) {
			this.TileX = tileX;
			this.TileY = tileY;
		}

		protected override void InitializeClientSendData() {
		}

		////

		protected override void Receive( int fromWho ) {
			ISet<(int TileX, int TileY)> houseTiles;
			bool isValid = HouseFramingKitItem.Validate( ref this.TileX, ref this.TileY, out houseTiles );

			if( isValid ) {
				HouseFramingKitItem.MakeHouseFrame( this.TileX, this.TileY );
			} else {
				LogHelpers.Alert( "Could not place house frame" );
			}
		}
	}
}
