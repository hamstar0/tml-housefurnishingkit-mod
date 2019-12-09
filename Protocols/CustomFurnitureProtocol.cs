using HamstarHelpers.Classes.Protocols.Packet.Interfaces;
using System;


namespace HouseKits.Protocols {
	class CustomFurnitureProtocol : PacketProtocolRequestToServer {
		public static void QuickRequest() {
			PacketProtocolRequestToServer.QuickRequest<CustomFurnitureProtocol>( -1 );
		}



		////////////////

		public int CustomFurnitureTileType;
		public int Custom3x3WallMount1TileType;
		public int Custom3x3WallMount2TileType;



		////////////////

		private CustomFurnitureProtocol() { }


		////////////////

		protected override void InitializeServerSendData( int fromWho ) {
			var mymod = HouseKitsMod.Instance;

			this.CustomFurnitureTileType = mymod.CustomFurniture;
			this.Custom3x3WallMount1TileType = mymod.CustomWallMount1;
			this.Custom3x3WallMount2TileType = mymod.CustomWallMount2;
		}


		////////////////

		protected override void ReceiveReply() {
			var mymod = HouseKitsMod.Instance;

			HouseKitsAPI.SetCustomFurniture( mymod.CustomFurniture );
			HouseKitsAPI.SetCustomWallMount1( mymod.CustomWallMount1 );
			HouseKitsAPI.SetCustomWallMount2( mymod.CustomWallMount2 );
		}
	}
}
