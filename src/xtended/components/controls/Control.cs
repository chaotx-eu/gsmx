namespace GSMXtended {
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;
    using System.Collections.Generic;

    /// A control item which may be contained by
    /// a container
    public class Control : ScreenComponent {
        // TODO => there is no need for MenuItem to inherit from Control,
        //         maybe let it inherit directly from ScreenComponent and
        //         give the Control class other more dedicatd functionality
    }
}