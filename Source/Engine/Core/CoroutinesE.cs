using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace TangentEngine {
	public class CoroutinesE {
		private List<Coroutine> coroutines;
		private Thread thread;
		private bool threadRunning;
		private static EventWaitHandle waitHandle = new ManualResetEvent(false);

		public CoroutinesE() {
			this.coroutines = new List<Coroutine>();
			this.thread = new Thread(this.UpdateThread);
			this.thread.Name = "CoRoutines";
			this.thread.IsBackground = true;
		}

		public void AddCoroutine(Coroutine coroutine) {
			this.coroutines.Add(coroutine);
		}

		public void RemoveCoroutine(Coroutine coroutine) {
			this.coroutines.Remove(coroutine);
		}

		public void Update() {
			for (int i = this.coroutines.Count - 1; i >= 0; i--) {
				this.coroutines[i].Tick();
				if (this.coroutines[i].finished) this.RemoveCoroutine(this.coroutines[i]);
			}
		}

		private void UpdateThread() {
			this.threadRunning = true;
			while (this.threadRunning) {
				waitHandle.WaitOne();
				this.Update();
			}
		}

		public void StartAsyncUpdate() {
			this.thread.Start();
		}

		public void StopAsyncUpdate() {
			this.threadRunning = false;
		}

		public void ResumeAsyncUpdate() {
			if (this.coroutines.Count > 0) {
				waitHandle.Set();
			}
		}

		public void PauseAsyncUpdate() {
			waitHandle.Reset();
		}

		public void Reset() {
			this.coroutines.Clear();
		}
	}

	public class Coroutine {
		public bool finished;
		private Action? onFinished;
		private IEnumerator routine;

		public Coroutine(IEnumerator routine, Action action = null) {
			this.routine = routine;
			this.finished = false;
			this.onFinished = action;
		}

		public void Tick() {
			if (!this.routine.MoveNext() && !this.finished) this.Finish();
		}

		public void Reset(IEnumerator routine) {
			this.finished = false;
			this.routine = routine;
		}

		public void Cancel() {
			this.finished = true;
		}

		public void Finish() {
			this.finished = true;
			this.onFinished?.Invoke();
		}

		public static IEnumerator WaitGameSeconds(float seconds) {
			double stopAt = Engine.time.elapsedGameTime + seconds;
			while (Engine.time.elapsedGameTime <= stopAt) {
				yield return null;
			}
		}

		public static IEnumerator WaitSeconds(float seconds) {
			double stopAt = Engine.time.elapsedRealTime + seconds;
			while (Engine.time.elapsedRealTime <= stopAt) {
				yield return null;
			}
		}

		public static IEnumerator WaitFrames(int frames) {
			double stopAt = Engine.time.elapsedFrames + frames;
			while (Engine.time.elapsedFrames <= stopAt) {
				yield return null;
			}
		}

		public static IEnumerator AfterCondition(Func<bool> after) {
			if (after.Invoke()) {
				yield return null;
			}
		}
	}
}