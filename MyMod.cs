using Terraria.ModLoader;


namespace HouseFurnishingKit {
	public class HouseFurnishingKitMod : Mod {
		public static string GithubUserName => "hamstar0";
		public static string GithubProjectName => "tml-adventuremode-mod";


		////////////////

		public static HouseFurnishingKitMod Instance { get; private set; }




		////////////////

		public (ushort TileType, int Width, int Height) CustomFurniture = (0, 0, 0);
		public (ushort TileType, int Width, int Height) Custom3x3WallMount1 = (0, 0, 0);
		public (ushort TileType, int Width, int Height) Custom3x3WallMount2 = (0, 0, 0);



		////////////////

		public HouseFurnishingKitMod() {
			HouseFurnishingKitMod.Instance = this;
		}

		public override void Unload() {
			HouseFurnishingKitMod.Instance = null;
		}
	}
}