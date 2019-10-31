using Terraria.ModLoader;


namespace HouseFurnishingKit {
	public class HouseFurnishingKitMod : Mod {
		public static string GithubUserName => "hamstar0";
		public static string GithubProjectName => "tml-adventuremode-mod";


		////////////////

		public static HouseFurnishingKitMod Instance { get; private set; }




		////////////////

		public HouseFurnishingKitMod() {
			HouseFurnishingKitMod.Instance = this;
		}

		public override void Unload() {
			HouseFurnishingKitMod.Instance = null;
		}
	}
}