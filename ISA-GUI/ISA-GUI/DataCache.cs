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

			l1Cache = new byte[16][];
			tagIndexCache = new int[16];
			//Setting up the cache to hold memory
			for(int x = 0; x < l1Cache.Length; x++)
            {
				tagIndexCache[x] = 0;
            }
		}
	}
}
