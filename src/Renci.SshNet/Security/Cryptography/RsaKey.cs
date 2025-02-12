﻿using System;
using Renci.SshNet.Common;
using Renci.SshNet.Security.Cryptography;

namespace Renci.SshNet.Security
{
    /// <summary>
    /// Contains the RSA private and public key.
    /// </summary>
    public class RsaKey : Key, IDisposable
    {
        private bool _isDisposed;
        private RsaDigitalSignature _digitalSignature;

        /// <summary>
        /// Gets the name of the key.
        /// </summary>
        /// <returns>
        /// The name of the key.
        /// </returns>
        public override string ToString()
        {
            return "ssh-rsa";
        }

        /// <summary>
        /// Gets the modulus.
        /// </summary>
        /// <value>
        /// The modulus.
        /// </value>
        public BigInteger Modulus
        {
            get
            {
                return _privateKey[0];
            }
        }

        /// <summary>
        /// Gets the exponent.
        /// </summary>
        /// <value>
        /// The exponent.
        /// </value>
        public BigInteger Exponent
        {
            get
            {
                return _privateKey[1];
            }
        }

        /// <summary>
        /// Gets the D.
        /// </summary>
        /// <value>
        /// The D.
        /// </value>
        public BigInteger D
        {
            get
            {
                if (_privateKey.Length > 2)
                {
                    return _privateKey[2];
                }

                return BigInteger.Zero;
            }
        }

        /// <summary>
        /// Gets the P.
        /// </summary>
        /// <value>
        /// The P.
        /// </value>
        public BigInteger P
        {
            get
            {
                if (_privateKey.Length > 3)
                {
                    return _privateKey[3];
                }

                return BigInteger.Zero;
            }
        }

        /// <summary>
        /// Gets the Q.
        /// </summary>
        /// <value>
        /// The Q.
        /// </value>
        public BigInteger Q
        {
            get
            {
                if (_privateKey.Length > 4)
                {
                    return _privateKey[4];
                }

                return BigInteger.Zero;
            }
        }

        /// <summary>
        /// Gets the DP.
        /// </summary>
        /// <value>
        /// The DP.
        /// </value>
        public BigInteger DP
        {
            get
            {
                if (_privateKey.Length > 5)
                {
                    return _privateKey[5];
                }

                return BigInteger.Zero;
            }
        }

        /// <summary>
        /// Gets the DQ.
        /// </summary>
        /// <value>
        /// The DQ.
        /// </value>
        public BigInteger DQ
        {
            get
            {
                if (_privateKey.Length > 6)
                {
                    return _privateKey[6];
                }

                return BigInteger.Zero;
            }
        }

        /// <summary>
        /// Gets the inverse Q.
        /// </summary>
        /// <value>
        /// The inverse Q.
        /// </value>
        public BigInteger InverseQ
        {
            get
            {
                if (_privateKey.Length > 7)
                {
                    return _privateKey[7];
                }

                return BigInteger.Zero;
            }
        }

        /// <summary>
        /// Gets the length of the key.
        /// </summary>
        /// <value>
        /// The length of the key.
        /// </value>
        public override int KeyLength
        {
            get
            {
                return Modulus.BitLength;
            }
        }

        /// <summary>
        /// Gets the digital signature implementation for this key.
        /// </summary>
        /// <returns>
        /// An implementation of an RSA digital signature using the SHA-1 hash algorithm.
        /// </returns>
        protected internal override DigitalSignature DigitalSignature
        {
            get
            {
                _digitalSignature ??= new RsaDigitalSignature(this);

                return _digitalSignature;
            }
        }

        /// <summary>
        /// Gets or sets the public.
        /// </summary>
        /// <value>
        /// The public.
        /// </value>
        public override BigInteger[] Public
        {
            get
            {
                return new[] { Exponent, Modulus };
            }
            set
            {
                if (value.Length != 2)
                {
                    throw new InvalidOperationException("Invalid private key.");
                }

                _privateKey = new[] { value[1], value[0] };
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RsaKey"/> class.
        /// </summary>
        public RsaKey()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RsaKey"/> class.
        /// </summary>
        /// <param name="data">DER encoded private key data.</param>
        public RsaKey(byte[] data)
            : base(data)
        {
            if (_privateKey.Length != 8)
            {
                throw new InvalidOperationException("Invalid private key.");
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RsaKey"/> class.
        /// </summary>
        /// <param name="modulus">The modulus.</param>
        /// <param name="exponent">The exponent.</param>
        /// <param name="d">The d.</param>
        /// <param name="p">The p.</param>
        /// <param name="q">The q.</param>
        /// <param name="inverseQ">The inverse Q.</param>
        public RsaKey(BigInteger modulus, BigInteger exponent, BigInteger d, BigInteger p, BigInteger q, BigInteger inverseQ)
        {
            _privateKey = new BigInteger[8]
                {
                    modulus,
                    exponent,
                    d,
                    p,
                    q,
                    PrimeExponent(d, p),
                    PrimeExponent(d, q),
                    inverseQ
                };
        }

        private static BigInteger PrimeExponent(BigInteger privateExponent, BigInteger prime)
        {
            var pe = prime - new BigInteger(1);
            return privateExponent % pe;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><see langword="true"/> to release both managed and unmanaged resources; <see langword="false"/> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed)
            {
                return;
            }

            if (disposing)
            {
                var digitalSignature = _digitalSignature;
                if (digitalSignature != null)
                {
                    digitalSignature.Dispose();
                    _digitalSignature = null;
                }

                _isDisposed = true;
            }
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="RsaKey"/> is reclaimed by garbage collection.
        /// </summary>
        ~RsaKey()
        {
            Dispose(disposing: false);
        }
    }
}
