using PrefabKits.Items;
using System;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;


namespace PrefabKits.Recipes {
	class TrackDeploymentKitRecipe : ModRecipe {
		public TrackDeploymentKitRecipe() : base( PrefabKitsMod.Instance ) {
			if( PrefabKitsConfig.Instance.TrackDeploymentKitRecipeTile >= 0 ) {
				this.AddTile( PrefabKitsConfig.Instance.TrackDeploymentKitRecipeTile );
			}
			
			this.AddIngredient( ItemID.MinecartTrack, PrefabKitsConfig.Instance.TrackDeploymentKitTracks );

			foreach( ItemDefinition itemDef in PrefabKitsConfig.Instance.TrackDeploymentKitRecipeExtraIngredient ) {
				this.AddIngredient( itemDef.Type );
			}

			this.SetResult( ModContent.ItemType<TrackDeploymentKitItem>() );
		}


		public override bool RecipeAvailable() {
			return PrefabKitsConfig.Instance.TrackDeploymentKitEnabled;
		}
	}
}
