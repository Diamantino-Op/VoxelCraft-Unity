using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Priority_Queue;
using System.Collections;
using VoxelCraft.World.Chunk;
using VoxelCraft.World.Block;
using VoxelCraft.IO;
using System.IO;
using Mirror;

namespace VoxelCraft.World
{
	public class World : NetworkBehaviour
	{
		private string worldPath;

        [HideInInspector]
		public GameManager gameManager;

		public WorldSettings worldSettings;

		[SerializeField] private GameObject _chunkPrefab;
		private Dictionary<ChunkPos, DataChunk> _chunks = new Dictionary<ChunkPos, DataChunk>();
		private Dictionary<ColumnPos, DataColumn> _columns = new Dictionary<ColumnPos, DataColumn>();
		private Dictionary<ChunkPos, DataChunk> _offloadChunks = new Dictionary<ChunkPos, DataChunk>();
		private SimplePriorityQueue<Chunk.Chunk> _loadQueue = new SimplePriorityQueue<Chunk.Chunk>();
		private List<Dimension.Dimension> dimensions = new List<Dimension.Dimension>();
		private bool _rendering;

		/// <summary>The size (width, breadth, and height) of chunks.</summary>
		public const int chunkSize = 16;
		[SerializeField] private int _viewRangeHorizontal = 3;
		[SerializeField] private int _viewRangeVertical = 3;
		private static ChunkPos _playerPos;

		void Start()
		{
			gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
			
			if (isServer)
            {
				worldPath = string.Concat(Application.persistentDataPath, "/Worlds/", gameManager.worldName, "/");

				worldSettings = new WorldSettings(worldPath);

				StartCoroutine(SaveChunks());
			}

			if (isClient)
            {
				_playerPos = new ChunkPos(Camera.main.transform.position / chunkSize);
				CmdGenerateChunks();
			}
		}

		void Update()
		{
			if (isClient)
			{
				ChunkPos temp = new ChunkPos(Camera.main.transform.position / chunkSize);

				// Did player move from their chunk?
				if (ChunkPos.CubeDistance(temp, _playerPos) > 0)
				{
					_playerPos = temp; // Set new pos
					CmdRenderPlayerChunks();
				}
			}
		}

        [Command(requiresAuthority = false)]
		public void CmdRenderPlayerChunks()
        {
			CmdGenerateChunks(); // Generate new chunks
			CmdPingChunks(); // Ping old chunks for deletion
		}

		public void OnDestroy()
		{
			if (isServer)
				SaveGame.SaveWorld(_chunks, worldPath);
		}

        [Server]
		IEnumerator SaveChunks()
		{
			foreach (KeyValuePair<ChunkPos, DataChunk> chunk in _chunks)
			{
				if (chunk.Value.hasChanged)
				{
					SaveGame.SaveChunk(chunk.Value, worldPath);

					chunk.Value.hasChanged = false;
				}

				yield return null;
			}
		}

        [Server]
		private void RenderThread()
		{
			while (_loadQueue.Count > 0)
			{
				Chunk.Chunk newChunkScript = _loadQueue.Dequeue();

				if (newChunkScript != null)
				{
					// Errors in threads need to be manually caught and sent to the main thread
					try
					{
						newChunkScript.GenerateBlocks();
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
					}
				}
			}

			_rendering = false;
		}

