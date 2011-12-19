// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReaderWriterLock.cs" company="">
//   
// </copyright>
// <summary>
//   A reader-writer lock implementation that is intended to be simple, yet very
//   efficient.  In particular only 1 interlocked operation is taken for any lock
//   operation (we use spin locks to achieve this).  The spin lock is never held
//   for more than a few instructions (in particular, we never call event APIs
//   or in fact any non-trivial API while holding the spin lock).
//   Currently this ReaderWriterLock does not support recurision, however it is
//   not hard to add
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#if SILVERLIGHT || NETCF
namespace System.Threading
{
    using System.Diagnostics;

    /// <summary>
    /// A reader-writer lock implementation that is intended to be simple, yet very
    /// efficient.  In particular only 1 interlocked operation is taken for any lock 
    /// operation (we use spin locks to achieve this).  The spin lock is never held
    /// for more than a few instructions (in particular, we never call event APIs
    /// or in fact any non-trivial API while holding the spin lock).   
    /// 
    /// Currently this ReaderWriterLock does not support recurision, however it is 
    /// not hard to add 
    /// </summary>
    /// <remarks>
    /// By Vance Morrison
    /// Taken from - http://blogs.msdn.com/vancem/archive/2006/03/28/563180.aspx
    /// Code at - http://blogs.msdn.com/vancem/attachment/563180.ashx
    /// </remarks>
    public class ReaderWriterLock
    {
        // Lock specifiation for myLock:  This lock protects exactly the local fields associted
        // instance of MyReaderWriterLock.  It does NOT protect the memory associted with the
        // the events that hang off this lock (eg writeEvent, readEvent upgradeEvent).
#region Constants and Fields

        /// <summary>
        /// The my lock.
        /// </summary>
        private int myLock;

        // Who owns the lock owners > 0 => readers
        // owners = -1 means there is one writer.  Owners must be >= -1.  

        /// <summary>
        /// The number read waiters.
        /// </summary>
        private uint numReadWaiters; // maximum number of threads that can be doing a WaitOne on the readEvent

        /// <summary>
        /// The number upgrade waiters.
        /// </summary>
        private uint numUpgradeWaiters; // maximum number of threads that can be doing a WaitOne on the upgradeEvent (at most 1). 

        /// <summary>
        /// The number write waiters.
        /// </summary>
        private uint numWriteWaiters; // maximum number of threads that can be doing a WaitOne on the writeEvent 

        /// <summary>
        /// The owners.
        /// </summary>
        private int owners;

        // conditions we wait on. 

        /// <summary>
        /// The read event.
        /// </summary>
        private EventWaitHandle readEvent; // threads waiting to aquire a read lock go here (will be released in bulk)

        /// <summary>
        /// The upgrade event.
        /// </summary>
        private EventWaitHandle upgradeEvent; // thread waiting to upgrade a read lock to a write lock go here (at most one)

        /// <summary>
        /// The write event.
        /// </summary>
        private EventWaitHandle writeEvent; // threads waiting to aquire a write lock go here.

        #endregion

#region Properties

        /// <summary>
        /// Gets a value indicating whether MyLockHeld.
        /// </summary>
        private bool MyLockHeld
        {
            get
            {
                return this.myLock != 0;
            }
        }

        #endregion

#region Public Methods

        /// <summary>
        /// The acquire reader lock.
        /// </summary>
        /// <param name="millisecondsTimeout">
        /// The milliseconds timeout.
        /// </param>
        public void AcquireReaderLock(int millisecondsTimeout)
        {
            this.EnterMyLock();
            for (;;)
            {
                // We can enter a read lock if there are only read-locks have been given out
                // and a writer is not trying to get in.  
                if (this.owners >= 0 && this.numWriteWaiters == 0)
                {
                    // Good case, there is no contention, we are basically done
                    this.owners++; // Indicate we have another reader
                    break;
                }

                // Drat, we need to wait.  Mark that we have waiters and wait.  
                if (this.readEvent == null)
                {
                    // Create the needed event 
                    this.LazyCreateEvent(ref this.readEvent, false);
                    continue; // since we left the lock, start over. 
                }

                this.WaitOnEvent(this.readEvent, ref this.numReadWaiters, millisecondsTimeout);
            }

            this.ExitMyLock();
        }

