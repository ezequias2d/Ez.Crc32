// Copyright (c) 2021 ezequias2d <ezequiasmoises@gmail.com> and the Ez.Crc32 contributors
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
using System;
using System.Runtime.CompilerServices;

namespace Ez.Crc32
{
    /// <summary>
    /// Provide generic <see cref="ICrc32"/> implementations.
    /// </summary>
    public static class ManagedCrc32
    {
        /// <summary>
        /// CRC32 polynomial.
        /// </summary>
        public const uint Crc32Polynomial = 0xEDB88320;

        /// <summary>
        /// CRC32C polynomial 
        /// </summary>
        public const uint Crc32CPolynomial = 0x82F63B78;

        /// <summary>
        /// Generic implementation of <see cref="ICrc32"/> with the <see cref="Crc32Polynomial"/> polynomial.
        /// </summary>
        public static readonly ICrc32 Crc32 = new GenericCrc32(Crc32Polynomial);

        /// <summary>
        /// Generic implementation of <see cref="ICrc32"/> with the <see cref="Crc32CPolynomial"/> polynomial.
        /// </summary>
        public static readonly ICrc32 Crc32C = new GenericCrc32(Crc32CPolynomial);

        /// <summary>
        /// Creates an <see cref="ICrc32"/> object that uses a custom polynomial.
        /// </summary>
        /// <param name="polynomial">Polynomial used by <see cref="ICrc32"/>.</param>
        /// <returns>Generic implementation of <see cref="ICrc32"/> with the <paramref name="polynomial"/>.</returns>
        public static ICrc32 Create(uint polynomial) => new GenericCrc32(polynomial);

        private class GenericCrc32 : ICrc32
        {

            private readonly uint _polynomial;
            private readonly uint[] _crc32LookupBlock;

            public GenericCrc32(uint polynomial)
            {
                _polynomial = polynomial;
                _crc32LookupBlock = new uint[256 * 24];
                Setup(_crc32LookupBlock, polynomial, 24);
            }
            public bool IsSupported => true;
            public uint Polynomial => _polynomial;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public unsafe uint Compute(uint previousCrc, void* data, ulong length)
            {
                if (!BitConverter.IsLittleEndian)
                    throw new NotImplementedException("This implementation does not support big endian.");

                fixed (void* table = _crc32LookupBlock)
                {
                    uint* current = (uint*)data;
                    void* max = (byte*)data + length;
                    void* maxUint = (byte*)data + length - (length % (sizeof(uint) * 6));

                    uint crc = ~previousCrc;
                    while (current < maxUint)
                    {
                        var one = *current++ ^ crc;
                        var two = *current++;
                        var three = *current++;
                        var four = *current++;
                        var five = *current++;
                        var six = *current++;
                        crc = Crc(table, 20, one) ^
                                Crc(table, 16, two) ^
                                Crc(table, 12, three) ^
                                Crc(table, 8, four) ^
                                Crc(table, 4, five) ^
                                Crc(table, 0, six);
                    }

                    byte* currentChar = (byte*)current;

                    while (currentChar < max)
                        crc = (crc >> 8) ^ Get(table, 0, (crc & 0xFF) ^ *currentChar++);

                    return ~crc;
                }
            }

