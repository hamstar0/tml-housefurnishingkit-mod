using HamstarHelpers.Classes.Protocols.Packet.Interfaces;
using System;


namespace HouseFurnishingKit.Protocols {
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
			var mymod = HouseFurnishingKitMod.Instance;

			this.CustomFurnitureTileType = mymod.CustomFurniture;
			this.Custom3x3WallMount1TileType = mymod.CustomWallMount1;
			this.Custom3x3WallMount2TileType = mymod.CustomWallMount2;
		}


		////////////////

		protected override void ReceiveReply() {
			var mymod = HouseFurnishingKitMod.Instance;

			HouseFurnishingKitAPI.SetCustomFurniture( mymod.CustomFurniture );
			HouseFurnishingKitAPI.SetCustomWallMount1( mymod.CustomWallMount1 );
			HouseFurnishingKitAPI.SetCustomWallMount2( mymod.CustomWallMount2 );
		}
	}
}
