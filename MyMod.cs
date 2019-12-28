using HamstarHelpers.Helpers.TModLoader.Mods;
using HamstarHelpers.Services.AnimatedColor;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PrefabKits.Items;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;


namespace PrefabKits {
	public class PrefabKitsMod : Mod {
		public static string GithubUserName => "hamstar0";
		public static string GithubProjectName => "tml-housefurnishingkit-mod";


		////////////////

		public static PrefabKitsMod Instance { get; private set; }




		////////////////

		internal IList<Func<int, int, Item, bool>> OnPreHouseCreate = new List<Func<int, int, Item, bool>>();
		internal IList<Action<int, int, Item>> OnPostHouseCreate = new List<Action<int, int, Item>>();



		////////////////

		public PrefabKitsMod() {
			PrefabKitsMod.Instance = this;
		}

		public override void Unload() {
			PrefabKitsMod.Instance = null;
		}


		////////////////

		public override object Call( params object[] args ) {
			return ModBoilerplateHelpers.HandleModCall( typeof(PrefabKitsAPI), args );
		}


		////////////////

		public override void PostDrawInterface( SpriteBatch sb ) {
			Item heldItem = Main.LocalPlayer.HeldItem;
			if( heldItem == null || heldItem.IsAir ) { return; }

			if( heldItem.type == ModContent.ItemType<TrackDeploymentKitItem>() ) {
				var position = new Vector2( Main.mouseX, Main.mouseY );
				position.X -= 14;
				position.Y -= 32;
				string dirText = Main.LocalPlayer.direction > 0
					? " >"
					: "< ";

				sb.DrawString(
					Main.fontMouseText,
					dirText,
					position,
					AnimatedColors.Strobe.CurrentColor * 0.75f,
					0f,
					default(Vector2),
					2f,
					SpriteEffects.None,
					1f
				);
			}
		}
	}
}