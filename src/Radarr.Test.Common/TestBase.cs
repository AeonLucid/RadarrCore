﻿using System;
using System.IO;
using System.Threading;
using FluentAssertions;
using Moq;
using NLog;
using NUnit.Framework;
using Radarr.Common.Cache;
using Radarr.Common.Cache.Interfaces;
using Radarr.Common.EnvironmentInfo;
using Radarr.Common.EnvironmentInfo.Interfaces;
using Radarr.Common.Messaging;
using Radarr.Core.Messaging.Events.Interfaces;
using Radarr.Test.Common.AutoMoq;

namespace Radarr.Test.Common
{
    public abstract class TestBase<TSubject> : TestBase where TSubject : class
    {
        private TSubject _subject;

        [SetUp]
        public void CoreTestSetup()
        {
            _subject = null;
        }

        protected TSubject Subject
        {
            get
            {
                if (_subject == null)
                {
                    _subject = Mocker.Resolve<TSubject>();
                }

                return _subject;
            }
        }
    }

    public abstract class TestBase : LoggingTest
    {
        private static readonly Random _random = new Random();

        private AutoMoqer _mocker;

        protected AutoMoqer Mocker
        {
            get
            {
                if (_mocker == null)
                {
                    _mocker = new AutoMoqer();
                    _mocker.SetConstant<ICacheManager>(new CacheManager());
                    _mocker.SetConstant<IStartupContext>(new StartupContext(new string[0]));
                    _mocker.SetConstant(TestLogger);
                }

                return _mocker;
            }
        }
        
        protected int RandomNumber
        {
            get
            {
                Thread.Sleep(1);
                return _random.Next(0, int.MaxValue);
            }
        }

        private string VirtualPath
        {
            get
            {
                var virtualPath = Path.Combine(TempFolder, "VirtualNzbDrone");
                if (!Directory.Exists(virtualPath)) Directory.CreateDirectory(virtualPath);

                return virtualPath;
            }
        }

        protected string TempFolder { get; private set; }

        [SetUp]
        public void TestBaseSetup()
        {
            GetType().IsPublic.Should().BeTrue("All Test fixtures should be public.");

            LogManager.ReconfigExistingLoggers();

            TempFolder = Path.Combine(TestContext.CurrentContext.TestDirectory, "_temp_" + DateTime.Now.Ticks);

            Directory.CreateDirectory(TempFolder);
        }

        [TearDown]
        public void TestBaseTearDown()
        {
            _mocker = null;

            try
            {
                var tempFolder = new DirectoryInfo(TempFolder);
                if (tempFolder.Exists)
                {
                    foreach (var file in tempFolder.GetFiles("*", SearchOption.AllDirectories))
                    {
                        file.IsReadOnly = false;
                    }

                    tempFolder.Delete(true);
                }
            }
            catch (Exception)
            {
            }
        }

        protected IAppFolderInfo TestFolderInfo { get; private set; }

        protected void WindowsOnly()
        {
            if (!OsInfoCore.IsWindows)
            {
                throw new IgnoreException("windows specific test");
            }
        }

        protected void UnixOnly()
        {
            if (!OsInfoCore.IsLinux && !OsInfoCore.IsOSX)
            {
                throw new IgnoreException("unix specific test");
            }
        }

        protected void WithTempAsAppPath()
        {
            Mocker.GetMock<IAppFolderInfo>()
                .SetupGet(c => c.AppDataFolder)
                .Returns(VirtualPath);

            TestFolderInfo = Mocker.GetMock<IAppFolderInfo>().Object;
        }

        protected string GetTestPath(string path)
        {
            return Path.Combine(TestContext.CurrentContext.TestDirectory, Path.Combine(path.Split('/')));
        }

        protected string ReadAllText(string path)
        {
            return File.ReadAllText(GetTestPath(path));
        }

        protected string GetTempFilePath()
        {
            return Path.Combine(TempFolder, Path.GetRandomFileName());
        }

        protected void VerifyEventPublished<TEvent>() where TEvent : class, IEvent
        {
            VerifyEventPublished<TEvent>(Times.Once());
        }

        protected void VerifyEventPublished<TEvent>(Times times) where TEvent : class, IEvent
        {
            Mocker.GetMock<IEventAggregator>().Verify(c => c.PublishEvent(It.IsAny<TEvent>()), times);
        }

        protected void VerifyEventNotPublished<TEvent>() where TEvent : class, IEvent
        {
            Mocker.GetMock<IEventAggregator>().Verify(c => c.PublishEvent(It.IsAny<TEvent>()), Times.Never());
        }
    }
}