using NUnit.Framework;
using System;
using System.IO;

namespace SMPL.Props
{
    [TestFixture]
    public class PropertyFileTests
    {
        [Test]
        public void Test000Setup()
        {
            if (File.Exists("Test.smpl"))
                File.Delete("Test.smpl");
        }

        [Test]
        public void Test001InitFile()
        {
            var propFile = new PropertyFile("Test.smpl");
        }

        [Test]
        public void Test002LoadFile()
        {
            var propFile = new PropertyFile("Test.smpl");
            propFile.Load();
        }

        [Test]
        public void Test003LoadEntry()
        {
            var propFile = new PropertyFile("Test.smpl");
            propFile.Load();
            var props = propFile.Properties;
            int number = props.Load("number", 11);
            Assert.AreEqual(number, 11);
        }

        [Test]
        public void Test004Save()
        {
            var propFile = new PropertyFile("Test.smpl");
            propFile.Load();
            var props = propFile.Properties;
            int number = props.Load("number", 12);
            propFile.Save();
        }

        [Test]
        public void Test005LoadFromSave()
        {
            var propFile = new PropertyFile("Test.smpl");
            propFile.Load();
            var props = propFile.Properties;
            int number = props.Load("number", 0);
            Assert.AreEqual(number, 12);
        }

        [Test]
        public void Test006LoadAndSet()
        {
            var propFile = new PropertyFile("Test.smpl");
            propFile.Load();
            var props = propFile.Properties;
            props.Set("number", 14);
            propFile.Save();
            propFile = new PropertyFile("Test.smpl");
            propFile.Load();
            props = propFile.Properties;
            int number = props.Load("number", 0);
            Assert.AreEqual(number, 14);
        }

        [Test]
        public void Test007AddEntry()
        {
            var propFile = new PropertyFile("Test.smpl");
            propFile.Load();
            var props = propFile.Properties;
            props.AddEntry(new PropertyEntry("word", "hello"));
            propFile.Save();

            propFile = new PropertyFile("Test.smpl");
            propFile.Load();
            props = propFile.Properties;
            string word = props.Load("word", string.Empty);
            Assert.AreEqual(word, "hello");
        }

        [Test]
        public void Test008RemoveEntry()
        {
            var propFile = new PropertyFile("Test.smpl");
            propFile.Load();
            var props = propFile.Properties;
            props.Remove("number");
            propFile.Save();

            propFile = new PropertyFile("Test.smpl");
            propFile.Load();
            props = propFile.Properties;
            Assert.IsFalse(props.Contains("number"));
        }

        [Test]
        public void Test009AddArray()
        {
            var propFile = new PropertyFile("Test.smpl");
            propFile.Load();
            var props = propFile.Properties;
            for (int i = 0; i < 10; i++)
            {
                props.AddArrayValue(i);
            }
            propFile.Save();
        }

        [Test]
        public void Test010LoadArray()
        {
            var propFile = new PropertyFile("Test.smpl");
            propFile.Load();
            var props = propFile.Properties;
            int[] intArray = props.LoadArray<int>();
            Assert.IsTrue(intArray.Length == 10);
            Assert.AreEqual(intArray[3], 3);
        }

        [Test]
        public void Test011RemoveArray()
        {
            var propFile = new PropertyFile("Test.smpl");
            propFile.Load();
            var props = propFile.Properties;
            var entry = props.GetArrayEntry<int>(4);
            props.Remove(entry);
            propFile.Save();

            propFile = new PropertyFile("Test.smpl");
            propFile.Load();
            props = propFile.Properties;
            int[] intArray = props.LoadArray<int>();
            Assert.IsTrue(intArray.Length > 0);
            Assert.AreEqual(intArray[4], 5);
        }

        [Test]
        public void Test012LoadList()
        {
            var propFile = new PropertyFile("Test.smpl");
            propFile.Load();
            var props = propFile.Properties;
            var list = props.LoadList("TheList");
            propFile.Save();

            propFile = new PropertyFile("Test.smpl");
            propFile.Load();
            props = propFile.Properties;
            Assert.IsNotNull(list);
        }