		/// <summary>
		/// Generates all possible chunk positions that are in view range if a chunk does not already exist at that position.
		/// </summary>
		[Command(requiresAuthority = false)]
		private void CmdGenerateChunks()
		{
			// Which direction is the player pointing in?
			Vector3 pov = Camera.main.transform.rotation * Vector3.forward;
			pov.y = 0; // Flatten it as we want it to be horizontal

			// Iterate through x, y, z
			for (int x = _playerPos.x - _viewRangeHorizontal - 1; x <= _playerPos.x + _viewRangeHorizontal + 1; ++x)
			{
				for (int z = _playerPos.z - _viewRangeHorizontal - 1; z <= _playerPos.z + _viewRangeHorizontal + 1; ++z)
				{
					ColumnPos grid = new ColumnPos(x, z);

					DataColumn newDataColumn;

					// Does column exist?
					if (!_columns.ContainsKey(grid))
					{
						// Create new data column
						newDataColumn = new DataColumn(grid);

						// Store in map
						_columns[grid] = newDataColumn;
					}
					else
					{
						newDataColumn = _columns[grid];
					}

					for (int y = calculateChunkY(_playerPos.y - _viewRangeVertical - 1); y <= _playerPos.y + _viewRangeVertical + 1; ++y)
					{
						ChunkPos pos = new ChunkPos(x, y, z);

						// Does chunk exist?
						if (!_chunks.ContainsKey(pos))// && ChunkPos.Distance(pos, _playerPos) <= _viewRangeHorizontal)
						{
							// Create new chunk and get corresponding script
							GameObject newChunk = Instantiate(_chunkPrefab, new Vector3(x * chunkSize, y * chunkSize, z * chunkSize), Quaternion.identity);

							newChunk.name = "Chunk_" + x + "_" + y + "_" + z;
							newChunk.transform.SetParent(transform);

							Chunk.Chunk newChunkScript = newChunk.GetComponent<Chunk.Chunk>();

							DataChunk newDataChunk;

							if (_offloadChunks.ContainsKey(pos))
							{
								// Retrieve from offload
								newDataChunk = _offloadChunks[pos];

								// Give data chunk gameobject
								newDataChunk.SetChunk(newChunkScript);

								// Remove from offload
								_offloadChunks.Remove(pos);
							}
							else if (SaveGame.ChunkSaveExists(pos, worldPath))
							{
								newDataChunk = SaveGame.LoadChunkData(pos, worldPath, newChunkScript);
							}
							else
							{
								// Create new data chunk
								newDataChunk = new DataChunk(pos, newChunkScript, newDataColumn);
							}

							newChunkScript.world = this;

							// Let chunk know its corresponding data chunk and position
							newChunkScript.LoadData(pos, newDataChunk);

							// Should chunk render yet?
							//newChunkScript.SetRender(CubeDistance(_playerPos, pos) <= _viewRangeHorizontal);

							// Get angle difference between vectors
							Vector3 dir = pos * chunkSize - Camera.main.transform.position;
							float dist = dir.magnitude;
							float diff = Vector3.Angle(pov, dir);
							float final = dist + diff;
							if (dist < chunkSize * 2f) // Prioritize chunks immediately closest
							{
								final = dist;
							}

							// Queue chunk for generation
							_loadQueue.Enqueue(newChunkScript, final);

							// Store in map
							_chunks[pos] = newDataChunk;

							NetworkServer.Spawn(newChunk);
						}
					}
				}
			}

			// Are there chunks that need generation?
			if (!_rendering && _loadQueue.Count > 0)
			{
				_rendering = true;
				new Thread(RenderThread).Start();
			}
		}

		public int calculateChunkY(int y)
        {
			if (y >= 0)
				return y;
			else
				return 0;
        }

		/// <summary>
		/// Checks whether chunks are still in view range or not, and destroys them if need be.
		/// </summary>
		[Command(requiresAuthority = false)]
		private void CmdPingChunks()
		{
			List<ChunkPos> temp = new List<ChunkPos>();

			// Collect all chunks that need to be deleted
			foreach (KeyValuePair<ChunkPos, DataChunk> pair in _chunks)
			{
				if (ChunkPos.CubeDistance(pair.Key, _playerPos) > _viewRangeHorizontal + 1)
				{
					temp.Add(pair.Key);
				}
				else
				{
					/*
					// Get chunk
					Chunk chunkScript = pair.Value.GetChunk();

					// Make only hidden chunks render!
					if (chunkScript.GetRender())
					{
						// Should chunk render yet?
						chunkScript.SetRender(CubeDistance(_playerPos, pair.Key) <= _viewRangeHorizontal);

						// Queue chunk for generation
						_loadQueue.Enqueue(chunkScript, 0);
					}
					*/
				}
			}

			// Are there chunks that need generation?
			if (!_rendering && _loadQueue.Count > 0)
			{
				_rendering = true;
				new Thread(RenderThread).Start();
			}

			// Destroy chunk
			foreach (ChunkPos key in temp)
			{
				DestroyChunk(key);
			}
		}

        /// <summary>
        /// Safely removes a <c>Chunk</c> from the chunk dictionary.
        /// </summary>
        /// <param name="pos">The chunk position.</param>
        [Server]
		private void DestroyChunk(ChunkPos pos)
		{
			_offloadChunks[pos] = _chunks[pos]; //Move chunk data to offload—technically should be disk or something

			SaveGame.SaveChunk(_chunks[pos], worldPath);

			NetworkServer.Destroy(_chunks[pos].GetChunk().gameObject); // Delete corresponding gameobject

			_chunks.Remove(pos); // Remove chunk from main list
		}

        //TODO: Optimize this for external chunks

