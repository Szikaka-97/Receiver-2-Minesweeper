using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Receiver2;
using SimpleJSON;
using Receiver2ModdingKit.Gamemodes;

namespace Minesweeper {
	public class MinesweeperGamemode : ModGameModeBase {
		public override string GameModeName {
			get => "Minesweeper";
		}

		public override string SceneName {
			get => "Minesweeper";
		}

		public override string SceneAssetBundleName {
			get => "minesweeper_assets";
		}

		public override void StartLevel() {
			PlayerGUI.Instance.RankDisplay.SetEnabled(false);
			PlayerGUI.Instance.TapeCounter.SetEnabled(false);
			PlayerGUI.Instance.inventory_slots[0].parent.parent.gameObject.SetActive(false);
			PlayerGUI.Instance.holster_slot.gameObject.SetActive(false);

			LevelObject level_object = FindObjectOfType<LevelObject>();
			level_object.loading_progress = 1;
			
			PlayerLoadout loadout = ReceiverCoreScript.Instance().weapon_loadout_asset.GetLoadoutPrefab(level_object.player_loadout);

			if (loadout == null) {
				loadout = ReceiverCoreScript.Instance().weapon_loadout_asset.GetLoadoutPrefab("IntroLoadout");
			}

			ReceiverCoreScript.Instance().SetLoadout(loadout);
		}

		public override void PostResetLevel() {

		}

		public override void Restart(bool complete_reset, bool reset_checkpoint) {
			ReceiverCoreScript.Instance().FadeAndLoad(SceneName);
		}

		public override JSONObject StoreCheckpoint(CheckpointTrigger trigger_name) {
			return new JSONObject();
		}

		public override void LoadCheckpoint(JSONObject checkpoint_data) {
			
		}
	}
}