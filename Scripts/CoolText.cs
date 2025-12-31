using Receiver2;
using UnityEngine;
using Coroutine = System.Collections.IEnumerator;

namespace Minesweeper {
	public class CoolText : MonoBehaviour {
		public TextMesh text_renderer;
		[TextArea]
		public string text;
		public float delay;
		public bool one_shot = false;

		private bool displaying;
		private Collider trigger_area;

		public void Awake() {
			this.trigger_area = GetComponent<Collider>();
			text_renderer.text = "";
		}

		private Coroutine DisplayText() {
			yield return new WaitForSeconds(delay);

			while (this.text_renderer.text.Length < this.text.Length) {
				if (!displaying) {
					yield break;
				}

				char char_to_add = this.text[this.text_renderer.text.Length];

				if (char_to_add == '>') {
					this.text_renderer.text += " ";

					yield return new WaitForSeconds(1.5f);
				}
				else {
					this.text_renderer.text += this.text[this.text_renderer.text.Length];

					yield return new WaitForSeconds(0.1f);
				}
			}
		}

		public void Update() {
			if (LocalAimHandler.TryGetInstance(out var lah)) {
				if (!this.displaying && trigger_area.ClosestPoint(lah.transform.position) == lah.transform.position) {
					displaying = true;
					StartCoroutine(DisplayText());
				}
				else if (this.displaying && trigger_area.ClosestPoint(lah.transform.position) != lah.transform.position) {
					displaying = false;
					this.text_renderer.text = "";

					if (one_shot) {
						this.enabled = false;
					}
				}
			}
		}
	}
}