        /// <summary>
        /// Fetches a block that has already been generated in the past.
        /// </summary>
        /// <param name="pos">The block position.</param>
        /// <returns>ID of block at given position.</returns>
		public Block.Block GetBlock(BlockPos pos)
        {
			return GenerateBlock(pos);
		}

        /// <summary>
        /// Generates a block for a given position.
        /// </summary>
        /// <param name="pos">The block position.</param>
        /// <returns>ID of generated block at given position.</returns>
		public Block.Block GenerateBlock(BlockPos pos)
		{
			int x = pos.GetWorldX();
			int y = pos.GetWorldY();
			int z = pos.GetWorldZ();
			DataColumn column = GetColumn(pos.chunkPos);

			// Topology
			float stone = column.GetSurface(x, z);
			float dirt = 3;

			if (y == 0)
            {
				return gameManager.blockManager.getBlockByName("Bedrock"); // Bedrock layer
			}
			else if (y <= stone)
			{
				// Caves
				float caves = SimplexNoise(x, y * 2, z, 40, 12, 1);
				caves += SimplexNoise(x, y, z, 30, 8, 0);
				caves += SimplexNoise(x, y, z, 10, 4, 0);

				if (caves > 16)
				{
					return gameManager.blockManager.getBlockByName("Air"); // Generating caves
				}

				// Underground ores
				float coal = SimplexNoise(x, y, z, 20, 20, 0);

				if (coal > 18)
				{
					return gameManager.blockManager.getBlockByName("Coal Ore");
				}

				return gameManager.blockManager.getBlockByName("Stone"); // Stone layer
			}
			else if (y <= stone + dirt)
			{
				return gameManager.blockManager.getBlockByName("Dirt"); // Dirt cover
			}
			else if (y <= stone + dirt + 1)
			{
				return gameManager.blockManager.getBlockByName("Grass Block"); // Grass cover
			}
			else
			{
				return gameManager.blockManager.getBlockByName("Air"); // Open Air
			}
		}

        /// <summary>
        /// Generates the topological height of the stone layer for a given coordinate.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="z">Y coordinate.</param>
        /// <returns>Height of terrain at given position.</returns>
        [Server]
		public static int GenerateTopology(int x, int z)
		{
			// Topology
			float stone = SimplexNoise(x, 0, z, 10, 3, 1.2f);
			stone += SimplexNoise(x, 300, z, 20, 4, 1f);
			stone += SimplexNoise(x, 500, z, 100, 20, 1f);

			// "Plateues"
			if (SimplexNoise(x, 100, z, 100, 10, 1f) >= 9f)
			{
				stone += 10;
			}

			stone += Mathf.Clamp(SimplexNoise(x, 0, z, 50, 10, 5f), 0, 10); // Craters?
																			//float dirt = PerlinNoise(x, 100, z, 50, 2, 0) + 3; // At least 3 dirt
																			//float dirt = 3;

			return (int)stone;
		}

        /// <summary>
        /// Returns simplex noise from a given point, modulated by some variables.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <param name="z">Z coordinate.</param>
        /// <param name="scale">Coordinate scaling.</param>
        /// <param name="height">Maximum height variation.</param>
        /// <param name="power">"sharpness".</param>
        /// <returns>Simplex noise from given point.</returns>
		public static float SimplexNoise(float x, float y, float z, float scale, float height, float power)
		{
			float rValue;
			rValue = Noise.Noise.GetNoise(((double)x) / scale, ((double)y) / scale, ((double)z) / scale);
			rValue *= height;

			if (power != 0)
			{
				rValue = Mathf.Pow(rValue, power);
			}

			return rValue;
		}

		/// <summary>
		/// Gets view range.
		/// </summary>
		/// <returns>View range.</returns>
		public int GetViewRange()
		{
			return _viewRangeHorizontal;
		}

		/// <summary>
		/// Gets the <c>DataChunk</c> from the given <paramref name="pos"/>.
		/// </summary>
		/// <param name="pos">Chunk position.</param>
		/// <returns><c>DataChunk</c> that exists at <paramref name="pos"/>.</returns>
		public DataChunk GetChunk(ChunkPos pos)
		{
			DataChunk chunk;
			_chunks.TryGetValue(pos, out chunk);
			return chunk;
		}

		/// <summary>
		/// Gets the <c>DataColumn</c> from the given <paramref name="pos"/>.
		/// </summary>
		/// <param name="pos">Column position.</param>
		/// <returns><c>DataColumn</c> that exists at <paramref name="pos"/>.</returns>
		public DataColumn GetColumn(ColumnPos pos)
		{
			DataColumn column;
			_columns.TryGetValue(pos, out column);
			return column;
		}
	}
}