using System;
using UnityEngine;
using Receiver2;
using Coroutine = System.Collections.IEnumerator;

namespace Minesweeper {
	public class Minefield : MonoBehaviour {
		public enum TileState : UInt32 {
			Unchecked,
			Highlighted,
			HasBomb,
			Checked_0,
			Checked_1,
			Checked_2,
			Checked_3,
			Checked_4,
			Checked_5,
			Checked_6,
			Checked_7,
			Checked_8,
			Flagged,
		}

		const float tile_size = 3;

		public InputStation mine_count_input;
		public InputStation minefield_size_input;
		public GameObject dramatic_entrance;
		public MinefieldDisplay main_minefield;
		public MinefieldDisplay sky_minefield;
		public MinefieldShovel shovel;
		public MinefieldFlag flag;

		[HideInInspector]
		public int mine_count;
		[HideInInspector]
		public int minefield_size;
		[HideInInspector]
		public TileState[] tile_states;
		public static Minefield instance;

		public ComputeBuffer tile_states_gpu;

		private bool[] mine_states;
		private GameObject[] flags;
		private bool started;

		public ref TileState GetTile(Vector2Int pos) {
			return ref GetTile(pos.x, pos.y);
		}

		public ref TileState GetTile(int x, int y) {
			return ref tile_states[minefield_size * y + x];
		}

		private ref bool GetMine(Vector2Int pos) {
			return ref GetMine(pos.x, pos.y);
		}

		private ref bool GetMine(int x, int y) {
			return ref mine_states[minefield_size * y + x];
		}

		public void Reload() {
			minefield_size = minefield_size_input.amount;

			if (mine_count != mine_count_input.amount) {
				mine_count = mine_count_input.amount;
				mine_count_input.amount = Mathf.Min(mine_count_input.amount, minefield_size * minefield_size - 1);

				mine_count_input.UpdateAmount();
			}

			tile_states = new TileState[minefield_size * minefield_size];
			mine_states = new bool[minefield_size * minefield_size];
			flags = new GameObject[minefield_size * minefield_size];

			if (tile_states_gpu != null) {
				tile_states_gpu.Release();
				tile_states_gpu = null;
			}

			if (minefield_size > 0) {
				tile_states_gpu = new ComputeBuffer(minefield_size * minefield_size, sizeof(TileState));
				tile_states_gpu.SetData(tile_states);
			}
		}

		private void Die() {
			LoadingOverlay.OverlayAlpha = 1.0f;
			LoadingOverlay.OverlayTargetAlpha = 1.0f;
			AudioManager.PlayOneShotAttached("event:/robots/trip_mine", LocalAimHandler.player_instance.main_camera.gameObject, 5);
			AudioManager.Instance().LoudSound(LocalAimHandler.player_instance.main_camera.transform.position, LocalAimHandler.player_instance.main_camera.transform.forward);

			ReceiverCoreScript.Instance().FadeAndLoad("Minesweeper");
		}

		public void Generate(Vector2Int pos) {
			Vector2Int[] tiles = new Vector2Int[minefield_size * minefield_size - 1];

			for (int x = 0, i = 0; x < minefield_size; x++) {
				for (int y = 0; y < minefield_size; y++) {
					if (pos.x != x || pos.y != y) {
						tiles[i] = new Vector2Int(x, y);

						i++;
					}
				}
			}

			tiles.Shuffle();

			for (int i = 0; i < mine_count; i++) {
				GetMine(tiles[i]) = true;
			}
		}

		private void UncoverTile(Vector2Int pos) {
			if (GetMine(pos)) {
				Die();
				
				return;
			}

			int surrounding_mines = 0;

			for (int x = -1; x <= 1; x++) {
				if (pos.x + x < 0 || pos.x + x >= minefield_size) {
					continue;
				}

				for (int y = -1; y <= 1; y++) {
					if (pos.y + y < 0 || pos.y + y >= minefield_size) {
						continue;
					}

					if (GetMine(pos.x + x, pos.y + y)) {
						surrounding_mines++;
					}
				}
			}

			if (GetTile(pos) == TileState.Unchecked) {
				GetTile(pos) = (TileState) ((int) TileState.Checked_0 + surrounding_mines);

				if (GetTile(pos) != TileState.Checked_0) {
					return;
				}
			}
			else {
				int surrounding_flags = 0;

				for (int x = -1; x <= 1; x++) {
					if (pos.x + x < 0 || pos.x + x >= minefield_size) {
						continue;
					}

					for (int y = -1; y <= 1; y++) {
						if (pos.y + y < 0 || pos.y + y >= minefield_size) {
							continue;
						}

						if (GetTile(pos.x + x, pos.y + y) == TileState.Flagged) {
							surrounding_flags++;
						}
					}
				}

				int how_many_flags = (int) GetTile(pos) - (int) TileState.Checked_0;

				Debug.Log(how_many_flags + " < " + surrounding_flags);

				if (
					how_many_flags < 0
					||
					how_many_flags > surrounding_flags
				) {
					return;
				}
			}

			for (int x = -1; x <= 1; x++) {
				if (pos.x + x < 0 || pos.x + x >= minefield_size) {
					continue;
				}

				for (int y = -1; y <= 1; y++) {
					if (pos.y + y < 0 || pos.y + y >= minefield_size) {
						continue;
					}

					if (GetTile(pos.x + x, pos.y + y) == TileState.Unchecked) {
						UncoverTile(new Vector2Int(pos.x + x, pos.y + y));
					}
				}
			}
		}

