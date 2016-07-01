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
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using System.Transactions;
using NLog;
using Summer.Batch.Common.Transaction.Support;

namespace Summer.Batch.Common.Transaction
{
    /// <summary>
    /// Utility class for managing transaction scopes.
    /// </summary>
    public static class TransactionScopeManager
    {
        private const string TransactionRollbackedMessage = "Transaction has been rollbacked.";
        private const string TransactionRollbackedDetailedMessage =
            "Transaction [{0}], with isolationLevel [{1}], started at [{2}] has been rollbacked.";

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static readonly IDictionary<TransactionScope, IList<ITransactionSynchronization>> Synchronizations =
            new ConcurrentDictionary<TransactionScope, IList<ITransactionSynchronization>>();
        private readonly static ThreadLocal<ISet<IEnlistmentNotification>> Resources =
            new ThreadLocal<ISet<IEnlistmentNotification>>(() => new HashSet<IEnlistmentNotification>());

        // Handle transaction completion event;
        // - Log rollbacked transactions
        // - delegates to known listeners so that they can handle the situation gracefully
        private static TransactionCompletedEventHandler GetTransactionCompleted(TransactionScope scope)
        {
            return (sender, args) =>
            {
                LogTransactionIfAborted(args);
                IList<ITransactionSynchronization> scopeSynchronizations;
                if (Synchronizations.TryGetValue(scope, out scopeSynchronizations))
                {
                    foreach (var synchronization in scopeSynchronizations)
                    {
                        synchronization.AfterCompletion(args.Transaction);
                    }
                }
                Synchronizations.Remove(scope);
            };
        }


        // Logs the transaction if rollbacked, given different details depending on logger level
        private static void LogTransactionIfAborted(TransactionEventArgs args)
        {            
            if (args.Transaction.TransactionInformation.Status.Equals(TransactionStatus.Aborted))
            {
                //Log any rollbacked transaction                
                Logger.Error(TransactionRollbackedMessage);
                if (Logger.IsInfoEnabled)
                {
                    Logger.Info(
                        TransactionRollbackedDetailedMessage,
                        args.Transaction.TransactionInformation.LocalIdentifier,
                        args.Transaction.IsolationLevel,
                        args.Transaction.TransactionInformation.CreationTime);
                }
            }
        }

        /// <summary>
        /// Creates a new transaction scope with the given options and registers the
        /// <see cref="ITransactionSynchronization"/> and <see cref="IEnlistmentNotification"/>
        /// with the current transaction.
        /// </summary>
        /// <param name="scopeOption">The scope options to use, <see cref="TransactionScopeOption.Required"/> by default.</param>
        /// <param name="isolationLevel">The isolation level, <see cref="IsolationLevel.ReadUncommitted"/> by default.</param>
        /// <param name="timeout">The transaction timeout, <see cref="TransactionManager.MaximumTimeout"/> by default.</param>
        /// <returns>A new <see cref="TransactionScope"/> with the given options</returns>
        public static TransactionScope CreateScope(TransactionScopeOption scopeOption = TransactionScopeOption.Required,
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, TimeSpan? timeout = null)
        {
            // Release connections so that new connections are created,
            // which will thus use the new transaction
            ConnectionUtil.ReleaseConnections();
            var options = new TransactionOptions
            {
                IsolationLevel = GetIsolationLevel(isolationLevel),
                Timeout = timeout ?? TransactionManager.MaximumTimeout
            };
            var scope = new TransactionScope(scopeOption, options);
            System.Transactions.Transaction.Current.TransactionCompleted += GetTransactionCompleted(scope);
            foreach (var resource in Resources.Value)
            {
                System.Transactions.Transaction.Current.EnlistVolatile(resource, EnlistmentOptions.None);
            }
            return scope;
        }

        /// <summary>
        /// Registers a new <see cref="ITransactionSynchronization"/> to a scope.
        /// </summary>
        /// <param name="scope">the scope to register the synchronization to</param>
        /// <param name="synchronization">the synchronization to register</param>
        public static void RegisterTransactionSynchronization(TransactionScope scope, ITransactionSynchronization synchronization)
        {
            IList<ITransactionSynchronization> scopeSynchronizations;
            if (!Synchronizations.TryGetValue(scope, out scopeSynchronizations))
            {
                scopeSynchronizations = new List<ITransactionSynchronization>();
                Synchronizations[scope] = scopeSynchronizations;
            }
            scopeSynchronizations.Add(synchronization);
        }

        /// <summary>
        /// Registers a new <see cref="IEnlistmentNotification"/>.
        /// </summary>
        /// <param name="resource">the resource to register</param>
        public static void RegisterResource(IEnlistmentNotification resource)
        {
            Resources.Value.Add(resource);
            if (System.Transactions.Transaction.Current != null)
            {
                System.Transactions.Transaction.Current.EnlistVolatile(resource, EnlistmentOptions.None);
            }
        }

        /// <summary>
        /// Unregisters a new <see cref="IEnlistmentNotification"/>.
        /// </summary>
        /// <param name="resource">the resource to unregister</param>
        public static void UnregisterResource(IEnlistmentNotification resource)
        {
            Resources.Value.Remove(resource);
        }

        private static IsolationLevel GetIsolationLevel(IsolationLevel defaultIsolationLevel)
        {
            var setting = ConfigurationManager.AppSettings["IsolationLevel"];
            IsolationLevel isolationLevel;
            return setting != null && Enum.TryParse(setting, out isolationLevel)
                ? isolationLevel
                : defaultIsolationLevel;
        }
    }
}