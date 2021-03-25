using System.Buffers;
using System.IO;
using System.IO.Pipelines;
using System.Text;
using System.Threading.Tasks;

namespace TrackingService.API.Decoders {
	/// <summary>
	/// All decoders should inherit from this class.
	/// Defines common behavior, allowing inheriting classes to just focus on decoding.
	/// </summary>
	public abstract class Decoder {
		/// <summary>Message received from device.</summary>
		protected string Data;
		private IDuplexPipe Pipe;

		/// <summary>
		/// Receives the connection and handles it according to implementation.
		/// </summary>
		public async Task ReceiveAsync(IDuplexPipe transport) {
			Pipe = transport;

			while (true) {
				var result = await Pipe.Input.ReadAsync();
				var buffer = result.Buffer;

				Data = Encoding.ASCII.GetString(buffer);
				await DecodeAsync();

				if (result.IsCompleted) {
					break;
				}

				transport.Input.AdvanceTo(buffer.End);
			}
		}

		/// <summary>
		/// Decodes the message and sends the decoded message to cache.
		/// </summary>
		/// <remarks>
		///	The message is stored in Data property. Use <see cref="Respond(string)"/> 
		///	to send a response to device.
		/// </remarks>
		protected abstract Task DecodeAsync();

		/// <summary>
		/// Sends a <paramref name="message"/> to connected device.
		/// </summary>
		protected async Task Respond(string message) {
			var buffer = Encoding.ASCII.GetBytes(message);
			await Respond(buffer);
		}

		/// <summary>
		/// Sends a <paramref name="buffer"/> to connected device.
		/// </summary>
		protected async Task Respond(byte[] buffer) {
			await Respond(new ReadOnlySequence<byte>(buffer));
		}

		/// <summary>
		/// Sends a <paramref name="buffer"/> to connected device.
		/// </summary>
		protected async Task Respond(ReadOnlySequence<byte> buffer) {
			foreach (var segment in buffer) {
				await Pipe.Output.WriteAsync(segment);
			}
		}
	}
}