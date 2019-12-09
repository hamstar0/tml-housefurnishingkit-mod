using Terraria;
using Terraria.ModLoader;
using System;
using HamstarHelpers.Helpers.Tiles.Draw;
using HamstarHelpers.Classes.Tiles.TilePattern;
using Microsoft.Xna.Framework;


namespace HouseFurnishingKit.Items {
	public partial class HouseFramingKitItem : ModItem {
		public static bool Validate( int tileX, int tileY ) {
			int width = HouseFramingKitItem.FrameWidth;
			int height = HouseFramingKitItem.FrameHeight;
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
					availableArea++;
					return null;
				}
			);

			return availableArea >= (width * height);
		}
	}
}
