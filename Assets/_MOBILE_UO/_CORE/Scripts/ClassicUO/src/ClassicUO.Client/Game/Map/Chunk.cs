﻿// SPDX-License-Identifier: BSD-2-Clause

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ClassicUO.Game.GameObjects;
using ClassicUO.Game.Managers;
using ClassicUO.Assets;
using ClassicUO.Utility;
using System;
using System.Runtime.InteropServices;
using System.Buffers;

namespace ClassicUO.Game.Map
{
    internal sealed class Chunk
    {
        //private static readonly QueuedPool<Chunk> _pool = new QueuedPool<Chunk>
        //(
        //    Constants.PREDICTABLE_CHUNKS,
        //    c =>
        //    {
        //        c.LastAccessTime = Time.Ticks + Constants.CLEAR_TEXTURES_DELAY;
        //        c.IsDestroyed = false;
        //    }
        //);

        private readonly World _world;

        public Chunk(World world)
        {
            _world = world;
        }

        public GameObject[,] Tiles { get; } = new GameObject[8, 8];
        public bool IsDestroyed;
        public long LastAccessTime;
        public LinkedListNode<int> Node;


        public int X;
        public int Y;


        public static Chunk Create(World world, int x, int y)
        {
            Chunk c = new Chunk(world); // _pool.GetOne();
            c.LastAccessTime = Time.Ticks + Constants.CLEAR_TEXTURES_DELAY;
            c.X = x;
            c.Y = y;

            return c;
        }


        public unsafe void Load(int index)
        {
            IsDestroyed = false;

            Map map = _world.Map;

            ref var im = ref GetIndex(index);

            if (!im.IsValid())
            {
                return;
            }

            im.MapFile.Seek((long)im.MapAddress, System.IO.SeekOrigin.Begin);
            // MobileUO: TODO: InlineArray feature is not available in Unity's C#
            var block = im.MapFile.ReadMapBlock();//Read<MapBlock>();

            var cells = block.Cells;
            int bx = X << 3;
            int by = Y << 3;

            for (int y = 0; y < 8; ++y)
            {
                int pos = y << 3;
                ushort tileY = (ushort)(by + y);

                for (int x = 0; x < 8; ++x, ++pos)
                {
                    ushort tileID = (ushort)(cells[pos].TileID & 0x3FFF);

                    sbyte z = cells[pos].Z;

                    Land land = Land.Create(_world, tileID);

                    ushort tileX = (ushort)(bx + x);

                    land.ApplyStretch(map, tileX, tileY, z);
                    land.X = tileX;
                    land.Y = tileY;
                    land.Z = z;
                    land.UpdateScreenPosition();

                    AddGameObject(land, x, y);
                }
            }

            if (im.StaticAddress != 0 && im.StaticCount > 0)
            {
                var staticsBlockBuffer = ArrayPool<StaticsBlock>.Shared.Rent((int)im.StaticCount);
                var staticsSpan = staticsBlockBuffer.AsSpan(0, (int)im.StaticCount);
                im.StaticFile.Seek((long)im.StaticAddress, System.IO.SeekOrigin.Begin);
                im.StaticFile.Read(MemoryMarshal.AsBytes(staticsSpan));

                foreach (ref var sb in staticsSpan)
                {
                    if (sb.Color != 0 && sb.Color != 0xFFFF)
                    {
                        int pos = (sb.Y << 3) + sb.X;

                        if (pos >= 64)
                        {
                            continue;
                        }

                        Static staticObject = Static.Create(_world, sb.Color, sb.Hue, pos);
                        staticObject.X = (ushort)(bx + sb.X);
                        staticObject.Y = (ushort)(by + sb.Y);
                        staticObject.Z = sb.Z;
                        staticObject.UpdateScreenPosition();

                        AddGameObject(staticObject, sb.X, sb.Y);
                    }
                }

                ArrayPool<StaticsBlock>.Shared.Return(staticsBlockBuffer);
            }
        }


