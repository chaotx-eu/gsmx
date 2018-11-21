namespace GSMXtended {
    using System;

    public class XtendedDialog : XtendedScreen {
        public XtendedDialog(Container mainContainer) : base(mainContainer) {
            TransitionOnTime = TimeSpan.FromMilliseconds(mainContainer.MillisPerAlpha);
            TransitionOffTime = TimeSpan.FromMilliseconds(mainContainer.MillisPerAlpha);
            IsPopup = true;
        }
    }
}