        [Test]
        public void Test013RemoveList()
        {
            var propFile = new PropertyFile("Test.smpl");
            propFile.Load();
            var props = propFile.Properties;
            var list = props.GetList("TheList");
            props.Remove(list);
            propFile.Save();

            propFile = new PropertyFile("Test.smpl");
            propFile.Load();
            props = propFile.Properties;
            list = props.GetList("TheList");
            Assert.IsNull(list);
        }

        [Test]
        public void Test014Comments()
        {
            var propFile = new PropertyFile("TestComments.smpl");
            propFile.Delete();
            propFile.Load();
            var props = propFile.Properties;
            props.Load("Rawr", "Derp", "Hello");
            props.AddArrayValue<string>("Herp", "Hello2");
            props.GetArrayEntry<string>(0).Loaded = true;
            props.LoadList("TheList2", "Hello3", "Hello4");
            props.AddEntry(new CommentEntry("Hello5")).Loaded = true;
            props.AddEntry(new BlockCommentEntry("Hello6")).Loaded = true;
            propFile.Save();

            propFile = new PropertyFile("TestComments.smpl");
            propFile.Load();
            props = propFile.Properties;
            var theEntry = props.GetEntry("Rawr");
            Assert.AreEqual(theEntry.Comment, "Hello");
            var theArrayEntry = props.GetArrayEntry<string>(0);
            Assert.AreEqual(theArrayEntry.Comment, "Hello2");
            var theList = props.GetList("TheList2");
            Assert.AreEqual(theList.Comment, "Hello3");
            Assert.AreEqual(theList.CloseComment, "Hello4");
            int commentIndex = props.IndexOf(theList) + 1;
            var theComment = props.Entries[commentIndex] as CommentEntry;
            Assert.AreEqual(theComment.StringValue, "Hello5");
            Assert.AreEqual(theComment.Comment, "Hello5");
            int blockCommentIndex = commentIndex + 1;
            var theBlockComment = props.Entries[blockCommentIndex] as BlockCommentEntry;
            Assert.AreEqual(theBlockComment.StringValue, "Hello6");
            Assert.AreEqual(theBlockComment.Comment, "Hello6");
        }

        [Test]
        public void Test015MultilineBlockComments()
        {
            var propFile = new PropertyFile("TestBlockComments.smpl");
            propFile.Delete();
            propFile.Load();
            var props = propFile.Properties;
            props.AddEntry(new BlockCommentEntry("Hello\nHello2")).Loaded = true;
            propFile.Save();

            propFile = new PropertyFile("TestBlockComments.smpl");
            propFile.Load();
            props = propFile.Properties;
            var theBlockComment = props.Entries[0] as BlockCommentEntry;
            Assert.AreEqual(theBlockComment.StringValue, "Hello\nHello2");
            Assert.AreEqual(theBlockComment.Comment, "Hello\nHello2");
        }

        [Test]
        public void Test016NewLines()
        {
            var propFile = new PropertyFile("TestNewLines.smpl");
            propFile.Delete();
            propFile.Load();
            var props = propFile.Properties;
            int num1 = props.Load("num1", 100);
            int num2 = props.Load("num2", 102);
            props.NewLine();
            int num3 = props.Load("num3", 103);
            propFile.Save();

            propFile = new PropertyFile("TestNewLines.smpl");
            propFile.Load();
            props = propFile.Properties;
            num1 = props.Load("num1", 100);
            num2 = props.Load("num2", 102);
            props.NewLine();
            num3 = props.Load("num3", 103);

            Assert.IsTrue(props[2] is NewLineEntry);
        }

        [Test]
        public void Test017Concatenations()
        {
            Assert.IsTrue(true);
        }

        [Test]
        public void Test018ConcatWithCommentsBetween()
        {
            Assert.IsTrue(true);
        }

        [Test]
        public void Test019ChangeComments()
        {
            Assert.IsTrue(true);
        }

        [Test]
        public void Test020ClearList()
        {
            Assert.IsTrue(true);
        }

        [Test]
        public void Test021OrderEntriesByLoadOrder()
        {
            Assert.IsTrue(true);
        }

        [Test]
        public void Test022CompressFile()
        {
            Assert.IsTrue(true);
        }

        [Test]
        public void Test023LoadCompressedFile()
        {
            Assert.IsTrue(true);
        }
    }
}