        private ref IndexMap GetIndex(int map)
        {
            Client.Game.UO.FileManager.Maps.SanitizeMapIndex(ref map);

            return ref Client.Game.UO.FileManager.Maps.GetIndex(map, X, Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GameObject GetHeadObject(int x, int y)
        {
            GameObject obj = Tiles[x, y];

            while (obj?.TPrevious != null)
            {
                obj = obj.TPrevious;
            }

            return obj;
        }

        public void AddGameObject(GameObject obj, int x, int y)
        {
            obj.RemoveFromTile();

            short priorityZ = obj.Z;
            sbyte state = -1;

            ushort graphic = obj.Graphic;

            switch (obj)
            {
                case Land tile:

                    if (tile.IsStretched)
                    {
                        priorityZ = (short) (tile.AverageZ - 1);
                    }
                    else
                    {
                        priorityZ--;
                    }

                    priorityZ -= 1;

                    state = 0;

                    break;

                case Mobile _:
                    priorityZ++;

                    break;

                case Item item:

                    if (item.IsCorpse)
                    {
                        priorityZ++;

                        break;
                    }
                    else if (item.IsMulti)
                    {
                        graphic = item.MultiGraphic;
                    }

                    goto default;

                case GameEffect _:
                    priorityZ += 2;

                    break;

                case Multi m:

                    state = 1;

                    if ((m.State & CUSTOM_HOUSE_MULTI_OBJECT_FLAGS.CHMOF_GENERIC_INTERNAL) != 0)
                    {
                        priorityZ--;

                        break;
                    }

                    if ((m.State & CUSTOM_HOUSE_MULTI_OBJECT_FLAGS.CHMOF_PREVIEW) != 0)
                    {
                        state = 2;
                        priorityZ++;
                    }
                    //else if ((m.ItemData.Flags & TileFlag.StairRight) != 0)
                    //{
                    //    priorityZ++;
                    //}

                    //if (m.IsMovable)
                    //{
                    //    priorityZ += 1;
                    //}

                    goto default;

                default:
                    ref StaticTiles data = ref Client.Game.UO.FileManager.TileData.StaticData[graphic];

                    if (data.IsBackground)
                    {
                        priorityZ--;
                    }

                    //if (data.IsSurface)
                    //{
                    //    priorityZ--;
                    //}

                    if (data.Height != 0)
                    {
                        priorityZ++;
                    }

                    if (data.IsMultiMovable)
                    {
                        priorityZ++;
                    }

                    break;
            }

            obj.PriorityZ = priorityZ;

            if (Tiles[x, y] == null)
            {
                Tiles[x, y] = obj;
                obj.TPrevious = null;
                obj.TNext = null;

                return;
            }


            GameObject o = Tiles[x, y];

            if (o == obj)
            {
                if (o.Previous != null)
                {
                    o = (GameObject) o.Previous;
                }
                else if (o.Next != null)
                {
                    o = (GameObject) o.Next;
                }
                else
                {
                    return;
                }
            }

            while (o?.TPrevious != null)
            {
                o = o.TPrevious;
            }

            GameObject found = null;
            GameObject start = o;

            while (o != null)
            {
                int testPriorityZ = o.PriorityZ;

                if (testPriorityZ > priorityZ || testPriorityZ == priorityZ && (state == 0 || state == 1 && !(o is Land)))
                {
                    break;
                }

                found = o;
                o = o.TNext;
            }

            if (found != null)
            {
                obj.TPrevious = found;
                GameObject next = found.TNext;
                obj.TNext = next;
                found.TNext = obj;

                if (next != null)
                {
                    next.TPrevious = obj;
                }
            }
            else if (start != null)
            {
                obj.TNext = start;
                start.TPrevious = obj;
                obj.TPrevious = null;
            }
        }

        public void RemoveGameObject(GameObject obj, int x, int y)
        {
            ref GameObject firstNode = ref Tiles[x, y];

            if (firstNode == null || obj == null)
            {
                return;
            }

            if (firstNode == obj)
            {
                firstNode = obj.TNext;
            }

            if (obj.TNext != null)
            {
                obj.TNext.TPrevious = obj.TPrevious;
            }

            if (obj.TPrevious != null)
            {
                obj.TPrevious.TNext = obj.TNext;
            }

            obj.TPrevious = null;
            obj.TNext = null;
        }


        public void Destroy()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    GameObject obj = Tiles[i, j];

                    if (obj == null)
                    {
                        continue;
                    }

                    GameObject first = GetHeadObject(i, j);

                    while (first != null)
                    {
                        GameObject next = first.TNext;

                        if (!ReferenceEquals(first, _world.Player))
                        {
                            first.Destroy();
                        }

                        first.TPrevious = null;
                        first.TNext = null;
                        first = next;
                    }

                    Tiles[i, j] = null;
                }
            }

            if (Node.Next != null || Node.Previous != null)
            {
                Node.List?.Remove(Node);
            }

            IsDestroyed = true;
            //_pool.ReturnOne(this);
        }

        public void Clear()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    GameObject obj = Tiles[i, j];

                    if (obj == null)
                    {
                        continue;
                    }

                    GameObject first = GetHeadObject(x: i, j);

                    while (first != null)
                    {
                        GameObject next = first.TNext;

                        if (!ReferenceEquals(first, _world.Player))
                        {
                            first.Destroy();
                        }

                        first.TPrevious = null;
                        first.TNext = null;
                        first = next;
                    }

                    Tiles[i, j] = null;
                }
            }

            if (Node.Next != null || Node.Previous != null)
            {
                Node.List?.Remove(Node);
            }

            IsDestroyed = true;
        }

        public bool HasNoExternalData()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    for (GameObject obj = GetHeadObject(i, j); obj != null; obj = obj.TNext)
                    {
                        if (!(obj is Land) && !(obj is Static) /*&& !(obj is Multi)*/)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }
    }
}