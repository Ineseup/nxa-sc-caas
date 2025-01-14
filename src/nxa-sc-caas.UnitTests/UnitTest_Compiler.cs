﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using NXA.SC.Caas.Models;

namespace nxa_sc_caas.UnitTests
{
    [TestClass]
    public class UnitTest_Compiler
    {
        [TestMethod]
        public void Test_SetsIdentifier()
        {
            var task = new CompilerTask();
            var newTask = task.SetIdentifier("id1");
            Assert.AreEqual(newTask.Identifier, "id1");
        }
        [TestMethod]
        public void Test_SetsStatus()
        {
            var task = new CompilerTask();
            var newTask = task.SetStatus(CompilerTaskStatus.CREATED);
            Assert.AreEqual(newTask.Status, CompilerTaskStatus.CREATED);
        }
        [TestMethod]
        public void Test_SetsCreate()
        {
            var task = new CompilerTask();
            var create = new CreateCompilerTask { ContractAuthorName = "name1" };
            var newTask = task.SetCreate(create);
            Assert.AreEqual(newTask.Create.ContractAuthorName, "name1");
        }
        [TestMethod]
        public void Test_SetsResult()
        {
            var task = new CompilerTask();
            var result = new CompilerResult(new byte[64], "manifestjson");
            var newTask = task.SetResult(result);
            Assert.AreEqual(newTask.Result.Manifest, "manifestjson");
        }
        [TestMethod]
        public void Test_SetsError()
        {
            var task = new CompilerTask();
            var error = new CompilerError("filestr", (uint)1, 123, "errormsg", string.Empty);
            var newTask = task.SetError(error);
            Assert.AreEqual(newTask.Error.Messsage, "errormsg");
        }
    }
}
