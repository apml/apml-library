using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace APML.Test {
  [TestFixture]
  public class LockHelperTest {
    private APMLLockHelper mHelper;
    private bool mLockCompletedCalled = false;

    [Test]
    public void TestWriteCompletedForSimpleWriteSession() {
      PrepareWriteCompletedTest();

      using (mHelper.OpenWriteSession()) {
      }

      Assert.IsTrue(mLockCompletedCalled, "Should have indicated lock completed");
    }

    [Test]
    public void TestExceptionForWriteInReadSession() {
      mLockCompletedCalled = false;

      APMLLockHelper helper = new APMLLockHelper();
      
      using (helper.OpenReadSession()) {
        try {
          using (helper.OpenWriteSession()) {
          }

          Assert.Fail("Should have thrown APMLPossibleDeadlockException");
        } catch (APMLPossibleDeadlockException) {
          
        }
      }
    }

    [Test]
    public void TestWriteCompletedForDoubleWriteSession() {
      mLockCompletedCalled = false;

      APMLLockHelper helper = new APMLLockHelper();
      helper.WriteCompleted += new WriteCompletedEventHandler(LockHelper_WriteCompleted);

      using (helper.OpenWriteSession()) {
        using (helper.OpenWriteSession()) {
        }

        Assert.IsFalse(mLockCompletedCalled, "Should not have indicated lock completed");
      }
      
      Assert.IsTrue(mLockCompletedCalled, "Should have indicated lock completed");
    }


    #region Helper Methods
    private void PrepareWriteCompletedTest() {
      mLockCompletedCalled = false;

      mHelper = new APMLLockHelper();
      mHelper.WriteCompleted += new WriteCompletedEventHandler(LockHelper_WriteCompleted);
    }
    #endregion

    #region Event Listeners
    private void LockHelper_WriteCompleted(APMLLockHelper pHelper) {
      mLockCompletedCalled = true;
    }
    #endregion
  }
}
