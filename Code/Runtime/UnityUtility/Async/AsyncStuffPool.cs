﻿using UU.Collections;
using UU.IDGenerating;
using UnityEngine.SceneManagement;

namespace UU.Async
{
    internal static class AsyncStuffPool
    {
        private class Data
        {
            private const string RUNNER_NAME = "Task";

            public ObjectPool<RoutineRunner> RunnersPool;
            public IDGenerator<uint> IdProvider;

            public Data()
            {
                IdProvider = new UintIDGenerator();
                RunnersPool = new ObjectPool<RoutineRunner>(f_create);

                SceneManager.sceneUnloaded += _ => RunnersPool.Clear();
            }

            private RoutineRunner f_create()
            {
                return Script.CreateInstance<RoutineRunner>(RUNNER_NAME);
            }
        }

        private static Data s_inst;

        static AsyncStuffPool()
        {
            s_inst = new Data();
        }

        internal static uint GetNewId()
        {
            return s_inst.IdProvider.GetNewId();
        }

        internal static RoutineRunner GetExecutor()
        {
            return s_inst.RunnersPool.Get();
        }

        internal static void Return(RoutineRunner runner)
        {
            s_inst.RunnersPool.Release(runner);
        }
    }
}
