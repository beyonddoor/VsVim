﻿using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using NUnit.Framework;
using EditorUtils;

namespace Vim.UnitTest
{
    [TestFixture]
    public sealed class ReplaceModeIntegrationTest : VimTestBase
    {
        private IVimBuffer _vimBuffer;
        private ITextView _textView;
        private ITextBuffer _textBuffer;

        private void Create(params string[] lines)
        {
            _textView = CreateTextView(lines);
            _textBuffer = _textView.TextBuffer;
            _vimBuffer = Vim.CreateVimBuffer(_textView);
        }

        /// <summary>
        /// Typing forward in replace mode should overwrite 
        /// </summary>
        [Test]
        public void TypeForwardShouldReplace()
        {
            Create("hello world");
            _vimBuffer.SwitchMode(ModeKind.Replace, ModeArgument.None);
            _vimBuffer.Process("again");
            Assert.AreEqual("again world", _textBuffer.GetLine(0).GetText());
            Assert.AreEqual(5, _textView.GetCaretPoint().Position);
        }

        /// <summary>
        /// Typing past the end of the line should extend it
        /// </summary>
        [Test]
        public void TypePastEndOfLine()
        {
            Create("cat", "dog");
            _vimBuffer.SwitchMode(ModeKind.Replace, ModeArgument.None);
            _vimBuffer.Process("big tree");
            Assert.AreEqual("big tree", _textBuffer.GetLine(0).GetText());
            Assert.AreEqual("dog", _textBuffer.GetLine(1).GetText());
        }

        /// <summary>
        /// Replace mode should respect the insert count 
        /// </summary>
        [Test]
        public void Repeat_InsertText()
        {
            Create("dog");
            _vimBuffer.SwitchMode(ModeKind.Replace, ModeArgument.NewInsertWithCount(2));
            _vimBuffer.Process("cat");
            _vimBuffer.Process(VimKey.Escape);
            Assert.AreEqual("catcat", _textBuffer.GetLine(0).GetText());
            Assert.AreEqual(5, _textView.GetCaretPoint().Position);
        }

        /// <summary>
        /// When repeating the insert it should write over the remaining text vs. inserting
        /// </summary>
        [Test]
        public void Repeat_InsertOver()
        {
            Create("fish tree");
            _vimBuffer.SwitchMode(ModeKind.Replace, ModeArgument.NewInsertWithCount(2));
            _vimBuffer.Process("cat");
            _vimBuffer.Process(VimKey.Escape);
            Assert.AreEqual("catcatree", _textBuffer.GetLine(0).GetText());
            Assert.AreEqual(5, _textView.GetCaretPoint().Position);
        }
    }
}
