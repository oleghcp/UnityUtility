﻿using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityUtility.Collections;
using UnityUtility.IdGenerating;

namespace UnityUtility.Async
{
    internal interface IAsyncSettings
    {
        bool CanBeStopped { get; }
        bool CanBeStoppedGlobally { get; }
        bool DoNotDestroyOnLoad { get; }
    }

    public interface ITaskStopper
    {
        event Action StopAllTasks_Event;
    }

    internal class TaskFactory
    {
        private class DefaultSettings : IAsyncSettings
        {
            bool IAsyncSettings.CanBeStopped => true;
            bool IAsyncSettings.CanBeStoppedGlobally => false;
            bool IAsyncSettings.DoNotDestroyOnLoad => true;
        }

        public event Action StopTasks_Event;

        private readonly string GAME_OBJECT_NAME;
        private readonly ObjectPool<RoutineRunner> m_runnersPool;
        private readonly IdGenerator<long> m_idProvider;

        private readonly bool m_canBeStopped;
        private readonly bool m_canBeStoppedGlobally;
        private readonly bool m_dontDestroyOnLoad;

        private ITaskStopper m_stopper;

        public bool CanBeStopped
        {
            get { return m_canBeStopped; }
        }

        public bool CanBeStoppedGlobally
        {
            get { return m_canBeStoppedGlobally; }
        }

        public TaskFactory(string gameObjectName = "Task")
        {
            GAME_OBJECT_NAME = gameObjectName;

            IAsyncSettings settings = GetSettings();

            m_canBeStopped = settings.CanBeStopped;
            m_canBeStoppedGlobally = settings.CanBeStoppedGlobally;
            m_dontDestroyOnLoad = settings.DoNotDestroyOnLoad;

            m_idProvider = new LongIdGenerator();
            m_runnersPool = new ObjectPool<RoutineRunner>(f_create);

            if (!m_dontDestroyOnLoad)
                SceneManager.sceneUnloaded += _ => m_runnersPool.Clear();
        }

        public void RegisterStopper(ITaskStopper stopper)
        {
            if (!m_canBeStoppedGlobally)
            {
                throw new InvalidOperationException($"Tasks cannot be stopped due to the current system option. Check {TaskSystem.SYSTEM_NAME} settings.");
            }

            if (m_stopper == stopper)
            {
                throw new InvalidOperationException("Stop object is already set.");
            }

            (m_stopper = stopper).StopAllTasks_Event += () => StopTasks_Event?.Invoke();
        }

        public long GetNewId()
        {
            return m_idProvider.GetNewId();
        }

        public void Release(ITask runner)
        {
            m_runnersPool.Release(runner as RoutineRunner);
        }

        public ITask GetRunner()
        {
            return m_runnersPool.Get();
        }

        // -- //

        private static IAsyncSettings GetSettings()
        {
            IAsyncSettings settings = Resources.Load<AsyncSystemSettings>(nameof(AsyncSystemSettings));

            return (settings == null ? new DefaultSettings() : settings) as IAsyncSettings;
        }

        private RoutineRunner f_create()
        {
            var taskRunner = Script.CreateInstance<RoutineRunner>(GAME_OBJECT_NAME);
            taskRunner.SetUp(this, m_dontDestroyOnLoad);
            return taskRunner;
        }
    }
}
