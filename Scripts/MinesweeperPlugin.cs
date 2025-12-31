using System;
using UnityEngine;
using BepInEx;
using HarmonyLib;
using Receiver2;
using Receiver2ModdingKit.Gamemodes;

namespace Minesweeper {
	[BepInPlugin("pl.szikaka.minesweeper", "Minesweeper", "1.0.0")]
	[BepInProcess("Receiver2")]
	[BepInDependency("pl.szikaka.receiver_2_modding_kit", "1.5.0")]
	public class MinesweeperPlugin : BaseUnityPlugin {
		[HarmonyPatch(typeof(LocalAimHandler), nameof(LocalAimHandler.IsTerminalFall), new Type[] { typeof(Vector3) })]
		[HarmonyPrefix]
		private static bool PatchFallDamage(ref bool __result) {
			if (ModGameModeManager.CurrentGameMode is MinesweeperGamemode) {
				__result = false;
				return false;
			}

			return true;
		}

		public void OnEnable() {
			Harmony.CreateAndPatchAll(GetType());
			Logger.LogInfo("Are you ready for Freddy?");
		}
	}
}
