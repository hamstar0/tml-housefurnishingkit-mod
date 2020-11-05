using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using HamstarHelpers.Helpers.Debug;
using HamstarHelpers.Classes.Tiles.TilePattern;


namespace PrefabKits.Items {
	public partial class TrackDeploymentKitItem : ModItem {
		public class PathTree {
			public int TileX;
			public int TileY;
			public int HighestDepthCount;

			public PathTree Top;
			public PathTree Mid;
			public PathTree Bot;


			public int Count() {
				int count = this.HighestDepthCount > 0 ? 1 : 0;
				count += this.Top?.Count() ?? 0;
				count += this.Mid?.Count() ?? 0;
				count += this.Bot?.Count() ?? 0;

				return count;
			}
		}
	}
}
