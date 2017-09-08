using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Win32;
using SHDocVw;

namespace DiagnoseAssistant1
{
    //键盘监听。无法监听到
    public class KeyboardHook
    {
        static Log log = new Log();
        #region Hook Variables

        public struct KeyboardHookStruct
        {
            public long vkCode;
            public long scanCode;
            public int flags;
            public long time;
            public long dwExtraInfo;

        };
        private const int HC_ACTION = 0;
        private const int LLKHF_EXTENDED = 0x01;
        private const int LLKHF_ALTDOWN = 0x20;
        private const long VK_T = 0x54;
        private const long VK_P = 0x50;
        private const long VK_W = 0x57;
        private const int VK_TAB = 0x9;
        private const int VK_CONTROL = 0x11; // tecla Ctrl
        //Private Const VK_MENU As Long = &H12        ' tecla Alt
        private const int VK_ESCAPE = 0x1B;

        private const int WH_KEYBOARD_LL = 13;

        protected IntPtr KeyboardHandle = IntPtr.Zero;
        private static int mHook;
        #endregion

        #region Hook Functions

        private delegate int KeyboardHookProcDelegate(int nCode, int wParam, int lParam);

        public void Install()
        {
            log.WriteLog("inside Install");

            //mHook = SetWindowsHookEx(WH_KEYBOARD_LL, new KeyboardHookProcDelegate(KeyboardHookProc), ((Marshal.GetHINSTANCE(System.Reflection.Assembly.GetExecutingAssembly().GetModules()[0])).ToInt32()), 0);
            mHook = SetWindowsHookEx(WH_KEYBOARD_LL, new KeyboardHookProcDelegate(KeyboardHookProc), (LoadLibrary("User32")).ToInt32(), 0);            
            if (mHook != 0)
            {
                log.WriteLog("unable to install");
            }
            else
                log.WriteLog("Success in Installing!");
        }


        public static int KeyboardHookProc(int nCode, int wParam, int lParam)
        {
            KeyboardHookStruct HookStruct;
            int ret = 0;
            log.WriteLog("inside HookProc");

            HookStruct = ((KeyboardHookStruct)Marshal.PtrToStructure(new IntPtr(lParam), typeof(KeyboardHookStruct)));
            long vkCode = HookStruct.vkCode;
            int flag = HookStruct.flags;

            if (nCode == HC_ACTION)
            {
                log.WriteLog("inside nCode==HC_ACTION");
                //System.Windows.Forms.Keys  KeyPressed = (Keys)wParam.ToInt32();

                // Insert + T  OR Alt + T
                if (vkCode == VK_T)
                {
                    log.WriteLog(" found T ");
                    if ((GetAsyncKeyState(VK_CONTROL) & 0x8000) != 0)
                    {
                        log.WriteLog("Play");
                        ret = 1;
                    }
                    else if ((flag & LLKHF_ALTDOWN) != 0)
                    {
                        log.WriteLog("Stop");
                        ret = 1;
                    }
                }
                else if (vkCode == VK_P)
                {
                    log.WriteLog(" found P ");
                    if ((GetAsyncKeyState(VK_CONTROL) & 0x8000) != 0)
                    { //Insert + P
                        log.WriteLog("pause");
                        ret = 1;
                    }
                }
                else if (vkCode == VK_W)
                {
                    log.WriteLog(" found W ");
                    if ((GetAsyncKeyState(VK_CONTROL) & 0x8000) != 0)
                    {
                        // Alt + S
                        log.WriteLog("Save");
                        ret = 1;
                    }
                }

                /*/	CopyMemory(HookStruct,lParam,sizeof(HookStruct);
                    if(IsHooked(HookStruct))
                        {
                            MyKeyboardProc=1;
                        }*/

                //	MyKeyboardProc=1;
            }
            if (ret == 0)
            {
                ret = CallNextHookEx(mHook, nCode, wParam, lParam);
            }
            return ret;
        }


        public void UnInstall()
        {
            log.WriteLog("inside UnInstall");
            if (mHook != 0)
            {
                log.WriteLog("inside mHook!=0 ");
                UnhookWindowsHookEx(mHook);
            }
        }


        #endregion

        #region Hook Win32Imports

        // Win32: SetWindowsHookEx()
        [DllImport("user32.dll")]
        private static extern int SetWindowsHookEx(int code, KeyboardHookProcDelegate func, int hInstance, int threadID);     

        // Win32: UnhookWindowsHookEx()
        [DllImport("user32.dll")]
        protected static extern bool UnhookWindowsHookEx(int hhook);


        // Win32: CallNextHookEx()
        [DllImport("user32.dll")]
        protected static extern int CallNextHookEx(int hhook,
            int code, int wParam, int lParam);

        // Win32 : GetAsyncKeyState
        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);

        /// <summary>
        /// Loads the library.
        /// </summary>
        /// <param name="lpFileName">Name of the library</param>
        /// <returns>A handle to the library</returns>
        [DllImport("kernel32.dll")]
        static extern IntPtr LoadLibrary(string lpFileName);

        #endregion
    }
}
