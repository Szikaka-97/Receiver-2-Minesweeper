using UnityEngine;
using Receiver2;

namespace Minesweeper {
	public class MinefieldFlag : GenericHoldable {
		public static bool tutorial_done = false;

		new public void Awake() {
			base.Awake();
		}

		public override void OnChangeInventorySlot(InventorySlot old_slot, InventorySlot new_slot, LocalAimHandler.Hand from_hand, LocalAimHandler.Hand to_hand) {
			if (new_slot == null) {
				return;
			}

			if (new_slot.type == InventorySlot.Type.LeftHand || new_slot.type == InventorySlot.Type.RightHand) {
				var holdEvent = this.pose_events[this.pose_events.Count - 1];

				if (LocalAimHandler.player_instance.main_camera.TryGetComponent<FlagAttachment>(out var attachment)) {
					holdEvent.parent = attachment.transform;
				}
				else {
					holdEvent.parent = FlagAttachment.AddToPlayer(this).transform;
				}

				if (!tutorial_done && !MinefieldShovel.tutorial_done) {
					tutorial_done = true;

					PlayerGUI.Instance.InfoList.QueueRows(new PlayerGUIInfoList.Row[] {
						new PlayerGUIInfoList.Row("Step on the tiles and use your mouse", 0.1f, 3f)
					});
				}
			}
		}

		protected override void OnCollisionEnter(Collision collision) {
			base.OnCollisionEnter(collision);

			if (collision.relativeVelocity.sqrMagnitude > 40*40) {
				AudioManager.PlayOneShot3D("event:/guns/Ground_impact", collision.GetContact(0).point, 2, 1);
				AudioManager.PlayOneShot3D("event:/bullets/hit_steel_target_large", collision.GetContact(0).point, 0.5f, 1);
			}
		}

		public override void OnDropped() {
			base.OnDropped();

			FlagAttachment.RemoveFromPlayer();
		}

		new public void Update() {
			base.Update();

			if (transform.position.y < -10) {
				Vector3 new_pos = Minefield.instance.main_minefield.root.position + new Vector3(1.1f, 0, 1) * Minefield.instance.main_minefield.size / 2;
				new_pos.y = 10.0f;

				transform.position = new_pos;
			}
		}
	}
}