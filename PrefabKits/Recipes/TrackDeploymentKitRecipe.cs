using System;
using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using PrefabKits.Items;


namespace PrefabKits.Recipes {
	class TrackDeploymentKitRecipe : ModRecipe {
		public TrackDeploymentKitRecipe() : base( PrefabKitsMod.Instance ) {
			var config = PrefabKitsConfig.Instance;

			if( config.Get<int>( nameof(config.TrackDeploymentKitRecipeTile) ) >= 0 ) {
				this.AddTile( config.Get<int>( nameof(config.TrackDeploymentKitRecipeTile) ) );
			}
			
			//
			
			int tracks = config.Get<int>( nameof(config.TrackDeploymentKitTracks) );
			this.AddIngredient( ItemID.MinecartTrack, tracks );

			string ingredConfigEntry = nameof( config.TrackDeploymentKitRecipeExtraIngredient );
			var ingredItems = config.Get<List<ItemDefinition>>( ingredConfigEntry );
			foreach( ItemDefinition itemDef in ingredItems ) {
				this.AddIngredient( itemDef.Type );
			}

			//

			this.SetResult( ModContent.ItemType<TrackDeploymentKitItem>() );
		}


		public override bool RecipeAvailable() {
			return PrefabKitsConfig.Instance.Get<bool>( nameof(PrefabKitsConfig.TrackDeploymentKitEnabled) );
		}
	}
}
