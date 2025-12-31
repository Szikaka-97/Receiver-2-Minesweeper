using Receiver2;
using UnityEngine;
using UnityEngine.Events;

namespace Minesweeper {
	[RequireComponent(typeof(MeshCollider))]
	public class ComputerButton : MonoBehaviour {
		public UnityEvent OnPress;

		private MeshCollider raycast_catch;
		private MeshRenderer button_renderer;

		private float pressed_amount;

		static bool tutorial_done;
		static float time_staring;

		public void Awake() {
			this.raycast_catch = GetComponent<MeshCollider>();
			this.button_renderer = GetComponent<MeshRenderer>();
		}

		public void Update() {
			Camera camera = Camera.main;

			if (!camera) {
				return;
			}

			Ray camera_ray = camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

			if (!tutorial_done && time_staring > 10) {
				PlayerGUI.Instance.InfoList.QueueRows(new PlayerGUIInfoList.Row[] {
					new PlayerGUIInfoList.Row("Just click on the thing", 0.1f, 4f),
					new PlayerGUIInfoList.Row("Never seen a touchscreen before?", 0.6f, 3.5f),
				});

				tutorial_done = true;
			}

			if (raycast_catch.Raycast(camera_ray, out var hit_info, 2)) {
				pressed_amount = Mathf.MoveTowards(pressed_amount, 0.03f, Time.deltaTime * 4);

				if (Input.GetMouseButtonDown(0)) {
					Debug.Log("Pressed: " + this.name);
					OnPress.Invoke();
					pressed_amount = 1;

					tutorial_done = true;
				}

				time_staring += Time.deltaTime;
			}
			else {
				pressed_amount = Mathf.MoveTowards(pressed_amount, 0.0f, Time.deltaTime * 4);
			}

			this.button_renderer.material.SetFloat("_PressedAmount", pressed_amount);
		}
	}
}
