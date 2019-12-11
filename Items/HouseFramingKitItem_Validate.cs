using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using HamstarHelpers.Helpers.Tiles.Draw;
using HamstarHelpers.Classes.Tiles.TilePattern;
using HamstarHelpers.Helpers.Tiles;


namespace HouseKits.Items {
	public partial class HouseFramingKitItem : ModItem {
		public static bool Validate( ref int tileX, ref int tileY, out ISet<(int, int)> tiles ) {
			int width = HouseFramingKitItem.FrameWidth;
			int height = HouseFramingKitItem.FrameHeight;
			var myTiles = new HashSet<(int, int)>();

			while( !TileHelpers.IsSolid( Main.tile[tileX, tileY], true, true ) ) {
				tileY++;
				if( tileY >= Main.maxTilesY ) {
					tiles = myTiles;
					return false;
				}
			}

			var outerRect = new Rectangle( tileX - (width / 2), tileY - height, width, height );
			int availableArea = 0;

			TileDrawPrimitivesHelpers.DrawRectangle(
				filter: TilePattern.AbsoluteAir,
				area: outerRect,
				hollow: null,//innerRect,
				place: ( x, y ) => {
					/*int timer = 50;
					Timers.SetTimer( "HFK_"+x+"_"+y, 2, false, () => {
						Dust.QuickDust( new Point(x, y), Color.Lime );
						return timer-- > 0;
					} );*/
					myTiles.Add( (x, y) );
					availableArea++;
					return null;
				}
			);

			tiles = myTiles;
			return availableArea >= (width * height);
		}
	}
}
