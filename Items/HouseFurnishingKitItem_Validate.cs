using HamstarHelpers.Classes.Tiles.TilePattern;
using HamstarHelpers.Helpers.Tiles;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using System;


namespace AdventureMode.Items {
	public enum HouseViabilityState {
		Good,
		TooSmall,
		TooLarge,
		SmallFloor
	}




	partial class HouseFurnishingKitItem : ModItem {
		public static string GetViabilityStateMessage( HouseViabilityState state, out Color color ) {
			switch( state ) {
			case HouseViabilityState.Good:
				color = Color.Lime;
				return "Valid town house space found. Note: Only above ground houses work automatically.";
			case HouseViabilityState.TooSmall:
				color = Color.Yellow;
				return "House too small.";
			case HouseViabilityState.TooLarge:
				color = Color.Yellow;
				return "House too large or not a closed space.";
			case HouseViabilityState.SmallFloor:
				color = Color.Yellow;
				return "Not enough floor space.";
			}

			color = Color.Transparent;
			return "";
		}


		////////////////

		public static HouseViabilityState IsValidHouse( int tileX, int tileY ) {
			IList<(ushort TileX, ushort TileY)> innerHouseSpace;
			IList<(ushort TileX, ushort TileY)> fullHouseSpace;
			int floorX, floorY;

			return HouseFurnishingKitItem.IsValidHouse( tileX, tileY, out innerHouseSpace, out fullHouseSpace, out floorX, out floorY );
		}


		public static HouseViabilityState IsValidHouse(
					int tileX,
					int tileY,
					out IList<(ushort TileX, ushort TileY)> innerHouseSpace,
					out IList<(ushort TileX, ushort TileY)> fullHouseSpace,
					out int floorX,
					out int floorY ) {
			bool customCheck( int x, int y ) {
				Tile tile = Main.tile[x, y];
				return !tile.active()
					|| ( !Main.tileSolid[tile.type] )	/*&& !Main.tileSolidTop[tile.type]*/
					|| ( Main.tileSolid[tile.type] && Main.tileSolidTop[tile.type] && tile.slope() != 0 );  //stair
			}

			TilePattern pattern1 = new TilePattern( new TilePatternBuilder {
				AreaFromCenter = new Rectangle( -2, -2, 4, 4 ),
				HasWater = false,
				HasHoney = false,
				HasLava = false,
				IsActuated = false,
				CustomCheck = customCheck
			} );
			TilePattern pattern2 = new TilePattern( new TilePatternBuilder {
				HasWater = false,
				HasHoney = false,
				HasLava = false,
				IsActuated = false,
				CustomCheck = customCheck
			} );
			HouseViabilityState state;

			state = HouseFurnishingKitItem.IsValidHouseByCriteria( pattern2, tileX, tileY, out fullHouseSpace, out floorX, out floorY );
			if( state != HouseViabilityState.Good ) {
				innerHouseSpace = fullHouseSpace;
				return state;
			}

			int altFloorY = floorY;
			state = HouseFurnishingKitItem.IsValidHouseByCriteria( pattern1, floorX, tileY, out innerHouseSpace, out floorX, out altFloorY );
			if( state != HouseViabilityState.Good ) {
				return state;
			}

			return state;
		}


		private static HouseViabilityState IsValidHouseByCriteria(
					TilePattern pattern,
					int tileX,
					int tileY,
					out IList<(ushort TileX, ushort TileY)> houseSpace,
					out int floorX,
					out int floorY ) {
			IList<(ushort TileX, ushort TileY)> unclosedTiles;
			houseSpace = TileFinderHelpers.GetAllContiguousMatchingTiles(
				pattern,
				tileX,
				tileY,
				out unclosedTiles,
				64
			);

			if( unclosedTiles.Count > 0 ) {
				floorX = floorY = 0;
				return HouseViabilityState.TooLarge;
			}

			if( houseSpace.Count < 80 ) {
				floorX = floorY = 0;
				return HouseViabilityState.TooSmall;
			}

			int floorWidth = TileFinderHelpers.GetFloorWidth( pattern, tileX, tileY - 2, 32, out floorX, out floorY );
			floorX += floorWidth / 2;

			if( floorWidth < 10 ) {
				return HouseViabilityState.SmallFloor;
			}

			return HouseViabilityState.Good;
		}
	}
}
