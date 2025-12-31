using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.PlayerLoop;

namespace Minesweeper {
	public class InputStation : MonoBehaviour {
		public UnityEvent OnInput;
		public TextMesh amount_renderer;
		public string amount_format = "{0}";

		public int amount;

		public int min_amount = 0;
		public int max_amount = 30;

		private bool direct_input = false;

		public void UpdateAmount() {
			if (this.amount < this.min_amount) {
				this.amount = this.min_amount;
			}
			if (this.amount > this.max_amount) {
				this.amount = this.max_amount;

				direct_input = false;
			}

			this.amount_renderer.text = string.Format(amount_format, this.amount);

			OnInput.Invoke();
		}

		public void OnPressNumber(int num) {
			if (direct_input) {
				this.amount = this.amount * 10 + num;

				if (this.amount > this.max_amount) {
					this.amount = num;
				}
			}
			else {
				direct_input = true;

				this.amount = num;
			}

			UpdateAmount();
		}
		
		public void OnPressPlus() {
			this.amount++;

			this.direct_input = false;

			UpdateAmount();
		}

		public void OnPressMinus() {
			this.amount--;

			this.direct_input = false;

			UpdateAmount();
		}

		public void OnPressReturn() {
			this.direct_input = false;

			UpdateAmount();
		}

		public void Awake() {
			UpdateAmount();
		}
	}
}
