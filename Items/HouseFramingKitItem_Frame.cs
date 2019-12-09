using Terraria;
using Terraria.ModLoader;
using System;
using HamstarHelpers.Helpers.Tiles.Draw;
using HamstarHelpers.Classes.Tiles.TilePattern;
using Terraria.ID;
using Microsoft.Xna.Framework;


namespace HouseFurnishingKit.Items {
	public partial class HouseFramingKitItem : ModItem {
		public static void MakeHouseFrame( int tileX, int tileY ) {
			int width = HouseFramingKitItem.FrameWidth;
			int height = HouseFramingKitItem.FrameHeight;
			var outerRect = new Rectangle( tileX - (width / 2), tileY - height, width, height );
			var innerRect = outerRect;
			innerRect.X += 1;
			innerRect.Y += 1;
			innerRect.Width -= 2;
			innerRect.Height -= 2;
			var frameTileDef = new TileDrawDefinition { TileType = TileID.WoodBlock };

			//

			TileDrawDefinition placeFrame( int x, int y, bool solidOnly ) {
				TileDrawDefinition myTileDef = solidOnly ? frameTileDef : null;
				int offX = x - outerRect.X;
				int offY = y - outerRect.Y;

				if( offX == 0 || offX == width - 1 ) {
					if( offY == (height - 2) ) {
						myTileDef = solidOnly ? null : new TileDrawDefinition { TileType = TileID.ClosedDoor };
					} else if( offY >= (height - 4) && offY <= (height - 3) ) {
						myTileDef = null;
					}
				} else if( offX >= (width / 2) - 3 && offX <= (width / 2) + 2 ) {
					if( offY == 0 ) {
						myTileDef = solidOnly ? null : new TileDrawDefinition { TileType = TileID.Platforms };
					}
				}
/*bool isActive = Main.tile[x, y].active();
int timer = 150;
Timers.SetTimer( "HFK0_"+x+"_"+y, 2, false, () => {
	Dust.QuickDust( new Point(x, y), isActive ? Color.Purple : Color.Blue );
	return timer-- > 0;
} );*/

				return myTileDef;
			}

			TileDrawDefinition placeSolidFrame( int x, int y ) {
				return placeFrame( x, y, true );
			}
			TileDrawDefinition placeFeatures( int x, int y ) {
				return placeFrame( x, y, false );
			}

			//

			TileDrawPrimitivesHelpers.DrawRectangle(
				filter: TilePattern.AbsoluteAir,
				area: outerRect,
				hollow: innerRect,
				place: placeSolidFrame
			);
			TileDrawPrimitivesHelpers.DrawRectangle(
				filter: TilePattern.AbsoluteAir,
				area: outerRect,
				hollow: innerRect,
				place: placeFeatures
			);

			//

			HouseFramingKitItem.MakeHouseSupports( outerRect, tileY );
		}


		private static void MakeHouseSupports( Rectangle rect, int tileY ) {
			var supportLeft = new Rectangle( rect.X, tileY, 1, 256 );
			var supportRight = new Rectangle( rect.X + rect.Width - 1, tileY, 1, 256 );
			int floorLeft = tileY + 256;
			int floorRight = tileY + 256;

			TileDrawDefinition placeSupportLeft( int x, int y ) {
				if( y >= floorLeft ) {
					return null;
				}

				if( Main.tile[x, y].active() ) {
					if( HamstarHelpers.Helpers.Tiles.Attributes.TileAttributeHelpers.IsBreakable(x, y) ) {
						WorldGen.KillTile( x, y, false, false, true );
					} else {
						floorLeft = y;
						return null;
					}
				}

				return new TileDrawDefinition { TileType = TileID.WoodenBeam };
			}

			TileDrawDefinition placeSupportRight( int x, int y ) {
				if( y >= floorRight ) {
					return null;
				}

				if( Main.tile[x, y].active() ) {
					if( HamstarHelpers.Helpers.Tiles.Attributes.TileAttributeHelpers.IsBreakable(x, y) ) {
						WorldGen.KillTile( x, y, false, false, true );
					} else {
						floorRight = y;
						return null;
					}
				}

				return new TileDrawDefinition { TileType = TileID.WoodenBeam };
			}

			//

			TileDrawPrimitivesHelpers.DrawRectangle(
				filter: TilePattern.Any,
				area: supportLeft,
				hollow: null,
				place: placeSupportLeft
			);
			TileDrawPrimitivesHelpers.DrawRectangle(
				filter: TilePattern.Any,
				area: supportRight,
				hollow: null,
				place: placeSupportRight
			);
		}
	}
}
