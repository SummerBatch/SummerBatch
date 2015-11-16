//
//   Copyright 2015 Blu Age Corporation - Plano, Texas
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
using System;
using System.Collections.Generic;
using System.IO;
using System.Transactions;
using Summer.Batch.Common.Transaction;


namespace Summer.Batch.Infrastructure.Support.Transaction
{
    /// <summary>
    /// File Stream with transactionnal write behaviour support.
    /// </summary>
    public class TransactionAwareFileStream : FileStream, ISinglePhaseNotification
    {

        private bool _shouldClose;
        private readonly List<byte> _internalBuffer;

        /// <summary>
        /// Custom constructor with path and FileMode.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="fileMode"></param>
        public TransactionAwareFileStream(string path, FileMode fileMode) : base(path, fileMode)
        {
            _internalBuffer = new List<byte>();
            TransactionScopeManager.RegisterResource(this);
        }

        /// <summary>
        /// Gets or sets the current position of this stream.
        /// </summary>
        /// <returns>
        /// The current position of this stream.
        /// </returns>
        public override long Position
        {
            get { return IsTransactionActive() ? base.Position + _internalBuffer.Count : base.Position; }
            set { base.Position = value; }
        }

        /// <summary>
        /// overrides Write(byte[] array, int offset, int count)
        /// Writes to internal buffer if transaction is active, otherwise delegates
        /// to direct base write.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        public override void Write(byte[] array, int offset, int count)
        {
            if (IsTransactionActive())
            {
                var subArray = new byte[count];
                Buffer.BlockCopy(array, offset, subArray, 0, count);
                _internalBuffer.AddRange(subArray);
                return;
            }
            //else return to default behaviour
            base.Write(array, offset, count);
        }

        /// <summary>
        /// Checks if current transaction is existing.
        /// </summary>
        /// <returns></returns>
        private static bool IsTransactionActive()
        {
            return System.Transactions.Transaction.Current != null;
        }

        /// <summary>
        /// @see ISinglePhaseNotification#Prepare .
        /// </summary>
        /// <param name="preparingEnlistment"></param>
        public void Prepare(PreparingEnlistment preparingEnlistment)
        {
            preparingEnlistment.Prepared();
        }

        /// <summary>
        /// @see ISinglePhaseNotification#Commit .
        /// </summary>
        /// <param name="enlistment"></param>
        public void Commit(Enlistment enlistment)
        {
            Complete();
            enlistment.Done();
        }

        /// <summary>
        /// @see ISinglePhaseNotification#Rollback .
        /// </summary>
        /// <param name="enlistment"></param>
        public void Rollback(Enlistment enlistment)
        {
            enlistment.Done();
        }

        /// <summary>
        /// @see ISinglePhaseNotification#InDoubt .
        /// </summary>
        /// <param name="enlistment"></param>
        public void InDoubt(Enlistment enlistment)
        {
            enlistment.Done();
        }

        /// <summary>
        /// @see ISinglePhaseNotification#SinglePhaseCommit .
        /// </summary>
        /// <param name="singlePhaseEnlistment"></param>
        public void SinglePhaseCommit(SinglePhaseEnlistment singlePhaseEnlistment)
        {
            Complete();
            singlePhaseEnlistment.Committed();
        }

        /// <summary>
        /// override Stream#Flush .
        /// </summary>
        public override void Flush()
        {
            //Flush is permitted only on non-transaction context
            if (!IsTransactionActive())
            {
                base.Flush();
            }
        }

        /// <summary>
        /// @see ISinglePhaseNotification#Complete .
        /// </summary>
        private void Complete()
        {
            //write buffer
            base.Write(_internalBuffer.ToArray(),0,_internalBuffer.Count);
            _internalBuffer.Clear();
            //flush
            base.Flush();
            if (_shouldClose)
            {
                base.Dispose(true);
            }
            
        }

        /// <summary>
        /// override Dispose .
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                TransactionScopeManager.UnregisterResource(this);
                if (IsTransactionActive())
                {
                    _shouldClose = true;
                }
                else
                {
                    base.Dispose(true);
                }
            }            
        }
    }
}