        /// <summary>
        /// The acquire writer lock.
        /// </summary>
        /// <param name="millisecondsTimeout">
        /// The milliseconds timeout.
        /// </param>
        public void AcquireWriterLock(int millisecondsTimeout)
        {
            this.EnterMyLock();
            for (;;)
            {
                if (this.owners == 0)
                {
                    // Good case, there is no contention, we are basically done
                    this.owners = -1; // indicate we have a writer.
                    break;
                }

                // Drat, we need to wait.  Mark that we have waiters and wait.
                if (this.writeEvent == null)
                {
                    // create the needed event.
                    this.LazyCreateEvent(ref this.writeEvent, true);
                    continue; // since we left the lock, start over. 
                }

                this.WaitOnEvent(this.writeEvent, ref this.numWriteWaiters, millisecondsTimeout);
            }

            this.ExitMyLock();
        }

        /// <summary>
        /// The downgrade to reader lock.
        /// </summary>
        /// <param name="lockCookie">The lock cookie.</param>
        public void DowngradeFromWriterLock(ref int lockCookie)
        {
            this.EnterMyLock();
            Debug.Assert(this.owners == -1, "Downgrading when no writer lock held");
            this.owners = 1;
            this.ExitAndWakeUpAppropriateWaiters();
        }

        /// <summary>
        /// The release reader lock.
        /// </summary>
        public void ReleaseReaderLock()
        {
            this.EnterMyLock();
            Debug.Assert(this.owners > 0, "ReleasingReaderLock: releasing lock and no read lock taken");
            --this.owners;
            this.ExitAndWakeUpAppropriateWaiters();
        }

        /// <summary>
        /// The release writer lock.
        /// </summary>
        public void ReleaseWriterLock()
        {
            this.EnterMyLock();
            Debug.Assert(this.owners == -1, "Calling ReleaseWriterLock when no write lock is held");
            Debug.Assert(this.numUpgradeWaiters > 0);
            this.owners++;
            this.ExitAndWakeUpAppropriateWaiters();
        }

        /// <summary>
        /// The upgrade to writer lock.
        /// </summary>
        /// <param name="millisecondsTimeout">
        /// The milliseconds timeout.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// </exception>
        public int UpgradeToWriterLock(int millisecondsTimeout)
        {
            this.EnterMyLock();
            for (;;)
            {
                Debug.Assert(this.owners > 0, "Upgrading when no reader lock held");
                if (this.owners == 1)
                {
                    // Good case, there is no contention, we are basically done
                    this.owners = -1; // inidicate we have a writer. 
                    break;
                }

                // Drat, we need to wait.  Mark that we have waiters and wait. 
                if (this.upgradeEvent == null)
                {
                    // Create the needed event
                    this.LazyCreateEvent(ref this.upgradeEvent, false);
                    continue; // since we left the lock, start over. 
                }

                if (this.numUpgradeWaiters > 0)
                {
                    this.ExitMyLock();
                    throw new InvalidOperationException("UpgradeToWriterLock already in process.  Deadlock!");
                }

                this.WaitOnEvent(this.upgradeEvent, ref this.numUpgradeWaiters, millisecondsTimeout);
            }

            this.ExitMyLock();
            return 0;
        }

        #endregion

#region Methods

        /// <summary>
        /// The enter my lock.
        /// </summary>
        private void EnterMyLock()
        {
            if (Interlocked.CompareExchange(ref this.myLock, 1, 0) != 0)
            {
                this.EnterMyLockSpin();
            }
        }

        /// <summary>
        /// The enter my lock spin.
        /// </summary>
        private void EnterMyLockSpin()
        {
            for (int i = 0;; i++)
            {
#if !NETCF
                if (i < 3 && Environment.ProcessorCount > 1)
                {
                    Thread.SpinWait(20); // Wait a few dozen instructions to let another processor release lock. 
                }
                else
                {
                    Thread.Sleep(0); // Give up my quantum.  
                }
#else
                Thread.Sleep(0); // Give up my quantum.  
#endif

                if (Interlocked.CompareExchange(ref this.myLock, 1, 0) == 0)
                {
                    return;
                }
            }
        }

