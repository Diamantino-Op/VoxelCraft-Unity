using ZstdNet;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using System;
using Mirror;
using VoxelCraft.World.Chunk;
using VoxelCraft.Compression;
using VoxelCraft.World.Block;

namespace VoxelCraft.IO
{
    public static class SaveGame
    {
        public static void TrainChunkDict()
        {
            List<byte[]> data = new List<byte[]>();

            for (int i = 0; i < 10000; i++)
            {
                DataChunk dataChunk = new DataChunk(new ChunkPos(i, i, i), new Chunk(), new DataColumn(new ColumnPos(i, i)));

                data.Add(ObjectToByteArray(new SaveDataChunk(dataChunk)));
            }

            Debug.Log("Data generation finished");

            byte[] dict = DictBuilder.TrainFromBuffer(data);

            File.WriteAllBytes(Application.persistentDataPath + "/ChunkDict.dict", dict);

            Debug.Log("Train Finished");
        }

        public static void SaveWorld(Dictionary<ChunkPos, DataChunk> chunks, string path)
        {
            foreach (KeyValuePair<ChunkPos, DataChunk> chunk in chunks)
            {
                SaveChunk(chunk.Value, path);
            }
        }

        public static void SaveChunk(DataChunk data, string path)
        {
            SaveDataChunk saveData = new SaveDataChunk(data);

            byte[] dataCmp = CompressionManager.Compress(ObjectToByteArray(saveData), CompressionDictionaries.chunkDict);

            if (!Directory.Exists(path + "Chunks"))
                Directory.CreateDirectory(path + "Chunks");

            File.WriteAllBytes(path + "Chunks/chunk_" + saveData._pos.x + "_" + saveData._pos.y + "_" + saveData._pos.z + ".cmcnk", dataCmp);
        }

        public static bool ChunkSaveExists(ChunkPos pos, string path)
        {
            if (File.Exists(path + "Chunks/chunk_" + pos.x + "_" + pos.y + "_" + pos.z + ".cmcnk"))
                return true;
            else
                return false;
        }

        public static DataChunk LoadChunkData(ChunkPos pos, string path, Chunk chunk)
        {
            if (File.Exists(path + "Chunks/chunk_" + pos.x + "_" + pos.y + "_" + pos.z + ".cmcnk"))
            {
                BinaryFormatter bf = new BinaryFormatter();
                byte[] dataCmp = File.ReadAllBytes(path + "Chunks/chunk_" + pos.x + "_" + pos.y + "_" + pos.z + ".cmcnk");

                byte[] data = CompressionManager.Decompress(dataCmp, CompressionDictionaries.chunkDict);

                MemoryStream stream = new MemoryStream(data);

                SaveDataChunk dataChunk = (SaveDataChunk)bf.Deserialize(stream);

                return new DataChunk(dataChunk._pos, dataChunk.GetBlocksFromNameList(dataChunk._blocks), dataChunk._column, chunk, dataChunk.dataVersion);
            }
            else
                return null;
        }

        public static byte[] ObjectToByteArray(object obj)
        {
            if (obj == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }
    }

    [Serializable]
    public class SaveDataChunk
    {
        public readonly ChunkPos _pos;
        public string[,,] _blocks = new string[World.World.chunkSize, World.World.chunkSize, World.World.chunkSize];
        public DataColumn _column;
        public string dataVersion;

        public SaveDataChunk(DataChunk data)
        {
            _pos = data._pos;
            _column = data._column;
            dataVersion = data.dataVersion;

            AddBlocksToNameList(data._blocks);
        }

        public void AddBlocksToNameList(Block[,,] blocks)
        {
            for (int x = 0; x < World.World.chunkSize; x++)
            {
                for (int y = 0; y < World.World.chunkSize; y++)
                {
                    for (int z = 0; z < World.World.chunkSize; z++)
                    {
                        if (blocks[x, y, z] != null)
                        {
                            _blocks[x, y, z] = blocks[x, y, z].name;
                        }
                    }
                }
            }
        }

        public Block[,,] GetBlocksFromNameList(string[,,] _blocks)
        {
            Block[,,] blocks = new Block[World.World.chunkSize, World.World.chunkSize, World.World.chunkSize];

            for (int x = 0; x < World.World.chunkSize; x++)
            {
                for (int y = 0; y < World.World.chunkSize; y++)
                {
                    for (int z = 0; z < World.World.chunkSize; z++)
                    {
                        blocks[x, y, z] = BlockManager.staticGetBlockByName(_blocks[x, y, z]);
                    }
                }
            }

            return blocks;
        }
    }
}