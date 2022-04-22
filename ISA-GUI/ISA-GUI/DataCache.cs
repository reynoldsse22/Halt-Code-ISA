// ---------------------------------------------------------------------------
// File name: DataCache.cs
// Project name: ISA-GUI
// ---------------------------------------------------------------------------
// Creators: Samuel Reynolds, Nick Farmer, Carlos Ortiz, & Brandon Beaudry						
// Course-Section: CSCI 4717-201
// Creation Date: 4/19/22		
// ---------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISA_GUI
{
	/**
	* Class Name: DataCache <br>
	* Class Purpose: Holds Main Memory
	* 
	* <hr>
	* Date created: 4/19/21 <br>
	* @author Samuel Reynolds
	*/
	internal class DataCache
	{
		public byte[][] l1Cache;
		//Used to check if the tags are the same from addresses
		public int[] tagIndexCache;
		//Number of bytes that are going to be in a block
		public int cacheBlock = 8;

		public int tag, index, offset;

		//Allows for configuration
		public int offsetBitAmount { get; set; }
		public int indexBitAmount { get; set; }
		//Number of words allowed in the cache
		public int numberOfWords { get; set; }
		public int offsetMask;
		public int indexMask;

		/**
	    * Method Name: DataCache <br>
	    * Method Purpose: Class constructor
	    * 
	    * <hr>
	    * Date created: 4/19/21 <br>
	    * @author Samuel Reynolds
	    */
		public DataCache()
		{
			tag = 0;
			index = 0;
			offset = 0;

			offsetBitAmount = 3;
			indexBitAmount = 4;

			numberOfWords = 10;
			//This should be configurable in the future to allow 2/4 way association
			offsetMask = (int)Math.Pow(2, offsetBitAmount) - 1;
			indexMask = (int)Math.Pow(2, indexBitAmount) - 1;

			l1Cache = new byte[16][];
			tagIndexCache = new int[16];
			//Setting up the cache to hold memory
			for(int x = 0; x < l1Cache.Length; x++)
            {
				tagIndexCache[x] = 0;
            }
		}

		/// <summary>Updates the cache.</summary>
		/// <param name="index">The index.</param>
		/// <param name="address">The address.</param>
		/// <param name="memory">The main memory.</param>
		/// <param name="dC">
		///   <para>
		/// The cache.
		/// </para>
		/// </param>
		/// <exception cref="System.NotImplementedException"></exception>
		public void updateCache(int address, ref DataMemory memory)
		{
			//Adds new byte of memory into the cache
			byte[] mem = new byte[numberOfWords];

			for (int x = 0; x < numberOfWords; x++)
			{
				mem[x] = (byte)memory.MainMemory[address + x];
			}

			l1Cache[index] = mem;
			tagIndexCache[index] = tag;
		}

		/// <summary>
		///   <para>
		/// Finds the cache variables in order to check if the address is in the cache.
		/// </para>
		/// </summary>
		/// <param name="inst">The instruction.</param>
		/// <param name="offset">The offset.</param>
		/// <param name="index">The index.</param>
		/// <param name="tag">The tag.</param>
		public void findCacheVariables(Instruction inst)
		{
			int address = inst.address;
			offset = address & offsetMask;
			address = address >> offsetBitAmount;
			index = address & indexMask;
			address = address >> indexBitAmount;
			tag = address;
		}
	}
}
