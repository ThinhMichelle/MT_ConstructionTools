using Autodesk.Revit.DB;
using MichelleTool.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MichelleTool
{
    public class CommonUtils
    {
        public static void ShowLoggerStatus()
        {
            if (Logger.Messages?.Count == 0)
            {
                MessageboxUtils.Show(MngLanguage.GetNameFromCurrentResource(() => Resources_ja_JP.GEN_Success), ShowType.Infomation);
            }
            //else if (Logger.Messages.All(k => k.MessageType == MessageType.Info))
            //{
            //    MessageboxUtils.Show(MngLanguage.GetNameFromCurrentResource(()=> Resources_ja_JP.GEN_Success), ShowType.Infomation);
            //}
            else
            {
                var window = WpfWindowControllerBase.GetResizeWindow(new StatusComponentUserControl(), 400, 320,
                    MngLanguage.GetNameFromCurrentResource(() => Resources_ja_JP.GEN_Information)
                    , WindownResize.RESIZE_BOTH);
                window.Show();
            }
        }
        public static void RunSafe(Action action, string overwriteMessage = null, MessageType messageTypeDef = MessageType.Error)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {

#if DEBUG
                Logger.Log(messageTypeDef, ex.StackTrace);
                Logger.Log(messageTypeDef, overwriteMessage == null ? ex.Message : overwriteMessage);
#else
                Logger.Log(messageTypeDef, overwriteMessage == null ? ex.Message : overwriteMessage);
#endif
            }
        }
        public static void ShowThreadInfo()
        {
            int currentThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
            bool isUiThread = System.Windows.Application.Current?.Dispatcher?.CheckAccess() ?? false;
            if (System.Windows.Application.Current == null) isUiThread = true;
            Debug.WriteLine($"ThreadId[{currentThreadId}] - Is UI Thread[{isUiThread}]");
        }
        private static FailureWarningAllContinue m_failure = new FailureWarningAllContinue();
        public static void RunSafeTransaction(Document doc, Action<Transaction> action, bool isSetFailure = false, bool isLogCatch = true)
        {
            Transaction tx = new Transaction(doc);
            FailureHandlingOptions faiopt = tx.GetFailureHandlingOptions();
            faiopt.SetFailuresPreprocessor(m_failure);
            faiopt.SetClearAfterRollback(true);
            tx.SetFailureHandlingOptions(faiopt);
            tx.Start("Start Transaction");
            try
            {
                action(tx);
                if (tx.GetStatus() == TransactionStatus.Started) tx.Commit();
            }
            catch (Exception ex)
            {
                if (isLogCatch) Logger.Log(MessageType.Warning, ex.Message);
                tx.RollBack();
            }
        }
       

    }
}
