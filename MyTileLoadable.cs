using System;
using HamstarHelpers.Classes.Loadable;
using HamstarHelpers.Services.Hooks.ExtendedHooks;


namespace PrefabKits {
	class PrefabKitsTileLoadable : ILoadable {
		public bool IsCreatingHouse { get; private set; }



		////////////////

		public void OnModsLoad() {
			PrefabKitsAPI.OnPreHouseCreate( ( tileX, tileY, item ) => {
				this.IsCreatingHouse = true;
				return true;
			} );

			PrefabKitsAPI.OnPostHouseCreate( ( tileX, tileY, item ) => {
				this.IsCreatingHouse = false;
			} );

			if( ExtendedTileHooks.NonGameplayKillTileCondition == null ) {
				ExtendedTileHooks.NonGameplayKillTileCondition = () => this.IsCreatingHouse;
			} else {
				Func<bool> oldHook = ExtendedTileHooks.NonGameplayKillTileCondition;
				ExtendedTileHooks.NonGameplayKillTileCondition = () => {
					return this.IsCreatingHouse || oldHook();
				};
			}
		}


		public void OnModsUnload() { }

		public void OnPostModsLoad() { }
	}
}
