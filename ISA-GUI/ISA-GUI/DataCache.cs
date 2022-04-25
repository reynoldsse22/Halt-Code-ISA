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

		//Sets up the association for the cache
		private int association = 4;
		private int memoryKicked = 0;
		private Random rand;

		public int tag, index, offset, cacheLines;

		//Allows for configuration
		public int offsetBitAmount { get; set; }
		public int indexBitAmount { get; set; }
		//Number of words allowed in the cache
		public int numberOfWords { get; set; }
		public int offsetMask;
		public int indexMask;
		public int indexSize;

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

			rand = new Random();
			offsetBitAmount = 3;
			indexBitAmount = 2;
			indexSize = (int)Math.Pow(2, indexBitAmount);

			cacheLines = 16; 
			numberOfWords = 10;
			//This should be configurable in the future to allow 2/4 way association
			offsetMask = (int)Math.Pow(2, offsetBitAmount) - 1;
			indexMask = (int)Math.Pow(2, indexBitAmount) - 1;

			l1Cache = new byte[cacheLines][];
			tagIndexCache = new int[cacheLines];
			//Setting up the cache to hold memory
			for(int x = 0; x < l1Cache.Length; x++)
            {
				tagIndexCache[x] = 0;
            }
		}

		/// <summary>Updates the cache.</summary>
		/// <param name="address">The address.</param>
		/// <param name="memory">The main memory.</param>
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

			l1Cache[index + indexSize * memoryKicked] = mem;
			tagIndexCache[index + indexSize * memoryKicked] = tag;
		}

        /// <summary>Updates the cache with write instructions.</summary>
        /// <param name="result">The result.</param>
        public void updateWriteCache(int result)
        {
			l1Cache[index][offset] = (byte)((result & 16711680) >> 16);     //Stores the MSB value of r0 at the address in cache
			l1Cache[index][offset + 1] = (byte)((result & 65280) >> 8);     //Stores the TSB value of r0 at the address in cache
			l1Cache[index][offset + 2] = (byte)(result & 255);				//Stores the LSB value of r0 at the address in cache
		}

        /// <summary>Updates the cache with the write results for float instructions.</summary>
        /// <param name="floatResult">The float result.</param>
        public void updateWriteCache(byte[] floatResult)
        {
			l1Cache[index][offset] = floatResult[3];                                           //Stores the MSB value of f0 at the address in cache
			l1Cache[index][offset + 1] = floatResult[2];                                       //Stores the TSB value of f0 at the address in cache
			l1Cache[index][offset + 2] = floatResult[1];                                       //Stores the LSB value of f0 at the address in cache
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

        /// <summary>Finds whether the instruction hit or missed in the cache.</summary>
        /// <param name="instruction">The instruction.</param>
        public void findInstructionInCache(ref Instruction instruction)
        {
			//finds the offset for the types of association
			int assocOffset = cacheLines / association;
			for(int x = index; x < l1Cache.Length; x = x + assocOffset)
            {
				if (l1Cache[x] == null)
				{
					index = x;
					instruction.hitOrMiss = Instruction.cacheHit.MISS;
					memoryKicked = 0;
					return;
				}
				else if (tagIndexCache[x] != tag)
				{
					instruction.hitOrMiss = Instruction.cacheHit.CONF;
				}
				else
				{
					index = x;
					instruction.hitOrMiss = Instruction.cacheHit.HIT;
					memoryKicked = 0;
					return;
				}
			}
			findRandomOffset();
		}

		public void clearCache()
        {
			for (int x = 0; x < l1Cache.Length; x++)
			{
				l1Cache[x] = null;
				tagIndexCache[x] = 0;
			}
			memoryKicked = 0;
		}

		public void findRandomOffset()
        {
            switch (association)
            {
				case 1:
					memoryKicked = 0;
					break;
				case 2:
				case 4:
					memoryKicked = rand.Next(association);
					break;
			}
        }
		/*
		public void buildCacheDataString(ref StringBuilder cacheString, Instruction instruction)
		{
			int address = instruction.address;
			string addressS = address.ToString();
			offset = address & offsetMask;
			address = address >> offsetBitAmount;
			index = address & indexMask;
			address = address >> indexBitAmount;
			tag = address;
			//string addressS = address.ToString();
			string offsets = offset.ToString();
			string indexs =index.ToString();
			string tagS = tag.ToString();
			string hitOrMiss = instruction.hitOrMiss.ToString();

			string output = (string.Format("\n{0, 7} {1,13} {2, 7} {3, 8} {4, 8}",
						   addressS.PadRight(7), offsets.PadRight(13), indexs.PadLeft(7), tagS.PadLeft(8),hitOrMiss.PadLeft(7)));

			cacheString.Append(output);
			
		}
		*/
	}

}
