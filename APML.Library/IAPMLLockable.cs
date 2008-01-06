/// Copyright 2007 Faraday Media
/// 
/// Licensed under the Apache License, Version 2.0 (the "License"); 
/// you may not use this file except in compliance with the License. 
/// You may obtain a copy of the License at 
/// 
///   http://www.apache.org/licenses/LICENSE-2.0 
/// 
/// Unless required by applicable law or agreed to in writing, software 
/// distributed under the License is distributed on an "AS IS" BASIS, 
/// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
/// See the License for the specific language governing permissions and 
/// limitations under the License.
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace APML {
  /// <summary>
  /// Interface signalling a class that supports locking hierarchies.
  /// </summary>
  public interface IAPMLLockable {
    /// <summary>
    /// Creates a read session with an APML object.
    /// </summary>
    /// <returns>the read session</returns>
    IAPMLReadSession OpenReadSession();

    /// <summary>
    /// Creates a write session with an APML object.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="APMLPossibleDeadlockException">if a read session is already held</exception>
    IAPMLWriteSession OpenWriteSession();
  }

  /// <summary>
  /// Exception indicating that an operation may have resulted in a deadlock, so was not
  /// allowed.
  /// </summary>
  public class APMLPossibleDeadlockException : ApplicationException {
    public APMLPossibleDeadlockException() {
    }

    public APMLPossibleDeadlockException(string pMessage) : base(pMessage) {
    }
  }

  /// <summary>
  /// Defines the base interface that APML access sessions (such as read and write)
  /// inherit from.
  /// </summary>
  public interface IAPMLAccessSession : IDisposable {
  }

  /// <summary>
  /// Defines a session that allows read access to an APMLDocument.
  /// </summary>
  public interface IAPMLReadSession : IAPMLAccessSession {
  }

  /// <summary>
  /// Defines a session that allows write access to an APMLDocument.
  /// </summary>
  public interface IAPMLWriteSession : IAPMLAccessSession {
  }

  /// <summary>
  /// Delegate called when a write operation has completed (ie, all write locks have been released).
  /// </summary>
  /// <param name="pHelper">the helper initiating the event</param>
  public delegate void WriteCompletedEventHandler(APMLLockHelper pHelper);

  /// <summary>
  /// Delegate called when a write session has completed.
  /// </summary>
  /// <param name="pSession">the session that is completed</param>
  public delegate void WriteSessionCompletedEventHandler(APMLWriteSession pSession);

  /// <summary>
  /// Helper class that locking calls can be delegated to.
  /// </summary>
  public sealed class APMLLockHelper : IAPMLLockable {
    private ReaderWriterLock mLock = new ReaderWriterLock();

    #region Pre-generated Sessions
    private IAPMLReadSession mReadSession;
    private APMLWriteSession mWriteReleaseSession;
    private NullAPMLSession mNullSession;
    #endregion

    #region Events
    /// <summary>
    /// Event raised when a write operation has completed (ie, all write locks have been released)
    /// </summary>
    public event WriteCompletedEventHandler WriteCompleted;
    #endregion

    #region Constructor
    public APMLLockHelper() {
      mReadSession = new APMLReadSession(mLock);
      mWriteReleaseSession = new APMLWriteSession(mLock);
      //mWriteDowngradeSession = new APMLWriteSession(mLock);
      mNullSession = new NullAPMLSession();

      mWriteReleaseSession.WriteCompleted += new WriteSessionCompletedEventHandler(WriterLock_SessionCompleted);
    }
    #endregion

    #region IAPMLLockable Members
    public IAPMLReadSession OpenReadSession() {
      // Check if we already have a write lock. If we do, then just ignore this.
      if (mLock.IsWriterLockHeld) {
        return mNullSession;
      }

      // Take a reader lock, and return a session that will dispose it
      mLock.AcquireReaderLock(-1);
      return mReadSession;
    }

    public IAPMLWriteSession OpenWriteSession() {
//      if (mLock.IsReaderLockHeld) {  // Check if we already have a read lock. If we do, then upgrade it.
//        APMLWriteSession session = new APMLWriteSession(mLock, mLock.UpgradeToWriterLock(-1));
//        session.WriteCompleted += new WriteSessionCompletedEventHandler(WriterLock_SessionCompleted);
//        return session;
      if (mLock.IsReaderLockHeld) { // Check if we already have a read lock. If we do, then throw an exception, since that could lead to a deadlock
        throw new APMLPossibleDeadlockException("Read session already held");
      } else if (mLock.IsWriterLockHeld) { // If we already hold a write lock, then do nothing
        return mNullSession;
      } else {   // Otherwise, acquire a write lock
        mLock.AcquireWriterLock(-1);

        return mWriteReleaseSession;
      }
    }
    #endregion

    #region Event Handlers
    private void WriterLock_SessionCompleted(APMLWriteSession pSession) {
      WriteCompletedEventHandler handler = WriteCompleted;

      if (handler != null) {
        handler(this);
      }
    }
    #endregion
  }

  /// <summary>
  /// NullSession used when no locking is needed. The Dipose method
  /// of this class does nothing.
  /// </summary>
  public class NullAPMLSession : IAPMLReadSession, IAPMLWriteSession {
    public void Dispose() {
    }
  }

  public abstract class APMLAccessSession : IAPMLAccessSession {
    protected ReaderWriterLock mLock;

    public APMLAccessSession(ReaderWriterLock pLock) {
      mLock = pLock;
    }

    public abstract void Dispose();
  }

  public class APMLReadSession : APMLAccessSession, IAPMLReadSession {
    public APMLReadSession(ReaderWriterLock pLock)
      : base(pLock) {
    }

    public override void Dispose() {
      mLock.ReleaseReaderLock();
    }
  }

  public class APMLWriteSession : APMLAccessSession, IAPMLWriteSession {
    private bool mReleaseOnExit;
    private LockCookie mCookie;

    public APMLWriteSession(ReaderWriterLock pLock)
      : base(pLock) {
      mReleaseOnExit = true;
    }

    public APMLWriteSession(ReaderWriterLock pLock, LockCookie pCookie)
      : base(pLock) {
      mReleaseOnExit = false;
      mCookie = pCookie;
    }

    public event WriteSessionCompletedEventHandler WriteCompleted;

    public override void Dispose() {
      if (mReleaseOnExit) {
        mLock.ReleaseWriterLock();
      } else {
        mLock.DowngradeFromWriterLock(ref mCookie);
      }

      WriteSessionCompletedEventHandler handler = WriteCompleted;
      if (handler != null) {
        handler(this);
      }
    }
  }
}