        /// <summary>
        /// Determines the appropriate events to set, leaves the locks, and sets the events. 
        /// </summary>
        private void ExitAndWakeUpAppropriateWaiters()
        {
            Debug.Assert(this.MyLockHeld);

            if (this.owners == 0 && this.numWriteWaiters > 0)
            {
                this.ExitMyLock(); // Exit before signaling to improve efficiency (wakee will need the lock)
                this.writeEvent.Set(); // release one writer. 
            }
            else if (this.owners == 1 && this.numUpgradeWaiters != 0)
            {
                this.ExitMyLock(); // Exit before signaling to improve efficiency (wakee will need the lock)
                this.upgradeEvent.Set(); // release all upgraders (however there can be at most one). 

                // two threads upgrading is a guarenteed deadlock, so we throw in that case. 
            }
            else if (this.owners >= 0 && this.numReadWaiters != 0)
            {
                this.ExitMyLock(); // Exit before signaling to improve efficiency (wakee will need the lock)
                this.readEvent.Set(); // release all readers. 
            }
            else
            {
                this.ExitMyLock();
            }
        }

        /// <summary>
        /// The exit my lock.
        /// </summary>
        private void ExitMyLock()
        {
            Debug.Assert(this.myLock != 0, "Exiting spin lock that is not held");
            this.myLock = 0;
        }

        /// <summary>
        /// A routine for lazily creating a event outside the lock (so if errors
        /// happen they are outside the lock and that we don't do much work
        /// while holding a spin lock).  If all goes well, reenter the lock and
        /// set 'waitEvent' 
        /// </summary>
        /// <param name="waitEvent">
        /// The wait Event.
        /// </param>
        /// <param name="makeAutoResetEvent">
        /// The make Auto Reset Event.
        /// </param>
        private void LazyCreateEvent(ref EventWaitHandle waitEvent, bool makeAutoResetEvent)
        {
            Debug.Assert(this.MyLockHeld);
            Debug.Assert(waitEvent == null);

            this.ExitMyLock();
            EventWaitHandle newEvent;
            if (makeAutoResetEvent)
            {
                newEvent = new AutoResetEvent(false);
            }
            else
            {
                newEvent = new ManualResetEvent(false);
            }

            this.EnterMyLock();
            waitEvent = newEvent;
        }

        /// <summary>
        /// Waits on 'waitEvent' with a timeout of 'millisceondsTimeout.  
        /// Before the wait 'numWaiters' is incremented and is restored before leaving this routine.
        /// </summary>
        /// <param name="waitEvent">
        /// The wait Event.
        /// </param>
        /// <param name="numWaiters">
        /// The num Waiters.
        /// </param>
        /// <param name="millisecondsTimeout">
        /// The milliseconds Timeout.
        /// </param>
        private void WaitOnEvent(EventWaitHandle waitEvent, ref uint numWaiters, int millisecondsTimeout)
        {
            Debug.Assert(this.MyLockHeld);

            waitEvent.Reset();
            numWaiters++;

            bool waitSuccessful = false;
            this.ExitMyLock(); // Do the wait outside of any lock 
            try
            {
#if !NETCF
                if (!waitEvent.WaitOne(millisecondsTimeout))
                {
                    throw new InvalidOperationException("ReaderWriterLock timeout expired");
                }
#else
                if (!waitEvent.WaitOne(millisecondsTimeout, false))
                {
                    throw new InvalidOperationException("ReaderWriterLock timeout expired");
                }
#endif

                waitSuccessful = true;
            }
            finally
            {
                this.EnterMyLock();
                --numWaiters;
                if (!waitSuccessful)
                {
                    // We are going to throw for some reason.  Exit myLock. 
                    this.ExitMyLock();
                }
            }
        }

#endregion
    }
}
#endif