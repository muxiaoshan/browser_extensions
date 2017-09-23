using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace DiagnoseAssistant1
{
    [ComVisible(true),
    Guid("4C1D2E51-018B-4A7C-8A07-618452573E42"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface JavaScriptExtension
    {
        [DispId(1)]
        string callBHO(string s);
    }
}
