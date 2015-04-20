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
            int number = props.LoadEntry("number", 11);
            Assert.AreEqual(number, 11);
        }

        [Test]
        public void Test004Save()
        {
            var propFile = new PropertyFile("Test.smpl");
            propFile.Load();
            var props = propFile.Properties;
            int number = props.LoadEntry("number", 12);
            propFile.Save();
        }

        [Test]
        public void Test005LoadFromSave()
        {
            var propFile = new PropertyFile("Test.smpl");
            propFile.Load();
            var props = propFile.Properties;
            int number = props.LoadEntry("number", 0);
            Assert.AreEqual(number, 12);
        }

        [Test]
        public void Test006LoadAndSet()
        {
            var propFile = new PropertyFile("Test.smpl");
            propFile.Load();
            var props = propFile.Properties;
            props.SetOrCreateEntry("number", 14);
            propFile.Save();
            propFile = new PropertyFile("Test.smpl");
            propFile.Load();
            props = propFile.Properties;
            int number = props.LoadEntry("number", 0);
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
            string word = props.LoadEntry("word", string.Empty);
            Assert.AreEqual(word, "hello");
        }

        [Test]
        public void Test008RemoveEntry()
        {
            var propFile = new PropertyFile("Test.smpl");
            propFile.Load();
            var props = propFile.Properties;
            props.RemoveEntry("number");
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
                props.AddArrayEntry(i);
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
            props.RemoveEntry(entry);
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
            if (!File.Exists("output.txt"))
                File.CreateText("output.txt").Close();
            File.AppendAllText("output.txt", "newFile\n");
            var propFile = new PropertyFile("Test.smpl");
            File.AppendAllText("output.txt", "load\n");
            propFile.Load();
            File.AppendAllText("output.txt", "props\n");
            var props = propFile.Properties;
            File.AppendAllText("output.txt", "load list\n");
            var list = props.LoadList("TheList");
            File.AppendAllText("output.txt", "save\n");
            propFile.Save();

            File.AppendAllText("output.txt", "new file\n");
            propFile = new PropertyFile("Test.smpl");
            File.AppendAllText("output.txt", "load\n");
            propFile.Load();
            File.AppendAllText("output.txt", "props\n");
            props = propFile.Properties;
            File.AppendAllText("output.txt", "list\n");
            list = props.GetList("TheList");
            File.AppendAllText("output.txt", "not null\n");
            Assert.IsNotNull(list);
        }

        [Test]
        public void Test013RemoveList()
        {
            Assert.IsTrue(false);
        }

        [Test]
        public void Test014Comments()
        {
            Assert.IsTrue(false);
        }

        [Test]
        public void Test015BlockComments()
        {
            Assert.IsTrue(false);
        }

        [Test]
        public void Test016NewLines()
        {
            Assert.IsTrue(false);
        }

        [Test]
        public void Test017Concatenations()
        {
            Assert.IsTrue(false);
        }

        [Test]
        public void Test018ConcatWithCommentsBetween()
        {
            Assert.IsTrue(false);
        }

        [Test]
        public void Test019ChangeComments()
        {
            Assert.IsTrue(false);
        }

        [Test]
        public void Test020ClearList()
        {
            Assert.IsTrue(false);
        }

        [Test]
        public void Test021OrderEntriesByLoadOrder()
        {
            Assert.IsTrue(false);
        }

        [Test]
        public void Test022CompressFile()
        {
            Assert.IsTrue(false);
        }

        [Test]
        public void Test023LoadCompressedFile()
        {
            Assert.IsTrue(false);
        }
    }
}

