/*
* Copyright 2007 ZXing authors
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*      http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/
using System;
namespace com.google.zxing.common.reedsolomon
{
    /// <summary> <p>Thrown when an exception occurs during Reed-Solomon decoding, such as when
    /// there are too many errors to correct.</p>
    ///
    /// </summary>
    /// <author>  Sean Owen
    /// </author>
    /// <author>www.Redivivus.in (suraj.supekar@redivivus.in) - Ported from ZXING Java Source
    /// </author>
    [Serializable]
    public sealed class ReedSolomonException : Exception
    {
        public ReedSolomonException(string message) : base(message)
        {
        }

        public ReedSolomonException()
        {
        }

        public ReedSolomonException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}