		private Coroutine DropShit() {
			yield return new WaitForSeconds(8);
			
			shovel.gameObject.SetActive(true);

			yield return new WaitForSeconds(2);

			flag.gameObject.SetActive(true);
		}

		private Coroutine Win() {
			LocalAimHandler.player_instance.MoveInventoryItem(shovel, null);
			shovel.gameObject.SetActive(false);

			LocalAimHandler.player_instance.MoveInventoryItem(flag, null);
			flag.gameObject.SetActive(false);

			LoadingOverlay.OverlayTargetAlpha = 1.0f;
			
			yield return new WaitForSeconds(1);

			PlayerGUI.Instance.InfoList.QueueRows(new PlayerGUIInfoList.Row[] {
				new PlayerGUIInfoList.Row("Good job soldier :3", 0.1f, 4f)
			});

			AudioManager.PlayOneShot3D("event:/UI/unlock_trophy", LocalAimHandler.player_instance.main_camera.transform.position, 1, 0.5f);

			yield return new WaitForSeconds(4.5f);

			ReceiverCoreScript.Instance().FadeAndLoad("Minesweeper");
		}

		public void CheckWin() {
			int remaining_mines = this.mine_count;
			int flagged_mines = this.mine_count;

			for (int i = 0; i < tile_states.Length; i++) {
				if (this.tile_states[i] == TileState.Flagged || this.tile_states[i] == TileState.Unchecked) {
					remaining_mines--;
				}
				if (this.tile_states[i] == TileState.Flagged) {
					flagged_mines--;
				}
			}

			sky_minefield.GetComponentInChildren<TextMesh>().text = flagged_mines.ToString();

			if (remaining_mines == 0) {
				StartCoroutine(Win());
			}
		}

		public void Tick(Vector2Int pos) {
			if (pos.x < 0 || pos.x >= minefield_size || pos.y < 0 || pos.y >= minefield_size) {
				return;
			}

			if (!started) {
				Generate(pos);

				started = true;
				dramatic_entrance.SetActive(false);

				main_minefield.size = minefield_size * tile_size;
				main_minefield.root.position = new Vector3(LocalAimHandler.player_instance.transform.position.x, 0, LocalAimHandler.player_instance.transform.position.z);
				main_minefield.root.position -= new Vector3(pos.x + 0.5f, 0, pos.y + 0.5f) * tile_size;
				main_minefield.root.GetComponent<BoxCollider>().center = new Vector3(main_minefield.size / 2, main_minefield.size / 2, 0.5f);
				main_minefield.root.GetComponent<BoxCollider>().size = new Vector3(main_minefield.size, main_minefield.size, 1);

				sky_minefield.transform.position = main_minefield.root.position + Vector3.right * (main_minefield.size / 2 - 10);
				sky_minefield.transform.position += Vector3.forward * (50 + tile_size * minefield_size);
				sky_minefield.transform.position += Vector3.up * 15;

				StartCoroutine(DropShit());

				sky_minefield.GetComponentInChildren<TextMesh>().text = mine_count.ToString();

				PlayerGUI.Instance.InfoList.QueueRows(new PlayerGUIInfoList.Row[] {
					new PlayerGUIInfoList.Row("Go get 'em tiger o7", 0.1f, 4f)
				});
			}

			if (GetTile(pos) != TileState.Flagged && GetTile(pos) != TileState.Checked_0) {
				UncoverTile(pos);
			}

			CheckWin();

			tile_states_gpu.SetData(tile_states);
		}

		public void Flag(Vector2Int pos) {
			if (pos.x < 0 || pos.x >= minefield_size || pos.y < 0 || pos.y >= minefield_size) {
				return;
			}

			if (GetTile(pos) == TileState.Flagged) {
				GetTile(pos) = TileState.Unchecked;

				GameObject tile_flag = flags[pos.y * minefield_size + pos.x];

				if (tile_flag != null) {
					tile_flag.SetActive(false);
				}
			}
			else if (GetTile(pos) == TileState.Unchecked) {
				GetTile(pos) = TileState.Flagged;

				GameObject tile_flag = flags[pos.y * minefield_size + pos.x];

				if (tile_flag != null) {
					tile_flag.SetActive(true);
				}
				else {
					tile_flag = GameObject.Instantiate(this.flag, main_minefield.root.position + new Vector3((pos.x + 0.5f) * tile_size, -this.flag.ground_pos_offset.localPosition.y * 0.75f, (pos.y + 0.5f) * tile_size), Quaternion.identity).gameObject;
					tile_flag.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);

					flags[pos.y * minefield_size + pos.x] = tile_flag;
				}
			}

			CheckWin();

			tile_states_gpu.SetData(tile_states);
		}

		public void Awake() {
			instance = this;
		}

		public void Update() {
			if (LocalAimHandler.TryGetInstance(out var lah)) {
				if (lah.transform.position.y < -10) {
					lah.transform.position = this.transform.position;
				}

				if (lah.IsDead()) {
					ReceiverCoreScript.Instance().FadeAndLoad("Minesweeper");
				}
			}
		}
	}
}