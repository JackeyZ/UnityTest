using UnityEngine;
using System.Collections;

namespace Whisper {

	/// <summary>
	/// Whisper Pool Exception. Special exception class used by the (Whisper) Object Pool
	/// </summary>
	public class WhisperPoolException : System.Exception {

		/// <summary>
		/// Initializes a new instance of the <see cref="Whisper.WhisperPoolException"/> class.
		/// </summary>
		public WhisperPoolException() : base() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="Whisper.WhisperPoolException"/> class.
		/// </summary>
		/// <param name="message">Message.</param>
		public WhisperPoolException(string message) : base(message) {}

		/// <summary>
		/// Initializes a new instance of the <see cref="Whisper.WhisperPoolException"/> class.
		/// </summary>
		/// <param name="message">Message.</param>
		/// <param name="inner">Inner Exception.</param>
		public WhisperPoolException(string message, System.Exception inner) : base(message, inner) {}
	}
}
	