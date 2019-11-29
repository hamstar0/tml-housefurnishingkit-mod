﻿using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using HamstarHelpers.Classes.Tiles.TilePattern;
using HamstarHelpers.Helpers.Tiles;


namespace HouseFurnishingKit.Items {
	public enum HouseViabilityState {
		Good,
		TooSmall,
		TooLarge,
		SmallFloor
	}




	public partial class HouseFurnishingKitItem : ModItem {
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
			bool isStairOrNotSolid( int x, int y ) {
				Tile tile = Main.tile[x, y];
				return !tile.active()
					|| ( !Main.tileSolid[tile.type] )   /*&& !Main.tileSolidTop[tile.type]*/
					|| ( Main.tileSolid[tile.type] && Main.tileSolidTop[tile.type] && tile.slope() != 0 );  //stair
			}
			bool isStairOrNotSolidOrNotDungeonWall( int x, int y ) {
				Tile tile = Main.tile[x, y];
				if( TileWallHelpers.UnsafeDungeonWallTypes.Contains( tile.wall ) || tile.wall == WallID.LihzahrdBrickUnsafe ) {
					return false;
				}
				return isStairOrNotSolid( x, y );
			}

			///

			TilePattern formPattern = new TilePattern( new TilePatternBuilder {
				CustomCheck = isStairOrNotSolid
			} );
			TilePattern fillPattern = new TilePattern( new TilePatternBuilder {
				AreaFromCenter = new Rectangle( -2, -3, 4, 5 ),
				HasWater = false,
				HasHoney = false,
				HasLava = false,
				IsActuated = false,
				CustomCheck = isStairOrNotSolidOrNotDungeonWall
			} );

			HouseViabilityState state;

			state = HouseFurnishingKitItem.IsValidHouseByCriteria(
				pattern: formPattern,
				tileX: tileX,
				tileY: tileY,
				minimumVolume: 80,
				minimumFloorWidth: 12,
				houseSpace: out fullHouseSpace,
				floorX: out floorX,
				floorY: out floorY
			);
			if( state != HouseViabilityState.Good ) {
				innerHouseSpace = fullHouseSpace;
				return state;
			}

			int altFloorY = floorY;
			state = HouseFurnishingKitItem.IsValidHouseByCriteria(
				pattern: fillPattern,
				tileX: floorX,
				tileY: tileY,
				minimumVolume: 60,
				minimumFloorWidth: 12,
				houseSpace: out innerHouseSpace,
				floorX: out floorX,
				floorY: out altFloorY
			);
			if( state != HouseViabilityState.Good ) {
				return state;
			}

			return state;
		}


		private static HouseViabilityState IsValidHouseByCriteria(
					TilePattern pattern,
					int tileX,
					int tileY,
					int minimumVolume,
					int minimumFloorWidth,
					out IList<(ushort TileX, ushort TileY)> houseSpace,
					out int floorX,
					out int floorY ) {
			IList<(ushort TileX, ushort TileY)> unclosedTiles;
			houseSpace = TileFinderHelpers.GetAllContiguousMatchingTiles(
				pattern: pattern,
				tileX: tileX,
				tileY: tileY,
				unclosedTiles: out unclosedTiles,
				maxRadius: 64
			);

			if( unclosedTiles.Count > 0 ) {
				floorX = floorY = 0;
				return HouseViabilityState.TooLarge;
			}

			if( houseSpace.Count < minimumVolume ) {
				floorX = floorY = 0;
				return HouseViabilityState.TooSmall;
			}

			int floorWidth = TileFinderHelpers.GetFloorWidth(
				nonFloorPattern: pattern,
				tileX: tileX,
				tileY: tileY - 2,
				maxFallRange: 32,
				floorX: out floorX,
				floorY: out floorY
			);
			floorX += floorWidth / 2;

			if( floorWidth < minimumFloorWidth ) {
				return HouseViabilityState.SmallFloor;
			}

			return HouseViabilityState.Good;
		}
	}
}
