﻿using System;
using System.Collections.Generic;
using System.IO;
using Melanchall.DryWetMidi.Smf;
using Melanchall.DryWetMidi.Tests.Utilities;
using NUnit.Framework;

namespace Melanchall.DryWetMidi.Tests.Smf
{
    [TestFixture]
    public sealed class MidiFileTests
    {
        #region Constants

        private static class DirectoriesNames
        {
            public const string InvalidChannelEventParameterValue = "Invalid Channel Event Parameter Value";
            public const string InvalidChunkSize = "Invalid Chunk Size";
            public const string InvalidKeySignatureKey = "Invalid Key Signature Key";
            public const string InvalidKeySignatureScale = "Invalid Key Signature Scale";
            public const string InvalidMetaEventParameterValue = "Invalid Meta Event Parameter Value";
            public const string InvalidSmpteFrames = "Invalid SMPTE Frames";
            public const string NoEndOfTrack = "No End of Track";
            public const string NoHeaderChunk = "No Header Chunk";
            public const string NotEnoughBytes = "Not Enough Bytes";
            public const string UnexpectedRunningStatus = "Unexpected Running Status";
            public const string UnknownChannelEvent = "Unknown Channel Event";
            public const string UnknownFileFormat = "Unknown File Format";
        }

        private const string InvalidFilesPath = @"..\..\..\Resources\MIDI files\Invalid";
        private const string ValidFilesPath = @"..\..\..\Resources\MIDI files\Valid";

        #endregion

        #region Properties

        public TestContext TestContext { get; set; }

        #endregion

        #region Test methods

        #region Read_Invalid

        [Test]
        [Description("Read MIDI file with invalid channel event parameter value and treat that as error.")]
        public void Read_InvalidChannelEventParameterValue_Abort()
        {
            ReadFilesWithException<InvalidChannelEventParameterValueException>(
                DirectoriesNames.InvalidChannelEventParameterValue,
                new ReadingSettings
                {
                    InvalidChannelEventParameterValuePolicy = InvalidChannelEventParameterValuePolicy.Abort
                });
        }

        [Test]
        [Description("Read MIDI file with invalid channel event parameter value and read such values taking lower 7 bits.")]
        public void Read_InvalidChannelEventParameterValue_ReadValid()
        {
            ReadInvalidFiles(
                DirectoriesNames.InvalidChannelEventParameterValue,
                new ReadingSettings
                {
                    InvalidChannelEventParameterValuePolicy = InvalidChannelEventParameterValuePolicy.ReadValid
                });
        }

        [Test]
        [Description("Read MIDI file with invalid channel event parameter value and snap such values to valid limits (0-127).")]
        public void Read_InvalidChannelEventParameterValue_SnapToLimits()
        {
            ReadInvalidFiles(
                DirectoriesNames.InvalidChannelEventParameterValue,
                new ReadingSettings
                {
                    InvalidChannelEventParameterValuePolicy = InvalidChannelEventParameterValuePolicy.SnapToLimits
                });
        }

        [Test]
        [Description("Read MIDI file with invalid size of a chunk and treat that as error.")]
        public void Read_InvalidChunkSize_Abort()
        {
            ReadFilesWithException<InvalidChunkSizeException>(
                DirectoriesNames.InvalidChunkSize,
                new ReadingSettings
                {
                    InvalidChunkSizePolicy = InvalidChunkSizePolicy.Abort
                });
        }

        [Test]
        [Description("Read MIDI file with invalid size of a chunk and ignore that.")]
        public void Read_InvalidChunkSize_Ignore()
        {
            ReadInvalidFiles(
                DirectoriesNames.InvalidChunkSize,
                new ReadingSettings
                {
                    InvalidChunkSizePolicy = InvalidChunkSizePolicy.Ignore
                });
        }

        [Test]
        [Description("Read MIDI file with invalid key of a Key Signature event and treat that as error.")]
        public void Read_InvalidKeySignatureKey_Abort()
        {
            ReadFilesWithException<InvalidMetaEventParameterValueException>(
                DirectoriesNames.InvalidKeySignatureKey,
                new ReadingSettings
                {
                    InvalidMetaEventParameterValuePolicy = InvalidMetaEventParameterValuePolicy.Abort
                });
        }

        [Test]
        [Description("Read MIDI file with invalid key of a Key Signature event and snap the value to valid limits.")]
        public void Read_InvalidKeySignatureKey_SnapToLimits()
        {
            ReadInvalidFiles(
                DirectoriesNames.InvalidKeySignatureKey,
                new ReadingSettings
                {
                    InvalidMetaEventParameterValuePolicy = InvalidMetaEventParameterValuePolicy.SnapToLimits
                });
        }

