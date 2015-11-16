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
namespace Summer.Batch.Common.Transaction.Support
{
    /// <summary>
    /// Interface for transaction completion callbacks.
    /// An instance can be register with the <see cref="T:Summer.Batch.TransactionTransactionScopeManager"/>,
    /// it then will be called after a transaction completion.
    /// </summary>
    public interface ITransactionSynchronization
    {
        /// <summary>
        /// Invoked after a transaction completion.
        /// </summary>
        /// <param name="transaction">the completed transaction</param>
        void AfterCompletion(System.Transactions.Transaction transaction);
    }
}