            #region wrappers
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public unsafe uint Compute(void* data, ulong length) =>
                Compute(0, data, length);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public uint Compute(IntPtr data, IntPtr length) =>
                Compute(0, data, length);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public uint Compute(ReadOnlySpan<byte> data) =>
                Compute(0, data);


            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public uint Compute(uint previousCrc32, ReadOnlySpan<byte> data)
            {
                unsafe
                {
                    fixed (void* ptr = data)
                        return Compute(previousCrc32, ptr, (ulong)data.Length);
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public uint Compute(uint previousCrc32, IntPtr data, IntPtr length)
            {
                unsafe
                {
                    return Compute(previousCrc32, (void*)data, (ulong)length);
                }
            }
            #endregion

            #region static
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private static unsafe void Setup(uint[] table, uint polynomial, uint n)
            {
                fixed (void* ptr = table)
                {
                    for (var i = 0u; i <= 0xFF; i++)
                    {
                        var crc = i;
                        for (var j = 0u; j < 8u; j++)
                            crc = (crc >> 1) ^ ((crc & 1) * polynomial);

                        Set(ptr, 0, i, crc);
                    }

                    for (var i = 0u; i <= 0xFF; i++)
                    {
                        for(var j = 1u; j < n; j++)
                        {
                            Set(ptr, j, i, (Get(ptr, j - 1, i) >> 8) ^ Get(ptr, 0, Get(ptr, j - 1, i) & 0xFF));
                        }
                        //Set(ptr, 1, i, (Get(ptr, 0, i) >> 8) ^ Get(ptr, 0, Get(ptr, 0, i) & 0xFF));
                        //Set(ptr, 2, i, (Get(ptr, 1, i) >> 8) ^ Get(ptr, 0, Get(ptr, 1, i) & 0xFF));
                        //Set(ptr, 3, i, (Get(ptr, 2, i) >> 8) ^ Get(ptr, 0, Get(ptr, 2, i) & 0xFF));
                        //Set(ptr, 4, i, (Get(ptr, 3, i) >> 8) ^ Get(ptr, 0, Get(ptr, 3, i) & 0xFF));
                        //Set(ptr, 5, i, (Get(ptr, 4, i) >> 8) ^ Get(ptr, 0, Get(ptr, 4, i) & 0xFF));
                        //Set(ptr, 6, i, (Get(ptr, 5, i) >> 8) ^ Get(ptr, 0, Get(ptr, 5, i) & 0xFF));
                        //Set(ptr, 7, i, (Get(ptr, 6, i) >> 8) ^ Get(ptr, 0, Get(ptr, 6, i) & 0xFF));

                        //Set(ptr, 8, i, (Get(ptr, 7, i) >> 8) ^ Get(ptr, 0, Get(ptr, 7, i) & 0xFF));
                        //Set(ptr, 9, i, (Get(ptr, 8, i) >> 8) ^ Get(ptr, 0, Get(ptr, 8, i) & 0xFF));
                        //Set(ptr, 10, i, (Get(ptr, 9, i) >> 8) ^ Get(ptr, 0, Get(ptr, 9, i) & 0xFF));
                        //Set(ptr, 11, i, (Get(ptr, 10, i) >> 8) ^ Get(ptr, 0, Get(ptr, 10, i) & 0xFF));
                        //Set(ptr, 12, i, (Get(ptr, 11, i) >> 8) ^ Get(ptr, 0, Get(ptr, 11, i) & 0xFF));
                        //Set(ptr, 13, i, (Get(ptr, 12, i) >> 8) ^ Get(ptr, 0, Get(ptr, 12, i) & 0xFF));
                        //Set(ptr, 14, i, (Get(ptr, 13, i) >> 8) ^ Get(ptr, 0, Get(ptr, 13, i) & 0xFF));
                        //Set(ptr, 15, i, (Get(ptr, 14, i) >> 8) ^ Get(ptr, 0, Get(ptr, 14, i) & 0xFF));

                        //Set(ptr, 16, i, (Get(ptr, 15, i) >> 8) ^ Get(ptr, 0, Get(ptr, 15, i) & 0xFF));
                        //Set(ptr, 17, i, (Get(ptr, 16, i) >> 8) ^ Get(ptr, 0, Get(ptr, 16, i) & 0xFF));
                        //Set(ptr, 18, i, (Get(ptr, 17, i) >> 8) ^ Get(ptr, 0, Get(ptr, 17, i) & 0xFF));
                        //Set(ptr, 19, i, (Get(ptr, 18, i) >> 8) ^ Get(ptr, 0, Get(ptr, 18, i) & 0xFF));
                    }

                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private unsafe static void Set(in void* ptr, in uint x, in uint y, in uint value) =>
                *(((uint*)ptr) + x * 256 + y) = value;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private static unsafe uint Get(in void* ptr, in uint x, in uint y) =>
                *(((uint*)ptr) + x * 256 + y);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private static unsafe uint Crc(in void* ptr, in uint x, in uint value) =>
                Get(ptr, x, (value >> 24) & 0xFF) ^
                 Get(ptr, x + 1, (value >> 16) & 0xFF) ^
                 Get(ptr, x + 2, (value >> 8) & 0xFF) ^
                 Get(ptr, x + 3, value & 0xFF);
            #endregion
        }
    }
}
