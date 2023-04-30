using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    [ExecuteAlways]
    public class Runner : Singleton<Runner>
    {
        private Dictionary<int, Routine> m_Routines = new Dictionary<int, Routine>();
        private int m_NextRoutineId = 1;

        public static int Run(IEnumerator routine)
        {
            Routine r = new Routine(routine);
            return r.ID;
        }

        public static void Stop(int id)
        {
            Routine r = null;
            if (Instance.m_Routines.TryGetValue(id, out r))
                r.Stop = true;
        }

        static public bool IsRunning(int id)
        {
            return Instance.m_Routines.ContainsKey(id);
        }

        public void UpdateRoutines()
        {
            if (m_Routines.Count > 0)
            {
                // we are not in play mode, so we must manually update our co-routines ourselves
                List<Routine> routines = new List<Routine>();
                foreach (var kp in m_Routines)
                    routines.Add(kp.Value);

                foreach (var r in routines)
                    r.MoveNext();
            }
        }


        private class Routine : IEnumerator
        {
            public int ID { get; private set; }
            public bool Stop { get; set; }

            private bool m_bMoveNext = false;
            private IEnumerator m_Enumerator = null;

            public Routine(IEnumerator a_enumerator)
            {
                m_Enumerator = a_enumerator;
                Runner.Instance.StartCoroutine(this);
                Stop = false;
                ID = Runner.Instance.m_NextRoutineId++;

                Runner.Instance.m_Routines[ID] = this;
            }

            public object Current { get { return m_Enumerator.Current; } }
            public bool MoveNext()
            {
                m_bMoveNext = m_Enumerator.MoveNext();
                if (m_bMoveNext && Stop)
                    m_bMoveNext = false;

                if (!m_bMoveNext)
                {
                    Runner.Instance.m_Routines.Remove(ID); // remove from the mapping
                    Runner.instance.StopCoroutine(this);
                }

                return m_bMoveNext;
            }
            public void Reset() { m_Enumerator.Reset(); }
        }
    }

}