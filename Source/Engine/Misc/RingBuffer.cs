using System;

namespace TangentEngine {
	public class RingBuffer<T> {
		private T[] data;
		private int index;
		private int size;

		public RingBuffer(int initialSize) {
			this.size = initialSize;
			this.data = new T[this.size];
			this.index = 0;
		}

		public void SetSize(int newSize) {
			this.size = newSize;
			this.data = new T[this.size];
			this.SetIndex(this.index);
		}

		public void SetIndex(int newIndex) {
			this.index = Helpers.MathH.Mod(newIndex, this.size);
		}

		public void PushData(T data) {
			this.data[this.index] = data;
			this.index = Helpers.MathH.Mod(this.index + 1, this.size);
		}

		public void PushData(T[] data) {
			for (int i = 0; i < data.Length; i++) {
				this.PushData(data[i]);
			}
		}

		public T PopData() {
			T data = this.data[this.index];
			this.index = Helpers.MathH.Mod(this.index - 1, this.size);
			return data;
		}

		public Span<T> PopData(int count) {
			this.SetIndex(this.index - count);
			int end = Helpers.MathH.Mod(this.index + count, this.size);
			return this.data.AsSpan(new Range(new Index(this.index), new Index(end)));
		}

		public T GetData() {
			return this.data[this.index];
		}

		public T[] GetAllData() {
			return this.data;
		}

		public Span<T> GetRangeData(int start, int end) {
			return this.data.AsSpan(new Range(new Index(start), new Index(end)));
		}
	}
}