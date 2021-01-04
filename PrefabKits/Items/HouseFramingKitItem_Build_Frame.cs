using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using HamstarHelpers.Helpers.Debug;
using HamstarHelpers.Classes.Tiles.TilePattern;
using HamstarHelpers.Helpers.Tiles.Draw;


namespace PrefabKits.Items {
	public partial class HouseFramingKitItem : ModItem {
		public static void MakeHouseFrame( int tileX, int tileY ) {
			int width = HouseFramingKitItem.FrameWidth;
			int height = HouseFramingKitItem.FrameHeight;
			var outerRect = new Rectangle(
				tileX - (width / 2),
				tileY - height,
				width,
				height
			);
			var innerRect = outerRect;
			innerRect.X += 1;
			innerRect.Y += 1;
			innerRect.Width -= 2;
			innerRect.Height -= 2;

			var frameTileDef = new TileDrawDefinition { TileType = TileID.WoodBlock };

			//

			bool isSolidFrame( int x, int y ) {
				int offX = x - outerRect.X;
				int offY = y - outerRect.Y;

				if( offX == 0 || offX == width - 1 ) {
					if( offY == (height - 2) ) {
						return false;
					} else if( offY >= (height - 4) && offY <= (height - 3) ) {
						return false;
					}
				} else if( offX >= (width / 2) - 3 && offX <= (width / 2) + 2 ) {
					if( offY == 0 ) {
						return false;
					}
				}
/*bool isActive = Main.tile[x, y].active();
int timer = 150;
Timers.SetTimer( "HFK0_"+x+"_"+y, 2, false, () => {
	Dust.QuickDust( new Point(x, y), isActive ? Color.Purple : Color.Blue );
	return timer-- > 0;
} );*/
				return true;
			}

			//

			TileDrawDefinition getTileFeatureAt( int x, int y ) {
				TileDrawDefinition myTileDef = null;
				int offX = x - outerRect.X;
				int offY = y - outerRect.Y;

				if( offX == 0 || offX == width - 1 ) {
					if( offY == (height - 2) ) {
						myTileDef = new TileDrawDefinition { TileType = TileID.ClosedDoor };
					} else if( offY >= (height - 4) && offY <= (height - 3) ) {
						myTileDef = null;
					}
				} else if( offX >= (width / 2) - 3 && offX <= (width / 2) + 2 ) {
					if( offY == 0 ) {
						myTileDef = new TileDrawDefinition { TileType = TileID.Platforms };
					}
				}

				return myTileDef;
			}

			//

			TileDrawPrimitivesHelpers.DrawRectangle(
				filter: TilePattern.NonActive,
				area: outerRect,
				hollow: innerRect,
				place: (x, y) => isSolidFrame(x, y) ? frameTileDef : null
			);
			TileDrawPrimitivesHelpers.DrawRectangle(
				filter: TilePattern.NonActive,
				area: outerRect,
				hollow: innerRect,
				place: getTileFeatureAt
			);
				
			if( Main.netMode == 2 ) {
				NetMessage.SendTileRange(
					whoAmi: -1,
					tileX: outerRect.X,
					tileY: outerRect.Y ,
					xSize: outerRect.Width,
					ySize: outerRect.Height
				);
			}

			//

			if( Main.netMode == 0 ) {
				HouseFramingKitItem.MakeHouseSupports( outerRect, tileY );
			}
		}
	}
}
