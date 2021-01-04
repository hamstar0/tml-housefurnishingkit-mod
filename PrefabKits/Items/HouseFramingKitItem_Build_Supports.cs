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
		private static void MakeHouseSupports( Rectangle rect, int tileY ) {
			var supportLeft = new Rectangle( rect.X, tileY, 1, 256 );
			var supportRight = new Rectangle( rect.X + rect.Width - 1, tileY, 1, 256 );
			int floorLeft = tileY + 256;
			int floorRight = tileY + 256;

			var woodBeamDef = new TileDrawDefinition { TileType = TileID.WoodenBeam };

			//

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

			//

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

			if( Main.netMode == 2 ) {
				NetMessage.SendTileRange(
					whoAmi: -1,
					tileX: supportLeft.X,
					tileY: supportLeft.Y,
					xSize: supportLeft.Width,
					ySize: supportLeft.Height
				);
				NetMessage.SendTileRange(
					whoAmi: -1,
					tileX: supportRight.X,
					tileY: supportRight.Y,
					xSize: supportRight.Width,
					ySize: supportRight.Height
				);
			}
		}
	}
}
