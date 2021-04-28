// Copyright (c) 2021 ezequias2d <ezequiasmoises@gmail.com> and the Ez.Crc32 contributors
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
using System;

namespace Ez.Crc32
{
    /// <summary>
    /// Represents an implementation of CRC32.
    /// </summary>
    public interface ICrc32
    {
        /// <summary>
        /// Indicates that the implementation is supported.
        /// </summary>
        bool IsSupported { get; }

        /// <summary>
        /// Gets the polynomial of this instance.
        /// </summary>
        uint Polynomial { get; }

        /// <summary>
        /// Computes CRC-32 from data buffer.
        /// </summary>
        /// <param name="data">The data buffer pointer for calculating the CRC-32.</param>
        /// <param name="length">The length of data in <paramref name="data"/> buffer.</param>
        /// <returns>CRC-32 of data buffer.</returns>
        unsafe uint Compute(void* data, ulong length);

        /// <summary>
        /// Computes CRC-32 from data buffer.
        /// </summary>
        /// <param name="data">The data buffer pointer for calculating the CRC-32.</param>
        /// <param name="length">The length of data in <paramref name="data"/> buffer.</param>
        /// <returns>CRC-32 of data buffer.</returns>
        uint Compute(IntPtr data, IntPtr length);

        /// <summary>
        /// Computes CRC-32 from data buffer.
        /// </summary>
        /// <param name="data">The data buffer for calculating the CRC-32.</param>
        /// <returns>CRC-32 of data buffer.</returns>
        uint Compute(ReadOnlySpan<byte> data);

        /// <summary>
        /// Computes CRC-32 from initial CRC-32 and a data buffer.
        /// Call this method several times to calculate CRC-32 from multiple buffers.
        /// </summary>
        /// <param name="previousCrc">Initial value of crc-32.</param>
        /// <param name="data">The data buffer pointer for calculating the CRC-32.</param>
        /// <param name="length">The length of data in <paramref name="data"/> buffer.</param>
        /// <returns>CRC-32 of data buffer.</returns>
        unsafe uint Compute(uint previousCrc, void* data, ulong length);

        /// <summary>
        /// Computes CRC-32 from initial CRC-32 and a data buffer.
        /// Call this method several times to calculate CRC-32 from multiple buffers.
        /// </summary>
        /// <param name="previousCrc">Initial value of crc-32.</param>
        /// <param name="data">The data buffer pointer for calculating the CRC-32.</param>
        /// <param name="length">The length of data in <paramref name="data"/> buffer.</param>
        /// <returns>CRC-32 of data buffer.</returns>
        uint Compute(uint previousCrc, IntPtr data, IntPtr length);

        /// <summary>
        /// Computes CRC-32 from data buffer.
        /// </summary>
        /// <param name="previousCrc">Initial value of crc-32.</param>
        /// <param name="data">The data buffer for calculating the CRC-32.</param>
        /// <returns>CRC-32 of data buffer.</returns>
        uint Compute(uint previousCrc, ReadOnlySpan<byte> data);
    }
}
