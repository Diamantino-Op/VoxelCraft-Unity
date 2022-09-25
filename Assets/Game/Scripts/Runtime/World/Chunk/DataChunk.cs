using VoxelCraft.World.Block;
using UnityEngine;

namespace VoxelCraft.World.Chunk
{
	public class DataChunk
	{
		[SerializeField]
		public readonly ChunkPos _pos;
		private Chunk _chunk;
		[SerializeField]
		public Block.Block[,,] _blocks;
		[SerializeField]
		public DataColumn _column;

		private bool _generated;
		private int _density;

		public bool hasChanged = false;

		[SerializeField]
		public string dataVersion = "1.0.0";

		/// <summary>
		/// Create a new <c>DataChunk</c>.
		/// </summary>
		/// <param name="pos">Chunk position.</param>
		/// <param name="chunk">Chunk GameObject.</param>
		/// <param name="column">Column data.</param>
		public DataChunk(ChunkPos pos, Chunk chunk, DataColumn column)
		{
			_pos = pos;
			_chunk = chunk;
			_blocks = new Block.Block[World.chunkSize, World.chunkSize, World.chunkSize];
			_column = column;

			_generated = false;
			_density = 0;
		}

		public DataChunk(ChunkPos pos, Block.Block[,,] blocks, DataColumn column, Chunk chunk, string dataVersion)
		{
			_pos = pos;
			_chunk = chunk;
			_blocks = blocks;
			_column = column;
			this.dataVersion = dataVersion;

			_generated = false;
			_density = 0;
		}

		/// <summary>
		/// Tell DataChunk to generate its blocks.
		/// </summary>
		public void GenerateBlocks()
		{
			for (int x = 0; x < World.chunkSize; ++x)
			{
				for (int y = 0; y < World.chunkSize; ++y)
				{
					for (int z = 0; z < World.chunkSize; ++z)
					{
						Block.Block block = _chunk.world.GenerateBlock(new BlockPos(x, y, z, _pos));

						// Skip air
						if (block == _chunk.world.gameManager.blockManager.getBlockByName("Air"))
						{
							continue;
						}

						block.pos = new BlockPos(x, y, z, _pos);

						_blocks[x, y, z] = block;

						++_density;
					}
				}
			}

			if (_density == 0)
			{
				_blocks = null;
			}

			_generated = true;
			_chunk.UpdateState();
		}

		/// <summary>
		/// Set block at given position.
		/// </summary>
		/// <param name="block">Block ID.</param>
		/// <param name="pos">Block position.</param>
		/// <remarks>To set <c>Atlas.ID.Air</c>, you need to use <see cref="DataChunk.RemoveBlock(BlockPos)"/></remarks>
		public void SetBlock(Block.Block block, BlockPos pos)
		{
			// Do not give us air!
			if (block == _chunk.world.gameManager.blockManager.getBlockByName("Air")) { return; }
			// Something is amiss, it should be our chunk pos
			if (pos.chunkPos != _pos) { return; }

			// Unnullify
			if (_blocks == null)
			{
				_blocks = new Block.Block[World.chunkSize, World.chunkSize, World.chunkSize];
			}

			_blocks[pos.x, pos.y, pos.z] = block;

			++_density;
		}

		/// <summary>
		/// Remove block at given position.
		/// </summary>
		/// <param name="pos">Block position.</param>
		public void RemoveBlock(BlockPos pos)
		{
			// Something is amiss, it should be our chunk pos
			if (pos.chunkPos != _pos) { return; }
			// Already empty!
			if (_blocks == null || _blocks[pos.x, pos.y, pos.z] == _chunk.world.gameManager.blockManager.getBlockByName("Air")) { return; }

			_blocks[pos.x, pos.y, pos.z] = _chunk.world.gameManager.blockManager.getBlockByName("Air");

			// Check for nullification
			if (--_density == 0)
			{
				_blocks = null;
			}
		}

		/// <summary>
		/// Get block at given position.
		/// </summary>
		/// <param name="pos">Block position.</param>
		/// <returns>Block ID at given position.</returns>
		public Block.Block GetBlock(BlockPos pos)
		{
			// Empty!
			if (_blocks == null)
			{
				return _chunk.world.gameManager.blockManager.getBlockByName("Air");
			}
			// Something is amiss, it should be our chunk pos
			if (pos.chunkPos != _pos) { return _chunk.world.gameManager.blockManager.getBlockByName("Air"); }

			if (_blocks[pos.x, pos.y, pos.z] == null) { return _chunk.world.gameManager.blockManager.getBlockByName("Air"); }

			return _blocks[pos.x, pos.y, pos.z];
		}

		/// <summary>
		/// Assign <c>Chunk</c> script.
		/// </summary>
		/// <param name="chunk"><c>Chunk</c> script.</param>
		public void SetChunk(Chunk chunk)
		{
			_chunk = chunk;
		}

		/// <summary>
		/// Get <c>Chunk</c> script.
		/// </summary>
		/// <returns><c>Chunk</c> script.</returns>
		public Chunk GetChunk()
		{
			return _chunk;
		}

		/// <summary>
		/// Get column data.
		/// </summary>
		/// <returns>Column data.</returns>
		public DataColumn GetColumn()
		{
			return _column;
		}

		/// <summary>
		/// Checks whether the data chunk's blocks are generated.
		/// </summary>
		/// <returns>State of generation.</returns>
		public bool IsGenerated()
		{
			return _generated;
		}

		/// <summary>
		/// Checks whether chunk is empty.
		/// </summary>
		/// <returns>State of emptiness.</returns>
		public bool IsEmpty()
		{
			return (_density == 0);
		}
	}
}