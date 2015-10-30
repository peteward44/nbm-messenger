using System;
using IO = System.IO;

// 2/7/03

namespace NBM.Plugin
{
	/// <summary>
	/// Summary description for CircularBuffer.
	/// </summary>
	public class CircularStream : IO.Stream
	{
		private const int defaultSize = 4092;
		private byte[] internalData;
		private int size;
		private int readPosition = 0, writePosition = 0;


		public int DataAvailable
		{
			get
			{
				if (this.readPosition == this.writePosition)
					return 0;
				else if (this.readPosition > this.writePosition)
					return this.size - this.readPosition + this.writePosition;
				else
					return this.writePosition - this.readPosition;
			}
		}


		public override bool CanRead
		{
			get { return true; }
		}

		public override bool CanWrite
		{
			get { return true; }
		}

		public override bool CanSeek
		{
			get { return false; }
		}


		public override long Length
		{
			get { return size - 1; }
		}

		public override long Position
		{
			get { throw new NotSupportedException(); }
			set { throw new NotSupportedException(); }
		}


		public CircularStream()
			: this(defaultSize)
		{
		}

		public CircularStream(int size)
		{
			this.size = size + 1;
			this.internalData = new byte[ this.size ];
		}


		public override void Flush()
		{
		}


		public override long Seek(long offset, IO.SeekOrigin origin)
		{
			throw new NotSupportedException();
		}


		public override void SetLength(long length)
		{
			int bytesTillEnd = this.size - this.writePosition;

			// copy data from the write pointer to the end into a new array
			byte[] rawData = new byte[ this.size ];
			Array.Copy(this.internalData, this.writePosition, rawData, 0, bytesTillEnd);

			// then append the rest of the data to the new array
			Array.Copy(this.internalData, 0, rawData, bytesTillEnd, this.writePosition);

			// now we have the data all in order, recreate the internal array and then
			// copy the new array back into it
			int copyLength = Math.Min((int)length, rawData.Length);
			this.internalData = new byte[ length+1 ];
			Array.Copy(rawData, 0, this.internalData, 0, copyLength);

			// set read pointer to the position relative to where it was
			if (this.readPosition > this.writePosition)
				this.readPosition -= this.writePosition + 1;
			else
				this.readPosition += bytesTillEnd;

			// if the new read position points at invalid data (if the stream was shrunk)
			// simply reset it
			if (this.readPosition >= length)
				this.readPosition = 0;

			// set write pointer to the end of the freshly written data
			this.writePosition = copyLength;

			this.size = (int)length + 1;
		}


		public override int Read(byte[] data, int offset, int length)
		{
			lock (this)
			{
				if (this.writePosition == this.readPosition)
					return 0; // empty buffer

				int amountRead = 0;

				if (this.writePosition < this.readPosition)
				{
					// write pointer is behind the read pointer, then read up until the end of
					// the buffer and read from the start till the write p
					int bytesTillEnd = this.size - this.readPosition;

					if (bytesTillEnd < length)
					{
						int bytesFromStart = Math.Min(this.writePosition, length - bytesTillEnd);
						Array.Copy(this.internalData, this.readPosition, data, offset, bytesTillEnd);
						Array.Copy(this.internalData, 0, data, offset + bytesTillEnd, bytesFromStart);
						amountRead = bytesTillEnd + bytesFromStart;
						this.readPosition = bytesFromStart;
					}
					else
					{
						Array.Copy(this.internalData, this.readPosition, data, offset, length);
						amountRead = length;
						this.readPosition += amountRead;
					}
				}
				else
				{
					// write pointer is ahead of the read pointer, just read up until then
					int amountToCopy = Math.Min(this.writePosition - this.readPosition, length);
					Array.Copy(this.internalData, this.readPosition, data, offset, amountToCopy);
					amountRead = amountToCopy;
					this.readPosition += amountToCopy;
				}

				return amountRead;
			}
		}


		public override void Write(byte[] data, int offset, int length)
		{
			lock (this)
			{
				// test if the buffer needs to be resized to accomodate the written data
				int spaceAvailable = 0;

				if (this.readPosition == this.writePosition)
					spaceAvailable = this.size - 1;
				else if (this.readPosition == this.writePosition + 1)
					spaceAvailable = 0;
				else if (this.readPosition < this.writePosition)
					spaceAvailable = this.size - this.writePosition + this.readPosition;
				else
					spaceAvailable = this.readPosition - this.writePosition;

				if (spaceAvailable < length)
					this.SetLength(this.size - spaceAvailable + length - 1);

				if (this.readPosition <= this.writePosition)
				{
					// read pointer is behind the write pointer, write till the end
					// of the buffer then go back to the start
					int bytesTillEnd = this.size - this.writePosition;

					if (bytesTillEnd - 1 < length)
					{
						// length to copy is greater than the size of the data until the end of the buffer,
						// so copy the remaining data until the end
						Array.Copy(data, offset, this.internalData, this.writePosition, bytesTillEnd);

						int bytesFromStart = Math.Min(this.readPosition-1, length - bytesTillEnd);

						if (bytesFromStart > 0)
						{
							Array.Copy(data, offset + bytesTillEnd, this.internalData, 0, bytesFromStart);
							this.writePosition = bytesFromStart;
						}
						else
							this.writePosition = this.size;
					}
					else
					{
						// length is not greater than the size of the data until the end, so
						// just copy it over
						Array.Copy(data, offset, this.internalData, this.writePosition, length);
						this.writePosition += length;
					}
				}
				else
				{
					// write pointer is behind the read pointer, simply write up until then
					int amountToCopy = Math.Min(this.readPosition - this.writePosition - 1, length);
					Array.Copy(data, offset, this.internalData, this.writePosition, amountToCopy);
					this.writePosition += amountToCopy;
				}
			}
		}
	}
}