﻿using System;
using System.IO;
using System.Linq;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Tests.Common;
using Melanchall.DryWetMidi.Tests.Utilities;
using NUnit.Framework;

namespace Melanchall.DryWetMidi.Tests.Core
{
    [TestFixture]
    public sealed partial class MidiFileTests
    {
        #region Test methods

        [Test]
        public void Write_Compression_NoCompression()
        {
            var midiFile = new MidiFile(
                new TrackChunk(
                    new NoteOnEvent((SevenBitNumber)100, (SevenBitNumber)50),
                    new NoteOffEvent((SevenBitNumber)100, (SevenBitNumber)50)));

            Write_Compression(
                midiFile,
                CompressionPolicy.NoCompression,
                (fileInfo1, fileInfo2) => Assert.AreEqual(fileInfo1.Length, fileInfo2.Length, "File size is invalid."));
        }

        [Test]
        public void Write_Compression_NoteOffAsSilentNoteOn()
        {
            var midiFile = new MidiFile(
                new TrackChunk(
                    new NoteOnEvent((SevenBitNumber)100, (SevenBitNumber)50),
                    new NoteOffEvent((SevenBitNumber)100, (SevenBitNumber)50)));

            Write_Compression(
                midiFile,
                CompressionPolicy.NoteOffAsSilentNoteOn,
                (fileInfo1, fileInfo2) =>
                {
                    var newMidiFile = MidiFile.Read(fileInfo2.FullName, new ReadingSettings { SilentNoteOnPolicy = SilentNoteOnPolicy.NoteOn });
                    CollectionAssert.IsEmpty(newMidiFile.GetTrackChunks().SelectMany(c => c.Events).OfType<NoteOffEvent>(), "There are Note Off events.");
                });
        }

        [Test]
        public void Write_Compression_UseRunningStatus()
        {
            var midiFile = new MidiFile(
                new TrackChunk(
                    new NoteOnEvent((SevenBitNumber)100, (SevenBitNumber)50),
                    new NoteOnEvent((SevenBitNumber)100, (SevenBitNumber)51),
                    new NoteOffEvent((SevenBitNumber)100, (SevenBitNumber)50),
                    new NoteOffEvent((SevenBitNumber)100, (SevenBitNumber)51)));

            Write_Compression(
                midiFile,
                CompressionPolicy.UseRunningStatus,
                (fileInfo1, fileInfo2) => Assert.Less(fileInfo2.Length, fileInfo1.Length, "File size is invalid."));
        }

        [Test]
        public void Write_Compression_DeleteUnknownMetaEvents()
        {
            var midiFile = new MidiFile(
                new TrackChunk(
                    new NoteOnEvent((SevenBitNumber)100, (SevenBitNumber)50),
                    new NoteOffEvent((SevenBitNumber)100, (SevenBitNumber)50),
                    new UnknownMetaEvent(254)));

            Write_Compression(
                midiFile,
                CompressionPolicy.DeleteUnknownMetaEvents,
                (fileInfo1, fileInfo2) =>
                {
                    var originalMidiFile = MidiFile.Read(fileInfo1.FullName);
                    CollectionAssert.IsNotEmpty(
                        originalMidiFile.GetTrackChunks().SelectMany(c => c.Events).OfType<UnknownMetaEvent>(),
                        "There are no Unknown Meta events in original file.");

                    var newMidiFile = MidiFile.Read(fileInfo2.FullName);
                    CollectionAssert.IsEmpty(
                        newMidiFile.GetTrackChunks().SelectMany(c => c.Events).OfType<UnknownMetaEvent>(),
                        "There are Unknown Meta events in new file.");
                });
        }

        [Test]
        public void Write_Compression_DeleteDefaultKeySignature()
        {
            var nonDefaultKeySignatureEvent = new KeySignatureEvent(-5, 1);

            var midiFile = new MidiFile(
                new TrackChunk(
                    new NoteOnEvent((SevenBitNumber)100, (SevenBitNumber)50),
                    new NoteOffEvent((SevenBitNumber)100, (SevenBitNumber)50),
                    new UnknownMetaEvent(254),
                    new KeySignatureEvent(),
                    nonDefaultKeySignatureEvent));

            Write_Compression(
                midiFile,
                CompressionPolicy.DeleteDefaultKeySignature,
                (fileInfo1, fileInfo2) =>
                {
                    var originalMidiFile = MidiFile.Read(fileInfo1.FullName);
                    Assert.AreEqual(
                        2,
                        originalMidiFile.GetTrackChunks().SelectMany(c => c.Events).OfType<KeySignatureEvent>().Count(),
                        "Invalid count of Key Signature events in original file.");

                    var newMidiFile = MidiFile.Read(fileInfo2.FullName);
                    var keySignatureEvents = newMidiFile.GetTrackChunks().SelectMany(c => c.Events).OfType<KeySignatureEvent>().ToArray();
                    Assert.AreEqual(
                        1,
                        keySignatureEvents.Length,
                        "Invalid count of Key Signature events in new file.");

                    MidiAsserts.AreEventsEqual(keySignatureEvents[0], nonDefaultKeySignatureEvent, false, "Invalid Key Signature event.");
                });
        }

