using AppKit;
using Foundation;

namespace coinTicker
{
    [Register("AppDelegate")]
    public class AppDelegate : NSApplicationDelegate
    {
        public override void DidFinishLaunching(NSNotification notification)
        {
            var statusBar = NSStatusBar.SystemStatusBar.CreateStatusItem(NSStatusItemLength.Variable);
            var mainViewController = new MainViewController(statusBar);
        }

        public override void WillTerminate(NSNotification notification)
        {
            // Insert code here to tear down your application
        }
    }
}
