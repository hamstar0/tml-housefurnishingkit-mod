using HamstarHelpers.Classes.Protocols.Packet.Interfaces;
using System;


namespace HouseFurnishingKit.Protocols {
	class CustomFurnitureProtocol : PacketProtocolRequestToServer {
		public static void QuickRequest() {
			PacketProtocolRequestToServer.QuickRequest<CustomFurnitureProtocol>( -1 );
		}



		////////////////

		public int CustomFurnitureTileType;
		public int CustomFurnitureWidth;
		public int CustomFurnitureHeight;

		public int Custom3x3WallMount1TileType;
		public int Custom3x3WallMount1Width;
		public int Custom3x3WallMount1Height;

		public int Custom3x3WallMount2TileType;
		public int Custom3x3WallMount2Width;
		public int Custom3x3WallMount2Height;



		////////////////

		private CustomFurnitureProtocol() { }


		////////////////

		protected override void InitializeServerSendData( int fromWho ) {
			var mymod = HouseFurnishingKitMod.Instance;

			this.CustomFurnitureTileType = mymod.CustomFurniture.TileType;
			this.CustomFurnitureWidth = mymod.CustomFurniture.Width;
			this.CustomFurnitureHeight = mymod.CustomFurniture.Height;
			this.Custom3x3WallMount1TileType = mymod.Custom3x3WallMount1.TileType;
			this.Custom3x3WallMount1Width = mymod.Custom3x3WallMount1.Width;
			this.Custom3x3WallMount1Height = mymod.Custom3x3WallMount1.Height;
			this.Custom3x3WallMount2TileType = mymod.Custom3x3WallMount2.TileType;
			this.Custom3x3WallMount2Width = mymod.Custom3x3WallMount2.Width;
			this.Custom3x3WallMount2Height = mymod.Custom3x3WallMount2.Height;
		}


		////////////////

		protected override void ReceiveReply() {
			var mymod = HouseFurnishingKitMod.Instance;

			HouseFurnishingKitAPI.SetCustomFurniture(
				mymod.CustomFurniture.TileType,
				mymod.CustomFurniture.Width,
				mymod.CustomFurniture.Height
			);
			HouseFurnishingKitAPI.SetCustomWallMount1(
				mymod.Custom3x3WallMount1.TileType,
				mymod.Custom3x3WallMount1.Width,
				mymod.Custom3x3WallMount1.Height
			);
			HouseFurnishingKitAPI.SetCustomWallMount2(
				mymod.Custom3x3WallMount2.TileType,
				mymod.Custom3x3WallMount2.Width,
				mymod.Custom3x3WallMount2.Height
			);
		}
	}
}
