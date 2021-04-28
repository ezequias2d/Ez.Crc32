// Copyright (c) 2021 ezequias2d <ezequiasmoises@gmail.com> and the Ez.Crc32 contributors
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

namespace Ez.Crc32
{
    /// <summary>
    /// Static class that chooses the best compatible implementation.
    /// </summary>
    public static class AutoCrc32
    {
        /// <summary>
        /// Gets best <see cref="ICrc32"/> implementation with the <see cref="ManagedCrc32.Crc32Polynomial"/> polynomial.
        /// </summary>
        public static ICrc32 Crc32 = ManagedCrc32.Crc32;

        /// <summary>
        /// Gets best <see cref="ICrc32"/> implementation with the <see cref="ManagedCrc32.Crc32CPolynomial"/> polynomial.
        /// </summary>
        public static ICrc32 Crc32C = NativeCrc32C.Default.IsSupported ?
            NativeCrc32C.Default : 
            ManagedCrc32.Crc32C;
    }
}
