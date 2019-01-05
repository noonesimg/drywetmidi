﻿using System;
using System.Runtime.InteropServices;
using System.Text;
using Melanchall.DryWetMidi.Common;

namespace Melanchall.DryWetMidi.Devices
{
    internal static class MidiWinApi
    {
        #region Types

        [StructLayout(LayoutKind.Sequential)]
        internal struct MIDIHDR
        {
            public IntPtr lpData;
            public int dwBufferLength;
            public int dwBytesRecorded;
            public IntPtr dwUser;
            public int dwFlags;
            public IntPtr lpNext;
            public IntPtr reserved;
            public int dwOffset;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public int[] dwReserved;
        }

        public delegate void MidiMessageCallback(IntPtr hMidi, MidiMessage wMsg, IntPtr dwInstance, IntPtr dwParam1, IntPtr dwParam2);

        public delegate MMRESULT ErrorTextGetter(MMRESULT mmrError, StringBuilder pszText, uint cchText);

        #endregion

        #region Constants

        public const uint MaxErrorLength = 256;
        public const uint CallbackFunction = 196608;

        public static readonly int MidiHeaderSize = Marshal.SizeOf(typeof(MIDIHDR));

        #endregion

        #region Methods

        public static void ProcessMmResult(Func<MMRESULT> method, ErrorTextGetter errorTextGetter)
        {
            var mmResult = method();
            if (mmResult == MMRESULT.MMSYSERR_NOERROR)
                return;

            var stringBuilder = new StringBuilder((int)MaxErrorLength);
            var getErrorTextResult = errorTextGetter(mmResult, stringBuilder, MaxErrorLength + 1);
            if (getErrorTextResult != MMRESULT.MMSYSERR_NOERROR)
                throw new MidiDeviceException("Error occured during operation on device.");

            var errorText = stringBuilder.ToString();
            throw new MidiDeviceException(errorText);
        }

        public static byte[] UnpackSysExBytes(IntPtr headerPointer)
        {
            var header = (MIDIHDR)Marshal.PtrToStructure(headerPointer, typeof(MIDIHDR));
            var data = new byte[header.dwBytesRecorded - 1];
            Marshal.Copy(IntPtr.Add(header.lpData, 1), data, 0, data.Length);

            return data;
        }

        public static void UnpackShortEventBytes(int message, out byte statusByte, out byte firstDataByte, out byte secondDataByte)
        {
            statusByte = message.GetFourthByte();
            firstDataByte = message.GetThirdByte();
            secondDataByte = message.GetSecondByte();
        }

        #endregion
    }
}