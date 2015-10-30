using System;
using IO = System.IO;

// 2/7/03

namespace NBM.Plugin
{
	/// <summary>
	/// Provides a self-expanding circular buffer.
	/// </summary>
	public class CircularStream : IO.Stream
	{
		private const int defaultSize = 4092;
		private byte[] internalData;
		private int size;
		private int readPosition = 0, writePosition = 0;


		/// <summary>
		/// Number of bytes that are available to read
		/// </summary>
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


		/// <summary>
		/// 
		/// </summary>
		public override bool CanRead
		{
			get { return true; }
		}


		/// <summary>
		/// 
		/// </summary>
		public override bool CanWrite
		{
			get { return true; }
		}


		/// <summary>
		/// 
		/// </summary>
		public override bool CanSeek
		{
			get { return false; }
		}


		/// <summary>
		/// 
		/// </summary>
		public override long Length
		{
			get { return size - 1; }
		}


		/// <summary>
		/// Not supported - a circular buffer does not have any positions!
		/// </summary>
		public override long Position
		{
			get { throw new NotSupportedException(); }
			set { throw new NotSupportedException(); }
		}


		/// <summary>
		/// Constructs a circular buffer of default size
		/// </summary>
		public CircularStream()
			: this(defaultSize)
		{
		}


		/// <summary>
		/// Constructs a circular buffer of specified initial size
		/// </summary>
		/// <param name="size"></param>
		public CircularStream(int size)
		{
			this.size = size;
			this.internalData = new byte[ this.size + 1 ];
		}


		/// <summary>
		/// Does nothing
		/// </summary>
		public override void Flush()
		{
		}


		/// <summary>
		/// Not supported - cannot seek in a circular buffer
		/// </summary>
		/// <param name="offset"></param>
		/// <param name="origin"></param>
		/// <returns></returns>
		public override long Seek(long offset, IO.SeekOrigin origin)
		{
			throw new NotSupportedException();
		}


		/// <summary>
		/// Sets the length of the circular buffer. Will truncate if length is 
		/// shorter than the size of the buffer
		/// </summary>
		/// <param name="length"></param>
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
			int streamLength = Math.Min((int)length, rawData.Length);
			this.internalData = new byte[ length+1 ];
			Array.Copy(rawData, 0, this.internalData, 0, streamLength);

			// set read pointer to the position relative to where it was
			if (this.readPosition > this.writePosition)
				this.readPosition -= this.writePosition;
			else
				this.readPosition += bytesTillEnd;

			// if the new read position points at invalid data (if the stream was shrunk)
			// simply reset it
			if (this.readPosition > length)
				this.readPosition = 0;

			// set write pointer to the end of the freshly written data
			this.writePosition = streamLength;

			this.size = (int)length;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public int Read(byte[] data)
		{
			return Read(data, 0, data.Length);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="data"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		public int Read(byte[] data, int length)
		{
			return Read(data, 0, length);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="data"></param>
		/// <param name="offset"></param>
		/// <param name="length"></param>
		/// <returns></returns>
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
					}
					else
					{
						Array.Copy(this.internalData, this.readPosition, data, offset, length);
						amountRead = length;
					}

					this.readPosition = (this.readPosition + amountRead) % size;
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


		/// <summary>
		/// 
		/// </summary>
		/// <param name="data"></param>
		public void Write(byte[] data)
		{
			Write(data, 0, data.Length);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="data"></param>
		/// <param name="length"></param>
		public void Write(byte[] data, int length)
		{
			Write(data, 0, length);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="data"></param>
		/// <param name="offset"></param>
		/// <param name="length"></param>
		public override void Write(byte[] data, int offset, int length)
		{
			lock (this)
			{
				// test if the buffer needs to be resized to accomodate the written data
				int spaceAvailable = 0;

				if (this.readPosition <= this.writePosition)
					spaceAvailable = this.size - this.writePosition + this.readPosition - 1;
				else
					spaceAvailable = this.readPosition - this.writePosition - 1;

				if (spaceAvailable < length)
					this.SetLength(this.size - spaceAvailable + length);

				if (this.readPosition <= this.writePosition)
				{
					// read pointer is behind the write pointer, write till the end
					// of the buffer then go back to the start
					int bytesTillEnd = this.size - this.writePosition;

					if (bytesTillEnd < length)
					{
						// length to copy is greater than the size of the data until the end of the buffer,
						// so copy the remaining data until the end
						Array.Copy(data, offset, this.internalData, this.writePosition, bytesTillEnd);

						int bytesFromStart = Math.Min(this.readPosition - 1, length - bytesTillEnd);

						if (bytesFromStart > 0)
						{
							Array.Copy(data, offset + bytesTillEnd, this.internalData, 0, bytesFromStart);
							this.writePosition = bytesFromStart;
						}
						else
							this.writePosition = this.size - 1;
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
					int amountToCopy = Math.Min(this.readPosition - this.writePosition, length);
					Array.Copy(data, offset, this.internalData, this.writePosition, amountToCopy);
					this.writePosition = this.readPosition - 1;
				}
			}
		}
	}
}