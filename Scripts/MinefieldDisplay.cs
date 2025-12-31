using Receiver2;
using UnityEngine;

namespace Minesweeper {
	public class MinefieldDisplay : MonoBehaviour {
		public Transform root;
		public Mesh tile_mesh;
		public Material tile_material;
		public Material highlight_material;
		public float size;
		public bool interactive;
		public bool shovellable;
		public Minefield minefield;
		public Collider screen_collider;

		public void Update() {
			if (minefield.minefield_size == 0 || Camera.main == null) {
				return;
			}
			
			Vector2 highlighted_pos = -Vector2.one;

			if (screen_collider != null && Camera.main != null) {
				Ray camera_ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

				if (shovellable) {
					camera_ray = new Ray(LocalAimHandler.player_instance.main_camera.transform.position, Vector3.down);
				}


				if (screen_collider.Raycast(camera_ray, out var hit_info, 3)) {
					Vector3 hit_pos = root.InverseTransformPoint(hit_info.point) * minefield.minefield_size / size;
					
					Vector2Int tile_pos = new Vector2Int(
						Mathf.FloorToInt(hit_pos.x),
						Mathf.FloorToInt(hit_pos.y)
					);

					Matrix4x4 highlight_transform = Matrix4x4.TRS(
						root.TransformPoint(new Vector3(tile_pos.x + 0.5f, tile_pos.y + 0.5f, -0.001f) * size / minefield.minefield_size),
						root.rotation,
						Vector3.one * size / minefield.minefield_size
					);

					Graphics.DrawMesh(
						tile_mesh,
						highlight_transform,
						highlight_material,
						0
					);

					if (interactive) {
						if (Input.GetMouseButtonDown(0)) {
							AudioManager.PlayOneShot3D("event:/UI/ui_butt_pausenested", hit_info.point, 0.3f, 1.3f);

							this.minefield.Tick(tile_pos);
						}
						// if (Input.GetMouseButtonDown(1)) {
						// 	this.minefield.Flag(tile_pos);
						// }
					}

					if (shovellable) {
						if (
							LocalAimHandler.player_instance.hands[0].state == LocalAimHandler.Hand.State.HoldingGenericItem
							&&
							LocalAimHandler.player_instance.hands[0].slot.contents[0] is MinefieldShovel
							&&
							Input.GetMouseButtonDown(0)
						) {
							AudioManager.PlayOneShot3D("event:/playermovement/material_bed_footstep", hit_info.point, 0.5f, 1.3f);

							this.minefield.Tick(tile_pos);
						}

						if (
							LocalAimHandler.player_instance.hands[0].state == LocalAimHandler.Hand.State.HoldingGenericItem
							&&
							LocalAimHandler.player_instance.hands[0].slot.contents[0] is MinefieldFlag
							&&
							Input.GetMouseButtonDown(0)
						) {
							AudioManager.PlayOneShot3D("event:/MallDLC/ball_pickup", hit_info.point, 0.6f, 1.3f);

							this.minefield.Flag(tile_pos);
						}
					}
				}
			}

			Matrix4x4 tile_transform = Matrix4x4.TRS(root.TransformPoint(new Vector2(0.5f, 0.5f) * size / minefield.minefield_size), root.rotation, Vector3.one * size / minefield.minefield_size);

			MaterialPropertyBlock tile_props = new MaterialPropertyBlock();
			tile_props.SetInt("_MinefieldSize", minefield.minefield_size);
			tile_props.SetMatrix("_RootPosition", tile_transform);
			tile_props.SetBuffer("_TileStates", minefield.tile_states_gpu);

			Graphics.DrawMeshInstancedProcedural(
				tile_mesh,
				0,
				tile_material,
				new Bounds(Vector3.zero, Vector3.one * 99999),
				minefield.minefield_size * minefield.minefield_size,
				tile_props
			);

			// Graphics.DrawMesh(tile_mesh, tile_transform, tile_material, 0, null, 0, tile_props);

			// foreach (Minefield.TileState state in minefield.tile_states) {


			// 	Minefield.TileState tile_state = state;
			// 	if (tile_state == Minefield.TileState.Unchecked && highlighted_pos == tile_pos) {
			// 		tile_state++;
			// 	}



			// 	tile_pos.x += 1;
			// 	if (tile_pos.x == minefield.minefield_size) {
			// 		tile_pos.x = 0;
			// 		tile_pos.y += 1;
			// 	}
			// }
		}
	}
}