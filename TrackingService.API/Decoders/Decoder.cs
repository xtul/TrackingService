using System;
using System.Buffers;
using System.IO;
using System.IO.Pipelines;
using System.Text;
using System.Threading.Tasks;
using TrackingService.API.Cache;
using TrackingService.Model.Objects;

namespace TrackingService.API.Decoders {
	/// <summary>
	/// All decoders should inherit from this class.
	/// Defines common behavior, allowing inheriting classes to just focus on decoding.
	/// </summary>
	public abstract class Decoder {
		/// <summary>Message received from device.</summary>
		private string Data;
		private IDuplexPipe Pipe;

		/// <summary>
		/// Receives the connection and handles it according to implementation.
		/// </summary>
		public async Task ReceiveAsync(IDuplexPipe transport) {
			Pipe = transport;

			while (true) {
				var result = await Pipe.Input.ReadAsync();
				var buffer = result.Buffer;

				// stringify message, decode it and send to cache
				Data = Encoding.ASCII.GetString(buffer);
				var position = await DecodeAsync(Data);
				if (position is not null) {
					Startup.PositionCache.Add(position);
				}

				if (result.IsCompleted) {
					break;
				}

				transport.Input.AdvanceTo(buffer.End);
			}
		}

		/// <summary>
		/// Decodes the position, which is later sent to cache.
		/// </summary>
		/// <param name="data">Data to decode.</param>
		/// <remarks>
		///	Use <see cref="Respond(string)"/> 
		///	to send a response to device.
		/// </remarks>
		/// <returns>
		///	A decoded position.
		/// </returns>
		public abstract Task<Position> DecodeAsync(string data);

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
			if (Pipe is not null)
			foreach (var segment in buffer) {
				await Pipe.Output.WriteAsync(segment);
			}
		}
	}
}