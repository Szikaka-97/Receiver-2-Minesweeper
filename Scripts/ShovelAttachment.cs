using UnityEngine;
using Receiver2;
using HarmonyLib;
using Receiver2ModdingKit;
using System.Linq;

namespace Minesweeper {
	public class ShovelAttachment : MonoBehaviour {
		public delegate float EyeHeightDelegate();

		// public static float swordLength = 1.3f;

		public LocalAimHandler player;
		public EyeHeightDelegate GetPlayerEyeHeight;
		public MinefieldShovel sword;

		private float offset;

		public static ShovelAttachment AddToPlayer(MinefieldShovel sword) {
			GameObject swordAttachmentObject = new GameObject("Sword Attachment Point");

			swordAttachmentObject.transform.SetParent(LocalAimHandler.player_instance.main_camera.transform);

			var result = swordAttachmentObject.AddComponent<ShovelAttachment>();

			result.sword = sword;

			return result;
		}

		public static void RemoveFromPlayer() {
			Transform swordAttachmentTransform = LocalAimHandler.player_instance.main_camera.transform.Find("Sword Attachment Point");

			if (swordAttachmentTransform != null) {
				GameObject.DestroyImmediate(swordAttachmentTransform.gameObject);
			}
		}

		public void Awake() {
			this.player = LocalAimHandler.player_instance;

			this.GetPlayerEyeHeight = AccessTools.MethodDelegate<EyeHeightDelegate>(AccessTools.Method(typeof(LocalAimHandler), "GetEyeHeight"), this.player);
		}

		public void FixedUpdate() {
			// var swordLength = (this.sword.transform.position - this.sword.ClosestPoint(this.sword.transform.position + Vector3.down * 10)).magnitude;
			// var swordLength = 3;

			if (Physics.Raycast(this.player.main_camera.transform.position, Vector3.down, out var hit, 2, ReceiverCoreScript.Instance().layer_mask_shootable)) {
				// Debug.Log(this.sword.transform.position * 100);
				// Debug.Log(swordLength);
				// Debug.Log(hit.distance);
				// Debug.Log(this.offset);
				// Debug.Log("----------------");

				this.offset = Vector3.Distance(this.player.main_camera.transform.position, hit.point) - 1;
			}
		}

		public void Update() {
			this.transform.position = this.player.main_camera.transform.position + Vector3.down * this.offset;
			
			this.transform.rotation = this.player.transform.rotation;
		}
	}
}