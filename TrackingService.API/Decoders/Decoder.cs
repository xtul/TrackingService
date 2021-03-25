using System.Buffers;
using System.IO;
using System.IO.Pipelines;
using System.Text;
using System.Threading.Tasks;

namespace TrackingService.API.Decoders {
	public abstract class Decoder {
		protected string Data;
		protected IDuplexPipe Pipe;

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

		protected abstract Task DecodeAsync();

		protected async Task Respond(string data) {
			var buffer = Encoding.ASCII.GetBytes(data);
			await Respond(buffer);
		}

		protected async Task Respond(byte[] buffer) {
			await Respond(new ReadOnlySequence<byte>(buffer));
		}

		protected async Task Respond(ReadOnlySequence<byte> buffer) {
			foreach (var segment in buffer) {
				await Pipe.Output.WriteAsync(segment);
			}
		}
	}
}