        [Test]
        [Description("Read MIDI file with invalid scale of a Key Signature event and treat that as error.")]
        public void Read_InvalidKeySignatureScale_Abort()
        {
            ReadFilesWithException<InvalidMetaEventParameterValueException>(
                DirectoriesNames.InvalidKeySignatureScale,
                new ReadingSettings
                {
                    InvalidMetaEventParameterValuePolicy = InvalidMetaEventParameterValuePolicy.Abort
                });
        }

        [Test]
        [Description("Read MIDI file with invalid scale of a Key Signature event and snap the value to valid limits.")]
        public void Read_InvalidKeySignatureScale_SnapToLimits()
        {
            ReadInvalidFiles(
                DirectoriesNames.InvalidKeySignatureScale,
                new ReadingSettings
                {
                    InvalidMetaEventParameterValuePolicy = InvalidMetaEventParameterValuePolicy.SnapToLimits
                });
        }

        [Test]
        [Description("Read MIDI file with invalid parameter of a meta event and treat that as error.")]
        public void Read_InvalidMetaEventParameterValue_Abort()
        {
            ReadFilesWithException<InvalidMetaEventParameterValueException>(
                DirectoriesNames.InvalidMetaEventParameterValue,
                new ReadingSettings
                {
                    InvalidMetaEventParameterValuePolicy = InvalidMetaEventParameterValuePolicy.Abort
                });
        }

        [Test]
        [Description("Read MIDI file with invalid parameter of a meta event and snap the value to valid limits.")]
        public void Read_InvalidMetaEventParameterValue_SnapToLimits()
        {
            ReadInvalidFiles(
                DirectoriesNames.InvalidMetaEventParameterValue,
                new ReadingSettings
                {
                    InvalidMetaEventParameterValuePolicy = InvalidMetaEventParameterValuePolicy.SnapToLimits
                });
        }

        [Test]
        [Description("Read MIDI file with invalid SMPTE frames number and treat that as error.")]
        public void Read_InvalidSmpteFrames_Abort()
        {
            ReadFilesWithException<InvalidMetaEventParameterValueException>(
                DirectoriesNames.InvalidSmpteFrames,
                new ReadingSettings
                {
                    InvalidMetaEventParameterValuePolicy = InvalidMetaEventParameterValuePolicy.Abort
                });
        }

        [Test]
        [Description("Read MIDI file with invalid SMPTE frames number and snap the value to valid limits.")]
        public void Read_InvalidSmpteFrames_SnapToLimits()
        {
            ReadInvalidFiles(
                DirectoriesNames.InvalidSmpteFrames,
                new ReadingSettings
                {
                    InvalidMetaEventParameterValuePolicy = InvalidMetaEventParameterValuePolicy.SnapToLimits
                });
        }

        [Test]
        [Description("Read MIDI file without End Of Track event and treat that as error.")]
        public void Read_NoEndOfTrack_Abort()
        {
            ReadFilesWithException<MissedEndOfTrackEventException>(
                DirectoriesNames.NoEndOfTrack,
                new ReadingSettings
                {
                    MissedEndOfTrackPolicy = MissedEndOfTrackPolicy.Abort
                });
        }

        [Test]
        [Description("Read MIDI file without End Of Track event and ignore that.")]
        public void Read_NoEndOfTrack_Ignore()
        {
            ReadInvalidFiles(
                DirectoriesNames.NoEndOfTrack,
                new ReadingSettings
                {
                    MissedEndOfTrackPolicy = MissedEndOfTrackPolicy.Ignore
                });
        }

        [Test]
        [Description("Read MIDI file without header chunk and treat that as error.")]
        public void Read_NoHeaderChunk_Abort()
        {
            ReadFilesWithException<NoHeaderChunkException>(
                DirectoriesNames.NoHeaderChunk,
                new ReadingSettings
                {
                    NoHeaderChunkPolicy = NoHeaderChunkPolicy.Abort,
                    NotEnoughBytesPolicy = NotEnoughBytesPolicy.Ignore,
                    InvalidChunkSizePolicy = InvalidChunkSizePolicy.Ignore
                });
        }

        [Test]
        [Description("Read MIDI file without header chunk and ignore that.")]
        public void Read_NoHeaderChunk_Ignore()
        {
            ReadInvalidFiles(
                DirectoriesNames.NoHeaderChunk,
                new ReadingSettings
                {
                    NoHeaderChunkPolicy = NoHeaderChunkPolicy.Ignore,
                    NotEnoughBytesPolicy = NotEnoughBytesPolicy.Ignore,
                    InvalidChunkSizePolicy = InvalidChunkSizePolicy.Ignore
                });
        }

