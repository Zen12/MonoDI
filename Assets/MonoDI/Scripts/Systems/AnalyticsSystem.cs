using MonoDI.Scripts.Core;
using UnityEngine;

namespace MonoDI.Scripts.Systems
{
    public class AnalyticsSystem : MonoBehaviour, IInitSystem
    {
        private IAnalytics[] _providers;
        private bool _wasSentTransition;

        public void OnInit()
        {
            _wasSentTransition = false;
            _providers = new IAnalytics[]
            {
#if UNITY_EDITOR
                new LoggerAnalytics(),
#endif
                //new GameAnalyticsProvider(),
                //new AppMetricaAnalytics(),
            };
        }

        public void LevelFinish(int index, int totalSeconds)
        {
            if (_wasSentTransition)
                return;
            
            foreach (var provider in _providers)
            {
                provider.LevelFinish(index, totalSeconds);
            }
            _wasSentTransition = true;
        }
        public void LevelStart(int index)
        {
            foreach (var provider in _providers)
            {
                provider.LevelStart(index);
            }
        }

        public void LevelFail(int index, int totalSeconds)
        {
            if (_wasSentTransition)
                return;
            foreach (var provider in _providers)
            {
                provider.FailLevel(index, totalSeconds);
            }
            _wasSentTransition = true;
        }

        public void Restart(int index, int totalSeconds)
        {
            if (_wasSentTransition)
                return;
            foreach (var provider in _providers)
            {
                provider.Restart(index, totalSeconds);
            }
            _wasSentTransition = true;
        }
        
        public void CustomEvent(string id)
        {
           // _provider.CustomEvent(id);
        }
    }

    public class LoggerAnalytics : IAnalytics
    {
        public LoggerAnalytics()
        {

        }

        public void CustomEvent(string id)
        {
            Debug.Log($"<color=red>Custom_{id}</color>");
        }

        public void LevelStart(int index)
        {
            Debug.Log($"<color=red>LevelStart_{index}</color>");

        }

        public void LevelFinish(int index, int totalSeconds)
        {
            Debug.Log($"<color=red>LevelFinish_{index}: {totalSeconds} seconds</color>");
        }

        public void FailLevel(int level, int totalSeconds)
        {
            Debug.Log($"<color=red>FailLevel_{level}: {totalSeconds} seconds</color>");
        }

        public void Restart(int level, int totalSeconds)
        {
            Debug.Log($"<color=red>Restart_{level}: {totalSeconds} seconds</color>");
        }
    }


    public interface IAnalytics
    {
        void CustomEvent(string id);
        void LevelStart(int index);
        void LevelFinish(int index, int totalSeconds);

        void FailLevel(int level, int totalSeconds);
        void Restart(int level, int totalSeconds);
    }
    
/*

    public class AppMetricaAnalytics : IAnalytics
    {
        public void CustomEvent(string id)
        {
            AppMetrica.Instance.ReportEvent(id);
            AppMetrica.Instance.SendEventsBuffer();
        }

        public void LevelStart(int index)
        {
            AppMetrica.Instance.ReportEvent("level_start", new Dictionary<string, object>
            {
                {"level_number", index}
            });
            AppMetrica.Instance.SendEventsBuffer();
        }

        public void LevelFinish(int index, int totalSeconds)
        {
            AppMetrica.Instance.ReportEvent("level_finish", new Dictionary<string, object>
            {
                {"level_number", index},
                {"result", "complete"},
                {"time", totalSeconds}
            });
            AppMetrica.Instance.SendEventsBuffer();
        }

        public void FailLevel(int level, int totalSeconds)
        {
            AppMetrica.Instance.ReportEvent("level_finish", new Dictionary<string, object>
            {
                {"level_number", level},
                {"result", "fail"},
                {"time", totalSeconds}
            });
            AppMetrica.Instance.SendEventsBuffer();
        }


        public void Restart(int level, int totalSeconds)
        {
            FailLevel(level, totalSeconds);
        }
    }
    
    public class GameAnalyticsProvider : IAnalytics
    {
        private static bool _wasInit;
        public GameAnalyticsProvider()
        {
            if (_wasInit == true)
                return;
            
            _wasInit = true;
            GameAnalyticsSDK.GameAnalytics.Initialize();
        }

        public void CustomEvent(string id)
        {
            GameAnalyticsSDK.GameAnalytics.NewDesignEvent(id);
        }

        public void LevelStart(int index)
        {
            GameAnalyticsSDK.GameAnalytics.NewProgressionEvent(GameAnalyticsSDK.GAProgressionStatus.Start, index.ToString("0000"));
        }

        public void LevelFinish(int index, int totalSeconds)
        {
            GameAnalyticsSDK.GameAnalytics.NewProgressionEvent(GameAnalyticsSDK.GAProgressionStatus.Complete, index.ToString("0000"));
        }

        public void FailLevel(int level, int totalSeconds)
        {
            GameAnalyticsSDK.GameAnalytics.NewProgressionEvent(GameAnalyticsSDK.GAProgressionStatus.Fail, level.ToString("0000"));

        }

        public void Restart(int level, int totalSeconds)
        {
            CustomEvent("Restart_" + level.ToString("0000"));
            GameAnalyticsSDK.GameAnalytics.NewProgressionEvent(GameAnalyticsSDK.GAProgressionStatus.Fail,level.ToString("0000"));
        }
    }
    */
}