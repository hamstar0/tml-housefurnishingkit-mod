using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using HamstarHelpers.Helpers.Debug;
using HamstarHelpers.Classes.Errors;
using HamstarHelpers.Classes.Protocols.Packet.Interfaces;
using PrefabKits.Items;


namespace PrefabKits.Protocols {
	class FurnishingKitProtocol : PacketProtocolSendToServer {
		public static void SendToServer( Player player, int tileX, int tileY ) {
			if( Main.netMode != 1 ) { throw new ModHelpersException( "Not client" ); }

			var protocol = new FurnishingKitProtocol( player, tileX, tileY );
			protocol.SendToServer( false );
		}



		////////////////

		public int PlayerWho;
		public int TileX;
		public int TileY;



		////////////////

		private FurnishingKitProtocol() { }

		private FurnishingKitProtocol( Player player, int tileX, int tileY ) {
			this.PlayerWho = player.whoAmI;
			this.TileX = tileX;
			this.TileY = tileY;
		}

		protected override void InitializeClientSendData() {
		}

		////

		protected override void Receive( int fromWho ) {
			IList<(ushort TileX, ushort TileY)> innerHouseSpace, fullHouseSpace;
			int floorX, floorY;

			HouseViabilityState state = HouseFurnishingKitItem.IsValidHouse(
				this.TileX,
				this.TileY,
				out innerHouseSpace,
				out fullHouseSpace,
				out floorX,
				out floorY
			);

			if( state == HouseViabilityState.Good ) {
				bool aborted = HouseFurnishingKitItem.FurnishHouseFull(
					Main.player[this.PlayerWho],
					this.TileX,
					this.TileY,
					innerHouseSpace,
					fullHouseSpace,
					floorX,
					floorY
				);
			}
		}
	}
}