        [Test]
        [Description("Read MIDI file in case of not enough bytes to read an object and treat that as error.")]
        public void Read_NotEnoughBytes_Abort()
        {
            ReadFilesWithException<NotEnoughBytesException>(
                DirectoriesNames.NotEnoughBytes,
                new ReadingSettings
                {
                    NotEnoughBytesPolicy = NotEnoughBytesPolicy.Abort
                });
        }

        [Test]
        [Description("Read MIDI file in case of not enough bytes to read an object and ignore that.")]
        public void Read_NotEnoughBytes_Ignore()
        {
            ReadInvalidFiles(
                DirectoriesNames.NotEnoughBytes,
                new ReadingSettings
                {
                    NotEnoughBytesPolicy = NotEnoughBytesPolicy.Ignore,
                    InvalidChunkSizePolicy = InvalidChunkSizePolicy.Ignore,
                    NoHeaderChunkPolicy = NoHeaderChunkPolicy.Ignore
                });
        }

        [Test]
        [Description("Read MIDI file with unexpected running status.")]
        public void Read_UnexpectedRunningStatus()
        {
            ReadFilesWithException<UnexpectedRunningStatusException>(
                DirectoriesNames.UnexpectedRunningStatus,
                new ReadingSettings
                {
                    InvalidChannelEventParameterValuePolicy = InvalidChannelEventParameterValuePolicy.ReadValid,
                    InvalidChunkSizePolicy = InvalidChunkSizePolicy.Ignore
                });
        }

        [Test]
        [Description("Read MIDI file with unknow channel event.")]
        public void Read_UnknownChannelEvent()
        {
            ReadFilesWithException<UnknownChannelEventException>(
                DirectoriesNames.UnknownChannelEvent,
                new ReadingSettings
                {
                    InvalidChannelEventParameterValuePolicy = InvalidChannelEventParameterValuePolicy.ReadValid
                });
        }

        [Test]
        [Description("Read MIDI file with unknown format and treat that as error.")]
        public void Read_UnknownFileFormat_Abort()
        {
            ReadFilesWithException<UnknownFileFormatException>(
                DirectoriesNames.UnknownFileFormat,
                new ReadingSettings
                {
                    UnknownFileFormatPolicy = UnknownFileFormatPolicy.Abort
                });
        }

        [Test]
        [Description("Read MIDI file with unknown format and ignore that.")]
        public void Read_UnknownFileFormat_Ignore()
        {
            ReadInvalidFiles(
                DirectoriesNames.UnknownFileFormat,
                new ReadingSettings
                {
                    UnknownFileFormatPolicy = UnknownFileFormatPolicy.Ignore,
                    InvalidChunkSizePolicy = InvalidChunkSizePolicy.Ignore,
                    NotEnoughBytesPolicy = NotEnoughBytesPolicy.Ignore
                });
        }

        #endregion

        #region Clone

        [Test]
        [Description("Check whether a clone of a MIDI file equals to the original file.")]
        public void Clone_Read()
        {
            foreach (var filePath in GetValidFiles())
            {
                var midiFile = MidiFile.Read(filePath);
                var clonedMidiFile = midiFile.Clone();

                Assert.IsTrue(MidiFileEquality.AreEqual(clonedMidiFile, midiFile, true),
                              $"Clone of the '{filePath}' doesn't equal to the original file.");
            }
        }

        #endregion

        #endregion

        #region Private methods

        private void ReadFilesWithException<TException>(string directoryName, ReadingSettings readingSettings)
            where TException : Exception
        {
            foreach (var filePath in GetInvalidFiles(directoryName))
            {
                Assert.Throws<TException>(() => MidiFile.Read(filePath, readingSettings), $"Exception is not thrown for {filePath}.");
            }
        }

        private void ReadInvalidFiles(string directoryName, ReadingSettings readingSettings)
        {
            foreach (var filePath in GetInvalidFiles(directoryName))
            {
                Assert.DoesNotThrow(() => MidiFile.Read(filePath, readingSettings));
            }
        }

        private IEnumerable<string> GetInvalidFiles(string directoryName)
        {
            return Directory.GetFiles(GetInvalidFilesDirectory(directoryName));
        }

        private IEnumerable<string> GetValidFiles()
        {
            return Directory.GetFiles(GetValidFilesDirectory(), "*.*", SearchOption.AllDirectories);
        }

        private string GetInvalidFilesDirectory(string directoryName)
        {
            return Path.Combine(TestContext.CurrentContext.TestDirectory, InvalidFilesPath, directoryName);
        }

        private string GetValidFilesDirectory()
        {
            return Path.Combine(TestContext.CurrentContext.TestDirectory, ValidFilesPath);
        }

        #endregion
    }
}