        [Test]
        public void Write_Compression_DeleteDefaultSetTempo()
        {
            var nonDefaultSetTempoEvent = new SetTempoEvent(100000);

            var midiFile = new MidiFile(
                new TrackChunk(
                    new NoteOnEvent((SevenBitNumber)100, (SevenBitNumber)50),
                    new NoteOffEvent((SevenBitNumber)100, (SevenBitNumber)50),
                    new UnknownMetaEvent(254),
                    new SetTempoEvent(),
                    nonDefaultSetTempoEvent));

            Write_Compression(
                midiFile,
                CompressionPolicy.DeleteDefaultSetTempo,
                (fileInfo1, fileInfo2) =>
                {
                    var originalMidiFile = MidiFile.Read(fileInfo1.FullName);
                    Assert.AreEqual(
                        2,
                        originalMidiFile.GetTrackChunks().SelectMany(c => c.Events).OfType<SetTempoEvent>().Count(),
                        "Invalid count of Set Tempo events in original file.");

                    var newMidiFile = MidiFile.Read(fileInfo2.FullName);
                    var setTempoEvents = newMidiFile.GetTrackChunks().SelectMany(c => c.Events).OfType<SetTempoEvent>().ToArray();
                    Assert.AreEqual(
                        1,
                        setTempoEvents.Length,
                        "Invalid count of Set Tempo events in new file.");

                    MidiAsserts.AreEventsEqual(setTempoEvents[0], nonDefaultSetTempoEvent, false, "Invalid Set Tempo event.");
                });
        }

        [Test]
        public void Write_Compression_DeleteDefaultTimeSignature()
        {
            var nonDefaultTimeSignatureEvent = new TimeSignatureEvent(2, 16);

            var midiFile = new MidiFile(
                new TrackChunk(
                    new NoteOnEvent((SevenBitNumber)100, (SevenBitNumber)50),
                    new NoteOffEvent((SevenBitNumber)100, (SevenBitNumber)50),
                    new UnknownMetaEvent(254),
                    new TimeSignatureEvent(),
                    nonDefaultTimeSignatureEvent));

            Write_Compression(
                midiFile,
                CompressionPolicy.DeleteDefaultTimeSignature,
                (fileInfo1, fileInfo2) =>
                {
                    var originalMidiFile = MidiFile.Read(fileInfo1.FullName);
                    Assert.AreEqual(
                        2,
                        originalMidiFile.GetTrackChunks().SelectMany(c => c.Events).OfType<TimeSignatureEvent>().Count(),
                        "Invalid count of Time Signature events in original file.");

                    var newMidiFile = MidiFile.Read(fileInfo2.FullName);
                    var timeSignatureEvents = newMidiFile.GetTrackChunks().SelectMany(c => c.Events).OfType<TimeSignatureEvent>().ToArray();
                    Assert.AreEqual(
                        1,
                        timeSignatureEvents.Length,
                        "Invalid count of Time Signature events in new file.");

                    MidiAsserts.AreEventsEqual(timeSignatureEvents[0], nonDefaultTimeSignatureEvent, false, "Invalid Time Signature event.");
                });
        }

        [Test]
        public void Write_Compression_DeleteUnknownChunks()
        {
            var midiFile = new MidiFile(
                new TrackChunk(
                    new NoteOnEvent((SevenBitNumber)100, (SevenBitNumber)50),
                    new NoteOffEvent((SevenBitNumber)100, (SevenBitNumber)50)),
                new UnknownChunk("abcd"));

            Write_Compression(
                midiFile,
                CompressionPolicy.DeleteUnknownChunks,
                (fileInfo1, fileInfo2) =>
                {
                    var originalMidiFile = MidiFile.Read(fileInfo1.FullName);
                    CollectionAssert.IsNotEmpty(
                        originalMidiFile.Chunks.OfType<UnknownChunk>(),
                        "There are no Unknown chunks in original file.");

                    var newMidiFile = MidiFile.Read(fileInfo2.FullName);
                    CollectionAssert.IsEmpty(
                        newMidiFile.Chunks.OfType<UnknownChunk>(),
                        "There are Unknown chunks in new file.");
                });
        }

        [Test]
        public void Write_StreamIsNotDisposed()
        {
            var midiFile = new MidiFile();

            using (var streamToWrite = new MemoryStream())
            {
                midiFile.Write(streamToWrite);
                Assert.DoesNotThrow(() => { var l = streamToWrite.Length; });
            }
        }

        #endregion

        #region Private methods

        private void Write_Compression(MidiFile midiFile, CompressionPolicy compressionPolicy, Action<FileInfo, FileInfo> fileInfosAction)
        {
            MidiFileTestUtilities.Write(
                midiFile,
                filePath =>
                {
                    var fileInfo = new FileInfo(filePath);

                    MidiFileTestUtilities.Write(
                        midiFile,
                        filePath2 =>
                        {
                            var fileInfo2 = new FileInfo(filePath2);

                            fileInfosAction(fileInfo, fileInfo2);
                        },
                        new WritingSettings { CompressionPolicy = compressionPolicy });
                },
                new WritingSettings { CompressionPolicy = CompressionPolicy.NoCompression });
        }

        #endregion
    }
}